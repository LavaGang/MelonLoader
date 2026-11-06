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
    // All addresses are file (unslid) addresses. Every offset and size read from the file is checked against the
    // mapped length first, so a truncated or unexpected file yields no results instead of an access violation,
    // which .NET can't catch.
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
        private const uint S_ATTR_PURE_INSTRUCTIONS = 0x80000000;
        private const int MachHeader64Size = 32;
        private const int LoadCommandSize = 8;
        private const int SymtabCommandSize = 24;
        private const int SegmentCommand64Size = 72;
        private const int Section64Size = 80;
        private const int Nlist64Size = 16;
        private const int FatHeaderSize = 8;
        private const int FatArchSize = 20;

        // Looks up each mangled name (with its leading underscore) and returns the address of every one that
        // is defined in a section. Names that aren't found are omitted.
        internal static unsafe Dictionary<string, ulong> FindSymbols(string imagePath, ICollection<string> names)
        {
            var found = new Dictionary<string, ulong>();
            var wanted = new List<(string Name, byte[] Bytes)>();
            foreach (var name in names)
                wanted.Add((name, Encoding.ASCII.GetBytes(name)));

            ReadImage(imagePath, image =>
            {
                for (uint i = 0; i < image.SymbolCount && found.Count < wanted.Count; i++)
                {
                    byte* nlist = image.Symbols + (i * Nlist64Size);
                    if (!IsDefinedInSection(nlist))
                        continue;

                    var name = image.GetName(nlist);
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

        // Returns the sorted start addresses of every symbol in __TEXT's code sections, plus each section's start
        // and end, so the distance from any code address to the next function boundary can be measured. Unity 6
        // places IL2CPP's generated game code in its own __TEXT,il2cpp section, separate from the runtime's __text.
        internal static unsafe ulong[] GetCodeBoundaries(string imagePath)
        {
            var boundaries = new List<ulong>();

            ReadImage(imagePath, image =>
            {
                var codeSections = FindCodeSections(image);
                if (codeSections.Count == 0)
                    return;

                var codeOrdinals = new bool[256];
                foreach (var (ordinal, start, end) in codeSections)
                {
                    codeOrdinals[ordinal] = true;
                    boundaries.Add(start);
                    boundaries.Add(end);
                }

                for (uint i = 0; i < image.SymbolCount; i++)
                {
                    byte* nlist = image.Symbols + (i * Nlist64Size);
                    if (IsDefinedInSection(nlist) && codeOrdinals[nlist[5]])
                        boundaries.Add(*(ulong*)(nlist + 8));
                }
            });

            boundaries.Sort();
            return boundaries.ToArray();
        }

        // Returns the name of a symbol defined at exactly this address, or null. Scans the whole table, so it's
        // meant for diagnostics rather than hot paths
        internal static unsafe string FindSymbolName(string imagePath, ulong address)
        {
            string found = null;

            ReadImage(imagePath, image =>
            {
                for (uint i = 0; i < image.SymbolCount; i++)
                {
                    byte* nlist = image.Symbols + (i * Nlist64Size);
                    if (!IsDefinedInSection(nlist) || *(ulong*)(nlist + 8) != address)
                        continue;

                    // Skip assembler-local labels such as ltmp0, which can share an address with the function
                    var name = image.GetName(nlist);
                    if (name.Length == 0 || name[0] == (byte)'l')
                        continue;

                    found = Encoding.ASCII.GetString(name);
                    return;
                }
            });

            return found;
        }

        // A validated view of one Mach-O image: its load commands and symbol table are known to lie within the mapping
        private readonly unsafe struct MachOImage
        {
            public readonly byte* Header;
            public readonly byte* CommandsEnd;
            public readonly byte* Symbols;
            public readonly uint SymbolCount;
            private readonly byte* _strings;
            private readonly uint _stringsSize;

            public MachOImage(byte* header, byte* commandsEnd, byte* symbols, uint symbolCount, byte* strings, uint stringsSize)
            {
                Header = header;
                CommandsEnd = commandsEnd;
                Symbols = symbols;
                SymbolCount = symbolCount;
                _strings = strings;
                _stringsSize = stringsSize;
            }

            // The symbol's name, cut at its terminator or at the end of the string table, whichever comes first
            public ReadOnlySpan<byte> GetName(byte* nlist)
            {
                uint offset = *(uint*)nlist;
                if (offset >= _stringsSize)
                    return ReadOnlySpan<byte>.Empty;

                var name = new ReadOnlySpan<byte>(_strings + offset, (int)(_stringsSize - offset));
                int end = name.IndexOf((byte)0);
                return end < 0 ? name : name.Slice(0, end);
            }
        }

        private delegate void ImageReader(MachOImage image);

        private static unsafe void ReadImage(string imagePath, ImageReader reader)
        {
            using var file = MemoryMappedFile.CreateFromFile(imagePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
            using var view = file.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);

            byte* mapping = null;
            try
            {
                view.SafeMemoryMappedViewHandle.AcquirePointer(ref mapping);
                byte* fileStart = mapping + view.PointerOffset;

                if (TryOpenImage(fileStart, (ulong)view.Capacity, out MachOImage image))
                    reader(image);
            }
            finally
            {
                if (mapping != null)
                    view.SafeMemoryMappedViewHandle.ReleasePointer();
            }
        }

        private static unsafe bool TryOpenImage(byte* fileStart, ulong fileLength, out MachOImage image)
        {
            image = default;
            if (!TryFindSlice(fileStart, fileLength, out byte* header, out ulong length))
                return false;

            if (length < MachHeader64Size || *(uint*)header != MH_MAGIC_64)
                return false;

            ulong commandsSize = *(uint*)(header + 20);
            if (commandsSize > length - MachHeader64Size)
                return false;
            byte* commandsEnd = header + MachHeader64Size + commandsSize;

            byte* symtab = FindLoadCommand(header, commandsEnd, LC_SYMTAB, SymtabCommandSize);
            if (symtab == null)
                return false;

            uint symbolsOffset = *(uint*)(symtab + 8);
            uint symbolCount = *(uint*)(symtab + 12);
            uint stringsOffset = *(uint*)(symtab + 16);
            uint stringsSize = *(uint*)(symtab + 20);
            if (!FitsWithin(symbolsOffset, (ulong)symbolCount * Nlist64Size, length) || !FitsWithin(stringsOffset, stringsSize, length))
                return false;

            image = new MachOImage(header, commandsEnd, header + symbolsOffset, symbolCount, header + stringsOffset, stringsSize);
            return true;
        }

        private static bool FitsWithin(ulong offset, ulong size, ulong length)
            => offset <= length && size <= length - offset;

        private static unsafe bool IsDefinedInSection(byte* nlist)
        {
            byte type = nlist[4];
            return (type & N_STAB) == 0 && (type & N_TYPE) == N_SECT;
        }

        // Walks the load commands, which are packed back to back after the header, stopping at any malformed one
        private static unsafe byte* FindLoadCommand(byte* header, byte* commandsEnd, uint wantedCmd, int minimumSize)
        {
            uint ncmds = *(uint*)(header + 16);
            byte* cmd = header + MachHeader64Size;
            for (uint i = 0; i < ncmds && TryGetCommandSize(cmd, commandsEnd, out uint cmdSize); i++)
            {
                if (*(uint*)cmd == wantedCmd)
                    return cmdSize >= minimumSize ? cmd : null;
                cmd += cmdSize;
            }
            return null;
        }

        private static unsafe bool TryGetCommandSize(byte* cmd, byte* commandsEnd, out uint size)
        {
            size = 0;
            if ((ulong)(commandsEnd - cmd) < LoadCommandSize)
                return false;

            size = *(uint*)(cmd + 4);
            return size >= LoadCommandSize && size <= (ulong)(commandsEnd - cmd);
        }

        // Every __TEXT section that holds only instructions (__text, il2cpp, __stubs, ...). Section ordinals in
        // nlist.n_sect are 1-based and count sections across all segments in load order
        private static unsafe List<(byte Ordinal, ulong Start, ulong End)> FindCodeSections(MachOImage image)
        {
            var sections = new List<(byte, ulong, ulong)>();

            uint ncmds = *(uint*)(image.Header + 16);
            byte* cmd = image.Header + MachHeader64Size;
            int sectionIndex = 0;
            for (uint i = 0; i < ncmds && TryGetCommandSize(cmd, image.CommandsEnd, out uint cmdSize); i++)
            {
                if (*(uint*)cmd == LC_SEGMENT_64 && cmdSize >= SegmentCommand64Size)
                {
                    // Never read past this command, even if nsects claims more sections than it holds
                    uint nsects = Math.Min(*(uint*)(cmd + 64), (cmdSize - SegmentCommand64Size) / Section64Size);
                    for (uint s = 0; s < nsects; s++)
                    {
                        sectionIndex++;
                        byte* section = cmd + SegmentCommand64Size + (s * Section64Size);
                        uint flags = *(uint*)(section + 64);
                        if (FixedName(section + 16) == "__TEXT" && (flags & S_ATTR_PURE_INSTRUCTIONS) != 0 && sectionIndex <= byte.MaxValue)
                        {
                            ulong start = *(ulong*)(section + 32);
                            sections.Add(((byte)sectionIndex, start, start + *(ulong*)(section + 40)));
                        }
                    }
                }
                cmd += cmdSize;
            }
            return sections;
        }

        private static unsafe string FixedName(byte* name16)
        {
            int length = 0;
            while (length < 16 && name16[length] != 0)
                length++;
            return Encoding.ASCII.GetString(name16, length);
        }

        // Finds the Mach-O image matching the running architecture, handling universal binaries
        private static unsafe bool TryFindSlice(byte* fileStart, ulong fileLength, out byte* slice, out ulong sliceLength)
        {
            slice = null;
            sliceLength = 0;
            if (fileLength < 4)
                return false;

            if (*(uint*)fileStart == MH_MAGIC_64)
            {
                slice = fileStart;
                sliceLength = fileLength;
                return true;
            }

            if (fileLength < FatHeaderSize || ReadBigEndian(fileStart) != FAT_MAGIC)
                return false;

            int wantedCpu = RuntimeInformation.ProcessArchitecture == Architecture.Arm64 ? CPU_TYPE_ARM64 : CPU_TYPE_X86_64;
            uint archCount = ReadBigEndian(fileStart + 4);
            for (uint i = 0; i < archCount; i++)
            {
                // fat_arch: cputype, cpusubtype, offset, size, align (all big-endian)
                ulong archOffset = FatHeaderSize + ((ulong)i * FatArchSize);
                if (!FitsWithin(archOffset, FatArchSize, fileLength))
                    return false;

                byte* arch = fileStart + archOffset;
                if ((int)ReadBigEndian(arch) != wantedCpu)
                    continue;

                uint offset = ReadBigEndian(arch + 8);
                uint size = ReadBigEndian(arch + 12);
                if (!FitsWithin(offset, size, fileLength))
                    return false;

                slice = fileStart + offset;
                sliceLength = size;
                return true;
            }

            return false;
        }

        private static unsafe uint ReadBigEndian(byte* p)
            => (uint)((p[0] << 24) | (p[1] << 16) | (p[2] << 8) | p[3]);
    }
}
#endif
