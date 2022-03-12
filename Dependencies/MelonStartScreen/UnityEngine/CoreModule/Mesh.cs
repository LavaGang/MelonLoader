using MelonLoader;
using MelonLoader.MelonStartScreen.NativeUtils;
using System;
using UnhollowerMini;
using MelonUnityEngine.Rendering;

namespace MelonUnityEngine
{
    internal sealed class Mesh : UnityObject
    {
        private delegate void SetArrayForChannelImpl_2017(IntPtr @this, int channel, int format, int dim, IntPtr values, int arraySize);
        private delegate void SetArrayForChannelImpl_2019(IntPtr @this, int channel, int format, int dim, IntPtr values, int arraySize, int valuesStart, int valuesCount);
        private delegate void SetArrayForChannelImpl_2020(IntPtr @this, int channel, int format, int dim, IntPtr values, int arraySize, int valuesStart, int valuesCount, int updateFlags);

        private static readonly IntPtr m_ctor;
        private static readonly IntPtr m_set_triangles;
        private static readonly IntPtr m_RecalculateBounds;

        private static readonly SetArrayForChannelImpl_2017 m_SetArrayForChannelImpl_2017;
        private static readonly SetArrayForChannelImpl_2019 m_SetArrayForChannelImpl_2019;
        private static readonly SetArrayForChannelImpl_2020 m_SetArrayForChannelImpl_2020;
        private static readonly int type_SetArrayForChannelImpl = -1;

        static Mesh()
        {
            InternalClassPointerStore<Mesh>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Mesh");
            UnityInternals.runtime_class_init(InternalClassPointerStore<Mesh>.NativeClassPtr);

            m_ctor = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, ".ctor", "System.Void");

            m_set_triangles = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "set_triangles", "System.Void", "System.Int32[]");
            m_RecalculateBounds = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "RecalculateBounds", "System.Void");

            if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new string[] { "2020.1.0" }))
            {
                m_SetArrayForChannelImpl_2020 = UnityInternals.ResolveICall<SetArrayForChannelImpl_2020>("UnityEngine.Mesh::SetArrayForChannelImpl");
                type_SetArrayForChannelImpl = 2;
            }
            else if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new string[] { "2019.3.0" }))
            {
                m_SetArrayForChannelImpl_2019 = UnityInternals.ResolveICall<SetArrayForChannelImpl_2019>("UnityEngine.Mesh::SetArrayForChannelImpl");
                type_SetArrayForChannelImpl = 1;
            }
            else if (NativeSignatureResolver.IsUnityVersionOverOrEqual(MelonLoader.InternalUtils.UnityInformationHandler.EngineVersion.ToStringWithoutType(), new string[] { "2017.1.0" }))
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

        private unsafe void SetArrayForChannelImpl(int channel, IntPtr values, int channelDimensions, int valuesCount)
        {
            if (type_SetArrayForChannelImpl == 0)
                m_SetArrayForChannelImpl_2017(UnityInternals.ObjectBaseToPtrNotNull(this), channel, 0 /* float */, channelDimensions, values, valuesCount);
            else if (type_SetArrayForChannelImpl == 1)
                m_SetArrayForChannelImpl_2019(UnityInternals.ObjectBaseToPtrNotNull(this), channel, 0 /* float */, channelDimensions, values, valuesCount, 0, valuesCount);
            else if (type_SetArrayForChannelImpl == 2)
                m_SetArrayForChannelImpl_2020(UnityInternals.ObjectBaseToPtrNotNull(this), channel, 0 /* float */, channelDimensions, values, valuesCount, 0, valuesCount, 0 /* MeshUpdateFlags.Default */);
            else
                throw new NotImplementedException("SetArrayForChannel isn't implemented for this version of Unity");
        }


        public unsafe Vector3[] vertices
        {
            set
            {
                int valuesCount = value.Length;

                IntPtr valueArrayPtr = UnityInternals.array_new(InternalClassPointerStore<Vector3>.NativeClassPtr, (ulong)valuesCount);
                for (var i = 0; i < valuesCount; i++)
                    ((Vector3*)((long)valueArrayPtr + 4 * IntPtr.Size))[i] = value[i];

                SetArrayForChannelImpl(VertexAttribute.Vertex, valueArrayPtr, 3, valuesCount);
            }
        }

        public unsafe Vector3[] normals
        {
            set
            {
                int valuesCount = value.Length;

                IntPtr valueArrayPtr = UnityInternals.array_new(InternalClassPointerStore<Vector3>.NativeClassPtr, (ulong)valuesCount);
                for (var i = 0; i < valuesCount; i++)
                    ((Vector3*)((long)valueArrayPtr + 4 * IntPtr.Size))[i] = value[i];

                SetArrayForChannelImpl(VertexAttribute.Normal, valueArrayPtr, 3, valuesCount);
            }
        }

        public unsafe Vector4[] tangents
        {
            set
            {
                int valuesCount = value.Length;

                IntPtr valueArrayPtr = UnityInternals.array_new(InternalClassPointerStore<Vector4>.NativeClassPtr, (ulong)valuesCount);
                for (var i = 0; i < valuesCount; i++)
                    ((Vector4*)((long)valueArrayPtr + 4 * IntPtr.Size))[i] = value[i];

                SetArrayForChannelImpl(VertexAttribute.Tangent, valueArrayPtr, 4, valuesCount);
            }
        }

        public unsafe Vector2[] uv
        {
            set
            {
                int valuesCount = value.Length;

                IntPtr valueArrayPtr = UnityInternals.array_new(InternalClassPointerStore<Vector2>.NativeClassPtr, (ulong)valuesCount);
                for (var i = 0; i < valuesCount; i++)
                    ((Vector2*)((long)valueArrayPtr + 4 * IntPtr.Size))[i] = value[i];

                SetArrayForChannelImpl(VertexAttribute.TexCoord0, valueArrayPtr, 2, valuesCount);
            }
        }

        public unsafe Color[] colors
        {
            set
            {
                int valuesCount = value.Length;

                IntPtr valueArrayPtr = UnityInternals.array_new(InternalClassPointerStore<Color>.NativeClassPtr, (ulong)valuesCount);
                for (var i = 0; i < valuesCount; i++)
                    ((Color*)((long)valueArrayPtr + 4 * IntPtr.Size))[i] = value[i];

                SetArrayForChannelImpl(VertexAttribute.Color, valueArrayPtr, 4, valuesCount);
            }
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
