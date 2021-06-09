using System;

namespace Harmony
{
	[Obsolete("Harmony.MethodType is Only Here for Compatibility Reasons. Please use HarmonyLib.MethodType instead.")]
	public enum MethodType
	{
		Normal = HarmonyLib.MethodType.Normal,
		Getter = HarmonyLib.MethodType.Getter,
		Setter = HarmonyLib.MethodType.Setter,
		Constructor = HarmonyLib.MethodType.Constructor,
		StaticConstructor = HarmonyLib.MethodType.StaticConstructor
	}

	[Obsolete("Harmony.PropertyMethod is Only Here for Compatibility Reasons. Please use HarmonyLib.MethodType instead.")]
	public enum PropertyMethod
	{
		Getter = HarmonyLib.MethodType.Getter,
		Setter = HarmonyLib.MethodType.Setter
	}

	[Obsolete("Harmony.ArgumentType is Only Here for Compatibility Reasons. Please use HarmonyLib.ArgumentType instead.")]
	public enum ArgumentType
	{
		Normal = HarmonyLib.ArgumentType.Normal,
		Ref = HarmonyLib.ArgumentType.Ref,
		Out = HarmonyLib.ArgumentType.Out,
		Pointer = HarmonyLib.ArgumentType.Pointer
	}

	[Obsolete("Harmony.HarmonyPatchType is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatchType instead.")]
	public enum HarmonyPatchType
	{
		All = HarmonyLib.HarmonyPatchType.All,
		Prefix = HarmonyLib.HarmonyPatchType.Prefix,
		Postfix = HarmonyLib.HarmonyPatchType.Postfix,
		Transpiler = HarmonyLib.HarmonyPatchType.Transpiler
	}

	[Obsolete("Harmony.HarmonyAttribute is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyAttribute instead.")]
	public class HarmonyAttribute : HarmonyLib.HarmonyAttribute { }

	[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Delegate | AttributeTargets.Method, AllowMultiple = true)]
	public class HarmonyPatch : HarmonyLib.HarmonyPatch
	{
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch() : base() { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type declaringType) : base(declaringType) { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type declaringType, Type[] argumentTypes) : base(declaringType, argumentTypes) { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type declaringType, string methodName) : base(declaringType, methodName) { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type declaringType, string methodName, params Type[] argumentTypes) : base(declaringType, methodName, argumentTypes) { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type declaringType, string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(declaringType, methodName, argumentTypes, Array.ConvertAll(argumentVariations, x => (HarmonyLib.ArgumentType)x)) { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type declaringType, MethodType methodType) : base(declaringType, (HarmonyLib.MethodType)methodType) { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type declaringType, MethodType methodType, params Type[] argumentTypes) : base(declaringType, (HarmonyLib.MethodType)methodType, argumentTypes) { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type declaringType, MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(declaringType, (HarmonyLib.MethodType)methodType, argumentTypes, Array.ConvertAll(argumentVariations, x => (HarmonyLib.ArgumentType)x)) { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type declaringType, string propertyName, MethodType methodType) : base(declaringType, propertyName, (HarmonyLib.MethodType)methodType) { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(string methodName) : base(methodName) { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(string methodName, params Type[] argumentTypes) : base(methodName, argumentTypes) { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations) : base(methodName, argumentTypes, Array.ConvertAll(argumentVariations, x => (HarmonyLib.ArgumentType)x)) { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(string propertyName, MethodType methodType) : base(propertyName, (HarmonyLib.MethodType)methodType) { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(MethodType methodType) : base((HarmonyLib.MethodType)methodType) { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(MethodType methodType, params Type[] argumentTypes) : base((HarmonyLib.MethodType)methodType, argumentTypes) { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations) : base((HarmonyLib.MethodType)methodType, argumentTypes, Array.ConvertAll(argumentVariations, x => (HarmonyLib.ArgumentType)x)) { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type[] argumentTypes) : base(argumentTypes) { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type[] argumentTypes, ArgumentType[] argumentVariations) : base(argumentTypes, Array.ConvertAll(argumentVariations, x => (HarmonyLib.ArgumentType)x)) { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(string propertyName, PropertyMethod type) : base(propertyName, (HarmonyLib.MethodType)type) { }
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(string assemblyQualifiedDeclaringType, string methodName, MethodType methodType, Type[] argumentTypes = null, ArgumentType[] argumentVariations = null) : base(assemblyQualifiedDeclaringType, methodName, (HarmonyLib.MethodType)methodType, argumentTypes, Array.ConvertAll(argumentVariations, x => (HarmonyLib.ArgumentType)x)) { }
	}

	[Obsolete("Harmony.HarmonyPatchAll is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatchAll instead.")]
	[AttributeUsage(AttributeTargets.Class)]
	public class HarmonyPatchAll : HarmonyLib.HarmonyPatchAll { }

	[Obsolete("Harmony.HarmonyPriority is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPriority instead.")]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class HarmonyPriority : HarmonyLib.HarmonyPriority
	{
		[Obsolete("Harmony.HarmonyPriority is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPriority instead.")]
		public HarmonyPriority(int prioritiy) : base(prioritiy) { }
	}

	[Obsolete("Harmony.HarmonyBefore is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyBefore instead.")]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class HarmonyBefore : HarmonyLib.HarmonyBefore
	{
		[Obsolete("Harmony.HarmonyBefore is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyBefore instead.")]
		public HarmonyBefore(params string[] before) : base(before) { }
	}

	[Obsolete("Harmony.HarmonyAfter is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyAfter instead.")]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
	public class HarmonyAfter : HarmonyLib.HarmonyAfter
	{
		[Obsolete("Harmony.HarmonyAfter is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyAfter instead.")]
		public HarmonyAfter(params string[] after) : base(after) { }
	}

	[Obsolete("Harmony.HarmonyPrepare is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPrepare instead.")]
	[AttributeUsage(AttributeTargets.Method)]
	public class HarmonyPrepare : HarmonyLib.HarmonyPrepare { }

	[Obsolete("Harmony.HarmonyCleanup is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyCleanup instead.")]
	[AttributeUsage(AttributeTargets.Method)]
	public class HarmonyCleanup : HarmonyLib.HarmonyCleanup { }

	[Obsolete("Harmony.HarmonyTargetMethod is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyTargetMethod instead.")]
	[AttributeUsage(AttributeTargets.Method)]
	public class HarmonyTargetMethod : HarmonyLib.HarmonyTargetMethod { }

	[Obsolete("Harmony.HarmonyTargetMethods is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyTargetMethods instead.")]
	[AttributeUsage(AttributeTargets.Method)]
	public class HarmonyTargetMethods : HarmonyLib.HarmonyTargetMethods { }

	[Obsolete("Harmony.HarmonyPrefix is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPrefix instead.")]
	[AttributeUsage(AttributeTargets.Method)]
	public class HarmonyPrefix : HarmonyLib.HarmonyPrefix { }

	[Obsolete("Harmony.HarmonyPostfix is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPostfix instead.")]
	[AttributeUsage(AttributeTargets.Method)]
	public class HarmonyPostfix : HarmonyLib.HarmonyPostfix { }

	[Obsolete("Harmony.HarmonyTranspiler is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyTranspiler instead.")]
	[AttributeUsage(AttributeTargets.Method)]
	public class HarmonyTranspiler : HarmonyLib.HarmonyTranspiler { }

	[Obsolete("Harmony.HarmonyArgument is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyArgument instead.")]
	[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
	public class HarmonyArgument : HarmonyLib.HarmonyArgument
	{
		[Obsolete("Harmony.HarmonyArgument is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyArgument instead.")]
		public HarmonyArgument(string originalName) : base(originalName, null) { }
		[Obsolete("Harmony.HarmonyArgument is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyArgument instead.")]
		public HarmonyArgument(int index) : base(index, null) { }
		[Obsolete("Harmony.HarmonyArgument is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyArgument instead.")]
		public HarmonyArgument(string originalName, string newName) : base(originalName, newName) { }
		[Obsolete("Harmony.HarmonyArgument is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyArgument instead.")]
		public HarmonyArgument(int index, string name) : base(index, name) { }
	}
}