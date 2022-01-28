using System.Runtime.InteropServices;

namespace MelonLoader.NativeUtils.PEParser
{
    [StructLayout(LayoutKind.Explicit)]
    public struct OptionalFileHeader32
    {
        [FieldOffset(96)]
        public ImageDataDirectory exportTable;
        [FieldOffset(112)]
        public ImageDataDirectory resourceTable;
    }
}