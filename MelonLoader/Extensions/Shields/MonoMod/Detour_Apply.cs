using System;
using System.Reflection;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace MelonLoader.PatchShields.MonoMod
{
    class Detour_Apply
    {
		private static RuntimeILReferenceBag.FastDelegateInvokers.Action<Detour> Original = null;

		internal static void Install()
        {
			try
			{
				IDetour detour = new Detour(
					typeof(Detour).GetMethod("Apply", BindingFlags.Public | BindingFlags.Instance),
					typeof(Detour_Apply).GetMethod("New", BindingFlags.NonPublic | BindingFlags.Static)
				);

				Original = detour.GenerateTrampoline<RuntimeILReferenceBag.FastDelegateInvokers.Action<Detour>>();
			}
			catch (Exception ex) { PatchShield.LogException(ex); }
		}

		private static void New(Detour self)
		{
			if ((self.Method.DeclaringType.Assembly.GetCustomAttributes(typeof(PatchShield), false).Length > 0)
				|| (self.Method.DeclaringType.GetCustomAttributes(typeof(PatchShield), false).Length > 0)
				|| (self.Method.GetCustomAttributes(typeof(PatchShield), false).Length > 0))
				return;
			Original(self);
		}
	}
}
