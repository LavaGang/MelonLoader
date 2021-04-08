using System.Reflection;
using MonoMod.RuntimeDetour;

namespace MelonLoader.PatchShields.MonoMod
{
    class Detour_OnDetour
	{
		internal static void Install() => Detour.OnDetour += New;
		private static bool New(Detour detour, MethodBase originalMethod, MethodBase patchMethod) =>
				(originalMethod.DeclaringType.Assembly.GetCustomAttributes(typeof(PatchShield), false).Length <= 0)
				&& (originalMethod.DeclaringType.GetCustomAttributes(typeof(PatchShield), false).Length <= 0)
				&& (originalMethod.GetCustomAttributes(typeof(PatchShield), false).Length <= 0);
	}
}