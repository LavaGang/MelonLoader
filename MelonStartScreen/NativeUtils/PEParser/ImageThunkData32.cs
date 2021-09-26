using System.Runtime.InteropServices;

namespace MelonLoader.MelonStartScreen.NativeUtils.PEParser
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct ImageThunkData32
    {
        [FieldOffset(0)]
        public uint forwarderString;
        [FieldOffset(0)]
        public uint function;
        [FieldOffset(0)]
        public uint ordinal;
        [FieldOffset(0)]
        public uint addressOfData;
    }
}
