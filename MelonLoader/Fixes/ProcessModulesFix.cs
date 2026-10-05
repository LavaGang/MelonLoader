#if OSX
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using HarmonyLib;

namespace MelonLoader.Fixes;

// On macOS, the Process.Modules / Process.MainModule APIs are broken in various amounts depending on the runtime.
// On CoreCLR, only the MainModule is partially useful, but there's no other modules loaded and Modules only contain one item being the MainModule.
// On MonoBleedingEdge, all modules are fetched, but the fetching of the base address / memory size is incorrect
// On Mono, the whole thing is COMPLETELY broken (returns a list of MainModule's duplicates) and horribly inefficient (takes SECONDS to fetch)
// Due to all of the above, we have to reimplement everything because Melon and il2cppinterop needs this API to work
public class ProcessModulesFix
{
    private const uint LcSegment64 = 0x19;
    private const uint LcMain = 0x28 | 0x80000000;

    [StructLayout(LayoutKind.Sequential)]
    private struct MachHeader64
    {
        internal uint magic;
        internal int cpuType;
        internal int cpuSubtype;
        internal uint fileType;
        internal uint nbrCommands;
        internal uint sizeOfCommands;
        internal uint flags;
        internal uint reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct LoadCommand
    {
        internal uint cmd;
        internal uint cmdSize;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct SegmentCommand64
    {
        internal uint cmd;
        internal uint cmdSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        internal byte[] segName;
        internal ulong vmAddr;
        internal ulong vmSize;
        internal ulong fileOff;
        internal ulong fileSize;
        internal int maxProt;
        internal int initProt;
        internal uint nSects;
        internal uint flags;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    private struct Section64
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        internal byte[] sectName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        internal byte[] segName;
        internal ulong addr;
        internal ulong size;
        internal uint offset;
        internal uint align;
        internal uint relOff;
        internal uint nRelOc;
        internal uint flags;
        internal uint reserved1;
        internal uint reserved2;
        internal uint reserved3;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct EntryPointCommand
    {
        internal uint cmd;
        internal uint cmdSize;
        internal ulong entryOff;
        internal ulong stackSize;
    }

    [DllImport("libSystem.B.dylib", EntryPoint = "_dyld_image_count", CallingConvention = CallingConvention.Cdecl)]
    private static extern uint DyLdImageCount();
    [DllImport("libSystem.B.dylib", EntryPoint = "_dyld_get_image_header", CallingConvention = CallingConvention.Cdecl)]
    private static extern nint DyLdGetImageHeader(uint index);
    [DllImport("libSystem.B.dylib", EntryPoint = "_dyld_get_image_name", CallingConvention = CallingConvention.Cdecl)]
    private static extern nint DyLdGetImageName(uint index);
    [DllImport("libSystem.B.dylib", EntryPoint = "_dyld_get_image_vmaddr_slide", CallingConvention = CallingConvention.Cdecl)]
    private static extern nint DyLdGetImageVmAddrSlide(uint index);

    private static readonly List<ProcessModule> ProcessModules = [];
    internal static void Install()
    {
        Type processFixType = typeof(ProcessModulesFix);
        Type processType = typeof(Process);

        try
        {
            Core.HarmonyInstance.Patch(AccessTools.PropertyGetter(processType, nameof(Process.Modules)), AccessTools.Method(processFixType, nameof(Modules)).ToNewHarmonyMethod());
            Core.HarmonyInstance.Patch(AccessTools.PropertyGetter(processType, nameof(Process.MainModule)), AccessTools.Method(processFixType, nameof(MainModule)).ToNewHarmonyMethod());
            Core.HarmonyInstance.Patch(AccessTools.Method(processType, nameof(Process.Refresh)), AccessTools.Method(processFixType, nameof(Refresh)).ToNewHarmonyMethod());
        }
        catch (Exception ex) { MelonLogger.Error(ex); }
    }

    private static bool Refresh()
    {
        RefreshProcessModules();
        return false;
    }

    private static bool MainModule(ref ProcessModule __result)
    {
        if (ProcessModules.Count > 0)
        {
            __result = ProcessModules[0];
            return false;
        }

        RefreshProcessModules();

        __result = ProcessModules[0];
        return false;
    }

    private static bool Modules(ref ProcessModuleCollection __result)
    {
        if (ProcessModules.Count > 0)
        {
            __result = new ProcessModuleCollection(ProcessModules.ToArray());
            return false;
        }

        RefreshProcessModules();

        __result = new ProcessModuleCollection(ProcessModules.ToArray());
        return false;
    }

    private static void RefreshProcessModules()
    {
        ProcessModules.Clear();

        uint count = DyLdImageCount();
        for (uint i = 0; i < count; i++)
        {
            nint imageNamePtr = DyLdGetImageName(i);
            string imageName = Marshal.PtrToStringAnsi(imageNamePtr) ?? "";
            nint slide = DyLdGetImageVmAddrSlide(i);
            // The header and load commands are mapped inside the first segment, so the sum of the segment sizes alone
            // covers the image. Adding them again would make the module extend past its last mapped page.
            int memorySize = 0;
            nint headerPtr = DyLdGetImageHeader(i);
            var header = (MachHeader64)Marshal.PtrToStructure(headerPtr, typeof(MachHeader64))!;
            nint commandPtr = headerPtr + Marshal.SizeOf(typeof(MachHeader64));
            nint entryOffset = 0;
            for (int j = 0; j < header.nbrCommands; j++)
            {
                var command = (LoadCommand)Marshal.PtrToStructure(commandPtr, typeof(LoadCommand))!;
                if (command.cmd == LcSegment64)
                {
                    var segmentCommand = Marshal.PtrToStructure(commandPtr, typeof(SegmentCommand64));
                    if (segmentCommand is not null)
                        memorySize += (int)((SegmentCommand64)segmentCommand).vmSize;
                }
                else if (command.cmd == LcMain)
                {
                    var entryCommand = Marshal.PtrToStructure(commandPtr, typeof(EntryPointCommand));
                    if (entryCommand is not null)
                        entryOffset = (nint)((EntryPointCommand)entryCommand).entryOff + slide;
                }
                commandPtr += (nint)command.cmdSize;
            }

            var module = CreateProcessModule(slide, memorySize, entryOffset, imageName);
            ProcessModules.Add((ProcessModule)module);
        }
    }

    private static object CreateProcessModule(nint slide, int memorySize, nint entryOffset, string imageName)
    {
#if NET6_0_OR_GREATER
        var processModuleCtor = AccessTools.Constructor(typeof(ProcessModule));
        var module = processModuleCtor.Invoke([]);
        AccessTools.Field(typeof(ProcessModule), $"<{nameof(ProcessModule.BaseAddress)}>k__BackingField").SetValue(module, slide);
        AccessTools.Field(typeof(ProcessModule), $"<{nameof(ProcessModule.ModuleMemorySize)}>k__BackingField").SetValue(module, memorySize);
        AccessTools.Field(typeof(ProcessModule), $"<{nameof(ProcessModule.EntryPointAddress)}>k__BackingField").SetValue(module, entryOffset);
        AccessTools.Field(typeof(ProcessModule), $"<{nameof(ProcessModule.FileName)}>k__BackingField").SetValue(module, imageName);
        // Work around an il2cppinterop bug where it won't check the correct extension
        AccessTools.Field(typeof(ProcessModule), $"<{nameof(ProcessModule.ModuleName)}>k__BackingField").SetValue(module,
            imageName.EndsWith("GameAssembly.dylib")
                ? Path.ChangeExtension(Path.GetFileName(imageName), "dll")
                : Path.GetFileNameWithoutExtension(imageName));
#else
        var module = AccessTools.Constructor(typeof(ProcessModule),
            [
                typeof(nint),
                typeof(nint),
                typeof(string),
                typeof(FileVersionInfo),
                typeof(int),
                typeof(string),
            ]).Invoke(
            [
                slide,
                entryOffset,
                imageName,
                null,
                memorySize,
                Path.GetFileNameWithoutExtension(imageName)
            ]);
#endif
        return module;
    }
}
#endif
