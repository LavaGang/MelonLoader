using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Utils;
using MonoMod.Utils.Cil;
using OpCode = System.Reflection.Emit.OpCode;
using OpCodes = System.Reflection.Emit.OpCodes;

namespace HarmonyLib.Internal.Util
{
    internal static class EmitterExtensions
    {
        private static DynamicMethodDefinition emitDMD;
        private static MethodInfo emitDMDMethod;
        private static Action<CecilILGenerator, OpCode, object> emitCodeDelegate;

        [MethodImpl(MethodImplOptions.Synchronized)]
        static EmitterExtensions()
        {
            if (emitDMD != null)
                return;
            InitEmitterHelperDMD();
        }

        public static Type OpenRefType(this Type t)
        {
            if (t.IsByRef)
                return t.GetElementType();
            return t;
        }

        private static void InitEmitterHelperDMD()
        {
            emitDMD = new DynamicMethodDefinition("EmitOpcodeWithOperand", typeof(void),
                                                  new[] {typeof(CecilILGenerator), typeof(OpCode), typeof(object)});
            var il = emitDMD.GetILGenerator();

            var current = il.DefineLabel();

            il.Emit(OpCodes.Ldarg_2);
            il.Emit(OpCodes.Brtrue, current);

            il.Emit(OpCodes.Ldstr, "Provided operand is null!");
            il.Emit(OpCodes.Newobj, typeof(Exception).GetConstructor(new[] {typeof(string)}));
            il.Emit(OpCodes.Throw);

            foreach (var method in typeof(CecilILGenerator).GetMethods().Where(m => m.Name == "Emit"))
            {
                var paramInfos = method.GetParameters();
                if (paramInfos.Length != 2)
                    continue;
                var types = paramInfos.Select(p => p.ParameterType).ToArray();
                if (types[0] != typeof(OpCode))
                    continue;

                var paramType = types[1];

                il.MarkLabel(current);
                current = il.DefineLabel();

                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Isinst, paramType);
                il.Emit(OpCodes.Brfalse, current);

                il.Emit(OpCodes.Ldarg_2);

                if (paramType.IsValueType)
                    il.Emit(OpCodes.Unbox_Any, paramType);

                var loc = il.DeclareLocal(paramType);
                il.Emit(OpCodes.Stloc, loc);

                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldloc, loc);
                il.Emit(OpCodes.Callvirt, method);
                il.Emit(OpCodes.Ret);
            }

            il.MarkLabel(current);
            il.Emit(OpCodes.Ldstr, "The operand is none of the supported types!");
            il.Emit(OpCodes.Newobj, typeof(Exception).GetConstructor(new[] {typeof(string)}));
            il.Emit(OpCodes.Throw);
            il.Emit(OpCodes.Ret);

            emitDMDMethod = emitDMD.Generate();
            emitCodeDelegate =
                (Action<CecilILGenerator, OpCode, object>) emitDMDMethod
                    .CreateDelegate<Action<CecilILGenerator, OpCode, object>>();
        }

        public static void Emit(this CecilILGenerator il, OpCode opcode, object operand)
        {
            emitCodeDelegate(il, opcode, operand);
        }

        public static void MarkBlockBefore(this CecilILGenerator il, ExceptionBlock block)
        {
            switch (block.blockType)
            {
                case ExceptionBlockType.BeginExceptionBlock:
                    il.BeginExceptionBlock();
                    return;
                case ExceptionBlockType.BeginCatchBlock:
                    il.BeginCatchBlock(block.catchType);
                    return;
                case ExceptionBlockType.BeginExceptFilterBlock:
                    il.BeginExceptFilterBlock();
                    return;
                case ExceptionBlockType.BeginFaultBlock:
                    il.BeginFaultBlock();
                    return;
                case ExceptionBlockType.BeginFinallyBlock:
                    il.BeginFinallyBlock();
                    return;
                case ExceptionBlockType.EndExceptionBlock:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static void MarkBlockAfter(this CecilILGenerator il, ExceptionBlock block)
        {
            if (block.blockType == ExceptionBlockType.EndExceptionBlock)
                il.EndExceptionBlock();
        }

        public static LocalBuilder GetLocal(this CecilILGenerator il, VariableDefinition varDef)
        {
            var vars = (Dictionary<LocalBuilder, VariableDefinition>) AccessTools
                                                                      .Field(typeof(CecilILGenerator), "_Variables")
                                                                      .GetValue(il);
            var loc = vars.FirstOrDefault(kv => kv.Value == varDef).Key;
            if (loc != null)
                return loc;
            // TODO: Remove once MonoMod allows to specify this manually
            var type = varDef.VariableType.ResolveReflection();
            var pinned = varDef.VariableType.IsPinned;
            var index = varDef.Index;
            loc = (LocalBuilder) (
                c_LocalBuilder_params == 4 ? c_LocalBuilder.Invoke(new object[] { index, type, null, pinned }) :
                c_LocalBuilder_params == 3 ? c_LocalBuilder.Invoke(new object[] { index, type, null }) :
                c_LocalBuilder_params == 2 ? c_LocalBuilder.Invoke(new object[] { type, null }) :
                c_LocalBuilder_params == 0 ? c_LocalBuilder.Invoke(new object[] { }) :
                throw new NotSupportedException()
            );

            f_LocalBuilder_position?.SetValue(loc, (ushort) index);
            f_LocalBuilder_is_pinned?.SetValue(loc, pinned);
            vars[loc] = varDef;
            return loc;
        }

        private static readonly ConstructorInfo c_LocalBuilder =
            typeof(LocalBuilder).GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                                .OrderByDescending(c => c.GetParameters().Length).First();
        private static readonly FieldInfo f_LocalBuilder_position =
            typeof(LocalBuilder).GetField("position", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo f_LocalBuilder_is_pinned =
            typeof(LocalBuilder).GetField("is_pinned", BindingFlags.NonPublic | BindingFlags.Instance);
        private static int c_LocalBuilder_params = c_LocalBuilder.GetParameters().Length;
    }
}