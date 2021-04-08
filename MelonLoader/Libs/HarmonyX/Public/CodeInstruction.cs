﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace HarmonyLib
{
	/// <summary>An abstract wrapper around OpCode and their operands. Used by transpilers</summary>
	///
	public class CodeInstruction
	{
		/// <summary>The opcode</summary>
		///
		public OpCode opcode;

		/// <summary>The operand</summary>
		///
		public object operand;

		/// <summary>All labels defined on this instruction</summary>
		///
		public List<Label> labels = new List<Label>();

		/// <summary>All exception block boundaries defined on this instruction</summary>
		///
		public List<ExceptionBlock> blocks = new List<ExceptionBlock>();

		// Internal parameterless constructor that AccessTools.CreateInstance can use, ensuring that labels/blocks are initialized.
		internal CodeInstruction()
		{
		}

		/// <summary>Creates a new CodeInstruction with a given opcode and optional operand</summary>
		/// <param name="opcode">The opcode</param>
		/// <param name="operand">The operand</param>
		///
		public CodeInstruction(OpCode opcode, object operand = null)
		{
			this.opcode = opcode;
			this.operand = operand;
		}

		/// <summary>Create a full copy (including labels and exception blocks) of a CodeInstruction</summary>
		/// <param name="instruction">The <see cref="CodeInstruction"/> to copy</param>
		///
		public CodeInstruction(CodeInstruction instruction)
		{
			opcode = instruction.opcode;
			operand = instruction.operand;
			labels = instruction.labels.ToList();
			blocks = instruction.blocks.ToList();
		}

		// --- CLONING

		/// <summary>Clones a CodeInstruction and resets its labels and exception blocks</summary>
		/// <returns>A lightweight copy of this code instruction</returns>
		///
		public CodeInstruction Clone()
		{
			return new CodeInstruction(this)
			{
				labels = new List<Label>(),
				blocks = new List<ExceptionBlock>()
			};
		}

		/// <summary>Clones a CodeInstruction, resets labels and exception blocks and sets its opcode</summary>
		/// <param name="opcode">The opcode</param>
		/// <returns>A copy of this CodeInstruction with a new opcode</returns>
		///
		public CodeInstruction Clone(OpCode opcode)
		{
			var instruction = Clone();
			instruction.opcode = opcode;
			return instruction;
		}

		/// <summary>Clones a CodeInstruction, resets labels and exception blocks and sets its operand</summary>
		/// <param name="operand">The operand</param>
		/// <returns>A copy of this CodeInstruction with a new operand</returns>
		///
		public CodeInstruction Clone(object operand)
		{
			var instruction = Clone();
			instruction.operand = operand;
			return instruction;
		}

		// --- CALLING

		/// <summary>Creates a CodeInstruction calling a method (CALL)</summary>
		/// <param name="type">The class/type where the method is declared</param>
		/// <param name="name">The name of the method (case sensitive)</param>
		/// <param name="parameters">Optional parameters to target a specific overload of the method</param>
		/// <param name="generics">Optional list of types that define the generic version of the method</param>
		/// <returns>A code instruction that calls the method matching the arguments</returns>
		///
		public static CodeInstruction Call(Type type, string name, Type[] parameters = null, Type[] generics = null)
		{
			var method = AccessTools.Method(type, name, parameters, generics);
			if (method is null) throw new ArgumentException($"No method found for type={type}, name={name}, parameters={parameters.Description()}, generics={generics.Description()}");
			return new CodeInstruction(OpCodes.Call, method);
		}

		/// <summary>Creates a CodeInstruction calling a method (CALL)</summary>
		/// <param name="typeColonMethodname">The target method in the form <c>TypeFullName:MethodName</c>, where the type name matches a form recognized by <a href="https://docs.microsoft.com/en-us/dotnet/api/system.type.gettype">Type.GetType</a> like <c>Some.Namespace.Type</c>.</param>
		/// <param name="parameters">Optional parameters to target a specific overload of the method</param>
		/// <param name="generics">Optional list of types that define the generic version of the method</param>
		/// <returns>A code instruction that calls the method matching the arguments</returns>
		///
		public static CodeInstruction Call(string typeColonMethodname, Type[] parameters = null, Type[] generics = null)
		{
			var method = AccessTools.Method(typeColonMethodname, parameters, generics);
			if (method is null) throw new ArgumentException($"No method found for {typeColonMethodname}, parameters={parameters.Description()}, generics={generics.Description()}");
			return new CodeInstruction(OpCodes.Call, method);
		}

		/// <summary>Creates a CodeInstruction calling a method (CALL)</summary>
		/// <param name="expression">The lambda expression using the method</param>
		/// <returns></returns>
		///
		public static CodeInstruction Call(Expression<Action> expression)
		{
			return new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(expression));
		}

		/// <summary>Creates a CodeInstruction calling a method (CALL)</summary>
		/// <param name="expression">The lambda expression using the method</param>
		/// <returns></returns>
		///
		public static CodeInstruction Call<T>(Expression<Action<T>> expression)
		{
			return new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(expression));
		}

		/// <summary>Creates a CodeInstruction calling a method (CALL)</summary>
		/// <param name="expression">The lambda expression using the method</param>
		/// <returns></returns>
		///
		public static CodeInstruction Call<T, TResult>(Expression<Func<T, TResult>> expression)
		{
			return new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(expression));
		}

		/// <summary>Creates a CodeInstruction calling a method (CALL)</summary>
		/// <param name="expression">The lambda expression using the method</param>
		/// <returns></returns>
		///
		public static CodeInstruction Call(LambdaExpression expression)
		{
			return new CodeInstruction(OpCodes.Call, SymbolExtensions.GetMethodInfo(expression));
		}

		// --- FIELDS

		/// <summary>Creates a CodeInstruction loading a field (LD[S]FLD[A])</summary>
		/// <param name="type">The class/type where the field is defined</param>
		/// <param name="name">The name of the field (case sensitive)</param>
		/// <param name="useAddress">Use address of field</param>
		/// <returns></returns>
		public static CodeInstruction LoadField(Type type, string name, bool useAddress = false)
		{
			var field = AccessTools.Field(type, name);
			if (field is null) throw new ArgumentException($"No field found for {type} and {name}");
			return new CodeInstruction(useAddress ? (field.IsStatic ? OpCodes.Ldsflda : OpCodes.Ldflda) : (field.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld), field);
		}

		/// <summary>Creates a CodeInstruction storing to a field (ST[S]FLD)</summary>
		/// <param name="type">The class/type where the field is defined</param>
		/// <param name="name">The name of the field (case sensitive)</param>
		/// <returns></returns>
		public static CodeInstruction StoreField(Type type, string name)
		{
			var field = AccessTools.Field(type, name);
			if (field is null) throw new ArgumentException($"No field found for {type} and {name}");
			return new CodeInstruction(field.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld, field);
		}

		// --- TOSTRING

		/// <summary>Returns a string representation of the code instruction</summary>
		/// <returns>A string representation of the code instruction</returns>
		///
		public override string ToString()
		{
			var list = new List<string>();
			foreach (var label in labels)
				list.Add($"Label{label.GetHashCode()}");
			foreach (var block in blocks)
				list.Add($"EX_{block.blockType.ToString().Replace("Block", "")}");

			var extras = list.Count > 0 ? $" [{string.Join(", ", list.ToArray())}]" : "";
			var operandStr = FormatArgument(operand);
			if (operandStr.Length > 0) operandStr = " " + operandStr;
			return opcode + operandStr + extras;
		}

		internal static string FormatArgument(object argument, string extra = null)
		{
			if (argument is null) return "NULL";
			var type = argument.GetType();

			if (argument is MethodBase method)
				return method.FullDescription() + (extra is object ? " " + extra : "");

			if (argument is FieldInfo field)
				return $"{field.FieldType.FullDescription()} {field.DeclaringType.FullDescription()}::{field.Name}";

			if (type == typeof(Label))
				return $"Label{((Label)argument).GetHashCode()}";

			if (type == typeof(Label[]))
				return $"Labels{string.Join(",", ((Label[])argument).Select(l => l.GetHashCode().ToString()).ToArray())}";

			if (type == typeof(LocalBuilder))
				return $"{((LocalBuilder)argument).LocalIndex} ({((LocalBuilder)argument).LocalType})";

			if (type == typeof(string))
				return argument.ToString().ToLiteral();

			return argument.ToString().Trim();
		}
	}
}
