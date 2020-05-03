using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MelonLoader
{
    internal class UnhollowerSupport
    {
        private static MethodInfo ConvertDelegateMethod = null;

        internal static bool IsGeneratedAssemblyType(Type type) => (NETFrameworkFix.Type_op_Inequality(Main.Il2CppObjectBaseType, null) && NETFrameworkFix.Type_op_Inequality(type, null) && type.IsSubclassOf(Main.Il2CppObjectBaseType));

        internal static IntPtr MethodBaseToIntPtr(MethodBase method)
        {
            if (IsGeneratedAssemblyType(method.DeclaringType))
            {
                FieldInfo methodptr = method.DeclaringType.GetFields(BindingFlags.Static | BindingFlags.NonPublic).First(x => x.Name.StartsWith(("NativeMethodInfoPtr_" + method.Name)));
                if (methodptr != null)
                    return (IntPtr)methodptr.GetValue(null);
            }
            else
                return method.MethodHandle.GetFunctionPointer();
            return IntPtr.Zero;
        }

        internal static IntPtr MethodInfoToIntPtr(MethodInfo method)
        {
            if (IsGeneratedAssemblyType(method.DeclaringType))
            {
                FieldInfo methodptr = method.DeclaringType.GetFields(BindingFlags.Static | BindingFlags.NonPublic).First(x => x.Name.StartsWith(("NativeMethodInfoPtr_" + method.Name)));
                if (methodptr != null)
                    return (IntPtr)methodptr.GetValue(null);
            }
            else
                return method.MethodHandle.GetFunctionPointer();
            return IntPtr.Zero;
        }

        internal static object ConvertDelegate(object action)
        {
            if (NETFrameworkFix.Assembly_op_Inequality(Main.UnhollowerRuntimeLib, null))
            {
                if (NETFrameworkFix.MethodBase_op_Equality(ConvertDelegateMethod, null))
                    ConvertDelegateMethod = Main.UnhollowerRuntimeLib.GetType("UnhollowerRuntimeLib.DelegateSupport").GetMethod("ConvertDelegate");
                if (NETFrameworkFix.MethodInfo_op_Inequality(ConvertDelegateMethod, null))
                    return ConvertDelegateMethod.Invoke(null, new object[] { action });
            }
            return null;
        }

        internal static void FixLoggerEvents()
        {
            Type LogSupportType = Main.UnhollowerBaseLib.GetType("UnhollowerBaseLib.LogSupport");
            if (NETFrameworkFix.Type_op_Inequality(LogSupportType, null))
            {
                if (Console.Enabled || Imports.IsDebugMode())
                {
                    EventInfo InfoHandlerEvent = LogSupportType.GetEvent("InfoHandler");
                    if (InfoHandlerEvent != null)
                    {
                        InfoHandlerEvent.RemoveEventHandler(null, GetHandler(InfoHandlerEvent, System.Console.WriteLine, GetParameters(InfoHandlerEvent)));
                        InfoHandlerEvent.AddEventHandler(null, Delegate.CreateDelegate(InfoHandlerEvent.EventHandlerType, null, new Action<string>(MelonModLogger.Log).Method));
                    }
                }

                EventInfo WarningHandlerEvent = LogSupportType.GetEvent("WarningHandler");
                if (WarningHandlerEvent != null)
                {
                    WarningHandlerEvent.RemoveEventHandler(null, GetHandler(WarningHandlerEvent, System.Console.WriteLine, GetParameters(WarningHandlerEvent)));
                    WarningHandlerEvent.AddEventHandler(null, Delegate.CreateDelegate(WarningHandlerEvent.EventHandlerType, null, new Action<string>(MelonModLogger.LogWarning).Method));
                }

                EventInfo ErrorHandlerEvent = LogSupportType.GetEvent("ErrorHandler");
                if (ErrorHandlerEvent != null)
                {
                    ErrorHandlerEvent.RemoveEventHandler(null, GetHandler(ErrorHandlerEvent, System.Console.WriteLine, GetParameters(ErrorHandlerEvent)));
                    ErrorHandlerEvent.AddEventHandler(null, Delegate.CreateDelegate(ErrorHandlerEvent.EventHandlerType, null, new Action<string>(MelonModLogger.LogError).Method));
                }

                if (Imports.IsDebugMode())
                {
                    EventInfo TraceHandlerEvent = LogSupportType.GetEvent("TraceHandler");
                    if (TraceHandlerEvent != null)
                        TraceHandlerEvent.AddEventHandler(null, Delegate.CreateDelegate(TraceHandlerEvent.EventHandlerType, null, new Action<string>(MelonModLogger.LogWarning).Method));
                }
            }
        }

        private static ParameterExpression[] GetParameters(EventInfo eventInfo) => eventInfo.EventHandlerType.GetMethod("Invoke").GetParameters().Select(parameter => Expression.Parameter(parameter.ParameterType, parameter.Name)).ToArray();
        private static Delegate GetHandler(EventInfo eventInfo, Action action, ParameterExpression[] parameters) => Expression.Lambda(eventInfo.EventHandlerType, Expression.Call(Expression.Constant(action), "Invoke", Type.EmptyTypes), parameters).Compile();
    }
}
