using System;
using System.Reflection;

namespace Harmony
{
	[Obsolete("Harmony.PatchInfoSerialization is Only Here for Compatibility Reasons. Please use HarmonyLib.PatchInfoSerialization instead.")]
	public static class PatchInfoSerialization
	{
		[Obsolete("Harmony.PatchInfoSerialization.Deserialize is Only Here for Compatibility Reasons. Please use HarmonyLib.PatchInfoSerialization.Deserialize instead.")]
		public static PatchInfo Deserialize(byte[] bytes) => (PatchInfo)HarmonyLib.PatchInfoSerialization.Deserialize(bytes);
		[Obsolete("Harmony.PatchInfoSerialization.PriorityComparer is Only Here for Compatibility Reasons. Please use HarmonyLib.PatchInfoSerialization.PriorityComparer instead.")]
		public static int PriorityComparer(object obj, int index, int priority, string[] before, string[] after) => HarmonyLib.PatchInfoSerialization.PriorityComparer(obj, index, priority);
	}

	[Obsolete("Harmony.PatchInfo is Only Here for Compatibility Reasons. Please use HarmonyLib.PatchInfo instead.")]
	[Serializable]
	public class PatchInfo : HarmonyLib.PatchInfo
	{
		[Obsolete("Harmony.PatchInfo.AddPrefix is Only Here for Compatibility Reasons. Please use HarmonyLib.PatchInfo.AddPrefix instead.")]
		public void AddPrefix(MethodInfo patch, string owner, int priority, string[] before, string[] after) => AddPrefixes(owner, new HarmonyLib.HarmonyMethod(patch, priority, before, after, false));
		[Obsolete("Harmony.PatchInfo.AddPostfix is Only Here for Compatibility Reasons. Please use HarmonyLib.PatchInfo.AddPostfix instead.")]
		public void AddPostfix(MethodInfo patch, string owner, int priority, string[] before, string[] after) => AddPostfixes(owner, new HarmonyLib.HarmonyMethod(patch, priority, before, after, false));
		[Obsolete("Harmony.PatchInfo.AddTranspiler is Only Here for Compatibility Reasons. Please use HarmonyLib.PatchInfo.AddTranspiler instead.")]
		public void AddTranspiler(MethodInfo patch, string owner, int priority, string[] before, string[] after) => AddTranspilers(owner, new HarmonyLib.HarmonyMethod(patch, priority, before, after, false));
	}

	[Obsolete("Harmony.Patch is Only Here for Compatibility Reasons. Please use HarmonyLib.Patch instead.")]
	[Serializable]
	public class Patch : IComparable
	{
		readonly public MethodInfo patch;
		private HarmonyLib.Patch patchWrapper;
		[Obsolete("Harmony.Patch is Only Here for Compatibility Reasons. Please use HarmonyLib.Patch instead.")]
		public Patch(MethodInfo patch, int index, string owner, int priority, string[] before, string[] after) 
		{
			this.patch = patch;
			patchWrapper = new HarmonyLib.Patch(patch, index, owner, priority, before, after, false);
		}
		public MethodInfo GetMethod(MethodBase original) => patchWrapper.GetMethod(original);
		public override bool Equals(object obj) => patchWrapper.Equals(obj);
		public int CompareTo(object obj) => patchWrapper.CompareTo(obj);
		public override int GetHashCode() => patchWrapper.GetHashCode();
	}
}