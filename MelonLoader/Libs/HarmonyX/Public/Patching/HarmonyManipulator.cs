using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib.Internal.Patching;
using HarmonyLib.Internal.Util;
using HarmonyLib.Tools;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace HarmonyLib.Public.Patching
{
	/// <summary>
	///    IL manipulator to create Harmony-style patches
	/// </summary>
	///
	public static class HarmonyManipulator
	{
		internal class PatchContext
		{
			public MethodInfo method;
			public bool wrapTryCatch;
		}

		private static readonly string INSTANCE_PARAM = "__instance";
		private static readonly string ORIGINAL_METHOD_PARAM = "__originalMethod";
		private static readonly string RUN_ORIGINAL_PARAM = "__runOriginal";
		private static readonly string RESULT_VAR = "__result";
		private static readonly string STATE_VAR = "__state";
		private static readonly string EXCEPTION_VAR = "__exception";
		private static readonly string PARAM_INDEX_PREFIX = "__";
		private static readonly string INSTANCE_FIELD_PREFIX = "___";

		//private static readonly MethodInfo GetMethodFromHandle1 =
		//	typeof(MethodBase).GetMethod("GetMethodFromHandle", new[] {typeof(RuntimeMethodHandle)});

		//private static readonly MethodInfo GetMethodFromHandle2 = typeof(MethodBase).GetMethod("GetMethodFromHandle",
		//	new[] {typeof(RuntimeMethodHandle), typeof(RuntimeTypeHandle)});

		//private static MethodInfo LogPatchExceptionMethod =
		//	AccessTools.Method(typeof(HarmonyManipulator), nameof(LogPatchException));

		private static void LogPatchException(object errorObject, string patch)
		{
			Logger.LogText(Logger.LogChannel.Error, $"Error while running {patch}. Error: {errorObject}");
		}

		private static void SortPatches(MethodBase original, PatchInfo patchInfo,
			out List<PatchContext> prefixes,
			out List<PatchContext> postfixes,
			out List<PatchContext> transpilers,
			out List<PatchContext> finalizers,
			out List<PatchContext> ilmanipulators)
		{
			Patch[] prefixesArr, postfixesArr, transpilersArr, finalizersArr, ilmanipulatorsArr;

			// Lock to ensure no more patches are added while we're sorting
			lock (patchInfo)
			{
				prefixesArr = patchInfo.prefixes.ToArray();
				postfixesArr = patchInfo.postfixes.ToArray();
				transpilersArr = patchInfo.transpilers.ToArray();
				finalizersArr = patchInfo.finalizers.ToArray();
				ilmanipulatorsArr = patchInfo.ilmanipulators.ToArray();
			}

			static List<PatchContext> Sort(MethodBase original, Patch[] patches)
				=> PatchFunctions.GetSortedPatchMethodsAsPatches(original, patches)
					.Select(p => new PatchContext {method = p.GetMethod(original), wrapTryCatch = p.wrapTryCatch})
					.ToList();

			// debug is useless; debug logs passed on-demand
			prefixes = Sort(original, prefixesArr);
			postfixes = Sort(original, postfixesArr);
			transpilers = Sort(original, transpilersArr);
			finalizers = Sort(original, finalizersArr);
			ilmanipulators = Sort(original, ilmanipulatorsArr);
		}

		/// <summary>
		/// Manipulates a <see cref="Mono.Cecil.Cil.MethodBody"/> by applying Harmony patches to it.
		/// </summary>
		/// <param name="original">Reference to the method that should be considered as original. Used to reference parameter and return types.</param>
		/// <param name="patchInfo">Collection of Harmony patches to apply.</param>
		/// <param name="ctx">Method body to manipulate as <see cref="ILContext"/> instance. Should contain instructions to patch.</param>
		/// <remarks>
		/// In most cases you will want to use <see cref="PatchManager.ToPatchInfo"/> to create or obtain global
		/// patch info for the method that contains aggregated info of all Harmony instances.
		/// </remarks>
		///
		public static void Manipulate(MethodBase original, PatchInfo patchInfo, ILContext ctx)
		{
			ManipulateCrashFix(original, patchInfo, ctx);
		}

		// TODO: this seems to be caused by patch shield crashing it ...
		public static void ManipulateCrashFix(MethodBase original, PatchInfo patchInfo, ILContext ctx)
		{
            SortPatches(original, patchInfo, out var sortedPrefixes, out var sortedPostfixes, out var sortedTranspilers,
                out var sortedFinalizers, out var sortedILManipulators);

            Logger.Log(Logger.LogChannel.Info, () =>
            {
                var sb = new StringBuilder();

                sb.AppendLine(
                    $"Patching {original.FullDescription()} with {sortedPrefixes.Count} prefixes, {sortedPostfixes.Count} postfixes, {sortedTranspilers.Count} transpilers, {sortedFinalizers.Count} finalizers");

                void Print(ICollection<PatchContext> list, string type)
                {
                    if (list.Count == 0)
                        return;
                    sb.AppendLine($"{list.Count} {type}:");
                    foreach (var fix in list)
                        sb.AppendLine($"* {fix.method.FullDescription()}");
                }

                Print(sortedPrefixes, "prefixes");
                Print(sortedPostfixes, "postfixes");
                Print(sortedTranspilers, "transpilers");
                Print(sortedFinalizers, "finalizers");
                Print(sortedILManipulators, "ilmanipulators");

                return sb.ToString();
            });

            MakePatched(original, ctx, sortedPrefixes, sortedPostfixes, sortedTranspilers, sortedFinalizers, sortedILManipulators);
        }

		private static void WriteTranspiledMethod(ILContext ctx, MethodBase original, List<PatchContext> transpilers)
		{
			if (transpilers.Count == 0)
				return;

			Logger.Log(Logger.LogChannel.Info, () => $"Transpiling {original.FullDescription()}");

			// Create a high-level manipulator for the method
			var manipulator = new ILManipulator(ctx.Body);

			// Add in all transpilers
			foreach (var transpiler in transpilers)
				manipulator.AddTranspiler(transpiler.method);

			// Write new manipulated code to our body
			manipulator.WriteTo(ctx.Body, original);
		}

		private static ILEmitter.Label MakeReturnLabel(ILEmitter il)
		{
			// We replace all `ret`s with a simple branch to force potential execution of post-original code

			// Create a helper label as well
			// We mark the label as not emitted so that potential postfix code can mark it
			var resultLabel = il.DeclareLabel();
			resultLabel.emitted = false;

			var hasRet = false;
			foreach (var ins in il.IL.Body.Instructions.Where(ins => ins.MatchRet()))
			{
				hasRet = true;
				ins.OpCode = OpCodes.Br;
				ins.Operand = resultLabel.instruction;
				resultLabel.targets.Add(ins);
			}

			// Pick `nop` if previously the method didn't have `ret` before, like in case of exception throwing
			resultLabel.instruction = Instruction.Create(hasRet ? OpCodes.Ret : OpCodes.Nop);

			// Already append ending label for other code to use as emitBefore point
			il.IL.Append(resultLabel.instruction);

			return resultLabel;
		}

		private static void WritePostfixes(ILEmitter il, MethodBase original, ILEmitter.Label returnLabel,
			Dictionary<string, VariableDefinition> variables, ICollection<PatchContext> postfixes)
		{
			// Postfix layout:
			// Make return value (if needed) into a variable
			// If method has return value, store the current stack value into it (since the value on the stack is the return value)
			// Call postfixes that modify return values by __return
			// Call postfixes that modify return values by chaining

			if (postfixes.Count == 0)
				return;

			Logger.Log(Logger.LogChannel.Info, () => "Writing postfixes");

			// Get the last instruction (expected to be `ret`)
			il.emitBefore = il.IL.Body.Instructions[il.IL.Body.Instructions.Count - 1];

			// Mark the original method return label here
			il.MarkLabel(returnLabel);

			if (!variables.TryGetValue(RESULT_VAR, out var returnValueVar))
			{
				var retVal = AccessTools.GetReturnedType(original);
				returnValueVar = variables[RESULT_VAR] = retVal == typeof(void) ? null : il.DeclareVariable(retVal);
			}

			if (returnValueVar != null)
				il.Emit(OpCodes.Stloc, returnValueVar);

			if (!variables.ContainsKey(RUN_ORIGINAL_PARAM))
			{
				// If __runOriginal wasn't defined in prefixes, create it and mark as true (since original was run)
				var runOriginalVar = variables[RUN_ORIGINAL_PARAM] = il.DeclareVariable(typeof(bool));
				il.Emit(OpCodes.Ldc_I4_1);
				il.Emit(OpCodes.Stloc, runOriginalVar);
			}

			foreach (var postfix in postfixes.Where(p => p.method.ReturnType == typeof(void)))
			{
				var method = postfix.method;
				var start = il.DeclareLabel();
				il.MarkLabel(start);

				EmitCallParameter(il, original, method, variables, true, out var tmpObjectVar, out var tmpBoxVars);
				il.Emit(OpCodes.Call, method);
				EmitResultUnbox(il, original, tmpObjectVar, returnValueVar);
				EmitArgUnbox(il, tmpBoxVars);

				if (postfix.wrapTryCatch)
					EmitTryCatchWrapper(il, method, start);
			}

			// Load the result for the final time, the chained postfixes will handle the rest
			if (returnValueVar != null)
				il.Emit(OpCodes.Ldloc, returnValueVar);

			// If postfix returns a value, it must be chainable
			// The first param is always the return of the previous
			foreach (var postfix in postfixes.Where(p => p.method.ReturnType != typeof(void)))
			{
				var method = postfix.method;

				// If we wrap into try/catch, we need to separate the code to keep the stack clean
				if (postfix.wrapTryCatch)
					il.Emit(OpCodes.Stloc, returnValueVar);

				var start = il.DeclareLabel();
				il.MarkLabel(start);

				if (postfix.wrapTryCatch)
					il.Emit(OpCodes.Ldloc, returnValueVar);

				EmitCallParameter(il, original, method, variables, true, out var tmpObjectVar, out var tmpBoxVars);
				il.Emit(OpCodes.Call, method);
				EmitResultUnbox(il, original, tmpObjectVar, returnValueVar);
				EmitArgUnbox(il, tmpBoxVars);

				var firstParam = method.GetParameters().FirstOrDefault();

				if (firstParam == null || method.ReturnType != firstParam.ParameterType)
				{
					if (firstParam != null)
						throw new InvalidHarmonyPatchArgumentException(
							$"Return type of pass through postfix {method.FullDescription()} does not match type of its first parameter",
							original, method);
					throw new InvalidHarmonyPatchArgumentException(
						$"Postfix patch {method.FullDescription()} must have `void` as return type", original, method);
				}

				if (postfix.wrapTryCatch)
				{
					// Store the result to clean the stack
					il.Emit(OpCodes.Stloc, returnValueVar);
					EmitTryCatchWrapper(il, method, start);
					// Load the return value back onto the stack for the next postfix
					il.Emit(OpCodes.Ldloc, returnValueVar);
				}
			}
		}

		private static bool WritePrefixes(ILEmitter il, MethodBase original, ILEmitter.Label returnLabel,
			Dictionary<string, VariableDefinition> variables, ICollection<PatchContext> prefixes)
		{
			// Prefix layout:
			// Make return value (if needed) into a variable
			// Call prefixes
			// If method returns a value, add additional logic to allow skipping original method

			if (prefixes.Count == 0)
				return false;

			Logger.Log(Logger.LogChannel.Info, () => "Writing prefixes");

			// Start emitting at the start
			il.emitBefore = il.IL.Body.Instructions[0];

			if (!variables.TryGetValue(RESULT_VAR, out var returnValueVar))
			{
				var retVal = AccessTools.GetReturnedType(original);
				returnValueVar = variables[RESULT_VAR] = retVal == typeof(void) ? null : il.DeclareVariable(retVal);
			}

			// A prefix that can modify control flow has one of the following:
			// * It returns a boolean
			// * It declares bool __runOriginal
			var canModifyControlFlow = prefixes.Any(p => p.method.ReturnType == typeof(bool) ||
			                                             p.method.GetParameters()
				                                             .Any(pp => pp.Name == RUN_ORIGINAL_PARAM &&
				                                                        pp.ParameterType.OpenRefType() == typeof(bool)));

			// Flag to check if the orignal method should be run (or was run)
			// Always present so other patchers can access it
			var runOriginal = variables[RUN_ORIGINAL_PARAM] = il.DeclareVariable(typeof(bool));
			// Init runOriginal to true
			il.Emit(OpCodes.Ldc_I4_1);
			il.Emit(OpCodes.Stloc, runOriginal);

			// If runOriginal flag exists, we need to add more logic to the method end
			var postProcessTarget = returnValueVar != null ? il.DeclareLabel() : returnLabel;

			foreach (var prefix in prefixes)
			{
				var method = prefix.method;
				var start = il.DeclareLabel();
				il.MarkLabel(start);

				EmitCallParameter(il, original, method, variables, false, out var tmpObjectVar, out var tmpBoxVars);
				il.Emit(OpCodes.Call, method);
				EmitResultUnbox(il, original, tmpObjectVar, returnValueVar);
				EmitArgUnbox(il, tmpBoxVars);

				if (!AccessTools.IsVoid(method.ReturnType))
				{
					if (method.ReturnType != typeof(bool))
						throw new InvalidHarmonyPatchArgumentException(
							$"Prefix patch {method.FullDescription()} has return type {method.ReturnType}, but only `bool` or `void` are permitted",
							original, method);

					if (canModifyControlFlow)
					{
						// AND the current runOriginal to return value of the method (if any)
						il.Emit(OpCodes.Ldloc, runOriginal);
						il.Emit(OpCodes.And);
						il.Emit(OpCodes.Stloc, runOriginal);
					}
				}

				if (prefix.wrapTryCatch)
					EmitTryCatchWrapper(il, method, start);
			}

			if (!canModifyControlFlow)
				return false;

			// If runOriginal is false, branch automatically to the end
			il.Emit(OpCodes.Ldloc, runOriginal);
			il.Emit(OpCodes.Brfalse, postProcessTarget);

			if (returnValueVar == null)
				return true;

			// Finally, load return value onto stack at the end
			il.emitBefore = il.IL.Body.Instructions[il.IL.Body.Instructions.Count - 1];
			il.MarkLabel(postProcessTarget);
			il.Emit(OpCodes.Ldloc, returnValueVar);

			return true;
		}

		private static bool WriteFinalizers(ILEmitter il, MethodBase original, ILEmitter.Label returnLabel,
			Dictionary<string, VariableDefinition> variables,
			ICollection<PatchContext> finalizers)
		{
			// Finalizer layout:
			// Create __exception variable to store exception info and a skip flag
			// Wrap the whole method into a try/catch
			// Call finalizers at the end of method (simulate `finally`)
			// If __exception got set, throw it
			// Begin catch block
			// Store exception into __exception
			// If skip flag is set, skip finalizers
			// Call finalizers
			// If __exception is still set, rethrow (if new exception set, otherwise throw the new exception)
			// End catch block

			if (finalizers.Count == 0)
				return false;

			Logger.Log(Logger.LogChannel.Info, () => "Writing finalizers");

			// Create variables to hold custom exception
			variables[EXCEPTION_VAR] = il.DeclareVariable(typeof(Exception));

			// Create a flag to signify that finalizers have been run
			// Cecil DMD fix: initialize it to false
			var skipFinalizersVar = il.DeclareVariable(typeof(bool));
			il.emitBefore = il.IL.Body.Instructions[0];
			il.Emit(OpCodes.Ldc_I4_0);
			il.Emit(OpCodes.Stloc, skipFinalizersVar);

			il.emitBefore = il.IL.Body.Instructions[il.IL.Body.Instructions.Count - 1];

			// Mark the original method return label here if it hasn't been yet
			il.MarkLabel(returnLabel);

			if (!variables.TryGetValue(RESULT_VAR, out var returnValueVar))
			{
				var retVal = AccessTools.GetReturnedType(original);
				returnValueVar = variables[RESULT_VAR] = retVal == typeof(void) ? null : il.DeclareVariable(retVal);
			}

			// Start main exception block
			var mainBlock = il.BeginExceptionBlock(il.DeclareLabelFor(il.IL.Body.Instructions[0]));

			bool WriteFinalizerCalls(bool suppressExceptions)
			{
				var canRethrow = true;

				foreach (var finalizer in finalizers)
				{
					var method = finalizer.method;
					var start = il.DeclareLabel();
					il.MarkLabel(start);

					EmitCallParameter(il, original, method, variables, false, out var tmpObjectVar, out var tmpBoxVars);
					il.Emit(OpCodes.Call, method);
					EmitResultUnbox(il, original, tmpObjectVar, returnValueVar);
					EmitArgUnbox(il, tmpBoxVars);

					if (method.ReturnType != typeof(void))
					{
						il.Emit(OpCodes.Stloc, variables[EXCEPTION_VAR]);
						canRethrow = false;
					}

					if (suppressExceptions || finalizer.wrapTryCatch)
						EmitTryCatchWrapper(il, method, start);
				}

				return canRethrow;
			}

			// First, store potential result into a variable and empty the stack
			if (returnValueVar != null)
				il.Emit(OpCodes.Stloc, returnValueVar);

			// Write finalizers inside the `try`
			WriteFinalizerCalls(false);

			// Mark finalizers as skipped so they won't rerun
			il.Emit(OpCodes.Ldc_I4_1);
			il.Emit(OpCodes.Stloc, skipFinalizersVar);

			// If __exception is not null, throw
			var skipLabel = il.DeclareLabel();
			il.Emit(OpCodes.Ldloc, variables[EXCEPTION_VAR]);
			il.Emit(OpCodes.Brfalse, skipLabel);
			il.Emit(OpCodes.Ldloc, variables[EXCEPTION_VAR]);
			il.Emit(OpCodes.Throw);
			il.MarkLabel(skipLabel);

			// Begin a generic `catch(Exception o)` here and capture exception into __exception
			il.BeginHandler(mainBlock, ExceptionHandlerType.Catch, typeof(Exception));
			il.Emit(OpCodes.Stloc, variables[EXCEPTION_VAR]);

			// Call finalizers or skip them if needed
			il.Emit(OpCodes.Ldloc, skipFinalizersVar);
			var postFinalizersLabel = il.DeclareLabel();
			il.Emit(OpCodes.Brtrue, postFinalizersLabel);

			var rethrowPossible = WriteFinalizerCalls(true);

			il.MarkLabel(postFinalizersLabel);

			// Possibly rethrow if __exception is still not null (i.e. suppressed)
			skipLabel = il.DeclareLabel();
			il.Emit(OpCodes.Ldloc, variables[EXCEPTION_VAR]);
			il.Emit(OpCodes.Brfalse, skipLabel);
			if (rethrowPossible)
				il.Emit(OpCodes.Rethrow);
			else
			{
				il.Emit(OpCodes.Ldloc, variables[EXCEPTION_VAR]);
				il.Emit(OpCodes.Throw);
			}

			il.MarkLabel(skipLabel);
			il.EndExceptionBlock(mainBlock);

			if (returnValueVar != null)
				il.Emit(OpCodes.Ldloc, returnValueVar);

			return true;
		}

		internal static void ApplyILManipulators(ILContext ctx, MethodBase original, ICollection<MethodInfo> manipulators, ILEmitter.Label retLabel)
		{
			// Define a label to branch to, if not passed a label create one to the last instruction
			var retILLabel = ctx.DefineLabel(retLabel?.instruction) ?? ctx.DefineLabel(ctx.Body.Instructions.Last());

			foreach (var method in manipulators)
			{
				List<object> manipulatorParameters = new List<object>();
				foreach (var type in method.GetParameters().Select(p => p.ParameterType))
				{
					if (type.IsAssignableFrom(typeof(ILContext)))
						manipulatorParameters.Add(ctx);
					if (type.IsAssignableFrom(typeof(MethodBase)))
						manipulatorParameters.Add(original);
					if (type.IsAssignableFrom(typeof(ILLabel)))
						manipulatorParameters.Add(retILLabel);
				}

				method.Invoke(null, manipulatorParameters.ToArray());
			}
		}

		private static void MakePatched(MethodBase original, ILContext ctx,
			List<PatchContext> prefixes,
			List<PatchContext> postfixes,
			List<PatchContext> transpilers,
			List<PatchContext> finalizers,
			List<PatchContext> ilmanipulators)
		{
			try
			{
				if (original == null)
					throw new ArgumentException(nameof(original));

				Logger.Log(Logger.LogChannel.Info, () => $"Running ILHook manipulator on {original.FullDescription()}");

				WriteTranspiledMethod(ctx, original, transpilers);

				// If no need to wrap anything, we're basically done!
				if (prefixes.Count + postfixes.Count + finalizers.Count + ilmanipulators.Count == 0)
				{
					Logger.Log(Logger.LogChannel.IL,
						() => $"Generated patch ({ctx.Method.FullName}):\n{ctx.Body.ToILDasmString()}");
					return;
				}

				var il = new ILEmitter(ctx.IL);
				var returnLabel = MakeReturnLabel(il);
				var variables = new Dictionary<string, VariableDefinition>();

				// Collect state variables
				foreach (var nfix in prefixes.Union(postfixes).Union(finalizers))
					if (nfix.method.DeclaringType != null && variables.ContainsKey(nfix.method.DeclaringType.FullName) == false)
						foreach (var patchParam in nfix
							.method
							.GetParameters()
							.Where(patchParam => patchParam.Name == STATE_VAR))
							variables[nfix.method.DeclaringType.FullName] =
								il.DeclareVariable(patchParam.ParameterType.OpenRefType()); // Fix possible reftype

				var modifiesControlFlow = false;
				modifiesControlFlow |= WritePrefixes(il, original, returnLabel, variables, prefixes);
				WritePostfixes(il, original, returnLabel, variables, postfixes);
				modifiesControlFlow |= WriteFinalizers(il, original, returnLabel, variables, finalizers);

				// Mark return label in case it hasn't been marked yet and close open labels to return
				il.MarkLabel(returnLabel);
				var lastInstruction = il.SetOpenLabelsTo(ctx.Instrs[ctx.Instrs.Count - 1]);

				// If we have finalizers, ensure the return label is `ret` and not `nop`
				if (modifiesControlFlow)
					lastInstruction.OpCode = OpCodes.Ret;

				ApplyILManipulators(ctx, original, ilmanipulators.Select(m => m.method).ToList(), returnLabel);

				Logger.Log(Logger.LogChannel.IL,
					() => $"Generated patch ({ctx.Method.FullName}):\n{ctx.Body.ToILDasmString()}");
			}
			catch (Exception e)
			{
				Logger.Log(Logger.LogChannel.Error, () => $"Failed to patch {original.FullDescription()}: {e}");
			}
		}

		private static OpCode GetIndOpcode(Type type)
		{
			if (type.IsEnum)
				return OpCodes.Ldind_I4;

			if (type == typeof(float)) return OpCodes.Ldind_R4;
			if (type == typeof(double)) return OpCodes.Ldind_R8;

			if (type == typeof(byte)) return OpCodes.Ldind_U1;
			if (type == typeof(ushort)) return OpCodes.Ldind_U2;
			if (type == typeof(uint)) return OpCodes.Ldind_U4;
			if (type == typeof(ulong)) return OpCodes.Ldind_I8;

			if (type == typeof(sbyte)) return OpCodes.Ldind_I1;
			if (type == typeof(short)) return OpCodes.Ldind_I2;
			if (type == typeof(int)) return OpCodes.Ldind_I4;
			if (type == typeof(long)) return OpCodes.Ldind_I8;

			return OpCodes.Ldind_Ref;
		}

		private static void EmitTryCatchWrapper(ILEmitter il, MethodInfo target, ILEmitter.Label start)
		{
			//var exBlock = il.BeginExceptionBlock(start);
			//il.BeginHandler(exBlock, ExceptionHandlerType.Catch, typeof(object));
			//il.Emit(OpCodes.Ldstr, target.FullDescription());
			//il.Emit(OpCodes.Call, LogPatchExceptionMethod);
			//il.EndExceptionBlock(exBlock);
			//// Force proper closure in case nothing is emitted after the catch
			//il.Emit(OpCodes.Nop);
		}

		private static void EmitResultUnbox(ILEmitter il, MethodBase original, VariableDefinition tmp, VariableDefinition result)
		{
			if (tmp == null)
				return;
			il.Emit(OpCodes.Ldloc, tmp);
			il.Emit(OpCodes.Unbox_Any, AccessTools.GetReturnedType(original));
			il.Emit(OpCodes.Stloc, result);
		}

		private static void EmitArgUnbox(ILEmitter il, List<ArgumentBoxInfo> boxInfo)
		{
			if (boxInfo == null)
				return;
			foreach (var info in boxInfo)
			{
				if (info.isByRef)
					il.Emit(OpCodes.Ldarg, info.index);
				il.Emit(OpCodes.Ldloc, info.tmpVar);
				il.Emit(OpCodes.Unbox_Any, info.type);
				if (info.isByRef)
					il.Emit(OpCodes.Stobj, info.type);
				else
					il.Emit(OpCodes.Starg, info.index);
			}
		}

		private static bool EmitOriginalBaseMethod(ILEmitter il, MethodBase original)
		{
			//if (original is MethodInfo method)
			//	il.Emit(OpCodes.Ldtoken, method);
			//else if (original is ConstructorInfo constructor)
			//	il.Emit(OpCodes.Ldtoken, constructor);
			//else return false;

			//var type = original.ReflectedType;
			//if (type.IsGenericType) il.Emit(OpCodes.Ldtoken, type);
			//il.Emit(OpCodes.Call, type.IsGenericType ? GetMethodFromHandle2 : GetMethodFromHandle1);
			return true;
		}

		class ArgumentBoxInfo
		{
			public int index;
			public VariableDefinition tmpVar;
			public Type type;
			public bool isByRef;
		}

		private static void EmitCallParameter(ILEmitter il, MethodBase original, MethodInfo patch,
			Dictionary<string, VariableDefinition> variables, bool allowFirsParamPassthrough,
			out VariableDefinition tmpObjectVar, out List<ArgumentBoxInfo> tmpBoxVars)
		{
			tmpObjectVar = null;
			tmpBoxVars = new List<ArgumentBoxInfo>();
			var isInstance = original.IsStatic is false;
			var originalParameters = original.GetParameters();
			var originalParameterNames = originalParameters.Select(p => p.Name).ToArray();

			// check for passthrough using first parameter (which must have same type as return type)
			var parameters = patch.GetParameters().ToList();
			if (allowFirsParamPassthrough && patch.ReturnType != typeof(void) && parameters.Count > 0 &&
			    parameters[0].ParameterType == patch.ReturnType)
				parameters.RemoveRange(0, 1);

			foreach (var patchParam in parameters)
			{
				if (patchParam.Name == ORIGINAL_METHOD_PARAM)
				{
					if (EmitOriginalBaseMethod(il, original))
						continue;

					il.Emit(OpCodes.Ldnull);
					continue;
				}

				if (patchParam.Name == INSTANCE_PARAM)
				{
					if (original.IsStatic)
						il.Emit(OpCodes.Ldnull);
					else
					{
						var instanceIsRef = original.DeclaringType is object && AccessTools.IsStruct(original.DeclaringType);
						var parameterIsRef = patchParam.ParameterType.IsByRef;
						if (instanceIsRef == parameterIsRef) il.Emit(OpCodes.Ldarg_0);
						if (instanceIsRef && parameterIsRef is false)
						{
							il.Emit(OpCodes.Ldarg_0);
							il.Emit(OpCodes.Ldobj, original.DeclaringType);
						}

						if (instanceIsRef is false && parameterIsRef) il.Emit(OpCodes.Ldarga, 0);
					}

					continue;
				}

				if (patchParam.Name.StartsWith(INSTANCE_FIELD_PREFIX, StringComparison.Ordinal))
				{
					var fieldName = patchParam.Name.Substring(INSTANCE_FIELD_PREFIX.Length);
					FieldInfo fieldInfo;
					if (fieldName.All(char.IsDigit))
					{
						// field access by index only works for declared fields
						fieldInfo = AccessTools.DeclaredField(original.DeclaringType, int.Parse(fieldName));
						if (fieldInfo is null)
							throw new ArgumentException(
								$"No field found at given index in class {original.DeclaringType.FullName}", fieldName);
					}
					else
					{
						fieldInfo = AccessTools.Field(original.DeclaringType, fieldName);
						if (fieldInfo is null)
							throw new ArgumentException($"No such field defined in class {original.DeclaringType.FullName}",
								fieldName);
					}

					if (fieldInfo.IsStatic)
						il.Emit(patchParam.ParameterType.IsByRef ? OpCodes.Ldsflda : OpCodes.Ldsfld, fieldInfo);
					else
					{
						il.Emit(OpCodes.Ldarg_0);
						il.Emit(patchParam.ParameterType.IsByRef ? OpCodes.Ldflda : OpCodes.Ldfld, fieldInfo);
					}

					continue;
				}

				// state is special too since each patch has its own local var
				if (patchParam.Name == STATE_VAR)
				{
					var ldlocCode = patchParam.ParameterType.IsByRef ? OpCodes.Ldloca : OpCodes.Ldloc;
					if (variables.TryGetValue(patch.DeclaringType.FullName, out var stateVar))
						il.Emit(ldlocCode, stateVar);
					else
						il.Emit(OpCodes.Ldnull);
					continue;
				}

				// treat __result var special
				if (patchParam.Name == RESULT_VAR)
				{
					var returnType = AccessTools.GetReturnedType(original);
					if (returnType == typeof(void))
						throw new Exception($"Cannot get result from void method {original.FullDescription()}");
					var resultType = patchParam.ParameterType;
					if (resultType.IsByRef && !returnType.IsByRef)
						resultType = resultType.GetElementType();
					if (resultType.IsAssignableFrom(returnType) is false)
						throw new Exception(
							$"Cannot assign method return type {returnType.FullName} to {RESULT_VAR} type {resultType.FullName} for method {original.FullDescription()}");
					var ldlocCode = patchParam.ParameterType.IsByRef && !returnType.IsByRef ? OpCodes.Ldloca : OpCodes.Ldloc;
					if (returnType.IsValueType && patchParam.ParameterType == typeof(object).MakeByRefType()) ldlocCode = OpCodes.Ldloc;
					il.Emit(ldlocCode, variables[RESULT_VAR]);
					if (returnType.IsValueType)
					{
						if (patchParam.ParameterType == typeof(object))
							il.Emit(OpCodes.Box, returnType);
						else if (patchParam.ParameterType == typeof(object).MakeByRefType())
						{
							il.Emit(OpCodes.Box, returnType);
							tmpObjectVar = il.DeclareVariable(typeof(object));
							il.Emit(OpCodes.Stloc, tmpObjectVar);
							il.Emit(OpCodes.Ldloca, tmpObjectVar);
						}
					}
					continue;
				}

				// any other declared variables
				if (variables.TryGetValue(patchParam.Name, out var localBuilder))
				{
					var ldlocCode = patchParam.ParameterType.IsByRef ? OpCodes.Ldloca : OpCodes.Ldloc;
					il.Emit(ldlocCode, localBuilder);
					continue;
				}

				int idx;
				if (patchParam.Name.StartsWith(PARAM_INDEX_PREFIX, StringComparison.Ordinal))
				{
					var val = patchParam.Name.Substring(PARAM_INDEX_PREFIX.Length);
					if (!int.TryParse(val, out idx))
						throw new Exception($"Parameter {patchParam.Name} does not contain a valid index");
					if (idx < 0 || idx >= originalParameters.Length)
						throw new Exception($"No parameter found at index {idx}");
				}
				else
				{
					idx = patch.GetArgumentIndex(originalParameterNames, patchParam);
					if (idx == -1)
					{
						var harmonyMethod = HarmonyMethodExtensions.GetMergedFromType(patchParam.ParameterType);
						if (harmonyMethod.methodType is null) // MethodType default is Normal
							harmonyMethod.methodType = MethodType.Normal;
						var delegateOriginal = harmonyMethod.GetOriginalMethod();
						if (delegateOriginal is MethodInfo methodInfo)
						{
							var delegateConstructor =
								patchParam.ParameterType.GetConstructor(new[] {typeof(object), typeof(IntPtr)});
							if (delegateConstructor is object)
							{
								var originalType = original.DeclaringType;
								if (methodInfo.IsStatic)
									il.Emit(OpCodes.Ldnull);
								else
								{
									il.Emit(OpCodes.Ldarg_0);
									if (originalType.IsValueType)
									{
										il.Emit(OpCodes.Ldobj, originalType);
										il.Emit(OpCodes.Box, originalType);
									}
								}

								if (methodInfo.IsStatic is false && harmonyMethod.nonVirtualDelegate is false)
								{
									il.Emit(OpCodes.Dup);
									il.Emit(OpCodes.Ldvirtftn, methodInfo);
								}
								else
									il.Emit(OpCodes.Ldftn, methodInfo);

								il.Emit(OpCodes.Newobj, delegateConstructor);
								continue;
							}
						}

						throw new Exception(
							$"Parameter \"{patchParam.Name}\" not found in method {original.FullDescription()}");
					}
				}

				//   original -> patch     opcode
				// --------------------------------------
				// 1 normal   -> normal  : LDARG
				// 2 normal   -> ref/out : LDARGA
				// 3 ref/out  -> normal  : LDARG, LDIND_x
				// 4 ref/out  -> ref/out : LDARG
				//
				var originalParamType = originalParameters[idx].ParameterType;
				var originalParamElementType = originalParamType.IsByRef ? originalParamType.GetElementType() : originalParamType;
				var patchParamType = patchParam.ParameterType;
				var patchParamElementType = patchParamType.IsByRef ? patchParamType.GetElementType() : patchParamType;
				var originalIsNormal = originalParameters[idx].IsOut is false && originalParamType.IsByRef is false;
				var patchIsNormal = patchParam.IsOut is false && patchParamType.IsByRef is false;
				var needsBoxing = originalParamElementType.IsValueType && patchParamElementType.IsValueType is false;
				var patchArgIndex = idx + (isInstance ? 1 : 0);

				// Case 1 + 4
				if (originalIsNormal == patchIsNormal)
				{
					il.Emit(OpCodes.Ldarg, patchArgIndex);
					if (needsBoxing)
					{
						if (patchIsNormal)
							il.Emit(OpCodes.Box, originalParamElementType);
						else
						{
							il.Emit(OpCodes.Ldobj, originalParamElementType);
							il.Emit(OpCodes.Box, originalParamElementType);
							var tmpBoxVar = il.DeclareVariable(patchParamElementType);
							il.Emit(OpCodes.Stloc, tmpBoxVar);
							il.Emit(OpCodes.Ldloca_S, tmpBoxVar);
							tmpBoxVars.Add(new ArgumentBoxInfo { index = patchArgIndex, type = originalParamElementType, tmpVar = tmpBoxVar, isByRef = true });
						}
					}
					continue;
				}

				// Case 2
				if (originalIsNormal && patchIsNormal is false)
				{
					if (needsBoxing)
					{
						il.Emit(OpCodes.Ldarg, patchArgIndex);
						il.Emit(OpCodes.Box, originalParamElementType);
						var tmpBoxVar = il.DeclareVariable(patchParamElementType);
						il.Emit(OpCodes.Stloc, tmpBoxVar);
						il.Emit(OpCodes.Ldloca_S, tmpBoxVar);
						// Store value for unboxing here as well since we want to replace the argument value
						// e.g. replace argument value in prefix
						tmpBoxVars.Add(new ArgumentBoxInfo { index = patchArgIndex, type = originalParamElementType, tmpVar = tmpBoxVar, isByRef = false });
					}
					else
						il.Emit(OpCodes.Ldarga, patchArgIndex);
					continue;
				}

				// Case 3
				il.Emit(OpCodes.Ldarg, patchArgIndex);
				if (needsBoxing)
				{
					il.Emit(OpCodes.Ldobj, originalParamElementType);
					il.Emit(OpCodes.Box, originalParamElementType);
				}
				else
				{
					if (originalParamElementType.IsValueType)
						il.Emit(OpCodes.Ldobj, originalParamElementType);
					else
						il.Emit(GetIndOpcode(originalParameters[idx].ParameterType));
				}
			}
		}
	}
}
