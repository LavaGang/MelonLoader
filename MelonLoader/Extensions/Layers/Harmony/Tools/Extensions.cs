using System;
using System.Collections.Generic;
using System.Reflection;

namespace Harmony
{
	public static class GeneralExtensions
	{
		public static string Join<T>(this IEnumerable<T> enumeration, Func<T, string> converter = null, string delimiter = ", ") => HarmonyLib.GeneralExtensions.Join(enumeration, converter, delimiter);
		public static string Description(this Type[] parameters) => HarmonyLib.GeneralExtensions.Description(parameters);
		public static string FullDescription(this MethodBase method) => HarmonyLib.GeneralExtensions.FullDescription(method);
		public static Type[] Types(this ParameterInfo[] pinfo) => HarmonyLib.GeneralExtensions.Types(pinfo);
		public static T GetValueSafe<S, T>(this Dictionary<S, T> dictionary, S key) => HarmonyLib.GeneralExtensions.GetValueSafe(dictionary, key);
		public static T GetTypedValue<T>(this Dictionary<string, object> dictionary, string key) => HarmonyLib.GeneralExtensions.GetTypedValue<T>(dictionary, key);
	}

	public static class CollectionExtensions
	{
		public static void Do<T>(this IEnumerable<T> sequence, Action<T> action) => HarmonyLib.CollectionExtensions.Do(sequence, action);
		public static void DoIf<T>(this IEnumerable<T> sequence, Func<T, bool> condition, Action<T> action) => HarmonyLib.CollectionExtensions.DoIf(sequence, condition, action);
		public static IEnumerable<T> Add<T>(this IEnumerable<T> sequence, T item) => HarmonyLib.CollectionExtensions.AddItem(sequence, item);
		public static T[] AddRangeToArray<T>(this T[] sequence, T[] items) => HarmonyLib.CollectionExtensions.AddRangeToArray(sequence, items);
		public static T[] AddToArray<T>(this T[] sequence, T item) => HarmonyLib.CollectionExtensions.AddToArray(sequence, item);
	}
}