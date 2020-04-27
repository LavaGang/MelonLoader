using Harmony.ILCopying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using MelonLoader;

namespace Harmony
{
	public static class PatchFunctions
	{
		public static void AddPrefix(PatchInfo patchInfo, string owner, HarmonyMethod info)
		{
			if (info == null || info.method == null) return;

			var priority = info.prioritiy == -1 ? Priority.Normal : info.prioritiy;
			var before = info.before ?? new string[0];
			var after = info.after ?? new string[0];

			patchInfo.AddPrefix(info.method, owner, priority, before, after);
		}

		public static void RemovePrefix(PatchInfo patchInfo, string owner)
		{
			patchInfo.RemovePrefix(owner);
		}

		public static void AddPostfix(PatchInfo patchInfo, string owner, HarmonyMethod info)
		{
			if (info == null || info.method == null) return;

			var priority = info.prioritiy == -1 ? Priority.Normal : info.prioritiy;
			var before = info.before ?? new string[0];
			var after = info.after ?? new string[0];

			patchInfo.AddPostfix(info.method, owner, priority, before, after);
		}

		public static void RemovePostfix(PatchInfo patchInfo, string owner)
		{
			patchInfo.RemovePostfix(owner);
		}

		public static void AddTranspiler(PatchInfo patchInfo, string owner, HarmonyMethod info)
		{
			if (info == null || info.method == null) return;

			var priority = info.prioritiy == -1 ? Priority.Normal : info.prioritiy;
			var before = info.before ?? new string[0];
			var after = info.after ?? new string[0];

			patchInfo.AddTranspiler(info.method, owner, priority, before, after);
		}

		public static void RemoveTranspiler(PatchInfo patchInfo, string owner)
		{
			patchInfo.RemoveTranspiler(owner);
		}

		public static void RemovePatch(PatchInfo patchInfo, MethodInfo patch)
		{
			patchInfo.RemovePatch(patch);
		}

		// pass in a generator that will create local variables for the returned instructions
		//
		public static List<ILInstruction> GetInstructions(ILGenerator generator, MethodBase method)
		{
			return MethodBodyReader.GetInstructions(generator, method);
		}

		public static List<MethodInfo> GetSortedPatchMethods(MethodBase original, Patch[] patches)
		{
			return patches
				.Where(p => p.patch != null)
				.OrderBy(p => p)
				.Select(p => p.GetMethod(original))
				.ToList();
		}

		public static DynamicMethod UpdateWrapper(MethodBase original, PatchInfo patchInfo, string instanceID)
		{
			var sortedPrefixes = GetSortedPatchMethods(original, patchInfo.prefixes);
			var sortedPostfixes = GetSortedPatchMethods(original, patchInfo.postfixes);
			var sortedTranspilers = GetSortedPatchMethods(original, patchInfo.transpilers);

			var replacement = MethodPatcher.CreatePatchedMethod(original, instanceID, sortedPrefixes, sortedPostfixes, sortedTranspilers);
			if (replacement == null) throw new MissingMethodException("Cannot create dynamic replacement for " + original.FullDescription());

			var errorString = Memory.DetourMethod(original, replacement);
			if (errorString != null)
				throw new FormatException("Method " + original.FullDescription() + " cannot be patched. Reason: " + errorString);

			if (IsIl2CppType(original.DeclaringType))
			{
				var il2CppShim = CreateIl2CppShim(replacement, original.DeclaringType);
				Imports.Hook(NET_SDK.SDK.GetClass(original.DeclaringType.FullName).GetMethod(original.Name).Ptr, il2CppShim.MethodHandle.GetFunctionPointer());

				PatchTools.RememberObject(original, new Tuple<MethodBase, MethodBase>(replacement, il2CppShim));
			}
			else
			{
				PatchTools.RememberObject(original, replacement); // no gc for new value + release old value to gc
			}

			return replacement;
		}

		private static DynamicMethod CreateIl2CppShim(DynamicMethod original, Type owner)
		{
			var patchName = original.Name + "_il2cpp";

			var parameters = original.GetParameters();
			var result = parameters.Types().ToList();
			var origParamTypes = result.ToArray();
			var paramTypes = new Type[origParamTypes.Length];
			for (int i = 0; i < paramTypes.Length; ++i)
			{
				paramTypes[i] = IsIl2CppType(origParamTypes[i]) ? typeof(IntPtr) : origParamTypes[i];
			}

			var origReturnType = AccessTools.GetReturnedType(original);
			var returnType = IsIl2CppType(origReturnType) ? typeof(IntPtr) : origReturnType;

			DynamicMethod method;
			method = new DynamicMethod(
					patchName,
					MethodAttributes.Public | MethodAttributes.Static,
					CallingConventions.Standard,
					returnType,
					paramTypes,
					owner,
					true
			);

			for (var i = 0; i < parameters.Length; i++)
				method.DefineParameter(i + 1, parameters[i].Attributes, parameters[i].Name);

			var il = method.GetILGenerator();

			// Load arguments, invoking the IntPrt -> Il2CppObject constructor for IL2CPP types
			for (int i = 0; i < origParamTypes.Length; ++i)
			{
				Emitter.Emit(il, OpCodes.Ldarg, i);
				if (IsIl2CppType(origParamTypes[i]))
				{
					Emitter.Emit(il, OpCodes.Newobj, Il2CppConstuctor(origParamTypes[i]));
				}
			}

			// Call the original patch with the now-correct types
			Emitter.Emit(il, OpCodes.Call, original);

			// If needed, unwrap the return value; then return
			if (IsIl2CppType(origReturnType))
			{
				var pointerGetter = AccessTools.DeclaredProperty(Main.Il2CppObjectBaseType, "Pointer").GetMethod;
				Emitter.Emit(il, OpCodes.Call, pointerGetter);
			}

			Emitter.Emit(il, OpCodes.Ret);

			DynamicTools.PrepareDynamicMethod(method);
			return method;
		}

		internal static bool IsIl2CppType(Type type) => ((Main.Il2CppObjectBaseType != null) && type.IsSubclassOf(Main.Il2CppObjectBaseType));
		private static ConstructorInfo Il2CppConstuctor(Type type) => AccessTools.DeclaredConstructor(type, new Type[] { typeof(IntPtr) });
	}
}