using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Mono.Cecil;
using MonoMod.Utils;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using MethodImplAttributes = Mono.Cecil.MethodImplAttributes;
using TypeAttributes = Mono.Cecil.TypeAttributes;

namespace HarmonyLib
{
	/// <summary>A factory to create delegate types</summary>
	public class DelegateTypeFactory
	{
		private static int counter;
		private static readonly Dictionary<MethodInfo, List<DelegateEntry>> TypeCache =
			new Dictionary<MethodInfo, List<DelegateEntry>>();
		private static readonly MethodBase CallingConvAttr = AccessTools.Constructor(
			typeof(UnmanagedFunctionPointerAttribute),
			new[] {typeof(CallingConvention)});

		/// <summary>
		///    Instance for the delegate type factory
		/// </summary>
		/// <remarks>
		///    Exists for API compatibility with Harmony
		/// </remarks>
		public static readonly DelegateTypeFactory instance = new DelegateTypeFactory();

		/// <summary>
		///    Creates a delegate type for a method
		/// </summary>
		/// <param name="returnType">Type of the return value</param>
		/// <param name="argTypes">Types of the arguments</param>
		/// <returns>The new delegate type for the given type info</returns>
		public Type CreateDelegateType(Type returnType, Type[] argTypes)
		{
			return CreateDelegateType(returnType, argTypes, null);
		}

		/// <summary>
		///    Creates a delegate type for a method
		/// </summary>
		/// <param name="returnType">Type of the return value</param>
		/// <param name="argTypes">Types of the arguments</param>
		/// <param name="convention">Calling convention. If specified, adds <see cref="UnmanagedFunctionPointerAttribute"/> to the delegate type</param>
		/// <returns>The new delegate type for the given type info</returns>
		public Type CreateDelegateType(Type returnType, Type[] argTypes, CallingConvention? convention)
		{
			counter++;
			var assembly = AssemblyDefinition.CreateAssembly(
				new AssemblyNameDefinition($"HarmonyDTFAssembly{counter}", new Version(1, 0)),
				$"HarmonyDTFModule{counter}", ModuleKind.Dll);

			var module = assembly.MainModule;
			var dtfType = new TypeDefinition("", $"HarmonyDTFType{counter}",
				TypeAttributes.Sealed | TypeAttributes.Public)
			{
				BaseType = module.ImportReference(typeof(MulticastDelegate))
			};
			module.Types.Add(dtfType);
			if (convention.HasValue)
			{
				var ca = new CustomAttribute(module.ImportReference(CallingConvAttr));
				ca.ConstructorArguments.Add(new CustomAttributeArgument(module.ImportReference(typeof(CallingConvention)),
					convention.Value));
				dtfType.CustomAttributes.Add(ca);
			}

			var ctor = new MethodDefinition(
				".ctor", MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public,
				module.ImportReference(typeof(void))) {ImplAttributes = MethodImplAttributes.CodeTypeMask};
			ctor.Parameters.AddRange(new[]
			{
				new ParameterDefinition(module.ImportReference(typeof(object))),
				new ParameterDefinition(module.ImportReference(typeof(IntPtr)))
			});
			dtfType.Methods.Add(ctor);

			var invokeMethod =
				new MethodDefinition(
					"Invoke", MethodAttributes.HideBySig | MethodAttributes.Virtual | MethodAttributes.Public,
					module.ImportReference(returnType)) {ImplAttributes = MethodImplAttributes.CodeTypeMask};

			invokeMethod.Parameters.AddRange(argTypes.Select(t => new ParameterDefinition(module.ImportReference(t))));
			dtfType.Methods.Add(invokeMethod);

			var loadedAss = ReflectionHelper.Load(assembly.MainModule);
			var delegateType = loadedAss.GetType($"HarmonyDTFType{counter}");
			return delegateType;
		}

		/// <summary>Creates a delegate type for a method</summary>
		/// <param name="method">The method</param>
		/// <returns>The new delegate type</returns>
		public Type CreateDelegateType(MethodInfo method)
		{
			return CreateDelegateType(method, null);
		}

		/// <summary>Creates a delegate type for a method</summary>
		/// <param name="method">The method</param>
		/// <param name="convention">Calling convention. If specified, adds <see cref="UnmanagedFunctionPointerAttribute"/> to the delegate type.</param>
		/// <returns>The new delegate type</returns>
		public Type CreateDelegateType(MethodInfo method, CallingConvention? convention)
		{
			DelegateEntry entry;
			if (TypeCache.TryGetValue(method, out var entries) &&
			    (entry = entries.FirstOrDefault(e => e.callingConvention == convention)) != null)
				return entry.delegateType;

			if (entries == null)
				TypeCache[method] = entries = new List<DelegateEntry>();

			entry = new DelegateEntry
			{
				delegateType = CreateDelegateType(method.ReturnType,
					method.GetParameters().Types().ToArray(), convention),
				callingConvention = convention
			};

			entries.Add(entry);
			return entry.delegateType;
		}

		private class DelegateEntry
		{
			public CallingConvention? callingConvention;
			public Type delegateType;
		}
	}
}
