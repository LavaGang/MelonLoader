using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
#pragma warning disable 0108
#pragma warning disable 0618

namespace Harmony
{
	public class HarmonyInstance : HarmonyLib.Harmony
	{
		[Obsolete("Harmony.HarmonyInstance is obsolete. Please use HarmonyLib.Harmony instead.")]
		public HarmonyInstance(string id) : base(id) { }

		[Obsolete("Harmony.HarmonyInstance.Create is obsolete. Please use HarmonyLib.Harmony.Create instead.")]
		public static HarmonyInstance Create(string id)
		{
			if (id == null) throw new Exception("id cannot be null");
			return new HarmonyInstance(id);
		}

		public void PatchAll() => base.PatchAll();
		public void PatchAll(Assembly assembly) => base.PatchAll(assembly);

		[Obsolete("Harmony.HarmonyInstance.Patch is obsolete. Please use HarmonyLib.Harmony.Patch instead.")]
		public DynamicMethod Patch(MethodBase original, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null)
		{
			base.Patch(original, prefix, postfix, transpiler);
			return null;
		}

		public void UnpatchAll(string harmonyID = null) => base.UnpatchAll(harmonyID);
        public void Unpatch(MethodBase original, HarmonyPatchType type, string harmonyID = null) => Unpatch(original, (HarmonyLib.HarmonyPatchType)type, harmonyID);
        public void Unpatch(MethodBase original, MethodInfo patch) => base.Unpatch(original, patch);

		public bool HasAnyPatches(string harmonyID) =>
			GetPatchedMethods()
				.Select(original => GetPatchInfo(original))
				.Any(info => info.Owners.Contains(harmonyID));

		public IEnumerable<MethodBase> GetPatchedMethods() => base.GetPatchedMethods();
		public Dictionary<string, Version> VersionInfo(out Version currentVersion) { currentVersion = null; return null; }
	}
}
