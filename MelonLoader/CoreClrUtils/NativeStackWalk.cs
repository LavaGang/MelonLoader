#if NET6_0

#nullable enable
#pragma warning disable CS0169
#pragma warning disable CS0649

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using MelonLoader.Utils;
using Microsoft.Diagnostics.Runtime;

namespace MelonLoader.CoreClrUtils;

// ReSharper disable UnusedMember.Local, InconsistentNaming
public static unsafe class NativeStackWalk
{
    private static MelonLogger.Instance Logger = new MelonLogger.Instance("NativeStackWalk", Color.GreenYellow);
    
    private static Dictionary<ulong, string> _registeredHooks = new();

    #region Native Structs

    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    private struct CONTEXT
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

    private struct CLIENT_ID
    {
        public void* UniqueProcess;
        public void* UniqueThread;
    }

    private struct THREAD_BASIC_INFORMATION
    {
        public uint ExitStatus;
        public NT_TIB* TebBaseAddress;
        public CLIENT_ID ClientId;
        public uint* AffinityMask;
        public uint Priority;
        public uint BasePriority;
    }

    private struct NT_TIB
    {
        void* ExceptionList;
        public void* StackBase;

        public void* StackLimit;
        // (...)
    }

    [Flags]
    private enum SymFlag : uint
    {
        VALUEPRESENT = 0x00000001,
        REGISTER = 0x00000008,
        REGREL = 0x00000010,
        FRAMEREL = 0x00000020,
        PARAMETER = 0x00000040,
        LOCAL = 0x00000080,
        CONSTANT = 0x00000100,
        EXPORT = 0x00000200,
        FORWARDER = 0x00000400,
        FUNCTION = 0x00000800,
        VIRTUAL = 0x00001000,
        THUNK = 0x00002000,
        TLSREL = 0x00004000,
        SLOT = 0x00008000,
        ILREL = 0x00010000,
        METADATA = 0x00020000,
        CLR_TOKEN = 0x00040000,
    }

    [Flags]
    private enum SymOptions : uint
    {
        SYMOPT_CASE_INSENSITIVE = 0x00000001,
        SYMOPT_UNDNAME = 0x00000002,
        SYMOPT_DEFERRED_LOADS = 0x00000004,
        SYMOPT_NO_CPP = 0x00000008,
        SYMOPT_LOAD_LINES = 0x00000010,
        SYMOPT_OMAP_FIND_NEAREST = 0x00000020,
        SYMOPT_LOAD_ANYTHING = 0x00000040,
        SYMOPT_IGNORE_CVREC = 0x00000080,
        SYMOPT_NO_UNQUALIFIED_LOADS = 0x00000100,
        SYMOPT_FAIL_CRITICAL_ERRORS = 0x00000200,
        SYMOPT_EXACT_SYMBOLS = 0x00000400,
        SYMOPT_ALLOW_ABSOLUTE_SYMBOLS = 0x00000800,
        SYMOPT_IGNORE_NT_SYMPATH = 0x00001000,
        SYMOPT_INCLUDE_32BIT_MODULES = 0x00002000,
        SYMOPT_PUBLICS_ONLY = 0x00004000,
        SYMOPT_NO_PUBLICS = 0x00008000,
        SYMOPT_AUTO_PUBLICS = 0x00010000,
        SYMOPT_NO_IMAGE_SEARCH = 0x00020000,
        SYMOPT_SECURE = 0x00040000,
        SYMOPT_NO_PROMPTS = 0x00080000,
        SYMOPT_OVERWRITE = 0x00100000,
        SYMOPT_IGNORE_IMAGEDIR = 0x00200000,
        SYMOPT_FLAT_DIRECTORY = 0x00400000,
        SYMOPT_DEBUG = 0x80000000,
    }

    [Flags]
    private enum SymTagEnum : uint
    {
        Null,
        Exe,
        Compiland,
        CompilandDetails,
        CompilandEnv,
        Function,
        Block,
        Data,
        Annotation,
        Label,
        PublicSymbol,
        UDT,
        Enum,
        FunctionType,
        PointerType,
        ArrayType,
        BaseType,
        Typedef,
        BaseClass,
        Friend,
        FunctionArgType,
        FuncDebugStart,
        FuncDebugEnd,
        UsingNamespace,
        VTableShape,
        VTable,
        Custom,
        Thunk,
        CustomType,
        ManagedType,
        Dimension
    };

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 8)]
    private struct SYMBOL_INFO
    {
        public uint SizeOfStruct;
        public uint TypeIndex;
        public ulong Reserved1;
        public ulong Reserved2;
        public uint Index;
        public uint Size;
        public ulong ModBase;
        public SymFlag Flags;
        public ulong Value;
        public ulong Address;
        public uint Register;
        public uint Scope;
        public SymTagEnum Tag;
        public uint NameLen;
        public uint MaxNameLen;

        public fixed char NameDummy[2];
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

    [DllImport("dbghelp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SymInitialize(void* hProcess, string UserSearchPath, int fInvadeProcess);

    [DllImport("dbghelp.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SymFromAddrW(void* hProcess, ulong Address, ulong* Displacement, SYMBOL_INFO* Symbol);
    
    //SymSetoptions import
    [DllImport("dbghelp.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SymSetOptions(SymOptions SymOptions);
    
    [DllImport("dbghelp.dll", SetLastError = true)]
    private static extern ulong SymGetModuleBase64(void* hProcess, ulong Address);

    [DllImport("dbghelp.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SymCleanup(void* hProcess);
    
    //GetModuleFileNameEx import
    [DllImport("psapi.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetModuleFileNameEx(void* hProcess, void* hModule, [Out] char* lpFilename, uint nSize);

    #endregion
    
    #region Helper Methods
    
    [MethodImpl(MethodImplOptions.NoOptimization)]
    private static bool IsValidAddress(ulong addr, bool is64Bit) => !IsBadReadPtr((void*)(addr - 6), 7) && (!is64Bit || addr > 0x70000000000);

    private static bool TryGetCalleeSite(ulong addr, bool is64Bit, out ulong callee)
    {
        callee = 0;
        while (true)
        {
            if (!IsValidAddress(addr, is64Bit))
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
                if (!IsValidAddress(addr + 1, is64Bit))
                    return false;

                addr += 5ul + (ulong)*(int*)(addr + 1);
            }
            else if (inst[0] == 0xFF && (inst[1] & 70) == 40)
            {
                //call or jmp absolute
                if (inst[1] == 0x25)
                {
                    if (!IsValidAddress(addr + 2, is64Bit))
                        return false;

                    var displace = *(uint*)(addr + 2);
                    if (!IsValidAddress(displace, is64Bit))
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

    private static bool IsReturnAddress(ulong addr, bool is64Bit, out ulong calledAddr)
    {
        calledAddr = 0;
        if (!IsValidAddress(addr, is64Bit))
            return false;
        
        var cursor = (byte*)addr;
        if (cursor[-5] == 0xE8)
        {
            //call relative address

            calledAddr = addr + *(uint*)(cursor - 4);
            if (IsValidAddress(calledAddr, is64Bit))
            {
                if (TryGetCalleeSite(calledAddr, is64Bit, out var callee))
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
    
    private static (IntPtr top, IntPtr end) GetStackBounds()
    {
        var tbi = stackalloc THREAD_BASIC_INFORMATION[1];
        var status = NtQueryInformationThread(GetCurrentThread(), 0, tbi, (uint)sizeof(THREAD_BASIC_INFORMATION), null);

        if (status != 0)
        {
            Console.WriteLine($"[Error] NtQueryInformationThread failed: 0x{status:X2}");
            return (IntPtr.Zero, IntPtr.Zero);
        }

        var tib = tbi->TebBaseAddress;
        var top = tib->StackLimit;
        var end = tib->StackBase;

        var ctxData = (CONTEXT*)NativeMemory.AllocZeroed(2048 * 2 * 2);
        RtlCaptureContext(ctxData);

        if ((nint)ctxData->Rsp > (nint)tib->StackLimit && (nint)ctxData->Rsp < (nint)tib->StackBase)
        {
            top = (void*)ctxData->Rsp;
        }

        NativeMemory.Free(ctxData);
        return ((IntPtr) top, (IntPtr) end);
    }

    private static int InitDbgHelp(void* handle)
    {
        var tempDir = Environment.GetEnvironmentVariable("TEMP") ?? throw new("TEMP not set");

        var cacheDir = Path.Combine(tempDir, "MelonLoader_SymServ_Cache");

        if (!Directory.Exists(cacheDir))
            Directory.CreateDirectory(cacheDir);

        var userPath = $"cache*{cacheDir};srv*https://msdl.microsoft.com/download/symbols;srv*https://symbolserver.unity3d.com;{MelonEnvironment.GameRootDirectory}";

        SymSetOptions(SymOptions.SYMOPT_UNDNAME | SymOptions.SYMOPT_DEFERRED_LOADS);

        if (!SymInitialize(handle, userPath, 1))
        {
            return Marshal.GetLastWin32Error();
        }

        return 0;
    }

    private static string? GetHookName(ulong ip)
    {
        foreach (var (addr, name) in _registeredHooks)
        {
            if (Math.Abs((long) ip - (long) addr) < 0x400)
                return name;
        }

        return null;
    }
    
    #endregion
    public class NativeStackFrame
    {
        public ulong Pointer;
        public string? ModulePath;
        public string? Function;
        public ulong Offset;

        public string? ModuleName => Path.GetFileName(ModulePath);
    }

    internal static void RegisterHookAddr(ulong addr, string hookName)
    {
        // Logger.Msg($"Registering hook: 0x{addr:X} => {hookName}");
        _registeredHooks[addr] = hookName;
    }

    [SupportedOSPlatform("windows")]
    [Pure]
    public static List<NativeStackFrame> GetNativeStackFrames()
    {
        //Get the top and bottom of the managed stack
        var (t, e) = GetStackBounds();
        
        if(t == IntPtr.Zero || e == IntPtr.Zero)
            throw new("Failed to get stack bounds");
        
        var top = (nint)t;
        var end = (nint)e;

        var current = top & ~IntPtr.Size; //Align

        var addresses = new List<nuint>();
        var ptrSize = IntPtr.Size;
        var is64bit = ptrSize == 8;

        //Loop through the stack looking for potential addresses
        while (current + ptrSize < end + 1)
        {
            var addr = *(ulong*)current;
            current += 4;
            
            if (!IsReturnAddress(addr, is64bit, out _)) 
                continue;

            addresses.Add((nuint)addr);
        }

        var handle = (void*)Process.GetCurrentProcess().Handle;

        //Initialize DbgHelp to get symbol names
        
        int initResult;
        if ((initResult = InitDbgHelp(handle)) != 0)
        {
            throw new($"InitDbgHelp failed (result {initResult})");
        }

        var symSize = sizeof(SYMBOL_INFO);
        var displacement = 0ul;
        var ret = new List<NativeStackFrame>();
        
        //Try to get information for each frame in the stack
        foreach (var address in addresses)
        {
            var frame = new NativeStackFrame { Pointer = address };
            ret.Add(frame);
            
            var moduleBase = SymGetModuleBase64(handle, address);

            if (moduleBase == 0)
            {
                //Jitted method?
                using (var dt = DataTarget.CreateSnapshotAndAttach(Environment.ProcessId))
                {
                    var runtime = dt.ClrVersions.First().CreateRuntime();

                    if (runtime.GetMethodByInstructionPointer(address) is { } method)
                    {
                        frame.Function = $"(managed) {method.Signature}";

                        if (MethodBaseHelper.GetMethodBaseFromHandle((IntPtr)method.MethodDesc) is { } md)
                        {
                            frame.ModulePath = md.Module.Assembly.Location;
                        }

                        frame.Offset = address - method.NativeCode;
                        continue;
                    }
                }

                if (frame.Function == null && GetHookName(address) is {} hookName)
                {
                    frame.Function = $"(detour) {hookName}";
                    frame.ModulePath = "(injected code)";
                }
            }

            //Get the path of the module from its base address
            var moduleNameBuff = (char*) NativeMemory.Alloc(256 * sizeof(char));
            if (GetModuleFileNameEx(handle, (void*)moduleBase, moduleNameBuff, 256))
            {
                var moduleName = Marshal.PtrToStringAnsi((IntPtr)moduleNameBuff);
                frame.ModulePath = moduleName;
            }
            NativeMemory.Free(moduleNameBuff);
            
            //Attempt to load actual symbol information via dbghelp
            var maxNameLen = 255u;
            var symbol = (SYMBOL_INFO*) NativeMemory.Alloc((nuint)(symSize + maxNameLen * sizeof(char)));

            symbol->SizeOfStruct = (uint)symSize;
            symbol->MaxNameLen = maxNameLen;

            if (SymFromAddrW(handle, address, &displacement, symbol))
            {
                //We have a symbol, get the name and offset
                var nameBuffer = new byte[symbol->NameLen * sizeof(char)];
                Marshal.Copy((IntPtr)symbol->NameDummy, nameBuffer, 0, nameBuffer.Length);
                frame.Function = Encoding.Unicode.GetString(nameBuffer);
                frame.Offset = displacement;
            }
            
            NativeMemory.Free(symbol);
        }

        //De-initialize DbgHelp
        SymCleanup(handle);

        return ret;
    }

    [SupportedOSPlatform("windows")]
    public static string NativeStackTrace
    {
        get
        {
            var sb = new StringBuilder();
            
            GetNativeStackFrames().ForEach(f => sb.AppendLine(FormatFrame(f)));

            return sb.ToString();
        }
    }

    private static string FormatFrame(NativeStackFrame frame)
    {
        var frameBuilder = new StringBuilder();
        
        if (frame.Function == null)
        {
            frameBuilder.Append($"at 0x{frame.Pointer:X}");
            
            if(frame.ModuleName is {} mn)
                frameBuilder.Append($" in {mn}");
            else
                frameBuilder.Append(" (unknown module)");
        }
        else
        {
            var offsetSigned = BitConverter.ToInt64(BitConverter.GetBytes(frame.Offset));
            var isSymbolAccurate = offsetSigned is >= 0 and < 0x4000;
            var isGa = frame.ModuleName == "GameAssembly.dll";

            frameBuilder.Append("at ");

            if (isSymbolAccurate && isGa)
                frameBuilder.Append("(IL2CPP) ");

            if (isSymbolAccurate)
            {
                frameBuilder.Append($"{frame.Function}");
                if (offsetSigned != 0)
                    frameBuilder.Append($" + 0x{frame.Offset:X}");
            }
            else
                frameBuilder.Append($"0x{frame.Pointer:X} <pdb not available or does not contain symbol>");
            
            if (!isSymbolAccurate || !isGa)
                frameBuilder.Append($" in {frame.ModuleName ?? "<err: null module name>"}");
        }

        return frameBuilder.ToString();
    }

    [SupportedOSPlatform("windows")]
    public static void LogNativeStackTrace() => GetNativeStackFrames().ForEach(f => Logger.Msg(FormatFrame(f)));
}
#endif