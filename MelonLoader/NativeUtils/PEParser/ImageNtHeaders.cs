using System.Runtime.InteropServices;

namespace MelonLoader.NativeUtils.PEParser
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ImageNtHeaders
    {
        [FieldOffset(0)]
        public uint signature;
        [FieldOffset(4)]
        public ImageFileHeader fileHeader;
        [FieldOffset(24)]
        public OptionalFileHeader64 optionalHeader64;
        [FieldOffset(24)]
        public OptionalFileHeader32 optionalHeader32;
    }
}
