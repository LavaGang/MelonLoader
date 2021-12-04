using System.Runtime.InteropServices;
using UnhollowerMini;

namespace Il2CppSystem
{
    [StructLayout(LayoutKind.Explicit)]
    internal class Int32
    {
        [FieldOffset(0)]
        public int m_value;

        static Int32()
        {
            InternalClassPointerStore<int>.NativeClassPtr = UnityInternals.GetClass("mscorlib.dll", "System", "Int32");
            UnityInternals.runtime_class_init(InternalClassPointerStore<int>.NativeClassPtr);
        }
    }
}
