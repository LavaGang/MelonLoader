#if NET6_0
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MelonLoader;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public unsafe class NativeStackWalk
{
    // private static MelonLogger.Instance log = new("NativeStackWalk");

    #region Native Structs

    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    public struct CONTEXT
    {
        public ulong P1Home;
        public ulong P2Home;
        public ulong P3Home;
        public ulong P4Home;
        public ulong P5Home;
        public ulong P6Home;
        public uint ContextFlags;
        public uint MxCsr;
        public short SegCs;
        public short SegDs;
        public short SegEs;
        public short SegFs;
        public short SegGs;
        public short SegSs;
        public uint EFlags;
        public ulong Dr0;
        public ulong Dr1;
        public ulong Dr2;
        public ulong Dr3;
        public ulong Dr6;
        public ulong Dr7;
        public ulong Rax;
        public ulong Rcx;
        public ulong Rdx;
        public ulong Rbx;
        public ulong Rsp;
        public ulong Rbp;
        public ulong Rsi;
        public ulong Rdi;
        public ulong R8;
        public ulong R9;
        public ulong R10;
        public ulong R11;
        public ulong R12;
        public ulong R13;
        public ulong R14;
        public ulong R15;
        public ulong Rip;
    }

    public struct CLIENT_ID
    {
        public void* UniqueProcess;
        public void* UniqueThread;
    }

    public struct THREAD_BASIC_INFORMATION
    {
        public uint ExitStatus;
        public void* TebBaseAddress;
        public CLIENT_ID ClientId;
        public uint* AffinityMask;
        public uint Priority;
        public uint BasePriority;
    }

    public struct NT_TIB
    {
        void* ExceptionList;
        public void* StackBase;

        public void* StackLimit;
        // (...)
    }

    #endregion

    #region Native Imports

    [DllImport("ntdll.dll")]
    private static extern uint NtQueryInformationThread(nint ThreadHandle, uint ThreadInformationClass, void* ThreadInformation, uint ThreadInformationLength, ulong* ReturnLength);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    private static extern bool IsBadReadPtr(void* lp, nint ucb);

    [DllImport("kernel32.dll")]
    private static extern void RtlCaptureContext(void* ContextRecord);

    [DllImport("kernel32.dll")]
    private static extern nint GetCurrentThread();

    #endregion

    [MethodImpl(MethodImplOptions.NoOptimization)]
    public static bool IsValidAddress(ulong addr) => !IsBadReadPtr((void*)(addr - 6), 7);

    public static bool TryGetCalleeSite(ulong addr, out ulong callee)
    {
        callee = 0;
        while (true)
        {
            if (!IsValidAddress(addr))
                return false;

            var inst = new[] { *(byte*)addr, *(byte*)(addr + 1) };
            if (inst[0] == 0xE8)
            {
                //call relative address
                addr += 2ul + inst[1];
            }
            else if (inst[0] == 0xE9)
            {
                //jmp relative
                if (!IsValidAddress(addr + 1))
                    return false;

                addr += 5ul + (ulong)*(int*)(addr + 1);
            }
            else if (inst[0] == 0xFF && (inst[1] & 70) == 40)
            {
                //call or jmp absolute
                if (inst[1] == 0x25)
                {
                    if (!IsValidAddress(addr + 2))
                        return false;

                    var displace = *(uint*)(addr + 2);
                    if (!IsValidAddress(displace))
                        return false;

                    addr = displace;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                callee = addr;
                return true;
            }
        }
    }

    public static bool IsReturnAddress(ulong addr, out ulong calledAddr)
    {
        var cursor = (byte*)addr;
        if (cursor[-5] == 0xE8)
        {
            //call relative address

            calledAddr = addr + *(uint*)(cursor - 4);
            if (IsValidAddress(calledAddr))
            {
                if (TryGetCalleeSite(calledAddr, out var callee))
                {
                    calledAddr = callee;
                }

                return true;
            }

            calledAddr = 0;
        }

        if (cursor[-6] == 0xFF && cursor[-5] == 25)
        {
            //call absolute address
            calledAddr = addr + *(uint*)(cursor - 4);
            return true;
        }

        // call [REG+XX]
        if (cursor[-3] == 0xFF && (cursor[-2] & ~7) == 0x78 && (cursor[-2] & 7) != 4)
        {
            calledAddr = 0xFFFFFFFF;
            return true;
        }

        if (cursor[-4] == 0xFF && cursor[-3] == 0124)
        {
            calledAddr = ulong.MaxValue;
            return true;
        }

        // call [REG+XXXX]
        if (cursor[-6] == 0xFF && (cursor[-5] & ~7) == 0xDC && (cursor[-5] & 7) != 4)
        {
            calledAddr = ulong.MaxValue;
            return true;
        }

        if (cursor[-7] == 0xFF && cursor[-6] == 0xE0)
        {
            calledAddr = ulong.MaxValue;
            return true;
        }

        // call [REG]
        if (cursor[-2] == 0xFF && (cursor[-1] & ~7) == 0x14 && (cursor[-1] & 7) != 4 && (cursor[-1] & 7) != 5)
        {
            calledAddr = ulong.MaxValue;
            return true;
        }

        // call REG
        if (cursor[-2] == 0xFF && (cursor[-1] & ~7) == 0x140 && (cursor[-1] & 7) != 4)
        {
            calledAddr = ulong.MaxValue;
            return true;
        }

        calledAddr = 0;
        return false;
    }

    public static void DumpStack()
    {
        Console.WriteLine("Don't use this.");
        //return;
        
        var tbi = stackalloc THREAD_BASIC_INFORMATION[1];
        var status = NtQueryInformationThread(GetCurrentThread(), 0, tbi, (uint)sizeof(THREAD_BASIC_INFORMATION), null);

        if (status != 0)
        {
            Console.WriteLine($"[Error] NtQueryInformationThread failed: 0x{status:X2}");
            return;
        }

        // The TEB's first field is the TIB
        var tib = (NT_TIB*)tbi->TebBaseAddress;
        var top = tib->StackLimit;
        var end = tib->StackBase;

        var ctxData = (CONTEXT*)NativeMemory.AllocZeroed(2048 * 2 * 2);
        RtlCaptureContext(ctxData);
        // var rip = (void*)ctxData->Rip;
        
        if((nint) ctxData->Rsp > (nint)tib->StackLimit && (nint)ctxData->Rsp < (nint)tib->StackBase)
        {
            top = (void*)ctxData->Rsp;
        }
        
        var current = (void**)((nint)top & (~IntPtr.Size));

        Console.WriteLine($"{(nint) end - (nint) top} bytes in stack");

        while ((nint)current < (nint)end)
        {
            var addr = *(ulong*)current;
            if (!IsValidAddress(addr) || !IsReturnAddress(addr, out var calledAddr)) goto next;

            Console.WriteLine($"Address: 0x{addr:X2} (Called: 0x{calledAddr:X2})");

            next:
            current ++;
        }

        NativeMemory.Free(ctxData);
        
        Console.WriteLine(Environment.StackTrace);
        Debugger.Break();
    }
}
#endif