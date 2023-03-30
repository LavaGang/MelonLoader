using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MelonLoader.NativeUtils
{
    public class NativeHook<T> where T : Delegate
    {
        #region Private Values
        private IntPtr _targetHandle;
        private IntPtr _detourHandle;
        private IntPtr _trampolineHandle;
        private T _trampoline;
        #endregion

        #region Public Properties
        public IntPtr Target 
        {
            get
            {
                return _targetHandle;
            }

            set
            {
                if (value == IntPtr.Zero)
                    throw new ArgumentNullException("value");

                _targetHandle = value;
            }
        }

        public IntPtr Detour
        {
            get
            {
                return _detourHandle;
            }

            set
            {
                if (value == IntPtr.Zero)
                    throw new ArgumentNullException("value");

                _detourHandle = value;
            }
        }

        public T Trampoline
        {
            get
            {
                if (_trampoline == null)
                    _trampoline = (T)Marshal.GetDelegateForFunctionPointer(_trampolineHandle, typeof(T));

                return _trampoline;
            }
        }

        public bool IsHooked { get; private set; }
        #endregion

        public NativeHook() { }

        public NativeHook(IntPtr target, IntPtr detour) 
        {
            if (target == IntPtr.Zero)
                throw new ArgumentNullException("target");

            if (detour == IntPtr.Zero)
                throw new ArgumentNullException("detour"); 

            _targetHandle = target;
            _detourHandle = detour;
        }

        public unsafe void Attach()
        {
            if (IsHooked)
                return;

            if (_targetHandle == IntPtr.Zero)
                throw new NullReferenceException("The NativeHook's target has not been set!");

            if (_detourHandle == IntPtr.Zero)
                throw new NullReferenceException("The NativeHook's detour has not been set!");

            IntPtr trampoline = _targetHandle;
            BootstrapInterop.NativeHookAttach((IntPtr)(&trampoline), _detourHandle);
            _trampolineHandle = trampoline;

            IsHooked = true;
        }

        public unsafe void Detach()
        {
            if (!IsHooked) 
                return;

            if (_targetHandle == IntPtr.Zero)
                throw new NullReferenceException("The NativeHook's target has not been set!");

            IntPtr original = _targetHandle;
            BootstrapInterop.NativeHookDetach((IntPtr)(&original), _detourHandle);

            IsHooked= false;
            _trampolineHandle = IntPtr.Zero;
        }
    }
}
