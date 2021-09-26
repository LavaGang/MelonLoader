using System;
using UnhollowerMini;

namespace UnityEngine
{
    internal sealed class Mesh : Il2CppObjectBase
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
            Il2CppClassPointerStore<Mesh>.NativeClassPtr = IL2CPP.GetIl2CppClass("UnityEngine.CoreModule.dll", "UnityEngine", "Mesh");
            IL2CPP.il2cpp_runtime_class_init(Il2CppClassPointerStore<Mesh>.NativeClassPtr);

            m_ctor = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Mesh>.NativeClassPtr, ".ctor", "System.Void");

            m_get_vertices = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Mesh>.NativeClassPtr, "get_vertices", "UnityEngine.Vector3[]");
            m_set_vertices = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Mesh>.NativeClassPtr, "set_vertices", "System.Void", "UnityEngine.Vector3[]");
            m_get_normals = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Mesh>.NativeClassPtr, "get_normals", "UnityEngine.Vector3[]");
            m_set_normals = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Mesh>.NativeClassPtr, "set_normals", "System.Void", "UnityEngine.Vector3[]");
            m_get_tangents = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Mesh>.NativeClassPtr, "get_tangents", "UnityEngine.Vector4[]");
            m_set_tangents = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Mesh>.NativeClassPtr, "set_tangents", "System.Void", "UnityEngine.Vector4[]");
            m_get_uv = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Mesh>.NativeClassPtr, "get_uv", "UnityEngine.Vector2[]");
            m_set_uv = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Mesh>.NativeClassPtr, "set_uv", "System.Void", "UnityEngine.Vector2[]");
            m_get_colors = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Mesh>.NativeClassPtr, "get_colors", "UnityEngine.Color[]");
            m_set_colors = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Mesh>.NativeClassPtr, "set_colors", "System.Void", "UnityEngine.Color[]");
            m_get_colors32 = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Mesh>.NativeClassPtr, "get_colors32", "UnityEngine.Color32[]");
            m_set_colors32 = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Mesh>.NativeClassPtr, "set_colors32", "System.Void", "UnityEngine.Color32[]");
            m_get_triangles = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Mesh>.NativeClassPtr, "get_triangles", "System.Int32[]");
            m_set_triangles = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Mesh>.NativeClassPtr, "set_triangles", "System.Void", "System.Int32[]");

            m_RecalculateBounds = IL2CPP.GetIl2CppMethod(Il2CppClassPointerStore<Mesh>.NativeClassPtr, "RecalculateBounds", "System.Void");
        }

        public Mesh(IntPtr ptr) : base(ptr) { }

        public unsafe Mesh() : base(IL2CPP.il2cpp_object_new(Il2CppClassPointerStore<Mesh>.NativeClassPtr))
        {
            IntPtr returnedException = default;
            IL2CPP.il2cpp_runtime_invoke(m_ctor, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
        }

        public unsafe Vector3[] vertices
        {
            get
            {
                IL2CPP.Il2CppObjectBaseToPtrNotNull(this);
                IntPtr returnedException = default;
                IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(m_get_vertices, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                if (intPtr == IntPtr.Zero) return null;
                Vector3[] arr = new Vector3[IL2CPP.il2cpp_array_length(intPtr)];
                for (int i = 0; i < arr.Length; ++i)
                    arr[i] = ((Vector3*)(intPtr + 4 * IntPtr.Size))[i];
                return arr;
            }
            set
            {
                IL2CPP.Il2CppObjectBaseToPtrNotNull(this);
                IntPtr valueArrayPtr = IL2CPP.il2cpp_array_new(Il2CppClassPointerStore<Vector3>.NativeClassPtr, (ulong)value.Length);
                for (var i = 0; i < value.Length; i++)
                    ((Vector3*)(valueArrayPtr + 4 * IntPtr.Size))[i] = value[i];
                void** ptr = stackalloc void*[1];
                ptr[0] = (void*)valueArrayPtr;
                IntPtr returnedException = default;
                IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(m_set_vertices, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), ptr, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
            }
        }

        public unsafe Vector3[] normals
        {
            get
            {
                IL2CPP.Il2CppObjectBaseToPtrNotNull(this);
                IntPtr returnedException = default;
                IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(m_get_normals, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                if (intPtr == IntPtr.Zero) return null;
                Vector3[] arr = new Vector3[IL2CPP.il2cpp_array_length(intPtr)];
                for (int i = 0; i < arr.Length; ++i)
                    arr[i] = ((Vector3*)(intPtr + 4 * IntPtr.Size))[i];
                return arr;
            }
            set
            {
                IL2CPP.Il2CppObjectBaseToPtrNotNull(this);
                IntPtr valueArrayPtr = IL2CPP.il2cpp_array_new(Il2CppClassPointerStore<Vector3>.NativeClassPtr, (ulong)value.Length);
                for (var i = 0; i < value.Length; i++)
                    ((Vector3*)(valueArrayPtr + 4 * IntPtr.Size))[i] = value[i];
                void** ptr = stackalloc void*[1];
                ptr[0] = (void*)valueArrayPtr;
                IntPtr returnedException = default;
                IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(m_set_normals, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), ptr, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
            }
        }

        public unsafe Vector4[] tangents
        {
            get
            {
                IL2CPP.Il2CppObjectBaseToPtrNotNull(this);
                IntPtr returnedException = default;
                IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(m_get_tangents, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                if (intPtr == IntPtr.Zero) return null;
                Vector4[] arr = new Vector4[IL2CPP.il2cpp_array_length(intPtr)];
                for (int i = 0; i < arr.Length; ++i)
                    arr[i] = ((Vector4*)(intPtr + 4 * IntPtr.Size))[i];
                return arr;
            }
            set
            {
                IL2CPP.Il2CppObjectBaseToPtrNotNull(this);
                IntPtr valueArrayPtr = IL2CPP.il2cpp_array_new(Il2CppClassPointerStore<Vector4>.NativeClassPtr, (ulong)value.Length);
                for (var i = 0; i < value.Length; i++)
                    ((Vector4*)(valueArrayPtr + 4 * IntPtr.Size))[i] = value[i];
                void** ptr = stackalloc void*[1];
                ptr[0] = (void*)valueArrayPtr;
                IntPtr returnedException = default;
                IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(m_set_tangents, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), ptr, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
            }
        }

        public unsafe Vector2[] uv
        {
            get
            {
                IL2CPP.Il2CppObjectBaseToPtrNotNull(this);
                IntPtr returnedException = default;
                IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(m_get_uv, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                if (intPtr == IntPtr.Zero) return null;
                Vector2[] arr = new Vector2[IL2CPP.il2cpp_array_length(intPtr)];
                for (int i = 0; i < arr.Length; ++i)
                    arr[i] = ((Vector2*)(intPtr + 4 * IntPtr.Size))[i];
                return arr;
            }
            set
            {
                IL2CPP.Il2CppObjectBaseToPtrNotNull(this);
                IntPtr valueArrayPtr = IL2CPP.il2cpp_array_new(Il2CppClassPointerStore<Vector2>.NativeClassPtr, (ulong)value.Length);
                for (var i = 0; i < value.Length; i++)
                    ((Vector2*)(valueArrayPtr + 4 * IntPtr.Size))[i] = value[i];
                void** ptr = stackalloc void*[1];
                ptr[0] = (void*)valueArrayPtr;
                IntPtr returnedException = default;
                IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(m_set_uv, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), ptr, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
            }
        }

        public unsafe Color[] colors
        {
            get
            {
                IL2CPP.Il2CppObjectBaseToPtrNotNull(this);
                IntPtr returnedException = default;
                IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(m_get_colors, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                if (intPtr == IntPtr.Zero) return null;
                Color[] arr = new Color[IL2CPP.il2cpp_array_length(intPtr)];
                for (int i = 0; i < arr.Length; ++i)
                    arr[i] = ((Color*)(intPtr + 4 * IntPtr.Size))[i];
                return arr;
            }
            set
            {
                IL2CPP.Il2CppObjectBaseToPtrNotNull(this);
                IntPtr valueArrayPtr = IL2CPP.il2cpp_array_new(Il2CppClassPointerStore<Color>.NativeClassPtr, (ulong)value.Length);
                for (var i = 0; i < value.Length; i++)
                    ((Color*)(valueArrayPtr + 4 * IntPtr.Size))[i] = value[i];
                void** ptr = stackalloc void*[1];
                ptr[0] = (void*)valueArrayPtr;
                IntPtr returnedException = default;
                IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(m_set_colors, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), ptr, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
            }
        }

        public unsafe Color32[] colors32
        {
            get
            {
                IL2CPP.Il2CppObjectBaseToPtrNotNull(this);
                IntPtr returnedException = default;
                IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(m_get_colors32, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                if (intPtr == IntPtr.Zero) return null;
                Color32[] arr = new Color32[IL2CPP.il2cpp_array_length(intPtr)];
                for (int i = 0; i < arr.Length; ++i)
                    arr[i] = ((Color32*)(intPtr + 4 * IntPtr.Size))[i];
                return arr;
            }
            set
            {
                IL2CPP.Il2CppObjectBaseToPtrNotNull(this);
                IntPtr valueArrayPtr = IL2CPP.il2cpp_array_new(Il2CppClassPointerStore<Color32>.NativeClassPtr, (ulong)value.Length);
                for (var i = 0; i < value.Length; i++)
                    ((Color32*)(valueArrayPtr + 4 * IntPtr.Size))[i] = value[i];
                void** ptr = stackalloc void*[1];
                ptr[0] = (void*)valueArrayPtr;
                IntPtr returnedException = default;
                IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(m_set_colors32, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), ptr, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
            }
        }

        public unsafe int[] triangles
        {
            get
            {
                IL2CPP.Il2CppObjectBaseToPtrNotNull(this);
                IntPtr returnedException = default;
                IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(m_get_triangles, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
                if (intPtr == IntPtr.Zero) return null;
                int[] arr = new int[IL2CPP.il2cpp_array_length(intPtr)];
                for (int i = 0; i < arr.Length; ++i)
                    arr[i] = ((int*)(intPtr + 4 * IntPtr.Size))[i];
                return arr;
            }
            set
            {
                IL2CPP.Il2CppObjectBaseToPtrNotNull(this);
                IntPtr valueArrayPtr = IL2CPP.il2cpp_array_new(Il2CppClassPointerStore<int>.NativeClassPtr, (ulong)value.Length);
                for (var i = 0; i < value.Length; i++)
                    ((int*)(valueArrayPtr + 4 * IntPtr.Size))[i] = value[i];
                void** ptr = stackalloc void*[1];
                ptr[0] = (void*)valueArrayPtr;
                IntPtr returnedException = default;
                IntPtr intPtr = IL2CPP.il2cpp_runtime_invoke(m_set_triangles, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), ptr, ref returnedException);
                Il2CppException.RaiseExceptionIfNecessary(returnedException);
            }
        }

        public unsafe void RecalculateBounds()
        {
            IL2CPP.Il2CppObjectBaseToPtrNotNull(this);
            IntPtr returnedException = default;
            IL2CPP.il2cpp_runtime_invoke(m_RecalculateBounds, IL2CPP.Il2CppObjectBaseToPtrNotNull(this), (void**)0, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
        }

    }
}
