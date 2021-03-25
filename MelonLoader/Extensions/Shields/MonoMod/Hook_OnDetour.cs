using System.Reflection;
using MonoMod.RuntimeDetour;

namespace MelonLoader.PatchShields.MonoMod
{
	class Hook_OnDetour
	{
		internal static void Install() => Hook.OnDetour += New;
		private static bool New(Hook detour, MethodBase originalMethod, MethodBase patchMethod, object delegateTarget) =>
				(originalMethod.DeclaringType.Assembly.GetCustomAttributes(typeof(PatchShield), false).Length <= 0)
				&& (originalMethod.DeclaringType.GetCustomAttributes(typeof(PatchShield), false).Length <= 0)
				&& (originalMethod.GetCustomAttributes(typeof(PatchShield), false).Length <= 0);
	}
}