using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MelonLoader
{
    internal class UnhollowerLogging
    {
        internal static void FixEvents()
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
    }
}
