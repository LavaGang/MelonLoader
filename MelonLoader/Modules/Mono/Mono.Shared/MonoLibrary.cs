using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if !NET6_0
using MelonLoader.NativeUtils;
#endif

namespace MelonLoader.Mono
{
    public class MonoLibrary
    {
        #region Private Members

        private static MonoLibrary _instance;

        #endregion

        #region Public Members

        public static MonoLibrary Instance
        {
            get
            {
                if (_instance == null)
                {
#if !NET6_0
                    _instance = MelonNativeLibrary.ReflectiveLoad<MonoLibrary>(GetLibPtr());
#endif
                }

                return _instance;
            }
            set
            {
                if (_instance != null)
                    return;
                _instance = value;
            }
        }

        #endregion

        #region Private Methods

#if !NET6_0
        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern IntPtr GetLibPtr();
#endif

        #endregion

        #region Exports

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr d_mono_jit_init_version(IntPtr name, IntPtr version);
        public d_mono_jit_init_version mono_jit_init_version;

        #endregion
    }
}
