using System;
using UnhollowerMini;

namespace Il2CppMono.Unity
{
    internal class UnityTls
    {
        internal static IntPtr m_GetUnityTlsInterface;

        static UnityTls()
        {
            InternalClassPointerStore<UnityTls>.NativeClassPtr = UnityInternals.GetClass("System.dll", "Mono.Unity", "UnityTls");
            m_GetUnityTlsInterface = UnityInternals.GetMethod(InternalClassPointerStore<UnityTls>.NativeClassPtr, "GetUnityTlsInterface", "System.IntPtr");
        }

        public unsafe static IntPtr GetUnityTlsInterface()
        {
            if (m_GetUnityTlsInterface == null)
                return IntPtr.Zero;

            IntPtr* param = null;
            IntPtr returnedException = IntPtr.Zero;
            IntPtr intPtr = UnityInternals.il2cpp_runtime_invoke(m_GetUnityTlsInterface, IntPtr.Zero, (void**)param, ref returnedException);
            Il2CppException.RaiseExceptionIfNecessary(returnedException);
            return *(IntPtr*)UnityInternals.il2cpp_object_unbox(intPtr);
        }
    }
}
