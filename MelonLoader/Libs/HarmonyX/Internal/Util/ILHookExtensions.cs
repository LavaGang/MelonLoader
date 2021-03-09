using System;
using System.Reflection;
using System.Reflection.Emit;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace HarmonyLib.Internal.Util
{
	internal static class ILHookExtensions
	{
		private static readonly MethodInfo IsAppliedSetter =
			AccessTools.PropertySetter(typeof(ILHook), nameof(ILHook.IsApplied));

		private static Func<ILHook, Detour> GetAppliedDetour;

		static ILHookExtensions()
		{
			var detourGetter = new DynamicMethodDefinition("GetDetour", typeof(Detour), new[] {typeof(ILHook)});
			var il = detourGetter.GetILGenerator();
			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Call, AccessTools.PropertyGetter(typeof(ILHook), "_Ctx"));
			il.Emit(OpCodes.Ldfld, AccessTools.Field(AccessTools.Inner(typeof(ILHook), "Context"), "Detour"));
			il.Emit(OpCodes.Ret);
			GetAppliedDetour = detourGetter.Generate().CreateDelegate<Func<ILHook, Detour>>() as Func<ILHook, Detour>;
		}

		public static ILHook MarkApply(this ILHook hook, bool apply)
		{
			if (hook == null)
				return null;

			// By manually resetting IsApplied we make it possible to rerun the manipulator
			IsAppliedSetter.Invoke(hook, new object[] {!apply});
			return hook;
		}

		public static MethodBase GetCurrentTarget(this ILHook hook)
		{
			var detour = GetAppliedDetour(hook);
			return detour.Target;
		}
	}
}
