using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using MonoMod.Utils;
using MonoMod.Utils.Cil;

namespace HarmonyLib
{
	/// <summary>A collection of commonly used transpilers</summary>
	///
	public static class Transpilers
	{
		// ReSharper disable once CollectionNeverQueried.Local => used by generated DMDs
		private static readonly Dictionary<int, Delegate> DelegateCache = new Dictionary<int, Delegate>();
		private static int delegateCounter;

		/// <summary>Returns an instruction to call the specified delegate</summary>
		/// <typeparam name="T">The delegate type to emit</typeparam>
		/// <param name="action">The delegate to emit</param>
		/// <returns>The instruction to call the specified action</returns>
		///
		public static CodeInstruction EmitDelegate<T>(T action) where T : Delegate
		{
			if (action.Method.IsStatic && action.Target == null) return new CodeInstruction(OpCodes.Call, action.Method);

			var paramTypes = action.Method.GetParameters().Select(x => x.ParameterType).ToArray();

			var dynamicMethod = new DynamicMethodDefinition(action.Method.Name,
				action.Method.ReturnType,
				paramTypes);

			var il = dynamicMethod.GetILGenerator();

			var targetType = action.Target.GetType();

			var preserveContext = action.Target != null && targetType.GetFields().Any(x => !x.IsStatic);

			if (preserveContext)
			{
				var currentDelegateCounter = delegateCounter++;

				DelegateCache[currentDelegateCounter] = action;

				var cacheField = AccessTools.Field(typeof(Transpilers), nameof(DelegateCache));

				var getMethod = AccessTools.Method(typeof(Dictionary<int, Delegate>), "get_Item");

				il.Emit(OpCodes.Ldsfld, cacheField);
				il.Emit(OpCodes.Ldc_I4, currentDelegateCounter);
				il.Emit(OpCodes.Callvirt, getMethod);
			}
			else
			{
				if (action.Target == null)
					il.Emit(OpCodes.Ldnull);
				else
					il.Emit(OpCodes.Newobj,
						AccessTools.FirstConstructor(targetType, x => x.GetParameters().Length == 0 && !x.IsStatic));

				il.Emit(OpCodes.Ldftn, action.Method);
				il.Emit(OpCodes.Newobj, AccessTools.Constructor(typeof(T), new[] {typeof(object), typeof(IntPtr)}));
			}

			for (var i = 0; i < paramTypes.Length; i++)
				il.Emit(OpCodes.Ldarg_S, (short)i);

			il.Emit(OpCodes.Callvirt, AccessTools.Method(typeof(T), "Invoke"));
			il.Emit(OpCodes.Ret);

			return new CodeInstruction(OpCodes.Call, dynamicMethod.Generate());
		}

		/// <summary>A transpiler that replaces all occurrences of a given method with another one using the same signature</summary>
		/// <param name="instructions">The enumeration of <see cref="CodeInstruction"/> to act on</param>
		/// <param name="from">Method or constructor to search for</param>
		/// <param name="to">Method or constructor to replace with</param>
		/// <returns>Modified enumeration of <see cref="CodeInstruction"/></returns>
		///
		public static IEnumerable<CodeInstruction> MethodReplacer(this IEnumerable<CodeInstruction> instructions,
			MethodBase from, MethodBase to)
		{
			if (from is null)
				throw new ArgumentException("Unexpected null argument", nameof(from));
			if (to is null)
				throw new ArgumentException("Unexpected null argument", nameof(to));

			foreach (var instruction in instructions)
			{
				var method = instruction.operand as MethodBase;
				if (method == from)
				{
					instruction.opcode = to.IsConstructor ? OpCodes.Newobj : OpCodes.Call;
					instruction.operand = to;
				}

				yield return instruction;
			}
		}

		/// <summary>A transpiler that alters instructions that match a predicate by calling an action</summary>
		/// <param name="instructions">The enumeration of <see cref="CodeInstruction"/> to act on</param>
		/// <param name="predicate">A predicate selecting the instructions to change</param>
		/// <param name="action">An action to apply to matching instructions</param>
		/// <returns>Modified enumeration of <see cref="CodeInstruction"/></returns>
		///
		public static IEnumerable<CodeInstruction> Manipulator(this IEnumerable<CodeInstruction> instructions,
			Func<CodeInstruction, bool> predicate, Action<CodeInstruction> action)
		{
			if (predicate is null)
				throw new ArgumentNullException(nameof(predicate));
			if (action is null)
				throw new ArgumentNullException(nameof(action));

			return instructions.Select(instruction =>
			{
				if (predicate(instruction))
					action(instruction);
				return instruction;
			}).AsEnumerable();
		}

		/// <summary>A transpiler that logs a text at the beginning of the method</summary>
		/// <param name="instructions">The instructions to act on</param>
		/// <param name="text">The log text</param>
		/// <returns>Modified enumeration of <see cref="CodeInstruction"/></returns>
		///
		public static IEnumerable<CodeInstruction> DebugLogger(this IEnumerable<CodeInstruction> instructions,
			string text)
		{
			yield return new CodeInstruction(OpCodes.Ldstr, text);
			yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(FileLog), nameof(FileLog.Log)));
			foreach (var instruction in instructions) yield return instruction;
		}

		/// <summary>A transpiler that replaces the entire body of the method with another one</summary>
		/// <param name="replacement">The replacement method. It's up to the caller of this transpiler to make sure that the signatures match.</param>
		/// <param name="ilGenerator"><see cref="ILGenerator"/> of the patch. This is passed via transpiler.</param>
		/// <returns>A collection of <see cref="CodeInstruction"/> that contains instructions of replacement method.</returns>
		/// <exception cref="ArgumentException">The replacement method is not a managed method that contains any IL.</exception>
		/// <remarks>This transpiler has a side effect of clearing up all previous locals and previous transpilers.
		/// Use <see cref="HarmonyPriority"/> to run this transpiler as early as possible.</remarks>
		public static IEnumerable<CodeInstruction> ReplaceWith(MethodBase replacement, ILGenerator ilGenerator)
		{
			var body = replacement.GetMethodBody();
			if (body == null)
				throw new ArgumentException("Replacement method must be a managed method", nameof(replacement));

			// Currently HarmonyX is based on DMD which always uses a special ILGenerator wrapper for CecilILGenerator
			var gen = Traverse.Create(ilGenerator).Field("Target").GetValue<CecilILGenerator>();
			// Clear all existing vars; this is fine for HarmonyX because of how it works
			gen.IL.Body.Variables.Clear();

			// Create a copy of replacement and reinsert back the locals correctly
			return PatchProcessor.GetOriginalInstructions(replacement, ilGenerator);
		}
	}
}
