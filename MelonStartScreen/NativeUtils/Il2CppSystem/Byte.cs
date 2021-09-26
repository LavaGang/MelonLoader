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
            Il2CppClassPointerStore<byte>.NativeClassPtr = IL2CPP.GetIl2CppClass("mscorlib.dll", "System", "Byte");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<byte>.NativeClassPtr);
        }
    }
}
