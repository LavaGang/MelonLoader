using MelonLoader;
using MelonLoader.MelonStartScreen.NativeUtils;
using System;
using System.Runtime.InteropServices;
using UnhollowerMini;
using UnityEngine.Rendering;

namespace UnityEngine
{
    internal sealed class Mesh : InternalObjectBase
    {
        private delegate void SetArrayForChannelImpl_2017(IntPtr @this, int channel, int format, int dim, Array values, int arraySize);
        private delegate void SetArrayForChannelImpl_2019(IntPtr @this, int channel, int format, int dim, Array values, int arraySize, int valuesStart, int valuesCount);
        private delegate void SetArrayForChannelImpl_2020(IntPtr @this, int channel, int format, int dim, Array values, int arraySize, int valuesStart, int valuesCount, int updateFlags);

        private static readonly IntPtr m_ctor;
        private static readonly IntPtr m_set_triangles;
        private static readonly IntPtr m_RecalculateBounds;

        private static readonly SetArrayForChannelImpl_2017 m_SetArrayForChannelImpl_2017;
        private static readonly SetArrayForChannelImpl_2019 m_SetArrayForChannelImpl_2019;
        private static readonly SetArrayForChannelImpl_2020 m_SetArrayForChannelImpl_2020;
        private static readonly int type_SetArrayForChannelImpl = -1;

        unsafe static Mesh()
        {
            InternalClassPointerStore<Mesh>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Mesh");
            UnityInternals.runtime_class_init(InternalClassPointerStore<Mesh>.NativeClassPtr);

            m_ctor = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, ".ctor", "System.Void");

            m_set_triangles = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "set_triangles", "System.Void", "System.Int32[]");
            m_RecalculateBounds = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "RecalculateBounds", "System.Void");

            if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonUtils.GetUnityVersion(), new string[] { "2020.1.0" }))
            {
                m_SetArrayForChannelImpl_2020 = UnityInternals.ResolveICall<SetArrayForChannelImpl_2020>("UnityEngine.Mesh::SetArrayForChannelImpl");
                type_SetArrayForChannelImpl = 2;
            }
            else if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonUtils.GetUnityVersion(), new string[] { "2019.3.0" }))
            {
                m_SetArrayForChannelImpl_2019 = UnityInternals.ResolveICall<SetArrayForChannelImpl_2019>("UnityEngine.Mesh::SetArrayForChannelImpl");
                type_SetArrayForChannelImpl = 1;
            }
            else if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonUtils.GetUnityVersion(), new string[] { "2017.1.0" }))
            {
                m_SetArrayForChannelImpl_2017 = UnityInternals.ResolveICall<SetArrayForChannelImpl_2017>("UnityEngine.Mesh::SetArrayForChannelImpl");
                type_SetArrayForChannelImpl = 0;
            }
        }

        public Mesh(IntPtr ptr) : base(ptr) { }

        public unsafe Mesh() : base(UnityInternals.object_new(InternalClassPointerStore<Mesh>.NativeClassPtr))
        {
            IntPtr returnedException = default;
            UnityInternals.runtime_invoke(m_ctor, UnityInternals.ObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
        }

        private unsafe void SetArrayForChannel(int channel, Array values, int channelDimensions)
        {
            int valuesCount = values.Length;

            if (type_SetArrayForChannelImpl == 0)
                m_SetArrayForChannelImpl_2017(UnityInternals.ObjectBaseToPtrNotNull(this), channel, 0 /* float */, channelDimensions, values, valuesCount);
            else if (type_SetArrayForChannelImpl == 1)
                m_SetArrayForChannelImpl_2019(UnityInternals.ObjectBaseToPtrNotNull(this), channel, 0 /* float */, channelDimensions, values, valuesCount, 0, valuesCount);
            else if (type_SetArrayForChannelImpl == 2)
                m_SetArrayForChannelImpl_2020(UnityInternals.ObjectBaseToPtrNotNull(this), channel, 0 /* float */, channelDimensions, values, valuesCount, 0, valuesCount, 0 /* MeshUpdateFlags.Default */);
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
