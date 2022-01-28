using System.Runtime.InteropServices;

namespace MelonLoader.NativeUtils.PEParser
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ImageResourceDirectory
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
        public uint numberOfNames;
        [FieldOffset(14)]
        public uint numberOfIds;
    }
}
