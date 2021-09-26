using System;
using UnhollowerMini;

namespace UnityEngine
{
    class TextGenerator : Il2CppObjectBase
    {
        private delegate int get_vertexCountDelegate(IntPtr @this);
        private delegate IntPtr GetVerticesArrayDelegate(IntPtr @this);

        private static readonly IntPtr m_ctor;
        private static readonly IntPtr m_Populate;
        private static readonly get_vertexCountDelegate fd_get_vertexCount;
        private static readonly GetVerticesArrayDelegate fd_GetVerticesArray;

        static TextGenerator()
        {
            Il2CppClassPointerStore<TextGenerator>.NativeClassPtr = IL2CPP.GetIl2CppClass("UnityEngine.TextRenderingModule.dll", "UnityEngine", "TextGenerator");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<TextGenerator>.NativeClassPtr);

            m_ctor = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<TextGenerator>.NativeClassPtr, ".ctor", "System.Void");

            m_Populate = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<TextGenerator>.NativeClassPtr, "Populate", "System.Boolean", "System.String", "UnityEngine.TextGenerationSettings");

            fd_get_vertexCount = IL2CPP.ResolveICall<get_vertexCountDelegate>("UnityEngine.TextGenerator::get_vertexCount");
            fd_GetVerticesArray = IL2CPP.ResolveICall<GetVerticesArrayDelegate>("UnityEngine.TextGenerator::GetVerticesArray");
        }

        public TextGenerator(IntPtr ptr) : base(ptr) { }

        public unsafe TextGenerator() : this(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<TextGenerator>.NativeClassPtr))
        {
            IntPtr returnedException = default;
            IL2CPP.il2cpp_runtime_invoke(m_ctor, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
        }

        public unsafe bool Populate(string str, TextGenerationSettings settings)
        {
            void** args = stackalloc void*[2];
            args[0] = (void*)IL2CPP.ManagedStringToIl2Cpp(str);
            args[1] = (void*)IL2CPP.il2cpp_object_unbox(IL2CPP.Il2CppObjectBaseToPtrNotNull(settings));
            IntPtr returnedException = default;
            IntPtr obj = IL2CPP.il2cpp_runtime_invoke(m_Populate, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), args, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
            return *(bool*)IL2CPP.il2cpp_object_unbox(obj);
        }

        public int vertexCount => fd_get_vertexCount(IL2CPP.Il2CppObjectBaseToPtrNotNull(this));

        public unsafe UIVertex[] GetVerticesArray()
        {
            IntPtr intPtr = fd_GetVerticesArray(IL2CPP.Il2CppObjectBaseToPtrNotNull(this));
            if (intPtr == IntPtr.Zero) return null;
            UIVertex[] arr = new UIVertex[IL2CPP.il2cpp_array_length(intPtr)];
            for (int i = 0; i < arr.Length; ++i)
                arr[i] = ((UIVertex*)(intPtr + 4 * IntPtr.Size))[i];
            return arr;
        }
    }
}
