using System.Diagnostics;
using MelonLoader;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace ModHelper;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ModLogger
{
    public static void Debug(object obj)
    {
        var frame = new StackTrace().GetFrame(1);
        var className = frame.GetMethod().ReflectedType.Name;
        var methodName = frame.GetMethod().Name;
        AddLog(className, methodName, obj);
    }

    public static void AddLog(string className, string methodName, object obj)
    {
        MelonLogger.Msg($"[{className}:{methodName}]: {obj}");
    }
}