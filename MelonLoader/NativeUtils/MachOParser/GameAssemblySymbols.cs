#if OSX && ARM64 && NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Linq;
using MelonLoader.Utils;

namespace MelonLoader.NativeUtils.MachOParser
{
    // Symbol lookups and hook-safety checks for the loaded GameAssembly.dylib on macOS ARM64
    internal static class GameAssemblySymbols
    {
        // Dobby's ARM64 inline patch (ldr x17, #8; br x17; .quad target)
        internal const int MinPatchSize = 16;
        private const int MaxBranchHops = 3;

        // Exports used to compute the ASLR slide, checked against each other. il2cpp_init and
        // il2cpp_runtime_invoke are avoided since the bootstrap redirects them through dlsym
        private static readonly string[] AnchorExports = { "il2cpp_object_get_virtual_method", "il2cpp_class_from_name" };

        private static long? _slide;
        private static bool _slideFailed;
        private static ulong[] _codeBoundaries;

        // Returns the loaded address of each mangled name that exists in GameAssembly's symbol table
        internal static Dictionary<string, IntPtr> Resolve(ICollection<string> symbols)
        {
            var resolved = new Dictionary<string, IntPtr>();
            if (!TryGetSlide(out long slide))
                return resolved;

            foreach (var pair in MachOSymbolTable.FindSymbols(MelonEnvironment.Il2CppGameAssemblyPath, symbols))
                resolved[pair.Key] = (IntPtr)((long)pair.Value + slide);
            return resolved;
        }

        // Il2cpp exports and small wrappers often compile to a single tail-call branch, and a hook on one would
        // overwrite the functions placed right after it. Follows up to MaxBranchHops unconditional branches and
        // returns the final function, or IntPtr.Zero if fewer than MinPatchSize bytes remain before the next symbol.
        internal static unsafe IntPtr GetPatchableTarget(IntPtr address)
        {
            if (!TryGetSlide(out long slide))
                return IntPtr.Zero;

            for (int hop = 0; hop < MaxBranchHops; hop++)
            {
                uint instruction = *(uint*)address;
                if ((instruction & 0xFC000000) != 0x14000000) // B imm26
                    break;

                // Sign-extend imm26 and scale by 4
                long offset = ((long)(instruction & 0x03FFFFFF) << 38) >> 36;
                address = (IntPtr)((long)address + offset);
            }

            long available = GetBytesToNextSymbol((ulong)((long)address - slide));
            if (available < MinPatchSize)
            {
                MelonLogger.Warning($"Refusing to hook 0x{address:X}: only {available} bytes before the next function, Dobby needs {MinPatchSize}");
                return IntPtr.Zero;
            }

            return address;
        }

        // Bytes from a loaded code address to the next function in GameAssembly, or -1 when the address isn't in
        // GameAssembly's code (another image, JIT code) or its symbols can't be read
        internal static long GetSpaceBeforeNextFunction(IntPtr address)
        {
            if (!TryGetSlide(out long slide))
                return -1;

            ulong[] boundaries = CodeBoundaries;
            ulong fileAddress = (ulong)((long)address - slide);
            if (boundaries.Length == 0 || fileAddress < boundaries[0] || fileAddress >= boundaries[^1])
                return -1;

            return GetBytesToNextSymbol(fileAddress);
        }

        // A readable name for the function at a loaded address, for log messages
        internal static string DescribeFunction(IntPtr address)
        {
            if (!TryGetSlide(out long slide))
                return $"0x{address:X}";

            // Only used for messages, so a failed lookup falls back to the address rather than failing the caller
            string name;
            try
            {
                name = MachOSymbolTable.FindSymbolName(MelonEnvironment.Il2CppGameAssemblyPath, (ulong)((long)address - slide));
            }
            catch (Exception)
            {
                name = null;
            }
            if (name == null)
                return $"0x{address:X}";

            // IL2CPP method symbols are _Type_Method_m<40 hex digit hash>
            name = name.TrimStart('_');
            int hash = name.LastIndexOf("_m", StringComparison.Ordinal);
            if (hash > 0 && name.Length - hash == 42)
                name = name.Substring(0, hash);
            return $"{name} (0x{address:X})";
        }

        private static long GetBytesToNextSymbol(ulong fileAddress)
        {
            ulong[] boundaries = CodeBoundaries;

            // Index of the first boundary strictly after the address
            int index = Array.BinarySearch(boundaries, fileAddress + 1);
            if (index < 0)
                index = ~index;

            // Outside __TEXT's code sections (or no symbol table): treat as unsafe
            if (index == 0 || index >= boundaries.Length)
                return 0;

            return (long)(boundaries[index] - fileAddress);
        }

        // Read once. Every native hook asks for these, so a failure to read them is logged once and leaves the table
        // empty, which makes every lookup "unknown" instead of throwing out of each hook attach
        private static ulong[] CodeBoundaries
        {
            get
            {
                if (_codeBoundaries != null)
                    return _codeBoundaries;

                try
                {
                    _codeBoundaries = MachOSymbolTable.GetCodeBoundaries(MelonEnvironment.Il2CppGameAssemblyPath);
                }
                catch (Exception e)
                {
                    MelonLogger.Error("Failed to read function boundaries from GameAssembly's symbol table", e);
                    _codeBoundaries = Array.Empty<ulong>();
                }
                return _codeBoundaries;
            }
        }

        private static bool TryGetSlide(out long slide)
        {
            if (_slide.HasValue)
            {
                slide = _slide.Value;
                return true;
            }

            slide = 0;
            if (_slideFailed)
                return false;

            IntPtr handle = IntPtr.Zero;
            try
            {
                // GameAssembly is already loaded; this only takes a reference for the export lookups, released below
                string gameAssemblyPath = MelonEnvironment.Il2CppGameAssemblyPath;
                handle = System.Runtime.InteropServices.NativeLibrary.Load(gameAssemblyPath);
                Dictionary<string, ulong> fileAddresses = MachOSymbolTable.FindSymbols(gameAssemblyPath,
                    AnchorExports.Select(x => "_" + x).ToList());

                // The slide (loaded address - file address) has to agree across every anchor export
                long? computed = null;
                foreach (string export in AnchorExports)
                {
                    if (!fileAddresses.TryGetValue("_" + export, out ulong fileAddress))
                        throw new Exception($"Anchor export {export} not found in GameAssembly's symbol table");
                    if (!System.Runtime.InteropServices.NativeLibrary.TryGetExport(handle, export, out IntPtr loadedAddress))
                        throw new Exception($"Anchor export {export} not found in the loaded GameAssembly");

                    long exportSlide = (long)loadedAddress - (long)fileAddress;
                    if (computed.HasValue && computed.Value != exportSlide)
                        throw new Exception($"Inconsistent GameAssembly slide from {export}: 0x{exportSlide:X} vs 0x{computed.Value:X}");
                    computed = exportSlide;
                }

                _slide = computed;
                slide = computed.Value;
                MelonDebug.Msg($"GameAssembly slide: 0x{slide:X}");
                return true;
            }
            catch (Exception e)
            {
                MelonLogger.Error("Failed to compute GameAssembly's slide from its symbol table", e);
                _slideFailed = true;
                return false;
            }
            finally
            {
                if (handle != IntPtr.Zero)
                    System.Runtime.InteropServices.NativeLibrary.Free(handle);
            }
        }
    }
}
#endif
