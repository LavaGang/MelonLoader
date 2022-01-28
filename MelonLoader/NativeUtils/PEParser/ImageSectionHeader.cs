using System.Runtime.InteropServices;

namespace MelonLoader.NativeUtils.PEParser
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ImageSectionHeader
    {
        //[FieldOffset(0)]
        //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        //public char[8] name;
        [FieldOffset(8)]
        public int virtualSize;
        [FieldOffset(12)]
        public int virtualAddress;
        [FieldOffset(16)]
        public int sizeOfRawData;
        [FieldOffset(20)]
        public int pointerToRawData;
        [FieldOffset(24)]
        public int pointerToRelocations;
        [FieldOffset(28)]
        public int pointerToLinenumbers;
        [FieldOffset(32)]
        public ushort numberOfRelocations;
        [FieldOffset(34)]
        public ushort numberOfLinenumbers;
        [FieldOffset(36)]
        public /* DataSectionFlags */ uint characteristics;
    }
}
