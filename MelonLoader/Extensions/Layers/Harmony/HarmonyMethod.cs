using System;
using System.Collections.Generic;
using System.Reflection;
#pragma warning disable 0618

namespace Harmony
{
	public class HarmonyMethod : HarmonyLib.HarmonyMethod
	{
		public int prioritiy = -1;

		[Obsolete("Harmony.HarmonyMethod is obsolete. Please use HarmonyLib.HarmonyMethod instead.")]
		public HarmonyMethod() : base() { }
		[Obsolete("Harmony.HarmonyMethod is obsolete. Please use HarmonyLib.HarmonyMethod instead.")]
		public HarmonyMethod(MethodInfo method) : base(method) { }
		[Obsolete("Harmony.HarmonyMethod is obsolete. Please use HarmonyLib.HarmonyMethod instead.")]
		public HarmonyMethod(Type type, string name, Type[] parameters = null) : base(type, name, parameters) { }

		public static HarmonyMethod Merge(List<HarmonyMethod> attributes)
		{
			List<HarmonyLib.HarmonyMethod> returnval = new List<HarmonyLib.HarmonyMethod>();
			foreach (HarmonyMethod x in attributes)
				returnval.Add(x);
            return (HarmonyMethod)Merge(returnval);
		}
		public override string ToString() => base.ToString();
	}

	public static class HarmonyMethodExtensions
	{
		public static void CopyTo(this HarmonyMethod from, HarmonyMethod to) => HarmonyLib.HarmonyMethodExtensions.CopyTo(from, to);
        public static HarmonyMethod Clone(this HarmonyMethod original) => (HarmonyMethod)HarmonyLib.HarmonyMethodExtensions.Clone(original);
        public static HarmonyMethod Merge(this HarmonyMethod master, HarmonyMethod detail) => (HarmonyMethod)HarmonyLib.HarmonyMethodExtensions.Merge(master, detail);
		public static List<HarmonyMethod> GetHarmonyMethods(this Type type)
        {
			List<HarmonyLib.HarmonyMethod> methodtbl = HarmonyLib.HarmonyMethodExtensions.GetFromType(type);
			List<HarmonyMethod> returnval = new List<HarmonyMethod>();
			foreach (HarmonyLib.HarmonyMethod x in methodtbl)
				returnval.Add((HarmonyMethod)x);
			return returnval;
		}

		public static List<HarmonyMethod> GetHarmonyMethods(this MethodBase method)
		{
			List<HarmonyLib.HarmonyMethod> methodtbl = HarmonyLib.HarmonyMethodExtensions.GetFromMethod(method);
			List<HarmonyMethod> returnval = new List<HarmonyMethod>();
			foreach (HarmonyLib.HarmonyMethod x in methodtbl)
				returnval.Add((HarmonyMethod)x);
			return returnval;
		}
	}
}