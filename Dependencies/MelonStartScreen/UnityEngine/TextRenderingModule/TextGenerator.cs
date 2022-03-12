using System;
using UnhollowerMini;

namespace MelonUnityEngine
{
    class TextGenerator : InternalObjectBase
    {
        private delegate int get_vertexCountDelegate(IntPtr @this);
        private delegate IntPtr GetVerticesArrayDelegate(IntPtr @this);

        private static readonly IntPtr m_ctor;
        private static readonly IntPtr m_Populate;
        private static readonly get_vertexCountDelegate fd_get_vertexCount;
        private static readonly GetVerticesArrayDelegate fd_GetVerticesArray;

        static TextGenerator()
        {
            InternalClassPointerStore<TextGenerator>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.TextRenderingModule.dll", "UnityEngine", "TextGenerator");
            UnityInternals.runtime_class_init(InternalClassPointerStore<TextGenerator>.NativeClassPtr);

            m_ctor = UnityInternals.GetMethod(InternalClassPointerStore<TextGenerator>.NativeClassPtr, ".ctor", "System.Void");

            m_Populate = UnityInternals.GetMethod(InternalClassPointerStore<TextGenerator>.NativeClassPtr, "Populate", "System.Boolean", "System.String", "UnityEngine.TextGenerationSettings");

            fd_get_vertexCount = UnityInternals.ResolveICall<get_vertexCountDelegate>("UnityEngine.TextGenerator::get_vertexCount");
            fd_GetVerticesArray = UnityInternals.ResolveICall<GetVerticesArrayDelegate>("UnityEngine.TextGenerator::GetVerticesArray");
        }

        public TextGenerator(IntPtr ptr) : base(ptr) { }

        public unsafe TextGenerator() : this(UnityInternals.object_new(InternalClassPointerStore<TextGenerator>.NativeClassPtr))
        {
            IntPtr returnedException = default;
            UnityInternals.runtime_invoke(m_ctor, UnityInternals.ObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
        }

        public unsafe bool Populate(string str, TextGenerationSettings settings)
        {
            void** args = stackalloc void*[2];
            args[0] = (void*)UnityInternals.ManagedStringToInternal(str);
            args[1] = (void*)UnityInternals.object_unbox(UnityInternals.ObjectBaseToPtrNotNull(settings));
            IntPtr returnedException = default;
            IntPtr obj = UnityInternals.runtime_invoke(m_Populate, UnityInternals.ObjectBaseToPtrNotNull(this), args, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
            return *(bool*)UnityInternals.object_unbox(obj);
        }

        public int vertexCount => fd_get_vertexCount(UnityInternals.ObjectBaseToPtrNotNull(this));

        public unsafe UIVertexWrapper[] GetVerticesArray()
        {
            IntPtr intPtr = fd_GetVerticesArray(UnityInternals.ObjectBaseToPtrNotNull(this));
            if (intPtr == IntPtr.Zero) return null;
            UIVertexWrapper[] arr = new UIVertexWrapper[UnityInternals.array_length(intPtr)];
            for (int i = 0; i < arr.Length; ++i)
                arr[i] = new UIVertexWrapper((IntPtr)((long)intPtr + 4 * IntPtr.Size + i * UIVertexWrapper.sizeOfElement));
            // arr[i] =  ( (UIVertex*)((long)intPtr + 4 * IntPtr.Size) )[i];
            // arr[i] = *( (UIVertex*)((long)intPtr + 4 * IntPtr.Size) + (i * sizeof(UIVertex)))
            return arr;
        }
    }
}
