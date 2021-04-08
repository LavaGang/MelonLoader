using System.Reflection;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace MelonLoader.PatchShields.MonoMod
{
	class ILHook_OnDetour
	{
		internal static void Install() => ILHook.OnDetour += New;
		private static bool New(ILHook detour, MethodBase originalMethod, ILContext.Manipulator ilmanipulator) =>
				(originalMethod.DeclaringType.Assembly.GetCustomAttributes(typeof(PatchShield), false).Length <= 0)
				&& (originalMethod.DeclaringType.GetCustomAttributes(typeof(PatchShield), false).Length <= 0)
				&& (originalMethod.GetCustomAttributes(typeof(PatchShield), false).Length <= 0);
	}
}