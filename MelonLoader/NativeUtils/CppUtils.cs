using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MelonLoader.NativeUtils;

public static class CppUtils
{
    // Credits: https://stackoverflow.com/a/9995303
    private static int GetHexVal(char hex)
    {
        var val = (int)hex;
        return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
    }

    [StructLayout(LayoutKind.Explicit)]
    private class ConvertClass
    {
        [FieldOffset(0)]
        public ulong valueULong;

        [FieldOffset(0)]
        public uint valueUInt;
    }

    public static unsafe IntPtr FunctionStart(IntPtr ptr)
    {
        var index = ptr.ToInt64();
        for (; *(byte*)index != 0x55 || *(long*)(index + 1) != 0xEC8B || *(byte*)(index + 4) != 0xEC; index--)
            ;
        return (IntPtr)index;
    }

    public static unsafe IntPtr ResolveRelativeInstruction(IntPtr instruction)
    {
        var opcode = *(byte*)instruction;
        if (opcode is not 0xE8 and not 0xE9)
            return IntPtr.Zero;

        return ResolvePtrOffset((IntPtr)((long)instruction + 1), (IntPtr)((long)instruction + 5)); // CALL: E8 [rel32] / JMP: E9 [rel32]
    }

    public static unsafe IntPtr ResolvePtrOffset(IntPtr offset32Ptr, IntPtr nextInstructionPtr)
    {
        var jmpOffset = *(uint*)offset32Ptr;
        var valueUInt = new ConvertClass() { valueULong = (ulong)nextInstructionPtr }.valueUInt;
        var delta = nextInstructionPtr.ToInt64() - valueUInt;
        var newPtrInt = unchecked(valueUInt + jmpOffset);
        return new IntPtr(newPtrInt + delta);
    }

    public static unsafe IntPtr[] SigscanAll(IntPtr module, int moduleSize, string signature)
    {
        List<IntPtr> ptrs = [];
        var signatureSpaceless = signature.Replace(" ", "");
        var signatureLength = signatureSpaceless.Length / 2;
        var signatureBytes = new byte[signatureLength];
        var signatureNullBytes = new bool[signatureLength];
        for (var i = 0; i < signatureLength; ++i)
        {
            if (signatureSpaceless[i * 2] == '?')
                signatureNullBytes[i] = true;
            else
                signatureBytes[i] = (byte)((GetHexVal(signatureSpaceless[i * 2]) << 4) + GetHexVal(signatureSpaceless[(i * 2) + 1]));
        }

        var index = module.ToInt64();
        var maxIndex = index + moduleSize;
        long tmpAddress = 0;
        var processed = 0;

        while (index < maxIndex)
        {
            if (signatureNullBytes[processed] || *(byte*)index == signatureBytes[processed])
            {
                if (processed == 0)
                    tmpAddress = index;

                ++processed;

                if (processed == signatureLength)
                {
                    ptrs.Add((IntPtr)tmpAddress);
                    processed = 0;
                }
            }
            else
            {
                processed = 0;
            }

            ++index;
        }

        return ptrs.ToArray();
    }

    public static unsafe IntPtr Sigscan(IntPtr module, int moduleSize, string signature)
    {
        var signatureSpaceless = signature.Replace(" ", "");
        var signatureLength = signatureSpaceless.Length / 2;
        var signatureBytes = new byte[signatureLength];
        var signatureNullBytes = new bool[signatureLength];
        for (var i = 0; i < signatureLength; ++i)
        {
            if (signatureSpaceless[i * 2] == '?')
                signatureNullBytes[i] = true;
            else
                signatureBytes[i] = (byte)((GetHexVal(signatureSpaceless[i * 2]) << 4) + GetHexVal(signatureSpaceless[(i * 2) + 1]));
        }

        var index = module.ToInt64();
        var maxIndex = index + moduleSize;
        long tmpAddress = 0;
        var processed = 0;

        while (index < maxIndex)
        {
            if (signatureNullBytes[processed] || *(byte*)index == signatureBytes[processed])
            {
                if (processed == 0)
                    tmpAddress = index;

                ++processed;

                if (processed == signatureLength)
                    return (IntPtr)tmpAddress;
            }
            else
            {
                processed = 0;
            }

            ++index;
        }

        return IntPtr.Zero;
    }
}
