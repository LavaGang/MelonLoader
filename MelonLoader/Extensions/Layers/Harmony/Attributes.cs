using System;
using System.Collections.Generic;

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
		public HarmonyPatch(Type declaringType) => info.declaringType = declaringType;
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type declaringType, Type[] argumentTypes)
		{
			info.declaringType = declaringType;
			info.argumentTypes = argumentTypes;
		}
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type declaringType, string methodName)
		{
			info.declaringType = declaringType;
			info.methodName = methodName;
		}
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type declaringType, string methodName, params Type[] argumentTypes)
		{
			info.declaringType = declaringType;
			info.methodName = methodName;
			info.argumentTypes = argumentTypes;
		}
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type declaringType, string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations)
		{
			info.declaringType = declaringType;
			info.methodName = methodName;
			ParseSpecialArguments(argumentTypes, argumentVariations);
		}
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type declaringType, MethodType methodType)
		{
			info.declaringType = declaringType;
			info.methodType = (HarmonyLib.MethodType)methodType;
		}
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type declaringType, MethodType methodType, params Type[] argumentTypes)
		{
			info.declaringType = declaringType;
			info.methodType = (HarmonyLib.MethodType)methodType;
			info.argumentTypes = argumentTypes;
		}
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type declaringType, MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations)
		{
			info.declaringType = declaringType;
			info.methodType = (HarmonyLib.MethodType)methodType;
			ParseSpecialArguments(argumentTypes, argumentVariations);
		}
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type declaringType, string propertyName, MethodType methodType)
		{
			info.declaringType = declaringType;
			info.methodName = propertyName;
			info.methodType = (HarmonyLib.MethodType)methodType;
		}
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(string methodName) => info.methodName = methodName;
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(string methodName, params Type[] argumentTypes)
		{
			info.methodName = methodName;
			info.argumentTypes = argumentTypes;
		}
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(string methodName, Type[] argumentTypes, ArgumentType[] argumentVariations)
		{
			info.methodName = methodName;
			ParseSpecialArguments(argumentTypes, argumentVariations);
		}
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(string propertyName, MethodType methodType)
		{
			info.methodName = propertyName;
			info.methodType = (HarmonyLib.MethodType)methodType;
		}
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(MethodType methodType) => info.methodType = (HarmonyLib.MethodType)methodType;
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(MethodType methodType, params Type[] argumentTypes)
		{
			info.methodType = (HarmonyLib.MethodType)methodType;
			info.argumentTypes = argumentTypes;
		}
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(MethodType methodType, Type[] argumentTypes, ArgumentType[] argumentVariations)
		{
			info.methodType = (HarmonyLib.MethodType)methodType;
			ParseSpecialArguments(argumentTypes, argumentVariations);
		}
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type[] argumentTypes) => info.argumentTypes = argumentTypes;
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(Type[] argumentTypes, ArgumentType[] argumentVariations) => ParseSpecialArguments(argumentTypes, argumentVariations);
		[Obsolete("Harmony.HarmonyPatch is Only Here for Compatibility Reasons. Please use HarmonyLib.HarmonyPatch instead.")]
		public HarmonyPatch(string propertyName, PropertyMethod type)
		{
			info.methodName = propertyName;
			info.methodType = type == PropertyMethod.Getter ? HarmonyLib.MethodType.Getter : HarmonyLib.MethodType.Setter;
		}

		private void ParseSpecialArguments(Type[] argumentTypes, ArgumentType[] argumentVariations)
		{
			if (argumentVariations == null || argumentVariations.Length == 0)
			{
				info.argumentTypes = argumentTypes;
				return;
			}

			if (argumentTypes.Length < argumentVariations.Length)
				throw new ArgumentException("argumentVariations contains more elements than argumentTypes", nameof(argumentVariations));

			var types = new List<Type>();
			for (var i = 0; i < argumentTypes.Length; i++)
			{
				var type = argumentTypes[i];
				switch (argumentVariations[i])
				{
					case ArgumentType.Ref:
					case ArgumentType.Out:
						type = type.MakeByRefType();
						break;
					case ArgumentType.Pointer:
						type = type.MakePointerType();
						break;
				}
				types.Add(type);
			}
			info.argumentTypes = types.ToArray();
		}
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