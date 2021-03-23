using System;
using System.Reflection;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;

namespace MelonLoader.PatchShields.MonoMod
{
    class Hook_Apply
	{
		private static RuntimeILReferenceBag.FastDelegateInvokers.Action<Hook> Original = null;

		internal static void Install()
        {
			try
			{
				IDetour detour = new Detour(
					typeof(Hook).GetMethod("Apply", BindingFlags.Public | BindingFlags.Instance),
					typeof(Hook_Apply).GetMethod("New", BindingFlags.NonPublic | BindingFlags.Static)
				);

				Original = detour.GenerateTrampoline<RuntimeILReferenceBag.FastDelegateInvokers.Action<Hook>>();
			}
			catch (Exception ex) { PatchShield.LogException(ex); }
		}

		private static void New(Hook self)
		{
			if ((self.Method.DeclaringType.Assembly.GetCustomAttributes(typeof(PatchShield), false).Length > 0)
				|| (self.Method.DeclaringType.GetCustomAttributes(typeof(PatchShield), false).Length > 0)
				|| (self.Method.GetCustomAttributes(typeof(PatchShield), false).Length > 0))
				return;
			Original(self);
		}
	}
}
