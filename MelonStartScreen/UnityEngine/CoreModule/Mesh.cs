using System;
using UnhollowerMini;

namespace UnityEngine
{
    internal sealed class Mesh : InternalObjectBase
    {
        private static readonly IntPtr m_ctor;
        private static readonly IntPtr m_get_vertices;
        private static readonly IntPtr m_set_vertices;
        private static readonly IntPtr m_get_normals;
        private static readonly IntPtr m_set_normals;
        private static readonly IntPtr m_get_tangents;
        private static readonly IntPtr m_set_tangents;
        private static readonly IntPtr m_get_uv;
        private static readonly IntPtr m_set_uv;
        private static readonly IntPtr m_get_colors;
        private static readonly IntPtr m_set_colors;
        private static readonly IntPtr m_get_colors32;
        private static readonly IntPtr m_set_colors32;
        private static readonly IntPtr m_get_triangles;
        private static readonly IntPtr m_set_triangles;
        private static readonly IntPtr m_RecalculateBounds;

        static Mesh()
        {
            InternalClassPointerStore<Mesh>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Mesh");
            UnityInternals.runtime_class_init(InternalClassPointerStore<Mesh>.NativeClassPtr);

            m_ctor = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, ".ctor", "System.Void");

            m_get_vertices = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "get_vertices", "UnityEngine.Vector3[]");
            m_set_vertices = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "set_vertices", "System.Void", "UnityEngine.Vector3[]");
            m_get_normals = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "get_normals", "UnityEngine.Vector3[]");
            m_set_normals = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "set_normals", "System.Void", "UnityEngine.Vector3[]");
            m_get_tangents = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "get_tangents", "UnityEngine.Vector4[]");
            m_set_tangents = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "set_tangents", "System.Void", "UnityEngine.Vector4[]");
            m_get_uv = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "get_uv", "UnityEngine.Vector2[]");
            m_set_uv = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "set_uv", "System.Void", "UnityEngine.Vector2[]");
            m_get_colors = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "get_colors", "UnityEngine.Color[]");
            m_set_colors = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "set_colors", "System.Void", "UnityEngine.Color[]");
            m_get_colors32 = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "get_colors32", "UnityEngine.Color32[]");
            m_set_colors32 = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "set_colors32", "System.Void", "UnityEngine.Color32[]");
            m_get_triangles = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "get_triangles", "System.Int32[]");
            m_set_triangles = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "set_triangles", "System.Void", "System.Int32[]");

            m_RecalculateBounds = UnityInternals.GetMethod(InternalClassPointerStore<Mesh>.NativeClassPtr, "RecalculateBounds", "System.Void");
        }

        public Mesh(IntPtr ptr) : base(ptr) { }

        public unsafe Mesh() : base(UnityInternals.object_new(InternalClassPointerStore<Mesh>.NativeClassPtr))
        {
            IntPtr returnedException = default;
            UnityInternals.runtime_invoke(m_ctor, UnityInternals.ObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
        }

        public unsafe Vector3[] vertices
        {
            get
            {
                UnityInternals.ObjectBaseToPtrNotNull(this);
                IntPtr returnedException = default;
                IntPtr intPtr = UnityInternals.runtime_invoke(m_get_vertices, UnityInternals.ObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                if (intPtr == IntPtr.Zero) return null;
                Vector3[] arr = new Vector3[UnityInternals.array_length(intPtr)];
                for (int i = 0; i < arr.Length; ++i)
                    arr[i] = ((Vector3*)((long)intPtr + 4 * IntPtr.Size))[i];
                return arr;
            }
            set
            {
                UnityInternals.ObjectBaseToPtrNotNull(this);
                IntPtr valueArrayPtr = UnityInternals.array_new(InternalClassPointerStore<Vector3>.NativeClassPtr, (ulong)value.Length);
                for (var i = 0; i < value.Length; i++)
                    ((Vector3*)((long)valueArrayPtr + 4 * IntPtr.Size))[i] = value[i];
                void** ptr = stackalloc void*[1];
                ptr[0] = (void*)valueArrayPtr;
                IntPtr returnedException = default;
                IntPtr intPtr = UnityInternals.runtime_invoke(m_set_vertices, UnityInternals.ObjectBaseToPtrNotNull(this), ptr, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
            }
        }

        public unsafe Vector3[] normals
        {
            get
            {
                UnityInternals.ObjectBaseToPtrNotNull(this);
                IntPtr returnedException = default;
                IntPtr intPtr = UnityInternals.runtime_invoke(m_get_normals, UnityInternals.ObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                if (intPtr == IntPtr.Zero) return null;
                Vector3[] arr = new Vector3[UnityInternals.array_length(intPtr)];
                for (int i = 0; i < arr.Length; ++i)
                    arr[i] = ((Vector3*)((long)intPtr + 4 * IntPtr.Size))[i];
                return arr;
            }
            set
            {
                UnityInternals.ObjectBaseToPtrNotNull(this);
                IntPtr valueArrayPtr = UnityInternals.array_new(InternalClassPointerStore<Vector3>.NativeClassPtr, (ulong)value.Length);
                for (var i = 0; i < value.Length; i++)
                    ((Vector3*)((long)valueArrayPtr + 4 * IntPtr.Size))[i] = value[i];
                void** ptr = stackalloc void*[1];
                ptr[0] = (void*)valueArrayPtr;
                IntPtr returnedException = default;
                IntPtr intPtr = UnityInternals.runtime_invoke(m_set_normals, UnityInternals.ObjectBaseToPtrNotNull(this), ptr, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
            }
        }

        public unsafe Vector4[] tangents
        {
            get
            {
                UnityInternals.ObjectBaseToPtrNotNull(this);
                IntPtr returnedException = default;
                IntPtr intPtr = UnityInternals.runtime_invoke(m_get_tangents, UnityInternals.ObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                if (intPtr == IntPtr.Zero) return null;
                Vector4[] arr = new Vector4[UnityInternals.array_length(intPtr)];
                for (int i = 0; i < arr.Length; ++i)
                    arr[i] = ((Vector4*)((long)intPtr + 4 * IntPtr.Size))[i];
                return arr;
            }
            set
            {
                UnityInternals.ObjectBaseToPtrNotNull(this);
                IntPtr valueArrayPtr = UnityInternals.array_new(InternalClassPointerStore<Vector4>.NativeClassPtr, (ulong)value.Length);
                for (var i = 0; i < value.Length; i++)
                    ((Vector4*)((long)valueArrayPtr + 4 * IntPtr.Size))[i] = value[i];
                void** ptr = stackalloc void*[1];
                ptr[0] = (void*)valueArrayPtr;
                IntPtr returnedException = default;
                IntPtr intPtr = UnityInternals.runtime_invoke(m_set_tangents, UnityInternals.ObjectBaseToPtrNotNull(this), ptr, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
            }
        }

        public unsafe Vector2[] uv
        {
            get
            {
                UnityInternals.ObjectBaseToPtrNotNull(this);
                IntPtr returnedException = default;
                IntPtr intPtr = UnityInternals.runtime_invoke(m_get_uv, UnityInternals.ObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                if (intPtr == IntPtr.Zero) return null;
                Vector2[] arr = new Vector2[UnityInternals.array_length(intPtr)];
                for (int i = 0; i < arr.Length; ++i)
                    arr[i] = ((Vector2*)((long)intPtr + 4 * IntPtr.Size))[i];
                return arr;
            }
            set
            {
                UnityInternals.ObjectBaseToPtrNotNull(this);
                IntPtr valueArrayPtr = UnityInternals.array_new(InternalClassPointerStore<Vector2>.NativeClassPtr, (ulong)value.Length);
                for (var i = 0; i < value.Length; i++)
                    ((Vector2*)((long)valueArrayPtr + 4 * IntPtr.Size))[i] = value[i];
                void** ptr = stackalloc void*[1];
                ptr[0] = (void*)valueArrayPtr;
                IntPtr returnedException = default;
                IntPtr intPtr = UnityInternals.runtime_invoke(m_set_uv, UnityInternals.ObjectBaseToPtrNotNull(this), ptr, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
            }
        }

        public unsafe Color[] colors
        {
            get
            {
                UnityInternals.ObjectBaseToPtrNotNull(this);
                IntPtr returnedException = default;
                IntPtr intPtr = UnityInternals.runtime_invoke(m_get_colors, UnityInternals.ObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                if (intPtr == IntPtr.Zero) return null;
                Color[] arr = new Color[UnityInternals.array_length(intPtr)];
                for (int i = 0; i < arr.Length; ++i)
                    arr[i] = ((Color*)((long)intPtr + 4 * IntPtr.Size))[i];
                return arr;
            }
            set
            {
                UnityInternals.ObjectBaseToPtrNotNull(this);
                IntPtr valueArrayPtr = UnityInternals.array_new(InternalClassPointerStore<Color>.NativeClassPtr, (ulong)value.Length);
                for (var i = 0; i < value.Length; i++)
                    ((Color*)((long)valueArrayPtr + 4 * IntPtr.Size))[i] = value[i];
                void** ptr = stackalloc void*[1];
                ptr[0] = (void*)valueArrayPtr;
                IntPtr returnedException = default;
                IntPtr intPtr = UnityInternals.runtime_invoke(m_set_colors, UnityInternals.ObjectBaseToPtrNotNull(this), ptr, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
            }
        }

        public unsafe Color32[] colors32
        {
            get
            {
                UnityInternals.ObjectBaseToPtrNotNull(this);
                IntPtr returnedException = default;
                IntPtr intPtr = UnityInternals.runtime_invoke(m_get_colors32, UnityInternals.ObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                if (intPtr == IntPtr.Zero) return null;
                Color32[] arr = new Color32[UnityInternals.array_length(intPtr)];
                for (int i = 0; i < arr.Length; ++i)
                    arr[i] = ((Color32*)((long)intPtr + 4 * IntPtr.Size))[i];
                return arr;
            }
            set
            {
                UnityInternals.ObjectBaseToPtrNotNull(this);
                IntPtr valueArrayPtr = UnityInternals.array_new(InternalClassPointerStore<Color32>.NativeClassPtr, (ulong)value.Length);
                for (var i = 0; i < value.Length; i++)
                    ((Color32*)((long)valueArrayPtr + 4 * IntPtr.Size))[i] = value[i];
                void** ptr = stackalloc void*[1];
                ptr[0] = (void*)valueArrayPtr;
                IntPtr returnedException = default;
                IntPtr intPtr = UnityInternals.runtime_invoke(m_set_colors32, UnityInternals.ObjectBaseToPtrNotNull(this), ptr, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
            }
        }

        public unsafe int[] triangles
        {
            get
            {
                UnityInternals.ObjectBaseToPtrNotNull(this);
                IntPtr returnedException = default;
                IntPtr intPtr = UnityInternals.runtime_invoke(m_get_triangles, UnityInternals.ObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                if (intPtr == IntPtr.Zero) return null;
                int[] arr = new int[UnityInternals.array_length(intPtr)];
                for (int i = 0; i < arr.Length; ++i)
                    arr[i] = ((int*)((long)intPtr + 4 * IntPtr.Size))[i];
                return arr;
            }
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
