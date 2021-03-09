using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using MonoMod.Utils;

namespace HarmonyLib.Internal.Util
{
    internal static class MethodBodyLogExtensions
    {
        /// <summary>
        ///     Write method body to a ILDasm -like representation
        /// </summary>
        /// <param name="mb">Method body to write</param>
        /// <returns>String representation of the method body (locals and instruction)</returns>
        /// <exception cref="ArgumentOutOfRangeException">Unexpected exception block type</exception>
        public static string ToILDasmString(this MethodBody mb)
        {
            var sb = new StringBuilder();

            var instrs = mb.Instructions;

            // Fixup the the offsets
            var prev = instrs.First();
            prev.Offset = 0;
            foreach (var ins in instrs.Skip(1))
            {
                var operand = prev.Operand;

                if (operand is ILLabel label)
                    prev.Operand = label.Target;
                else if (operand is ILLabel[] labels)
                    prev.Operand = labels.Select(l => l.Target).ToArray();

                ins.Offset = prev.Offset + prev.GetSize();

                prev.Operand = operand;

                prev = ins;
            }

            // Cache exception blocks for pretty printing
            var exBlocks = new Dictionary<Instruction, List<ExceptionBlock>>();

            ExceptionBlock AddBlock(Instruction ins, ExceptionBlockType t)
            {
                if (ins == null)
                    return new ExceptionBlock(0);
                if (!exBlocks.TryGetValue(ins, out var list))
                    exBlocks[ins] = list = new List<ExceptionBlock>();
                var block = new ExceptionBlock(t);
                list.Add(block);
                return block;
            }

            foreach (var exHandler in mb.ExceptionHandlers)
            {
                AddBlock(exHandler.TryStart, ExceptionBlockType.BeginExceptionBlock);
                AddBlock(exHandler.TryEnd, ExceptionBlockType.EndExceptionBlock);
                AddBlock(exHandler.HandlerEnd, ExceptionBlockType.EndExceptionBlock);
                switch (exHandler.HandlerType)
                {
                    case ExceptionHandlerType.Catch:
                        AddBlock(exHandler.HandlerStart, ExceptionBlockType.BeginCatchBlock).catchType =
                            exHandler.CatchType.ResolveReflection();
                        break;
                    case ExceptionHandlerType.Filter:
                        AddBlock(exHandler.FilterStart, ExceptionBlockType.BeginExceptFilterBlock);
                        break;
                    case ExceptionHandlerType.Finally:
                        AddBlock(exHandler.HandlerStart, ExceptionBlockType.BeginFinallyBlock);
                        break;
                    case ExceptionHandlerType.Fault:
                        AddBlock(exHandler.HandlerStart, ExceptionBlockType.BeginFaultBlock);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var indent = 0;
            const int indentAmount = 2;

            void WriteLine(string text)
            {
                sb.Append(new string(' ', indent)).AppendLine(text);
            }

            WriteLine(".locals init (");
            indent += indentAmount * 2;
            for (var index = 0; index < mb.Variables.Count; index++)
            {
                var variableDefinition = mb.Variables[index];
                WriteLine($"{variableDefinition.VariableType.FullName} V_{index}");
            }

            indent -= indentAmount * 2;
            WriteLine(")");

            var handlerStack = new Stack<string>();
            foreach (var ins in instrs)
            {
                var blocks = exBlocks.GetValueSafe(ins) ?? new List<ExceptionBlock>();

                // Force exception close to the start for correct output
                blocks.Sort((a, b) => a.blockType == ExceptionBlockType.EndExceptionBlock ? -1 : 0);

                foreach (var exceptionBlock in blocks)
                    switch (exceptionBlock.blockType)
                    {
                        case ExceptionBlockType.BeginExceptionBlock:
                            WriteLine(".try");
                            WriteLine("{");
                            indent += indentAmount;
                            handlerStack.Push(".try");
                            break;
                        case ExceptionBlockType.BeginCatchBlock:
                            WriteLine($"catch {exceptionBlock.catchType.FullName}");
                            WriteLine("{");
                            indent += indentAmount;
                            handlerStack.Push("handler (catch)");
                            break;
                        case ExceptionBlockType.BeginExceptFilterBlock:
                            WriteLine("filter");
                            WriteLine("{");
                            indent += indentAmount;
                            handlerStack.Push("handler (filter)");
                            break;
                        case ExceptionBlockType.BeginFaultBlock:
                            WriteLine("fault");
                            WriteLine("{");
                            indent += indentAmount;
                            handlerStack.Push("handler (fault)");
                            break;
                        case ExceptionBlockType.BeginFinallyBlock:
                            WriteLine("finally");
                            WriteLine("{");
                            indent += indentAmount;
                            handlerStack.Push("handler (finally)");
                            break;
                        case ExceptionBlockType.EndExceptionBlock:
                            indent -= indentAmount;
                            WriteLine($"}} // end {handlerStack.Pop()}");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                var operand = ins.Operand;

                if (operand is ILLabel label)
                    ins.Operand = label.Target;
                else if (operand is ILLabel[] labels)
                    ins.Operand = labels.Select(l => l.Target).ToArray();

                WriteLine(ins.ToString());

                ins.Operand = operand;
            }

            return sb.ToString();
        }
    }
}