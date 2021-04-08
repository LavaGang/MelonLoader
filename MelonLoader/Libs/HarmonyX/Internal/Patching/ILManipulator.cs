using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib.Internal.Util;
using HarmonyLib.Tools;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;
using MonoMod.Utils.Cil;
using MethodBody = Mono.Cecil.Cil.MethodBody;
using SRE = System.Reflection.Emit;

namespace HarmonyLib.Internal.Patching
{
	/// <summary>
	///    High-level IL code manipulator for MonoMod that allows to manipulate a method as a stream of CodeInstructions.
	/// </summary>
	internal class ILManipulator
	{
		private static readonly Dictionary<short, SRE.OpCode> SREOpCodes = new Dictionary<short, SRE.OpCode>();
		private static readonly Dictionary<short, OpCode> CecilOpCodes = new Dictionary<short, OpCode>();

		private static readonly Dictionary<SRE.OpCode, SRE.OpCode> ShortToLongMap = new Dictionary<SRE.OpCode, SRE.OpCode>
		{
			[SRE.OpCodes.Beq_S] = SRE.OpCodes.Beq,
			[SRE.OpCodes.Bge_S] = SRE.OpCodes.Bge,
			[SRE.OpCodes.Bge_Un_S] = SRE.OpCodes.Bge_Un,
			[SRE.OpCodes.Bgt_S] = SRE.OpCodes.Bgt,
			[SRE.OpCodes.Bgt_Un_S] = SRE.OpCodes.Bgt_Un,
			[SRE.OpCodes.Ble_S] = SRE.OpCodes.Ble,
			[SRE.OpCodes.Ble_Un_S] = SRE.OpCodes.Ble_Un,
			[SRE.OpCodes.Blt_S] = SRE.OpCodes.Blt,
			[SRE.OpCodes.Blt_Un_S] = SRE.OpCodes.Blt_Un,
			[SRE.OpCodes.Bne_Un_S] = SRE.OpCodes.Bne_Un,
			[SRE.OpCodes.Brfalse_S] = SRE.OpCodes.Brfalse,
			[SRE.OpCodes.Brtrue_S] = SRE.OpCodes.Brtrue,
			[SRE.OpCodes.Br_S] = SRE.OpCodes.Br,
			[SRE.OpCodes.Leave_S] = SRE.OpCodes.Leave
		};

		private readonly IEnumerable<RawInstruction> codeInstructions;
		private readonly List<MethodInfo> transpilers = new List<MethodInfo>();
		private readonly Dictionary<VariableDefinition, SRE.LocalBuilder> localsCache = new Dictionary<VariableDefinition, SRE.LocalBuilder>();

		static ILManipulator()
		{
			foreach (var field in typeof(SRE.OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
			{
				var sreOpCode = (SRE.OpCode)field.GetValue(null);
				SREOpCodes[sreOpCode.Value] = sreOpCode;
			}

			foreach (var field in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
			{
				var cecilOpCode = (OpCode)field.GetValue(null);
				CecilOpCodes[cecilOpCode.Value] = cecilOpCode;
			}
		}

		/// <summary>
		///    Initialize IL transpiler
		/// </summary>
		/// <param name="body">Body of the method to transpile</param>
		public ILManipulator(MethodBody body)
		{
			Body = body;
			codeInstructions = ReadBody(body);
		}

		public MethodBody Body { get; }

		private int GetTarget(MethodBody body, object insOp)
		{
			if (insOp is ILLabel lab)
				return body.Instructions.IndexOf(lab.Target);
			if (insOp is Instruction ins)
				return body.Instructions.IndexOf(ins);
			return -1;
		}

		private int[] GetTargets(MethodBody body, object insOp)
		{
			int[] Result<T>(IEnumerable<T> arr, Func<T, Instruction> insGetter)
			{
				return arr.Select(i => body.Instructions.IndexOf(insGetter(i))).ToArray();
			}

			if (insOp is ILLabel[] labs)
				return Result(labs, l => l.Target);
			if (insOp is Instruction[] ins)
				return Result(ins, i => i);
			return new int[0];
		}

		private IEnumerable<RawInstruction> ReadBody(MethodBody body)
		{
			var instructions = new List<RawInstruction>(body.Instructions.Count);

			RawInstruction ReadInstruction(Instruction ins)
			{
				return new RawInstruction
				{
					Instruction = new CodeInstruction(SREOpCodes[ins.OpCode.Value]),
					Operand = ins.OpCode.OperandType switch
					{
						OperandType.InlineField => ((MemberReference)ins.Operand).ResolveReflection(),
						OperandType.InlineMethod => ((MemberReference)ins.Operand).ResolveReflection(),
						OperandType.InlineType => ((MemberReference)ins.Operand).ResolveReflection(),
						OperandType.InlineTok => ((MemberReference)ins.Operand).ResolveReflection(),
						OperandType.InlineVar => (VariableDefinition)ins.Operand,
						OperandType.ShortInlineVar => (VariableDefinition)ins.Operand,
						// Handle Harmony's speciality of using smaller types for indices in ld/starg
						OperandType.InlineArg => (short)((ParameterDefinition)ins.Operand).Index,
						OperandType.ShortInlineArg => (byte)((ParameterDefinition)ins.Operand).Index,
						OperandType.InlineBrTarget => GetTarget(body, ins.Operand),
						OperandType.ShortInlineBrTarget => GetTarget(body, ins.Operand),
						OperandType.InlineSwitch => GetTargets(body, ins.Operand),
						_ => ins.Operand
					},
					CILInstruction = ins
				};
			}

			// Pass 1: Convert IL to base abstract CodeInstructions
			instructions.AddRange(body.Instructions.Select(ReadInstruction));

			//Pass 2: Resolve CodeInstructions for branch parameters
			foreach (var uIns in instructions)
				uIns.Operand = uIns.Instruction.opcode.OperandType switch
				{
					SRE.OperandType.ShortInlineBrTarget => instructions[(int)uIns.Operand].Instruction,
					SRE.OperandType.InlineBrTarget => instructions[(int)uIns.Operand].Instruction,
					SRE.OperandType.InlineSwitch => ((int[])uIns.Operand).Select(i => instructions[i].Instruction).ToArray(),
					_ => uIns.Operand
				};

			// Pass 3: Attach exception blocks to each code instruction
			foreach (var exception in body.ExceptionHandlers)
			{
				var tryStart = instructions[body.Instructions.IndexOf(exception.TryStart)].Instruction;
				var tryEnd = instructions[body.Instructions.IndexOf(exception.TryEnd)].Instruction;
				var handlerStart = instructions[body.Instructions.IndexOf(exception.HandlerStart)].Instruction;
				var handlerEnd = instructions[body.Instructions.IndexOf(exception.HandlerEnd.Previous)].Instruction;

				tryStart.blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginExceptionBlock));
				handlerEnd.blocks.Add(new ExceptionBlock(ExceptionBlockType.EndExceptionBlock));

				switch (exception.HandlerType)
				{
					case ExceptionHandlerType.Catch:
						handlerStart.blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginCatchBlock,
							exception.CatchType.ResolveReflection()));
						break;
					case ExceptionHandlerType.Filter:
						var filterStart = instructions[body.Instructions.IndexOf(exception.FilterStart)].Instruction;
						filterStart.blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginExceptFilterBlock));
						break;
					case ExceptionHandlerType.Finally:
						handlerStart.blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginFinallyBlock));
						break;
					case ExceptionHandlerType.Fault:
						handlerStart.blocks.Add(new ExceptionBlock(ExceptionBlockType.BeginFaultBlock));
						break;
				}
			}

			return instructions;
		}

		/// <summary>
		///    Adds a transpiler method that edits the IL of the given method
		/// </summary>
		/// <param name="transpiler">Transpiler method</param>
		/// <exception cref="NotImplementedException">Currently not implemented</exception>
		public void AddTranspiler(MethodInfo transpiler)
		{
			transpilers.Add(transpiler);
		}

		private object[] GetTranspilerArguments(SRE.ILGenerator il, MethodInfo transpiler,
			IEnumerable<CodeInstruction> instructions, MethodBase orignal = null)
		{
			var result = new List<object>();

			foreach (var type in transpiler.GetParameters().Select(p => p.ParameterType))
				if (type.IsAssignableFrom(typeof(SRE.ILGenerator)))
					result.Add(il);
				else if (type.IsAssignableFrom(typeof(MethodBase)) && orignal != null)
					result.Add(orignal);
				else if (type.IsAssignableFrom(typeof(IEnumerable<CodeInstruction>)))
					result.Add(instructions);

			return result.ToArray();
		}

		public IEnumerable<KeyValuePair<SRE.OpCode, object>> GetRawInstructions()
		{
			return codeInstructions.Select(i => new KeyValuePair<SRE.OpCode, object>(i.Instruction.opcode, i.Operand));
		}

		public List<CodeInstruction> GetInstructions(SRE.ILGenerator il, MethodBase original = null)
		{
			return MakeBranchesLong(ApplyTranspilers(il, original,
				vDef => il.DeclareLocal(vDef.VariableType.ResolveReflection()), il.DefineLabel)).ToList();
		}

		private IEnumerable<CodeInstruction> ApplyTranspilers(SRE.ILGenerator il, MethodBase original,
			Func<VariableDefinition, SRE.LocalBuilder> getLocal, Func<SRE.Label> defineLabel)
		{
			// Step 1: Prepare labels for instructions. Use ToList to force
			var instructions = Prepare(getLocal, defineLabel).Select(i => i.Instruction).ToList();

			if (transpilers.Count == 0)
				return instructions;

			// Step 2: Run the code instructions through transpilers
			var tempInstructions = MakeBranchesLong(instructions);

			foreach (var transpiler in transpilers)
			{
				var args = GetTranspilerArguments(il, transpiler, tempInstructions, original);

				Logger.Log(Logger.LogChannel.Info, () => $"Running transpiler {transpiler.FullDescription()}");
				var newInstructions = transpiler.Invoke(null, args) as IEnumerable<CodeInstruction>;
				tempInstructions = MakeBranchesLong(newInstructions).ToList();
			}

			return tempInstructions;
		}

		public Dictionary<int, CodeInstruction> GetIndexedInstructions(SRE.ILGenerator il)
		{
			static int Grow(ref int i, int s)
			{
				var result = i;
				i += s;
				return result;
			}

			var size = 0;
			return Prepare(vDef => il.DeclareLocal(vDef.VariableType.ResolveReflection()), il.DefineLabel)
				.ToDictionary(i => Grow(ref size, i.CILInstruction.GetSize()), i => i.Instruction);
		}

		private IEnumerable<RawInstruction> Prepare(Func<VariableDefinition, SRE.LocalBuilder> getLocal,
			Func<SRE.Label> defineLabel)
		{
			// First resolve all variables properly so that they are all defined in case of (st|ld)loc_N is used
			// This is especially useful
			localsCache.Clear();
			foreach (var variableDefinition in Body.Variables)
				localsCache[variableDefinition] = getLocal(variableDefinition);

			foreach (var unresolvedInstruction in codeInstructions)
			{
				// Set operand to the same as the IL operand (in most cases they are the same)
				unresolvedInstruction.Instruction.operand = unresolvedInstruction.Operand;

				switch (unresolvedInstruction.Instruction.opcode.OperandType)
				{
					case SRE.OperandType.InlineVar:
					case SRE.OperandType.ShortInlineVar:
					{
						if (unresolvedInstruction.Operand is VariableDefinition varDef)
							unresolvedInstruction.Instruction.operand = localsCache[varDef];
					}
						break;
					case SRE.OperandType.InlineSwitch when unresolvedInstruction.Operand is CodeInstruction[] targets:
					{
						var labels = new List<SRE.Label>();
						foreach (var target in targets)
						{
							var label = defineLabel();
							target.labels.Add(label);
							labels.Add(label);
						}

						unresolvedInstruction.Instruction.operand = labels.ToArray();
					}
						break;
					case SRE.OperandType.ShortInlineBrTarget:
					case SRE.OperandType.InlineBrTarget:
					{
						if (unresolvedInstruction.Instruction.operand is CodeInstruction target)
						{
							var label = defineLabel();
							target.labels.Add(label);
							unresolvedInstruction.Instruction.operand = label;
						}
					}
						break;
				}
			}

			return codeInstructions;
		}

		/// <summary>
		///    Processes and writes IL to the provided method body.
		///    Note that this cleans the existing method body (removes insturctions and exception handlers).
		/// </summary>
		/// <param name="body">Method body to write to.</param>
		/// <param name="original">Original method that transpiler can optionally call into</param>
		/// <exception cref="NotSupportedException">
		///    One of IL opcodes contains a CallSide (e.g. calli), which is currently not
		///    fully supported.
		/// </exception>
		/// <exception cref="ArgumentNullException">One of IL opcodes with an operand contains a null operand.</exception>
		public void WriteTo(MethodBody body, MethodBase original = null)
		{
			// Clean up the body of the target method
			body.Instructions.Clear();
			body.ExceptionHandlers.Clear();

			var il = new CecilILGenerator(body.GetILProcessor());
			var cil = il.GetProxy();

			// Define an "empty" label
			// In Harmony, the first label can point to the end of the method
			// Apparently, some transpilers naively call new Label() to define a label and thus end up
			// using the first label without knowing it
			// By defining the first label we'll ensure label count is correct
			il.DefineLabel();

			// Step 1: Apply transpilers
			// We don't remove trailing `ret`s because we need to do so only if prefixes/postfixes are present
			var newInstructions = ApplyTranspilers(cil, original, vDef => il.GetLocal(vDef), il.DefineLabel);

			// Step 2: Emit code
			foreach (var ins in newInstructions)
			{
				ins.labels.ForEach(l => il.MarkLabel(l));
				ins.blocks.ForEach(b => il.MarkBlockBefore(b));

				// We don't replace `ret`s yet because we might not need to
				// We do that only if we add prefixes/postfixes
				// We also don't need to care for long/short forms thanks to Cecil/MonoMod

				// Temporary fix: CecilILGenerator doesn't properly handle ldarg
				switch (ins.opcode.OperandType)
				{
					case SRE.OperandType.InlineNone:
						il.Emit(ins.opcode);
						break;
					case SRE.OperandType.InlineSig:
						throw new NotSupportedException(
							"Emitting opcodes with CallSites is currently not fully implemented");
					default:
						if (ins.operand == null)
							throw new ArgumentNullException(nameof(ins.operand), $"Invalid argument for {ins}");

						il.Emit(ins.opcode, ins.operand);
						break;
				}

				ins.blocks.ForEach(b => il.MarkBlockAfter(b));
			}

			// Note: We lose all unassigned labels here along with any way to log them
			// On the contrary, we gain better logging anyway down the line by using Cecil
		}

		/// <summary>
		///    Converts all branches to long types. This exists to mimic the behaviour of Harmony 2
		/// </summary>
		/// <param name="instrs">Enumerable of instructions</param>
		/// <returns>Enumerable of fixed instructions</returns>
		private static IEnumerable<CodeInstruction> MakeBranchesLong(IEnumerable<CodeInstruction> instrs)
		{
			// Yes, we mutate original objects to save speed
			foreach (var ins in instrs)
			{
				if (ShortToLongMap.TryGetValue(ins.opcode, out var longOpCode))
					ins.opcode = longOpCode;
				yield return ins;
			}
		}

		public static Dictionary<int, CodeInstruction> GetInstructions(MethodBody body)
		{
			if (body == null)
				return null;
			try
			{
				return new ILManipulator(body).GetIndexedInstructions(PatchProcessor.CreateILGenerator());
			}
			catch (Exception e)
			{
				Logger.Log(Logger.LogChannel.Warn,
					() => $"Could not read instructions of {body.Method.GetID()}: {e.Message}");
				return null;
			}
		}

		private class RawInstruction
		{
			public CodeInstruction Instruction { get; set; }
			public object Operand { get; set; }

			public Instruction CILInstruction { get; set; }
		}
	}
}
