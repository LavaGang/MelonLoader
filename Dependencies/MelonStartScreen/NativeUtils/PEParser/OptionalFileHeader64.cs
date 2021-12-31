using System.Runtime.InteropServices;

namespace MelonLoader.MelonStartScreen.NativeUtils.PEParser
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct OptionalFileHeader64
    {
        [FieldOffset(112)]
        public ImageDataDirectory exportTable;
    }
}