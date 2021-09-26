using System;
using System.Runtime.InteropServices;
using UnhollowerMini;

namespace UnityEngine
{
    internal class Resources
    {
        private static readonly IntPtr m_GetBuiltinResource;

        static Resources()
        {

            Il2CppClassPointerStore<Resources>.NativeClassPtr = IL2CPP.GetIl2CppClass("UnityEngine.CoreModule.dll", "UnityEngine", "Resources");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<Resources>.NativeClassPtr);

            /*
            IntPtr mptr = IntPtr.Zero;
            IntPtr iter = IntPtr.Zero;
            while ((mptr = IL2CPP.il2cpp_class_get_methods(Il2CppClassPointerStore<Resources>.NativeClassPtr, ref iter)) != IntPtr.Zero)
            {
                uint paramCount = IL2CPP.il2cpp_method_get_param_count(mptr);
                string[] paramTypes = new string[paramCount];
                for (uint i = 0; i < paramCount; ++i)
                    paramTypes[i] = Marshal.PtrToStringAnsi(IL2CPP.il2cpp_type_get_name(IL2CPP.il2cpp_method_get_param(mptr, i)));
                MelonLoader.MelonLogger.Msg($" {Marshal.PtrToStringAnsi(IL2CPP.il2cpp_type_get_name(IL2CPP.il2cpp_method_get_return_type(mptr)))} {Marshal.PtrToStringAnsi(IL2CPP.il2cpp_method_get_name(mptr))}({string.Join(", ", paramTypes)})");
            }
            */
            // T GetBuiltinResource(System.String)

            m_GetBuiltinResource = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Resources>.NativeClassPtr, "GetBuiltinResource", "UnityEngine.Object", "System.Type", "System.String");

        }

        public unsafe static IntPtr GetBuiltinResource(Il2CppSystem.Type type, string path)
        {
            void** ptr = stackalloc void*[2];
            ptr[0] = (void*)IL2CPP.Il2CppObjectBaseToPtr(type);
            ptr[1] = (void*)IL2CPP.ManagedStringToIl2Cpp(path);
            IntPtr returnedException = default;
            IntPtr objectPointer = IL2CPP.il2cpp_runtime_invoke(m_GetBuiltinResource, IntPtr.Zero, ptr, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
            return objectPointer;
        }

        public static T GetBuiltinResource<T>(string path) where T : Il2CppObjectBase
        {
            IntPtr ptr = GetBuiltinResource(Il2CppType.Of<T>(), path);
            return ptr != IntPtr.Zero ? (T)typeof(T).GetConstructor(new[] { typeof(IntPtr) }).Invoke(new object[] { ptr }) : null;
        }
    }
}
