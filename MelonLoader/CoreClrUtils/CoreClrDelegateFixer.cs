#if NET6_0_OR_GREATER

using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;

namespace MelonLoader.CoreClrUtils
{
    internal class CoreClrDelegateFixer
    {
        private static readonly AssemblyBuilder assembly = AssemblyBuilder.DefineDynamicAssembly(new("MelonLoaderFixedHooks"), AssemblyBuilderAccess.Run);
        private static readonly ModuleBuilder module = assembly.DefineDynamicModule("MelonLoaderFixedHooks");
        private static readonly Dictionary<MethodInfo, Delegate> PinnedFixedDelegates = new();

        internal static bool SanityCheckDetour(ref IntPtr detour)
        {
                using DataTarget dt = DataTarget.CreateSnapshotAndAttach(Environment.ProcessId);
                var version = dt.ClrVersions.FirstOrDefault();
                if (version == null) {
                    MelonLogger.Error("Could not find CLR version for ClrMD!");
                    return true;
                }
                ClrRuntime runtime = version.CreateRuntime();

                ClrMethod method = runtime.GetMethodByInstructionPointer((ulong)detour.ToInt64());

                if (method != null)
                {
                    var managedMethod = MethodBaseHelper.GetMethodBaseFromHandle((IntPtr)method.MethodDesc);

                    if (managedMethod?.GetCustomAttribute<UnmanagedCallersOnlyAttribute>() == null)
                    {
                        //We have provided a direct managed method as the pointer to detour to. This doesn't work under CoreCLR, so we yell at the user and stop
                        var melon = MelonUtils.GetMelonFromStackTrace(new System.Diagnostics.StackTrace(), true);

                        var logger = melon?.LoggerInstance ?? new MelonLogger.Instance("Bad Delegate");
                        var modName = melon?.Info.Name ?? "Unknown mod";

                        //Try and patch the delegate if we can
                        if(melon != null && managedMethod is MethodInfo methodInfo)
                        {
                            if (PinnedFixedDelegates.TryGetValue(methodInfo, out var pinnedDel))
                            {
                                detour = Marshal.GetFunctionPointerForDelegate(pinnedDel);
                                logger.Warning($"Encountered a dodgy native hook to a managed method in melon {modName}: {methodInfo.DeclaringType.FullName}::{methodInfo.Name}. It has been wrapped in a proper unmanaged delegate, but please fix your mod! You also won't be able to detach this hook!");
                                return true;
                            }

                            try
                            {
                                var wrapperType = GetHookWrapperDelegateType(melon, methodInfo);

                                var del = Delegate.CreateDelegate(wrapperType, methodInfo);
                                
                                PinnedFixedDelegates[methodInfo] = del;

                                detour = Marshal.GetFunctionPointerForDelegate(del);

                                logger.Warning($"Encountered a dodgy native hook to a managed method in melon {modName}: {methodInfo.DeclaringType.FullName}::{methodInfo.Name}. It has been wrapped in a proper unmanaged delegate, but please fix your mod! You also won't be able to detach this hook!");

                                return true;
                            } catch(Exception ex)
                            {
                                MelonLogger.Error("Failed to repair invalid native hook: ", ex);
                                //Ignore, fall down to error below
                            }
                        } else
                        {
                            logger.Error($"Failed to resolve the offending melon from the stack and/or the managed method target. ManagedMethod is {managedMethod}, of type {managedMethod.GetType()}, stack is {Environment.StackTrace}");
                        }
                        
                        PrintDirtyDelegateWarning(logger, modName, managedMethod);
                        return false;
                    }
                }

            return true;
        }

        private static Type GetHookWrapperDelegateType(MelonBase melon, MethodInfo managedMethod)
        {
            string melonName = melon?.Info?.Name?.Replace(' ', '_') ?? "Internal";
            string typeNamespace = managedMethod?.DeclaringType?.Namespace?.Replace('.', '_') ?? "Global";
            string typeNameString = managedMethod?.DeclaringType?.Name ?? "DynamicType";
            string methodName = managedMethod?.Name ?? "UnknownMethod";            
            int methodHash = managedMethod?.GetHashCode() ?? 0;
            var typeName = $"FixedHook_{melonName}_{typeNamespace}_{typeNameString}_{methodName}_{methodHash}";

            if (module.GetType(typeName) is { } ret)
                return ret;

            var type = module.DefineType(typeName, TypeAttributes.Sealed | TypeAttributes.Public, typeof(MulticastDelegate));
            type.SetCustomAttribute(new CustomAttributeBuilder(typeof(UnmanagedFunctionPointerAttribute).GetConstructor(new[] { typeof(CallingConvention) }), new object[] { CallingConvention.Cdecl }));

            var ctor = type.DefineConstructor(MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Public, CallingConventions.HasThis, new[] { typeof(object), typeof(IntPtr) });
            ctor.SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            var parameterTypes = managedMethod.GetParameters().Select(p => p.ParameterType).ToArray();

            type.DefineMethod(
                "Invoke",
                MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.Public,
                CallingConventions.HasThis,
                managedMethod.ReturnType,
                parameterTypes
            ).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            type.DefineMethod(
                "BeginInvoke",
                MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.Public,
                CallingConventions.HasThis, typeof(IAsyncResult),
                parameterTypes.Concat(new[] { typeof(AsyncCallback), typeof(object) }).ToArray()
            ).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            type.DefineMethod(
                "EndInvoke",
                MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.Public,
                CallingConventions.HasThis,
                managedMethod.ReturnType,
                new[] { typeof(IAsyncResult) }
            ).SetImplementationFlags(MethodImplAttributes.CodeTypeMask);

            return type.CreateType();
        }

        internal static void Unpin(MethodInfo method)
            => PinnedFixedDelegates.Remove(method);

        internal static IntPtr GetFixedPointerForDelegate(Delegate del)
        {
            if (del == null) return IntPtr.Zero;

            if (del.Method.GetType().Name.Contains("DynamicMethod") || del.Method.DeclaringType == null)
            {
                return Marshal.GetFunctionPointerForDelegate(del);
            }
            
            if (PinnedFixedDelegates.TryGetValue(del.Method, out var pinnedDel))
                return Marshal.GetFunctionPointerForDelegate(pinnedDel);

            try
            {
                var wrapperType = GetHookWrapperDelegateType(null, del.Method);
                var wrappedDel = Delegate.CreateDelegate(wrapperType, del.Target, del.Method);
                
                PinnedFixedDelegates[del.Method] = wrappedDel;
                
                return Marshal.GetFunctionPointerForDelegate(wrappedDel);
            }
            catch (Exception ex)
            {
                return Marshal.GetFunctionPointerForDelegate(del); 
            }
        }

        private static void PrintDirtyDelegateWarning(MelonLogger.Instance offendingMelonLogger, string offendingMelonName, MethodBase offendingMethod)
        {
            offendingMelonLogger.BigError(
                   $"The mod {offendingMelonName} has attempted to detour a native method to a managed one.\n"
                 + $"The managed method target is {offendingMethod.DeclaringType.Name}::{offendingMethod.Name}\n"
                 + "If this hadn't been stopped, the runtime would have crashed.\n"
                 + "Modder: Either create an [UnmanagedFunctionPointer] delegate from your function, and use Marshal.GetFunctionPointerFromDelegate,\n"
                 + "or annotate your patch function as [UnmanagedCallersOnly] (target net5.0), and then you can directly use &Method as the hook target."
            );
        }
    }
}
#endif