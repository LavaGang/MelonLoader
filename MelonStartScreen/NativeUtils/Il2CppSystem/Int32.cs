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
            Il2CppClassPointerStore<int>.NativeClassPtr = IL2CPP.GetIl2CppClass("mscorlib.dll", "System", "Int32");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<int>.NativeClassPtr);
        }
    }
}
