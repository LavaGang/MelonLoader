using System;
using UnhollowerMini;

namespace MelonUnityEngine
{
    internal class UnityObject : InternalObjectBase
    {
        private delegate HideFlags get_hideFlags_Delegate(IntPtr obj);
        private static get_hideFlags_Delegate m_get_hideFlags;
        private delegate void set_hideFlags_Delegate(IntPtr obj, HideFlags hideFlags);
        private static set_hideFlags_Delegate m_set_hideFlags;

        private static IntPtr m_DestroyImmediate;
        private static IntPtr m_DontDestroyOnLoad;
        unsafe static UnityObject()
        {
            InternalClassPointerStore<UnityObject>.NativeClassPtr = UnityInternals.GetClass("UnityEngine.CoreModule.dll", "UnityEngine", "Object");
            //UnityInternals.runtime_class_init(InternalClassPointerStore<UnityObject>.NativeClassPtr);

            m_DestroyImmediate = UnityInternals.GetMethod(InternalClassPointerStore<UnityObject>.NativeClassPtr, "DestroyImmediate", "System.Void", "UnityEngine.Object");
            m_DontDestroyOnLoad = UnityInternals.GetMethod(InternalClassPointerStore<UnityObject>.NativeClassPtr, "DontDestroyOnLoad", "System.Void", "UnityEngine.Object");

            m_get_hideFlags = UnityInternals.ResolveICall<get_hideFlags_Delegate>("UnityEngine.Object::get_hideFlags(UnityEngine.Object)");
            m_set_hideFlags = UnityInternals.ResolveICall<set_hideFlags_Delegate>("UnityEngine.Object::set_hideFlags(UnityEngine.Object)");
        }

        public UnityObject(IntPtr ptr) : base(ptr) { }

        unsafe public HideFlags hideFlags
        {
            get
            {
                if (Pointer == IntPtr.Zero)
                    return HideFlags.None;
                return m_get_hideFlags(Pointer);
            }
            set
            {
                if (Pointer == IntPtr.Zero)
                    return;
                m_set_hideFlags(Pointer, value);
            }
        }

        unsafe public void DestroyImmediate()
        {
            if (Pointer == IntPtr.Zero)
                return;

            void** args = stackalloc void*[1];
            args[0] = Pointer.ToPointer();

            IntPtr returnedException = IntPtr.Zero;
            UnityInternals.runtime_invoke(m_DestroyImmediate, IntPtr.Zero, args, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
        }

        unsafe public void DontDestroyOnLoad()
        {
            if (Pointer == IntPtr.Zero)
                return;

            void** args = stackalloc void*[1];
            args[0] = Pointer.ToPointer();

            IntPtr returnedException = IntPtr.Zero;
            UnityInternals.runtime_invoke(m_DontDestroyOnLoad, IntPtr.Zero, args, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
        }
    }
}
