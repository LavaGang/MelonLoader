using System;
using System.Collections.Generic;
using System.Reflection;

namespace Harmony
{
	[Obsolete("Harmony.GeneralExtensions is Only Here for Compatibility Reasons. Please use HarmonyLib.GeneralExtensions instead.")]
	public static class GeneralExtensions
	{
		[Obsolete("Harmony.GeneralExtensions.Join is Only Here for Compatibility Reasons. Please use HarmonyLib.GeneralExtensions.Join instead.")]
		public static string Join<T>(this IEnumerable<T> enumeration, Func<T, string> converter = null, string delimiter = ", ") => HarmonyLib.GeneralExtensions.Join(enumeration, converter, delimiter);
		[Obsolete("Harmony.GeneralExtensions.Description is Only Here for Compatibility Reasons. Please use HarmonyLib.GeneralExtensions.Description instead.")]
		public static string Description(this Type[] parameters) => HarmonyLib.GeneralExtensions.Description(parameters);
		[Obsolete("Harmony.GeneralExtensions.FullDescription is Only Here for Compatibility Reasons. Please use HarmonyLib.GeneralExtensions.FullDescription instead.")]
		public static string FullDescription(this MethodBase method) => HarmonyLib.GeneralExtensions.FullDescription(method);
		[Obsolete("Harmony.GeneralExtensions.Types is Only Here for Compatibility Reasons. Please use HarmonyLib.GeneralExtensions.Types instead.")]
		public static Type[] Types(this ParameterInfo[] pinfo) => HarmonyLib.GeneralExtensions.Types(pinfo);
		[Obsolete("Harmony.GeneralExtensions.GetValueSafe is Only Here for Compatibility Reasons. Please use HarmonyLib.GeneralExtensions.GetValueSafe instead.")]
		public static T GetValueSafe<S, T>(this Dictionary<S, T> dictionary, S key) => HarmonyLib.GeneralExtensions.GetValueSafe(dictionary, key);
		[Obsolete("Harmony.GeneralExtensions.GetTypedValue is Only Here for Compatibility Reasons. Please use HarmonyLib.GeneralExtensions.GetTypedValue instead.")]
		public static T GetTypedValue<T>(this Dictionary<string, object> dictionary, string key) => HarmonyLib.GeneralExtensions.GetTypedValue<T>(dictionary, key);
	}

	[Obsolete("Harmony.CollectionExtensions is Only Here for Compatibility Reasons. Please use HarmonyLib.CollectionExtensions instead.")]
	public static class CollectionExtensions
	{
		[Obsolete("Harmony.CollectionExtensions.Do is Only Here for Compatibility Reasons. Please use HarmonyLib.CollectionExtensions.Do instead.")]
		public static void Do<T>(this IEnumerable<T> sequence, Action<T> action) => HarmonyLib.CollectionExtensions.Do(sequence, action);
		[Obsolete("Harmony.CollectionExtensions.DoIf is Only Here for Compatibility Reasons. Please use HarmonyLib.CollectionExtensions.DoIf instead.")]
		public static void DoIf<T>(this IEnumerable<T> sequence, Func<T, bool> condition, Action<T> action) => HarmonyLib.CollectionExtensions.DoIf(sequence, condition, action);
		[Obsolete("Harmony.CollectionExtensions.Add is Only Here for Compatibility Reasons. Please use HarmonyLib.CollectionExtensions.Add instead.")]
		public static IEnumerable<T> Add<T>(this IEnumerable<T> sequence, T item) => HarmonyLib.CollectionExtensions.AddItem(sequence, item);
		[Obsolete("Harmony.CollectionExtensions.AddRangeToArray is Only Here for Compatibility Reasons. Please use HarmonyLib.CollectionExtensions.AddRangeToArray instead.")]
		public static T[] AddRangeToArray<T>(this T[] sequence, T[] items) => HarmonyLib.CollectionExtensions.AddRangeToArray(sequence, items);
		[Obsolete("Harmony.CollectionExtensions.AddToArray is Only Here for Compatibility Reasons. Please use HarmonyLib.CollectionExtensions.AddToArray instead.")]
		public static T[] AddToArray<T>(this T[] sequence, T item) => HarmonyLib.CollectionExtensions.AddToArray(sequence, item);
	}
}