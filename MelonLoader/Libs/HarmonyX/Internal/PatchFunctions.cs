using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib.Internal.Patching;
using HarmonyLib.Internal.Util;
using HarmonyLib.Public.Patching;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MethodBody = Mono.Cecil.Cil.MethodBody;
using OpCodes = Mono.Cecil.Cil.OpCodes;

namespace HarmonyLib
{
	/// <summary>Patch function helpers</summary>
	internal static class PatchFunctions
	{
		/// <summary>Sorts patch methods by their priority rules</summary>
		/// <param name="original">The original method</param>
		/// <param name="patches">Patches to sort</param>
		/// <param name="debug">Use debug mode. Present for source parity with Harmony 2, don't use.</param>
		/// <returns>The sorted patch methods</returns>
		///
		internal static List<MethodInfo> GetSortedPatchMethods(MethodBase original, Patch[] patches, bool debug = false)
		{
			return new PatchSorter(patches, debug).Sort(original);
		}

		/// <summary>Sorts patch methods by their priority rules</summary>
		/// <param name="original">The original method</param>
		/// <param name="patches">Patches to sort</param>
		/// <returns>The sorted patch methods</returns>
		///
		internal static Patch[] GetSortedPatchMethodsAsPatches(MethodBase original, Patch[] patches)
		{
			return new PatchSorter(patches).SortAsPatches(original);
		}

		/// <summary>Creates new replacement method with the latest patches and detours the original method</summary>
		/// <param name="original">The original method</param>
		/// <param name="patchInfo">Information describing the patches</param>
		/// <returns>The newly created replacement method</returns>
		///
		internal static MethodInfo UpdateWrapper(MethodBase original, PatchInfo patchInfo)
		{
			var patcher = original.GetMethodPatcher();
			var dmd = patcher.PrepareOriginal();

			if (dmd != null)
			{
				var ctx = new ILContext(dmd.Definition);
				HarmonyManipulator.Manipulate(original, patchInfo, ctx);
			}

			try { return patcher.DetourTo(dmd?.Generate()) as MethodInfo; }
			catch (Exception ex) { throw HarmonyException.Create(ex, dmd?.Definition?.Body); }
		}

		internal static MethodInfo ReversePatch(HarmonyMethod standin, MethodBase original, MethodInfo postTranspiler)
		{
			if (standin is null)
				throw new ArgumentNullException(nameof(standin));
			if (standin.method is null)
				throw new ArgumentNullException($"{nameof(standin)}.{nameof(standin.method)}");

			var transpilers = new List<MethodInfo>();
			if (standin.reversePatchType == HarmonyReversePatchType.Snapshot)
			{
				var info = Harmony.GetPatchInfo(original);
				transpilers.AddRange(GetSortedPatchMethods(original, info.Transpilers.ToArray()));
			}
			if (postTranspiler is object) transpilers.Add(postTranspiler);

			MethodBody patchBody = null;
			var hook = new ILHook(standin.method, ctx =>
			{
				if (!(original is MethodInfo mi))
					return;

				patchBody = ctx.Body;

				var patcher = mi.GetMethodPatcher();
				var dmd = patcher.CopyOriginal();

				if (dmd == null)
					throw new NullReferenceException($"Cannot reverse patch {mi.FullDescription()}: method patcher ({patcher.GetType().FullDescription()}) can't copy original method body");

				var manipulator = new ILManipulator(dmd.Definition.Body);

				// Copy over variables from the original code
				ctx.Body.Variables.Clear();
				foreach (var variableDefinition in manipulator.Body.Variables)
					ctx.Body.Variables.Add(new VariableDefinition(ctx.Module.ImportReference(variableDefinition.VariableType)));

				foreach (var methodInfo in transpilers)
					manipulator.AddTranspiler(methodInfo);

				manipulator.WriteTo(ctx.Body, standin.method);

				// Write a ret in case it got removed (wrt. HarmonyManipulator)
				ctx.IL.Emit(OpCodes.Ret);
			}, new ILHookConfig { ManualApply = true });

			try
			{
				hook.Apply();
			}
			catch (Exception ex)
			{
				throw HarmonyException.Create(ex, patchBody);
			}

			var replacement = hook.GetCurrentTarget() as MethodInfo;
			PatchTools.RememberObject(standin.method, replacement);
			return replacement;
		}

		internal static IEnumerable<CodeInstruction> ApplyTranspilers(MethodBase methodBase, ILGenerator generator, int maxTranspilers = 0)
		{
			var patcher = methodBase.GetMethodPatcher();
			var dmd = patcher.CopyOriginal();

			if (dmd == null)
				throw new NullReferenceException($"Cannot reverse patch {methodBase.FullDescription()}: method patcher ({patcher.GetType().FullDescription()}) can't copy original method body");

			var manipulator = new ILManipulator(dmd.Definition.Body);

			var info = methodBase.GetPatchInfo();
			if (info is object)
			{
				var sortedTranspilers = GetSortedPatchMethods(methodBase, info.transpilers);
				for (var i = 0; i < maxTranspilers && i < sortedTranspilers.Count; i++)
					manipulator.AddTranspiler(sortedTranspilers[i]);
			}

			return manipulator.GetInstructions(generator, methodBase);
		}
	}
}
