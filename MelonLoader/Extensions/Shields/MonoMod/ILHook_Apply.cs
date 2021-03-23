using System;
using System.Reflection;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace MelonLoader.PatchShields.MonoMod
{
    class ILHook_Apply
	{
		private static RuntimeILReferenceBag.FastDelegateInvokers.Action<ILHook> Original = null;

		internal static void Install()
        {
			try
			{
				IDetour detour = new Detour(
					typeof(ILHook).GetMethod("Apply", BindingFlags.Public | BindingFlags.Instance),
					typeof(ILHook_Apply).GetMethod("New", BindingFlags.NonPublic | BindingFlags.Static)
				);

				Original = detour.GenerateTrampoline<RuntimeILReferenceBag.FastDelegateInvokers.Action<ILHook>>();
			}
			catch (Exception ex) { PatchShield.LogException(ex); }
		}

		private static void New(ILHook self)
		{
			if ((self.Method.DeclaringType.Assembly.GetCustomAttributes(typeof(PatchShield), false).Length > 0)
				|| (self.Method.DeclaringType.GetCustomAttributes(typeof(PatchShield), false).Length > 0)
				|| (self.Method.GetCustomAttributes(typeof(PatchShield), false).Length > 0))
				return;
			Original(self);
		}
	}
}
