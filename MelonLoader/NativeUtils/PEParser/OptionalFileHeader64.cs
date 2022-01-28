using System.Runtime.InteropServices;

namespace MelonLoader.NativeUtils.PEParser
{
    [StructLayout(LayoutKind.Explicit)]
    public struct OptionalFileHeader64
    {
        [FieldOffset(112)]
        public ImageDataDirectory exportTable;
        [FieldOffset(128)]
        public ImageDataDirectory resourceTable;
    }
}