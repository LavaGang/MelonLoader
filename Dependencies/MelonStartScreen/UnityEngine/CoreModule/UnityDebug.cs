using MelonLoader;
using System;
using System.Runtime.InteropServices;
using UnhollowerMini;

namespace MelonUnityEngine
{
    internal static class UnityDebug
    {
        private delegate bool get_isDebugBuild_Delegate();
        private static get_isDebugBuild_Delegate get_isDebugBuild_Ptr;

        static UnityDebug()
        {
            IntPtr ptr = UnityInternals.ResolveICall("UnityEngine.Debug::get_isDebugBuild");
            if (ptr != IntPtr.Zero)
                get_isDebugBuild_Ptr = (get_isDebugBuild_Delegate)Marshal.GetDelegateForFunctionPointer(ptr, typeof(get_isDebugBuild_Delegate));
            else
                MelonLogger.Error("Failed to resolve icall UnityEngine.Debug::get_isDebugBuild");
        }

        internal static bool isDebugBuild { get => get_isDebugBuild_Ptr(); }
    }
}
