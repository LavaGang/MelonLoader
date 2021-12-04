using System.Runtime.InteropServices;
using UnhollowerMini;

namespace Il2CppSystem
{
    [StructLayout(LayoutKind.Explicit)]
    internal class Byte
    {
        [FieldOffset(0)]
        public byte m_value;

        static Byte()
        {
            InternalClassPointerStore<byte>.NativeClassPtr = UnityInternals.GetClass("mscorlib.dll", "System", "Byte");
            UnityInternals.runtime_class_init(InternalClassPointerStore<byte>.NativeClassPtr);
        }
    }
}
