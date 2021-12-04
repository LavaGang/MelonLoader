using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Harmony
{
	public delegate object FastInvokeHandler(object target, object[] paramters);
	public class MethodInvoker
	{
		public static FastInvokeHandler GetHandler(DynamicMethod methodInfo, Module module) =>
			ConvertFastInvokeHandler(HarmonyLib.MethodInvoker.GetHandler(methodInfo));
		public static FastInvokeHandler GetHandler(MethodInfo methodInfo) =>
			ConvertFastInvokeHandler(HarmonyLib.MethodInvoker.GetHandler(methodInfo));
		private static FastInvokeHandler ConvertFastInvokeHandler(HarmonyLib.FastInvokeHandler sourceDelegate) =>
			(FastInvokeHandler)Delegate.CreateDelegate(typeof(FastInvokeHandler), sourceDelegate.Target, sourceDelegate.Method);
	}
}