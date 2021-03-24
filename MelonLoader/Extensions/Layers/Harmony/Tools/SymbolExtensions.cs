using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Harmony
{
	public static class SymbolExtensions
	{
		public static MethodInfo GetMethodInfo(Expression<Action> expression) => HarmonyLib.SymbolExtensions.GetMethodInfo(expression);
		public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression) => GetMethodInfo((LambdaExpression)expression);
		public static MethodInfo GetMethodInfo<T, TResult>(Expression<Func<T, TResult>> expression) =>  GetMethodInfo((LambdaExpression)expression);
		public static MethodInfo GetMethodInfo(LambdaExpression expression) => HarmonyLib.SymbolExtensions.GetMethodInfo(expression);
	}
}