using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using HarmonyLib;
using HarmonyLib.Public.Patching;
using HarmonyLib.Tools;
using Mono.Cecil;
using MonoMod.Utils;
using MonoMod.Cil;
using MelonLoader.InternalUtils;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;

#if NET6_0
using MelonLoader.CoreClrUtils;
#endif

namespace MelonLoader.Support
{
    //Should be removeable
    internal class HarmonyMethodPatcher : MethodPatcher
    {
        private static AccessTools.FieldRef<ILGenerator, int> codeLenGetter = null;
        private static AccessTools.FieldRef<ILGenerator, LocalBuilder[]> localsGetter = null;
        private static AccessTools.FieldRef<ILGenerator, int> localCountGetter = null;
        private IntPtr originalMethodInfoPointer;
        private IntPtr copiedMethodInfoPointer;
        private IntPtr methodDetourPointer;
        private bool hasWarned = false;

        private delegate void PatchTools_RememberObject_Delegate(object key, object value);
        private static PatchTools_RememberObject_Delegate PatchTools_RememberObject = null;

        static HarmonyMethodPatcher()
        {
            try { codeLenGetter = AccessTools.FieldRefAccess<ILGenerator, int>("code_len"); }
            catch { codeLenGetter = AccessTools.FieldRefAccess<ILGenerator, int>("m_length"); }
            try { localsGetter = AccessTools.FieldRefAccess<ILGenerator, LocalBuilder[]>("locals"); } catch { }
            try { localCountGetter = AccessTools.FieldRefAccess<ILGenerator, int>("m_localCount"); } catch { }
            PatchTools_RememberObject = AccessTools.Method("HarmonyLib.PatchTools:RememberObject").CreateDelegate<PatchTools_RememberObject_Delegate>();
        }

        internal static void TryResolve(object sender, PatchManager.PatcherResolverEventArgs args)
        {
            if (Main.Interop.IsInheritedFromIl2CppObjectBase(args.Original.DeclaringType)
                && !Main.Interop.IsInjectedType(args.Original.DeclaringType))
                args.MethodPatcher = new HarmonyMethodPatcher(args.Original);
        }

        private HarmonyMethodPatcher(MethodBase original) : base(original)
        {
            originalMethodInfoPointer = InteropSupport.MethodBaseToIl2CppMethodInfoPointer(Original);
            copiedMethodInfoPointer = Main.Interop.CopyMethodInfoStruct(originalMethodInfoPointer);
        }

        public override MethodBase DetourTo(MethodBase replacement)
        {
            DebugCheck();

            DynamicMethodDefinition newreplacementdmd = CopyOriginal();
            HarmonyManipulator.Manipulate(Original, Original.GetPatchInfo(), new ILContext(newreplacementdmd.Definition));
            MethodInfo newreplacement = newreplacementdmd.Generate();

            MethodInfo il2CppShim = CreateIl2CppShim(newreplacement).Generate();
            Type il2CppShimDelegateType = DelegateTypeFactory.instance.CreateDelegateType(il2CppShim, CallingConvention.Cdecl);
            Delegate il2CppShimDelegate = il2CppShim.CreateDelegate(il2CppShimDelegateType);
            IntPtr il2CppShimDelegatePtr = il2CppShimDelegate.GetFunctionPointer();

            if (methodDetourPointer != IntPtr.Zero)
                MelonUtils.NativeHookDetach(copiedMethodInfoPointer, methodDetourPointer);
            
            MelonUtils.NativeHookAttachDirect(copiedMethodInfoPointer, il2CppShimDelegatePtr);

#if NET6_0
            NativeStackWalk.RegisterHookAddr((ulong)il2CppShimDelegatePtr, $"Harmony Hook for {Original.FullDescription()}");
#endif
            
            methodDetourPointer = il2CppShimDelegatePtr;

            PatchTools_RememberObject(Original, new LemonTuple<MethodInfo, MethodInfo, Delegate> { Item1 = newreplacement, Item2 = il2CppShim, Item3 = il2CppShimDelegate });

            return newreplacement;
        }

        public override DynamicMethodDefinition CopyOriginal()
        {
            DynamicMethodDefinition method = Original.ToNewDynamicMethodDefinition();
            method.Definition.Name += "_wrapper";
            ILContext ilcontext = new ILContext(method.Definition);
            ILCursor ilcursor = new ILCursor(ilcontext);
            FieldReference tempfieldreference = null;
            if (ilcursor.TryGotoNext(x => x.MatchLdsfld(out tempfieldreference), x => x.MatchCall(typeof(IL2CPP), "il2cpp_object_get_virtual_method")))
            {
                // Virtual method: Replace the sequence
                // - ldarg.0
                // - call native int[UnhollowerBaseLib] UnhollowerBaseLib.IL2CPP::Il2CppObjectBaseToPtr(class [UnhollowerBaseLib] UnhollowerBaseLib.Il2CppObjectBase)
                // - ldsfld native int SomeClass::NativeMethodInfoPtr_Etc
                // - call native int[UnhollowerBaseLib] UnhollowerBaseLib.IL2CPP::il2cpp_object_get_virtual_method(native int, native int)

                ilcursor.Index -= 2;
                ilcursor.RemoveRange(4);
            }
            else if (ilcursor.TryGotoNext(x => x.MatchLdsfld(Main.Interop.MethodBaseToIl2CppFieldInfo(Original))))
                ilcursor.Remove();
            else
            {
                MelonLogger.Error("Harmony Patcher could not rewrite Il2Cpp Unhollowed Method. Expect a stack overflow.");
                return method;
            }
            ilcursor.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I8, copiedMethodInfoPointer.ToInt64());
            ilcursor.Emit(Mono.Cecil.Cil.OpCodes.Conv_I);
            return method;
        }

        private DynamicMethodDefinition CreateIl2CppShim(MethodInfo patch)
        {
            string patchName = patch.Name + "_il2cpp";

            ParameterInfo[] patchParams = patch.GetParameters();
            Type[] patchParamTypes = patchParams.Types().ToArray();
            Type[] il2cppParamTypes = patchParamTypes.Select(Il2CppTypeForPatchType).ToArray();
            Type patchReturnType = AccessTools.GetReturnedType(patch);
            Type il2cppReturnType = Il2CppTypeForPatchType(patchReturnType);

            var method = new DynamicMethodDefinition(patchName, il2cppReturnType, il2cppParamTypes);
            var il = method.GetILGenerator();

            LocalBuilder returnLocal = null;
            if (patchReturnType != typeof(void))
            {
                returnLocal = il.DeclareLocal(patchReturnType);
                LogLocalVariable(il, returnLocal);
            }
            Type exceptionType = typeof(Exception);
            LocalBuilder exceptionLocal = il.DeclareLocal(exceptionType);
            LogLocalVariable(il, exceptionLocal);

            // Start a try-block for the call to the original patch
            il.BeginExceptionBlock();

            // Load arguments, invoking the IntPrt -> Il2CppObject constructor for IL2CPP types
            LocalBuilder[] byRefValues = new LocalBuilder[patchParams.Length];
            for (int i = 0; i < patchParamTypes.Length; ++i)
            {
                il.Emit(OpCodes.Ldarg, i);
                ConvertArgument(il, patchParamTypes[i], ref byRefValues[i]);
                if (byRefValues[i] != null)
                    LogLocalVariable(il, byRefValues[i]);
            }

            // Call the original patch with the now-correct types
            il.Emit(OpCodes.Call, patch);

            // Store the result, if any
            if (returnLocal != null)
                il.Emit(OpCodes.Stloc, returnLocal);

            // Catch any exceptions that may have been thrown
            il.BeginCatchBlock(exceptionType);

            // MelonLogger.LogError("Exception in ...\n" + exception.ToString());
            il.Emit(OpCodes.Stloc, exceptionLocal);
            il.Emit(OpCodes.Ldstr, $"Exception in Harmony patch of method {Original.FullDescription()}:\n");
            il.Emit(OpCodes.Ldloc, exceptionLocal);
            il.Emit(OpCodes.Call, AccessTools.DeclaredMethod(typeof(Exception), "ToString", new Type[0]));
            il.Emit(OpCodes.Call, AccessTools.DeclaredMethod(typeof(string), "Concat", new Type[] { typeof(string), typeof(string) }));
            il.Emit(OpCodes.Call, AccessTools.DeclaredMethod(typeof(MelonLogger), "Error", new Type[] { typeof(string) }));

            // Close the exception block
            il.EndExceptionBlock();

            // Write back the pointers of ref arguments
            for (var i = 0; i < patchParamTypes.Length; ++i)
            {
                if (byRefValues[i] == null)
                    continue;
                il.Emit(OpCodes.Ldarg_S, i);
                il.Emit(OpCodes.Ldloc, byRefValues[i]);
                ConvertTypeToIl2Cpp(il, patchParamTypes[i].GetElementType());
                il.Emit(OpCodes.Stind_I);
            }

            // Load the return value, if any, and unwrap it if required
            if (returnLocal != null)
            {
                il.Emit(OpCodes.Ldloc, returnLocal);
                ConvertTypeToIl2Cpp(il, patchReturnType);
            }

            il.Emit(OpCodes.Ret);
            return method;
        }

        private static Type Il2CppTypeForPatchType(Type type)
        {
            if (type.IsByRef)
            {
                Type element = type.GetElementType();
                if (element == typeof(string) || Main.Interop.IsInheritedFromIl2CppObjectBase(element))
                    return typeof(IntPtr*);
            }
            else if (type == typeof(string) || Main.Interop.IsInheritedFromIl2CppObjectBase(type))
                return typeof(IntPtr);
            return type;
        }

        private static void ConvertArgument(ILGenerator il, Type paramType, ref LocalBuilder byRefLocal)
        {
            if (paramType.IsValueType)
                return;

            Type currentType = paramType;
            bool byRef = paramType.IsByRef;
            if (byRef)
                currentType = paramType.GetElementType();

            if (currentType == typeof(string))
            {
                // return Il2CppStringToManaged(ptr);

                // byRefLocal = Il2CppStringToManaged(*ptr);
                // return ref byRefLocal;

                if (byRef)
                {
                    byRefLocal = il.DeclareLocal(currentType);
                    il.Emit(OpCodes.Ldind_I);
                }

                il.Emit(OpCodes.Call, AccessTools.DeclaredMethod(typeof(IL2CPP), "Il2CppStringToManaged", new Type[] { typeof(IntPtr) }));

                if (byRef)
                {
                    il.Emit(OpCodes.Stloc, byRefLocal);
                    il.Emit(OpCodes.Ldloca, byRefLocal);
                }
            }
            else if (Main.Interop.IsInheritedFromIl2CppObjectBase(currentType))
            {
                // return ptr == 0 ? null : new SomeType(ptr);

                // byRefLocal = *ptr == 0 ? null : new SomeType(*ptr);
                // return ref byRefLocal;

                Label ptrNonZero = il.DefineLabel();
                Label done = il.DefineLabel();

                if (byRef)
                {
                    byRefLocal = il.DeclareLocal(currentType);
                    il.Emit(OpCodes.Ldind_I);
                }

                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Brtrue_S, ptrNonZero);
                il.Emit(OpCodes.Pop);

                if (!byRef)
                    il.Emit(OpCodes.Ldnull);

                il.Emit(OpCodes.Br_S, done);
                il.MarkLabel(ptrNonZero);
                il.Emit(OpCodes.Newobj, Il2CppConstuctor(currentType));

                if (byRef)
                    il.Emit(OpCodes.Stloc, byRefLocal);

                il.MarkLabel(done);

                if (byRef)
                    il.Emit(OpCodes.Ldloca, byRefLocal);
            }
        }

        private static void ConvertTypeToIl2Cpp(ILGenerator il, Type returnType)
        {
            if (returnType == typeof(string))
                il.Emit(OpCodes.Call, AccessTools.DeclaredMethod(typeof(IL2CPP), "ManagedStringToIl2Cpp", new Type[] { typeof(string) }));
            else if (!returnType.IsValueType && Main.Interop.IsInheritedFromIl2CppObjectBase(returnType))
                il.Emit(OpCodes.Call, AccessTools.DeclaredMethod(typeof(IL2CPP), "Il2CppObjectBaseToPtr", new Type[] { typeof(Il2CppObjectBase) }));
        }

        private static string CodePos(ILGenerator il)
        {
            int offset = 0;
            if (codeLenGetter != null)
                offset = codeLenGetter(il);
            return string.Format("L_{0:x4}: ", offset);
        }

        private static void LogLocalVariable(ILGenerator il, LocalBuilder variable)
        {
            if (!HarmonyFileLog.Enabled)
                return;
            var localCount = -1;
            var localsArray = localsGetter != null ? localsGetter(il) : null;
            if (localsArray != null && localsArray.Length > 0)
                localCount = localsArray.Length;
            else if (localCountGetter != null)
                localCount = localCountGetter(il);
            else
                localCount = 0;
            var str = string.Format("{0}Local var {1}: {2}{3}", CodePos(il), localCount - 1, variable.LocalType.FullName, variable.IsPinned ? "(pinned)" : "");
            HarmonyFileLog.Writer.WriteLine(str);
        }

        private void DebugCheck()
        {
            if (!MelonDebug.IsEnabled() || hasWarned)
                return;
            hasWarned = true;

            PatchInfo patchInfo = Original.GetPatchInfo();

            Patch basePatch = patchInfo.prefixes.Count() > 0 ? patchInfo.prefixes.First()
                : patchInfo.postfixes.Count() > 0 ? patchInfo.postfixes.First()
                : patchInfo.transpilers.Count() > 0 ? patchInfo.transpilers.First()
                : patchInfo.finalizers.Count() > 0 ? patchInfo.finalizers.First() : null;

            MelonLogger.Instance loggerInstance = FindMelon(melon => basePatch != null && melon.HarmonyInstance.Id.Equals(basePatch.owner));
            if (loggerInstance == null && basePatch != null)
            {
                // Patching using a custom Harmony instance; try to infer the melon assembly from the container type, prefix, postfix, or transpiler.
                Assembly melonAssembly = basePatch.PatchMethod.DeclaringType?.Assembly;
                if (melonAssembly != null)
                    loggerInstance = FindMelon(melon => melon.MelonAssembly.Assembly == melonAssembly);
            }

            WarnIfHasTranspiler(patchInfo, loggerInstance);
            WarnIfOriginalMethodIsInlined(loggerInstance);
        }

        private void WarnIfOriginalMethodIsInlined(MelonLogger.Instance loggerInstance)
        {
            int callerCount = Main.Interop.GetIl2CppMethodCallerCount(Original) ?? -1;
            if (callerCount > 0
                || UnityMagicMethods.IsUnityMagicMethod(Original))
                return;
            string txt = $"Harmony: Method {Original.FullDescription()} does not appear to get called directly from anywhere, " +
                "suggesting it may have been inlined and your patch may not be called.";
            if (loggerInstance != null)
                loggerInstance.Warning(txt);
            else
                MelonLogger.Warning(txt);
        }

        private void WarnIfHasTranspiler(PatchInfo patchInfo, MelonLogger.Instance loggerInstance)
        {
            if (patchInfo.transpilers.Length <= 0)
                return;
            string txt = $"Harmony: Method {Original.FullDescription()} will only have its Unhollowed IL available to Transpilers, " +
                "suggesting you either don't use any Transpilers when Patching this Method or ignore this Warning if modifying the Unhollowed IL is your goal.";
            if (loggerInstance != null)
                loggerInstance.Warning(txt);
            else
                MelonLogger.Warning(txt);
        }

        private static MelonLogger.Instance FindMelon(Predicate<MelonBase> criterion)
        {
            MelonLogger.Instance loggerInstance = null;

            LemonEnumerator<MelonPlugin> PluginEnumerator = new(MelonPlugin.RegisteredMelons);
            while (PluginEnumerator.MoveNext())
                if (criterion(PluginEnumerator.Current))
                {
                    loggerInstance = PluginEnumerator.Current.LoggerInstance;
                    break;
                }

            if (loggerInstance == null)
            {
                LemonEnumerator<MelonMod> ModEnumerator = new(MelonMod.RegisteredMelons);
                while (ModEnumerator.MoveNext())
                    if (criterion(ModEnumerator.Current))
                    {
                        loggerInstance = ModEnumerator.Current.LoggerInstance;
                        break;
                    }
            }

            return loggerInstance;
        }

        private static ConstructorInfo Il2CppConstuctor(Type type) => AccessTools.DeclaredConstructor(type, new Type[] { typeof(IntPtr) });
        public override DynamicMethodDefinition PrepareOriginal() => null;
    }
}