using System;
using System.Collections.Generic;
using System.Reflection;

namespace Harmony
{
	[Obsolete("Harmony.AccessTools is Only Here for Compatibility Reasons. Please use HarmonyLib.AccessTools instead.")]
	public static class AccessTools
	{
		public static BindingFlags all = HarmonyLib.AccessTools.all;
		public static Type TypeByName(string name) => HarmonyLib.AccessTools.TypeByName(name);
        public static T FindIncludingBaseTypes<T>(Type type, Func<Type, T> action) where T : class => HarmonyLib.AccessTools.FindIncludingBaseTypes<T>(type, action);
        public static T FindIncludingInnerTypes<T>(Type type, Func<Type, T> action) where T : class => HarmonyLib.AccessTools.FindIncludingInnerTypes<T>(type, action);
		public static FieldInfo Field(Type type, string name) => HarmonyLib.AccessTools.Field(type, name);
		public static FieldInfo Field(Type type, int idx) => HarmonyLib.AccessTools.DeclaredField(type, idx);
		public static PropertyInfo DeclaredProperty(Type type, string name) => HarmonyLib.AccessTools.DeclaredProperty(type, name);
		public static PropertyInfo Property(Type type, string name) => HarmonyLib.AccessTools.Property(type, name);
		public static MethodInfo DeclaredMethod(Type type, string name, Type[] parameters = null, Type[] generics = null) => HarmonyLib.AccessTools.DeclaredMethod(type, name, parameters, generics);
		public static MethodInfo Method(Type type, string name, Type[] parameters = null, Type[] generics = null) => HarmonyLib.AccessTools.Method(type, name, parameters, generics);
		public static MethodInfo Method(string typeColonMethodname, Type[] parameters = null, Type[] generics = null) => HarmonyLib.AccessTools.Method(typeColonMethodname, parameters, generics);
		public static List<string> GetMethodNames(Type type) => HarmonyLib.AccessTools.GetMethodNames(type);
		public static List<string> GetMethodNames(object instance) => HarmonyLib.AccessTools.GetMethodNames(instance);
		public static ConstructorInfo DeclaredConstructor(Type type, Type[] parameters = null) => HarmonyLib.AccessTools.DeclaredConstructor(type, parameters);
		public static ConstructorInfo Constructor(Type type, Type[] parameters = null) => HarmonyLib.AccessTools.Constructor(type, parameters);
		public static List<ConstructorInfo> GetDeclaredConstructors(Type type) => HarmonyLib.AccessTools.GetDeclaredConstructors(type);
		public static List<MethodInfo> GetDeclaredMethods(Type type) => HarmonyLib.AccessTools.GetDeclaredMethods(type);
		public static List<PropertyInfo> GetDeclaredProperties(Type type) => HarmonyLib.AccessTools.GetDeclaredProperties(type);
		public static List<FieldInfo> GetDeclaredFields(Type type) => HarmonyLib.AccessTools.GetDeclaredFields(type);
		public static Type GetReturnedType(MethodBase method) => HarmonyLib.AccessTools.GetReturnedType(method);
		public static Type Inner(Type type, string name) => HarmonyLib.AccessTools.Inner(type, name);
		public static Type FirstInner(Type type, Func<Type, bool> predicate) => HarmonyLib.AccessTools.FirstInner(type, predicate);
		public static MethodInfo FirstMethod(Type type, Func<MethodInfo, bool> predicate) => HarmonyLib.AccessTools.FirstMethod(type, predicate);
		public static ConstructorInfo FirstConstructor(Type type, Func<ConstructorInfo, bool> predicate) => HarmonyLib.AccessTools.FirstConstructor(type, predicate);
		public static PropertyInfo FirstProperty(Type type, Func<PropertyInfo, bool> predicate) => HarmonyLib.AccessTools.FirstProperty(type, predicate);
		public static Type[] GetTypes(object[] parameters) => HarmonyLib.AccessTools.GetTypes(parameters);
		public static List<string> GetFieldNames(Type type) => HarmonyLib.AccessTools.GetFieldNames(type);
		public static List<string> GetFieldNames(object instance) => HarmonyLib.AccessTools.GetFieldNames(instance);
		public static List<string> GetPropertyNames(Type type) => HarmonyLib.AccessTools.GetPropertyNames(type);
		public static List<string> GetPropertyNames(object instance) => HarmonyLib.AccessTools.GetPropertyNames(instance);
		public delegate ref U FieldRef<T, U>(T obj);
		public static FieldRef<T, U> FieldRefAccess<T, U>(string fieldName) => ConvertFieldRef(HarmonyLib.AccessTools.FieldRefAccess<T, U>(fieldName));
		public static ref U FieldRefAccess<T, U>(T instance, string fieldName) => ref FieldRefAccess<T, U>(fieldName)(instance);
		private static FieldRef<T, U> ConvertFieldRef<T, U>(HarmonyLib.AccessTools.FieldRef<T, U> sourceDelegate) =>
			(FieldRef<T, U>)Delegate.CreateDelegate(typeof(FieldRef<T, U>), sourceDelegate.Target, sourceDelegate.Method);
		public static void ThrowMissingMemberException(Type type, params string[] names) => HarmonyLib.AccessTools.ThrowMissingMemberException(type, names);
		public static object GetDefaultValue(Type type) => HarmonyLib.AccessTools.GetDefaultValue(type);
		public static object CreateInstance(Type type) => HarmonyLib.AccessTools.CreateInstance(type);
		public static bool IsStruct(Type type) => HarmonyLib.AccessTools.IsStruct(type);
		public static bool IsClass(Type type) => HarmonyLib.AccessTools.IsClass(type);
		public static bool IsValue(Type type) => HarmonyLib.AccessTools.IsValue(type);
		public static bool IsVoid(Type type) => HarmonyLib.AccessTools.IsVoid(type);
	}
}