using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using HarmonyLib.Tools;
using MonoMod.RuntimeDetour;

namespace HarmonyLib.Internal.RuntimeFixes
{
    /// <summary>
    /// Patching methods potentially messes up the stack.
    /// Especially calls to GetExecutingAssembly won't turn in correct methods
    /// </summary>
    internal static class StackTraceFixes
    {
        private static bool _applied;

        private static readonly Dictionary<MethodBase, MethodBase> RealMethodMap =
            new Dictionary<MethodBase, MethodBase>();

        private static Func<Assembly> _realGetAss;
        private static Func<StackFrame, MethodBase> _origGetMethod;
        private static Action<object> _origRefresh;

        public static void Install()
        {
            if (_applied)
                return;

            try
            {
	            var refreshDet = new Detour(AccessTools.Method(AccessTools.Inner(typeof(ILHook), "Context"), "Refresh"),
		            AccessTools.Method(typeof(StackTraceFixes), nameof(OnILChainRefresh)));
	            _origRefresh = refreshDet.GenerateTrampoline<Action<object>>();

	            var getMethodDet = new Detour(AccessTools.Method(typeof(StackFrame), nameof(StackFrame.GetMethod)),
		            AccessTools.Method(typeof(StackTraceFixes), nameof(GetMethodFix)));
	            _origGetMethod = getMethodDet.GenerateTrampoline<Func<StackFrame, MethodBase>>();

	            var nat = new NativeDetour(AccessTools.Method(typeof(Assembly), nameof(Assembly.GetExecutingAssembly)),
		            AccessTools.Method(typeof(StackTraceFixes), nameof(GetAssemblyFix)));
	            _realGetAss = nat.GenerateTrampoline<Func<Assembly>>();
            }
            catch (Exception e)
            {
	            Logger.LogText(Logger.LogChannel.Error, $"Failed to apply stack trace fix: ({e.GetType().FullName}) {e.Message}");
            }
            _applied = true;
        }

        // Fix StackFrame's GetMethod to map patched method to unpatched one instead
        private static MethodBase GetMethodFix(StackFrame self)
        {
            var m = _origGetMethod(self);
            if (m == null)
                return null;
            lock (RealMethodMap)
            {
                return RealMethodMap.TryGetValue(m, out var real) ? real : m;
            }
        }

        // We need to force GetExecutingAssembly make use of stack trace
        // This is to fix cases where calling assembly is actually the patch
        // This solves issues with code where it uses the method to get current filepath etc
        private static Assembly GetAssemblyFix()
        {
            return new StackFrame(1).GetMethod()?.Module.Assembly ?? _realGetAss();
        }

        // Helper to save the detour info after patch is complete
        private static void OnILChainRefresh(object self)
        {
            _origRefresh(self);

            if (!(AccessTools.Field(self.GetType(), "Detour").GetValue(self) is Detour detour))
                return;

            lock (RealMethodMap)
            {
                RealMethodMap[detour.Target] = detour.Method;
            }
        }
    }
}
