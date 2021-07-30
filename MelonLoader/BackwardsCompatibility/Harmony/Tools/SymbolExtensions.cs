using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Harmony
{
	[Obsolete("Harmony.SymbolExtensions is Only Here for Compatibility Reasons. Please use HarmonyLib.SymbolExtensions instead.")]
	public static class SymbolExtensions
	{
		[Obsolete("Harmony.SymbolExtensions.GetMethodInfo is Only Here for Compatibility Reasons. Please use HarmonyLib.SymbolExtensions.GetMethodInfo instead.")]
		public static MethodInfo GetMethodInfo(Expression<Action> expression) => HarmonyLib.SymbolExtensions.GetMethodInfo(expression);
		[Obsolete("Harmony.SymbolExtensions.GetMethodInfo is Only Here for Compatibility Reasons. Please use HarmonyLib.SymbolExtensions.GetMethodInfo instead.")]
		public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression) => GetMethodInfo((LambdaExpression)expression);
		[Obsolete("Harmony.SymbolExtensions.GetMethodInfo is Only Here for Compatibility Reasons. Please use HarmonyLib.SymbolExtensions.GetMethodInfo instead.")]
		public static MethodInfo GetMethodInfo<T, TResult>(Expression<Func<T, TResult>> expression) =>  GetMethodInfo((LambdaExpression)expression);
		[Obsolete("Harmony.SymbolExtensions.GetMethodInfo is Only Here for Compatibility Reasons. Please use HarmonyLib.SymbolExtensions.GetMethodInfo instead.")]
		public static MethodInfo GetMethodInfo(LambdaExpression expression) => HarmonyLib.SymbolExtensions.GetMethodInfo(expression);
	}
}