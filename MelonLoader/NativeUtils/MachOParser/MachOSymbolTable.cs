#if OSX && NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Text;

namespace MelonLoader.NativeUtils.MachOParser
{
    // Reads a Mach-O image's LC_SYMTAB, including non-exported (local) symbols that dlsym can't see.
    // All addresses are file (unslid) addresses.
    internal static class MachOSymbolTable
    {
        private const uint MH_MAGIC_64 = 0xFEEDFACF;
        private const uint FAT_MAGIC = 0xCAFEBABE;
        private const uint LC_SYMTAB = 0x2;
        private const uint LC_SEGMENT_64 = 0x19;
        private const int CPU_TYPE_ARM64 = 0x0100000C;
        private const int CPU_TYPE_X86_64 = 0x01000007;
        private const byte N_STAB = 0xE0;
        private const byte N_TYPE = 0x0E;
        private const byte N_SECT = 0x0E;
        private const int MachHeader64Size = 32;
        private const int SegmentCommand64Size = 72;
        private const int Section64Size = 80;
        private const int Nlist64Size = 16;

        // Looks up each mangled name (with its leading underscore) and returns the address of every one that
        // is defined in a section. Names that aren't found are omitted.
        internal static unsafe Dictionary<string, ulong> FindSymbols(string imagePath, ICollection<string> names)
        {
            var found = new Dictionary<string, ulong>();
            var wanted = new List<(string Name, byte[] Bytes)>();
            foreach (var name in names)
                wanted.Add((name, Encoding.ASCII.GetBytes(name)));

            ReadImage(imagePath, (image, symtab) =>
            {
                for (uint i = 0; i < symtab.Count && found.Count < wanted.Count; i++)
                {
                    byte* nlist = symtab.Symbols + (i * Nlist64Size);
                    if (!IsDefinedInSection(nlist))
                        continue;

                    var name = MemoryMarshal.CreateReadOnlySpanFromNullTerminated(symtab.Strings + *(uint*)nlist);
                    foreach (var (wantedName, wantedBytes) in wanted)
                    {
                        if (name.SequenceEqual(wantedBytes))
                        {
                            found.TryAdd(wantedName, *(ulong*)(nlist + 8));
                            break;
                        }
                    }
                }
            });

            return found;
        }

        // Returns the sorted start addresses of every symbol in __TEXT,__text, ending with the section's end
        // address, so the distance from any code address to the next function boundary can be measured.
        internal static unsafe ulong[] GetCodeBoundaries(string imagePath)
        {
            var boundaries = new List<ulong>();

            ReadImage(imagePath, (image, symtab) =>
            {
                if (!TryFindTextSection(image, out byte textOrdinal, out ulong textStart, out ulong textEnd))
                    return;

                for (uint i = 0; i < symtab.Count; i++)
                {
                    byte* nlist = symtab.Symbols + (i * Nlist64Size);
                    if (IsDefinedInSection(nlist) && nlist[5] == textOrdinal)
                        boundaries.Add(*(ulong*)(nlist + 8));
                }

                boundaries.Add(textStart);
                boundaries.Add(textEnd);
            });

            boundaries.Sort();
            return boundaries.ToArray();
        }

        private unsafe struct SymbolTable
        {
            public byte* Symbols;
            public uint Count;
            public byte* Strings;
        }

        private unsafe delegate void ImageReader(byte* image, SymbolTable symtab);

        private static unsafe void ReadImage(string imagePath, ImageReader reader)
        {
            using var file = MemoryMappedFile.CreateFromFile(imagePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
            using var view = file.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);

            byte* fileStart = null;
            view.SafeMemoryMappedViewHandle.AcquirePointer(ref fileStart);
            try
            {
                byte* image = FindSlice(fileStart);
                if (image == null)
                    return;

                byte* cmd = FindLoadCommand(image, LC_SYMTAB);
                if (cmd == null)
                    return;

                reader(image, new SymbolTable
                {
                    Symbols = image + *(uint*)(cmd + 8),
                    Count = *(uint*)(cmd + 12),
                    Strings = image + *(uint*)(cmd + 16),
                });
            }
            finally
            {
                view.SafeMemoryMappedViewHandle.ReleasePointer();
            }
        }

        private static unsafe bool IsDefinedInSection(byte* nlist)
        {
            byte type = nlist[4];
            return (type & N_STAB) == 0 && (type & N_TYPE) == N_SECT;
        }

        private static unsafe byte* FindLoadCommand(byte* image, uint wantedCmd)
        {
            uint ncmds = *(uint*)(image + 16);
            byte* cmd = image + MachHeader64Size;
            for (uint i = 0; i < ncmds; i++)
            {
                if (*(uint*)cmd == wantedCmd)
                    return cmd;
                cmd += *(uint*)(cmd + 4);
            }
            return null;
        }

        // Section ordinals in nlist.n_sect are 1-based and count sections across all segments in load order
        private static unsafe bool TryFindTextSection(byte* image, out byte ordinal, out ulong start, out ulong end)
        {
            ordinal = 0;
            start = end = 0;

            uint ncmds = *(uint*)(image + 16);
            byte* cmd = image + MachHeader64Size;
            int sectionIndex = 0;
            for (uint i = 0; i < ncmds; i++)
            {
                if (*(uint*)cmd == LC_SEGMENT_64)
                {
                    uint nsects = *(uint*)(cmd + 64);
                    for (uint s = 0; s < nsects; s++)
                    {
                        sectionIndex++;
                        byte* section = cmd + SegmentCommand64Size + (s * Section64Size);
                        if (FixedName(section) == "__text" && FixedName(section + 16) == "__TEXT")
                        {
                            ordinal = (byte)sectionIndex;
                            start = *(ulong*)(section + 32);
                            end = start + *(ulong*)(section + 40);
                            return true;
                        }
                    }
                }
                cmd += *(uint*)(cmd + 4);
            }
            return false;
        }

        private static unsafe string FixedName(byte* name16)
        {
            int length = 0;
            while (length < 16 && name16[length] != 0)
                length++;
            return Encoding.ASCII.GetString(name16, length);
        }

        // Returns the start of the Mach-O image matching the running architecture, handling universal binaries
        private static unsafe byte* FindSlice(byte* fileStart)
        {
            if (*(uint*)fileStart == MH_MAGIC_64)
                return fileStart;

            if (ReadBigEndian(fileStart) != FAT_MAGIC)
                return null;

            int wantedCpu = RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? CPU_TYPE_ARM64 : CPU_TYPE_X86_64;
            uint archCount = ReadBigEndian(fileStart + 4);
            for (uint i = 0; i < archCount; i++)
            {
                // fat_arch: cputype, cpusubtype, offset, size, align (all big-endian)
                byte* arch = fileStart + 8 + (i * 20);
                if ((int)ReadBigEndian(arch) == wantedCpu)
                    return fileStart + ReadBigEndian(arch + 8);
            }

            return null;
        }

        private static unsafe uint ReadBigEndian(byte* p)
            => (uint)((p[0] << 24) | (p[1] << 16) | (p[2] << 8) | p[3]);
    }
}
#endif
