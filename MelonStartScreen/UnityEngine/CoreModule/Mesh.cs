using System;
using UnhollowerMini;
using UnityEngine.Rendering;

namespace UnityEngine
{
    internal sealed class Mesh : InternalObjectBase
    {
        private static readonly IntPtr m_ctor;
        private static readonly IntPtr m_set_triangles;
        private static readonly IntPtr m_RecalculateBounds;

        private static unsafe readonly delegate* unmanaged[Cdecl]<IntPtr, int, int, int, IntPtr, int, void> m_SetArrayForChannelImpl;

        unsafe static Mesh()
        {
            InternalClassPointerStore<Mesh>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Mesh");
            UnityInternals.runtime_class_init(InternalClassPointerStore<Mesh>.NativeClassPtr);

            m_ctor = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, ".ctor", "System.Void");

            m_set_triangles = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "set_triangles", "System.Void", "System.Int32[]");
            m_RecalculateBounds = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "RecalculateBounds", "System.Void");
            m_SetArrayForChannelImpl = (delegate* unmanaged[Cdecl]<IntPtr, int, int, int, IntPtr, int, void>)UnityInternals.ResolveICall("UnityEngine.Mesh::SetArrayForChannelImpl");
        }

        public Mesh(IntPtr ptr) : base(ptr) { }

        public unsafe Mesh() : base(UnityInternals.object_new(InternalClassPointerStore<Mesh>.NativeClassPtr))
        {
            IntPtr returnedException = default;
            UnityInternals.runtime_invoke(m_ctor, UnityInternals.ObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
        }

        private unsafe void SetArrayForChannel<T>(int channel, T[] values, int channelDimensions) where T : unmanaged
        {
            UnityInternals.ObjectBaseToPtrNotNull(this);
            IntPtr valueArrayPtr = UnityInternals.array_new(InternalClassPointerStore<T>.NativeClassPtr, (ulong)values.Length);
            for (var i = 0; i < values.Length; i++)
                ((T*)((long)valueArrayPtr + 4 * IntPtr.Size))[i] = values[i];

            m_SetArrayForChannelImpl(UnityInternals.ObjectBaseToPtrNotNull(this), channel, 0 /* float */, channelDimensions, valueArrayPtr, (values == null) ? 0 : values.Length);
        }


        public unsafe Vector3[] vertices
        {
            set => SetArrayForChannel(VertexAttribute.Vertex, value, 3);
        }

        public unsafe Vector3[] normals
        {
            set => SetArrayForChannel(VertexAttribute.Normal, value, 3);
        }

        public unsafe Vector4[] tangents
        {
            set => SetArrayForChannel(VertexAttribute.Tangent, value, 4);
        }

        public unsafe Vector2[] uv
        {
            set => SetArrayForChannel(VertexAttribute.TexCoord0, value, 2);
        }

        public unsafe Color[] colors
        {
            set => SetArrayForChannel(VertexAttribute.Color, value, 4);
        }

        public unsafe int[] triangles
        {
            set
            {
                UnityInternals.ObjectBaseToPtrNotNull(this);
                IntPtr valueArrayPtr = UnityInternals.array_new(InternalClassPointerStore<int>.NativeClassPtr, (ulong)value.Length);
                for (var i = 0; i < value.Length; i++)
                    ((int*)((long)valueArrayPtr + 4 * IntPtr.Size))[i] = value[i];
                void** ptr = stackalloc void*[1];
                ptr[0] = (void*)valueArrayPtr;
                IntPtr returnedException = default;
                IntPtr intPtr = UnityInternals.runtime_invoke(m_set_triangles, UnityInternals.ObjectBaseToPtrNotNull(this), ptr, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
            }
        }

        public unsafe void RecalculateBounds()
        {
            UnityInternals.ObjectBaseToPtrNotNull(this);
            IntPtr returnedException = default;
            UnityInternals.runtime_invoke(m_RecalculateBounds, UnityInternals.ObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
        }

    }
}
