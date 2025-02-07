using System.Runtime.InteropServices;

namespace MelonLoader.Runtime.Mono
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct MonoAssemblyName
    {
        public nint Name;

        // Non-marshalled strings
        public nint Culture;
        public nint HashValue;
        public nint PublicKey;

        public fixed byte PublicKeyToken[17];

        public uint HashAlg;
        public uint HashLength;

        public uint Flags;
        public ushort Major;
        public ushort Minor;
        public ushort Build;
        public ushort Revision;
        public uint Arch;
    }
}
