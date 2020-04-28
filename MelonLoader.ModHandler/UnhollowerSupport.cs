using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MelonLoader
{
    internal class UnhollowerSupport
    {
        internal static bool IsGeneratedAssemblyType(Type type) => ((Main.Il2CppObjectBaseType != null) && (type != null) && type.IsSubclassOf(Main.Il2CppObjectBaseType));
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

        internal static void FixLoggerEvents()
        {
            Type LogSupportType = Main.UnhollowerBaseLib.GetType("UnhollowerBaseLib.LogSupport");
            if (LogSupportType != null)
            {
                EventInfo InfoHandlerEvent = LogSupportType.GetEvent("InfoHandler");
                if (InfoHandlerEvent != null)
                {
                    InfoHandlerEvent.RemoveEventHandler(null, GetHandler(InfoHandlerEvent, System.Console.WriteLine, GetParameters(InfoHandlerEvent)));
                    InfoHandlerEvent.AddEventHandler(null, Delegate.CreateDelegate(InfoHandlerEvent.EventHandlerType, null, typeof(MelonModLogger).GetMethods().First(x => (x.Name.Equals("Log") && (x.GetParameters().Count() == 1) && (x.GetParameters()[0].ParameterType == typeof(string))))));
                }

                EventInfo WarningHandlerEvent = LogSupportType.GetEvent("WarningHandler");
                if (WarningHandlerEvent != null)
                {
                    WarningHandlerEvent.RemoveEventHandler(null, GetHandler(WarningHandlerEvent, System.Console.WriteLine, GetParameters(WarningHandlerEvent)));
                    WarningHandlerEvent.AddEventHandler(null, Delegate.CreateDelegate(WarningHandlerEvent.EventHandlerType, null, typeof(MelonModLogger).GetMethods().First(x => (x.Name.Equals("Log") && (x.GetParameters().Count() == 1) && (x.GetParameters()[0].ParameterType == typeof(string))))));
                }

                EventInfo ErrorHandlerEvent = LogSupportType.GetEvent("ErrorHandler");
                if (ErrorHandlerEvent != null)
                {
                    ErrorHandlerEvent.RemoveEventHandler(null, GetHandler(ErrorHandlerEvent, System.Console.WriteLine, GetParameters(ErrorHandlerEvent)));
                    ErrorHandlerEvent.AddEventHandler(null, Delegate.CreateDelegate(ErrorHandlerEvent.EventHandlerType, null, typeof(MelonModLogger).GetMethods().First(x => (x.Name.Equals("LogError") && (x.GetParameters().Count() == 1) && (x.GetParameters()[0].ParameterType == typeof(string))))));
                }
            }
        }

        private static ParameterExpression[] GetParameters(EventInfo eventInfo) => eventInfo.EventHandlerType.GetMethod("Invoke").GetParameters().Select(parameter => Expression.Parameter(parameter.ParameterType)).ToArray();
        private static Delegate GetHandler(EventInfo eventInfo, Action action, ParameterExpression[] parameters) => Expression.Lambda(eventInfo.EventHandlerType, Expression.Call(Expression.Constant(action), "Invoke", Type.EmptyTypes), parameters).Compile();

        private static Type UnhollowerBaseLib_IL2CPP = null;
        private static MethodInfo GetIl2CppClass_Method = null;
        private static MethodInfo GetIl2CppField_Method = null;
        internal static IntPtr GetIl2CppClass(string assemblyName, string namespaze, string className)
        {
            if (Main.UnhollowerBaseLib != null)
            {
                if (GetIl2CppClass_Method == null)
                { 
                    if (UnhollowerBaseLib_IL2CPP == null)
                        UnhollowerBaseLib_IL2CPP = Main.UnhollowerBaseLib.GetType("UnhollowerBaseLib.IL2CPP");
                    if (UnhollowerBaseLib_IL2CPP != null)
                        GetIl2CppClass_Method = UnhollowerBaseLib_IL2CPP.GetMethod("GetIl2CppClass");
                }
                if (GetIl2CppClass_Method != null)
                    return (IntPtr)GetIl2CppClass_Method.Invoke(null, new object[] { assemblyName, namespaze, className });
            }
            return IntPtr.Zero;
        }
        internal static IntPtr GetIl2CppField(IntPtr clazz, string fieldName)
        {
            if (Main.UnhollowerBaseLib != null)
            {
                if (GetIl2CppField_Method == null)
                {
                    if (UnhollowerBaseLib_IL2CPP == null)
                        UnhollowerBaseLib_IL2CPP = Main.UnhollowerBaseLib.GetType("UnhollowerBaseLib.IL2CPP");
                    if (UnhollowerBaseLib_IL2CPP != null)
                        GetIl2CppField_Method = UnhollowerBaseLib_IL2CPP.GetMethod("GetIl2CppField");
                }
                if (GetIl2CppField_Method != null)
                    return (IntPtr)GetIl2CppField_Method.Invoke(null, new object[] { clazz, fieldName });
            }
            return IntPtr.Zero;
        }
    }
}
