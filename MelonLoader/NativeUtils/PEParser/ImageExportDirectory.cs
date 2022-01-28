using System.Runtime.InteropServices;

namespace MelonLoader.NativeUtils.PEParser
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ImageExportDirectory
    {
        [FieldOffset(0)]
        public uint characteristics;
        [FieldOffset(4)]
        public uint timedatestamp;
        [FieldOffset(8)]
        public ushort majorVersion;
        [FieldOffset(10)]
        public ushort minorVersion;
        [FieldOffset(12)]
        public uint name;
        [FieldOffset(16)]
        public uint base_;
        [FieldOffset(20)]
        public uint numberOfFunctions;
        [FieldOffset(24)]
        public uint numberOfNames;
        [FieldOffset(28)]
        public uint addressOfFunctions;
        [FieldOffset(32)]
        public uint addressOfNames;
        [FieldOffset(36)]
        public uint addressOfNameOrdinals;
    }
}
