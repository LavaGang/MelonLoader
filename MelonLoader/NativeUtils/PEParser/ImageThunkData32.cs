using System.Runtime.InteropServices;

namespace MelonLoader.NativeUtils.PEParser
{
    [StructLayout(LayoutKind.Explicit)]
    public struct ImageThunkData32
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
