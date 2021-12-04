using System.Runtime.InteropServices;

namespace MelonLoader.MelonStartScreen.NativeUtils.PEParser
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct OptionalFileHeader32
    {
        [FieldOffset(96)]
        public ImageDataDirectory exportTable;
    }
}