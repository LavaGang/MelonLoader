using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Utils;

namespace HarmonyLib.Internal.Util
{
    /// <summary>
    /// Helper wrapper around ILProcessor to allow emitting code at certain positions
    /// </summary>
    internal class ILEmitter
    {
        public readonly ILProcessor IL;

        private readonly List<LabelledExceptionHandler> pendingExceptionHandlers = new List<LabelledExceptionHandler>();

        private readonly List<Label> pendingLabels = new List<Label>();
        public Instruction emitBefore;

        public ILEmitter(ILProcessor il)
        {
            IL = il;
            if (IL.Body.Instructions.Count == 0)
                IL.Emit(OpCodes.Nop);
        }

        private Instruction Target => emitBefore ?? IL.Body.Instructions[IL.Body.Instructions.Count - 1];

        public ExceptionBlock BeginExceptionBlock(Label start)
        {
            return new ExceptionBlock {start = start};
        }

        public void EndExceptionBlock(ExceptionBlock block)
        {
            EndHandler(block, block.cur);
        }

        public void BeginHandler(ExceptionBlock block, ExceptionHandlerType handlerType, Type exceptionType = null)
        {
            var prev = block.prev = block.cur;
            if (prev != null)
                EndHandler(block, prev);

            block.skip = DeclareLabel();

            Emit(OpCodes.Leave, block.skip);

            var handlerLabel = DeclareLabel();
            MarkLabel(handlerLabel);
            block.cur = new LabelledExceptionHandler
            {
                tryStart = block.start,
                tryEnd = handlerLabel,
                handlerType = handlerType,
                handlerEnd = block.skip,
                exceptionType = exceptionType != null ? IL.Import(exceptionType) : null
            };
            if (handlerType == ExceptionHandlerType.Filter)
                block.cur.filterStart = handlerLabel;
            else
                block.cur.handlerStart = handlerLabel;
        }

        public void EndHandler(ExceptionBlock block, LabelledExceptionHandler handler)
        {
            switch (handler.handlerType)
            {
                case ExceptionHandlerType.Filter:
                    Emit(OpCodes.Endfilter);
                    break;
                case ExceptionHandlerType.Finally:
                    Emit(OpCodes.Endfinally);
                    break;
                default:
                    Emit(OpCodes.Leave, block.skip);
                    break;
            }

            MarkLabel(block.skip);
            pendingExceptionHandlers.Add(block.cur);
        }

        public VariableDefinition DeclareVariable(Type type)
        {
            var varDef = new VariableDefinition(IL.Import(type));
            IL.Body.Variables.Add(varDef);
            return varDef;
        }

        public Label DeclareLabel()
        {
            return new Label();
        }

        public Label DeclareLabelFor(Instruction ins)
        {
            return new Label
            {
                emitted = true,
                instruction = ins
            };
        }

        public void MarkLabel(Label label)
        {
            if (label.emitted)
                return;
            pendingLabels.Add(label);
        }

        public Instruction SetOpenLabelsTo(Instruction ins)
        {
            if (pendingLabels.Count != 0)
            {
                foreach (var pendingLabel in pendingLabels)
                {
                    foreach (var targetIns in pendingLabel.targets)
                        if (targetIns.Operand is Instruction)
                            targetIns.Operand = ins;
                        else if (targetIns.Operand is Instruction[] targets)
                            for (var i = 0; i < targets.Length; i++)
                                if (targets[i] == pendingLabel.instruction)
                                {
                                    targets[i] = ins;
                                    break;
                                }

                    pendingLabel.instruction = ins;
                    pendingLabel.emitted = true;
                }

                pendingLabels.Clear();
            }

            if (pendingExceptionHandlers.Count != 0)
            {
                foreach (var exHandler in pendingExceptionHandlers)
                    IL.Body.ExceptionHandlers.Add(new ExceptionHandler(exHandler.handlerType)
                    {
                        TryStart = exHandler.tryStart?.instruction,
                        TryEnd = exHandler.tryEnd?.instruction,
                        FilterStart = exHandler.filterStart?.instruction,
                        HandlerStart = exHandler.handlerStart?.instruction,
                        HandlerEnd = exHandler.handlerEnd?.instruction,
                        CatchType = exHandler.exceptionType
                    });

                pendingExceptionHandlers.Clear();
            }

            return ins;
        }

        public void Emit(OpCode opcode)
        {
            IL.InsertBefore(Target, SetOpenLabelsTo(IL.Create(opcode)));
        }

        public void Emit(OpCode opcode, Label label)
        {
            var ins = SetOpenLabelsTo(IL.Create(opcode, label.instruction));
            label.targets.Add(ins);
            IL.InsertBefore(Target, ins);
        }

        public void Emit(OpCode opcode, ConstructorInfo cInfo)
        {
            IL.InsertBefore(Target, SetOpenLabelsTo(IL.Create(opcode, IL.Import(cInfo))));
        }

        public void Emit(OpCode opcode, MethodInfo mInfo)
        {
            IL.InsertBefore(Target, SetOpenLabelsTo(IL.Create(opcode, IL.Import(mInfo))));
        }

        public void Emit(OpCode opcode, Type cls)
        {
            IL.InsertBefore(Target, SetOpenLabelsTo(IL.Create(opcode, IL.Import(cls))));
        }

        public void Emit(OpCode opcode, int arg)
        {
            IL.InsertBefore(Target, SetOpenLabelsTo(IL.Create(opcode, arg)));
        }

        public void Emit(OpCode opcode, string arg)
        {
	        IL.InsertBefore(Target, SetOpenLabelsTo(IL.Create(opcode, arg)));
        }

        public void Emit(OpCode opcode, FieldInfo fInfo)
        {
            IL.InsertBefore(Target, SetOpenLabelsTo(IL.Create(opcode, IL.Import(fInfo))));
        }

        public void Emit(OpCode opcode, VariableDefinition varDef)
        {
            IL.InsertBefore(Target, SetOpenLabelsTo(IL.Create(opcode, varDef)));
        }

        public class Label
        {
            public bool emitted;
            public Instruction instruction = Instruction.Create(OpCodes.Nop);
            public List<Instruction> targets = new List<Instruction>();
        }

        public class ExceptionBlock
        {
            public LabelledExceptionHandler prev, cur;
            public Label start, skip;
        }

        public class LabelledExceptionHandler
        {
            public TypeReference exceptionType;
            public ExceptionHandlerType handlerType;
            public Label tryStart, tryEnd, filterStart, handlerStart, handlerEnd;
        }
    }
}
