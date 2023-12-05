using System;
using System.Runtime.InteropServices;
using MelonLoader.Bootstrap;

namespace MelonLoader.NativeUtils
{
    public class MelonNativeHook<T> where T : Delegate
    {
        #region Private Members

        private IntPtr _targetHandle;
        private IntPtr _detourHandle;
        private IntPtr _trampolineHandle;
        private T _trampoline;

        #endregion

        #region Public Members

        public IntPtr Target
        {
            get => _targetHandle;
            set
            {
                if (value == IntPtr.Zero)
                    throw new ArgumentNullException("value");

                _targetHandle = value;
            }
        }

        public IntPtr Detour
        {
            get => _detourHandle;
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

        public MelonNativeHook(IntPtr target, IntPtr detour)
        {
            if (target == IntPtr.Zero)
                throw new ArgumentNullException("target");

            if (detour == IntPtr.Zero)
                throw new ArgumentNullException("detour");

            _targetHandle = target;
            _detourHandle = detour;
        }

        public MelonNativeHook()
        {
            _targetHandle = IntPtr.Zero;
            _detourHandle = IntPtr.Zero;
        }

        public unsafe void Attach()
        {
            if (IsHooked)
                return;

            if (_targetHandle == IntPtr.Zero)
                throw new NullReferenceException("The NativeHook's target has not been set!");

            if (_detourHandle == IntPtr.Zero)
                throw new NullReferenceException("The NativeHook's detour has not been set!");
            
            
            _trampolineHandle = BootstrapInterop.NativeHookAttach(_targetHandle, _detourHandle);
            
            IsHooked = true;
        }

        public unsafe void Detach()
        {
            if (!IsHooked)
                return;

            if (_targetHandle == IntPtr.Zero)
                throw new NullReferenceException("The NativeHook's target has not been set!");

            BootstrapInterop.NativeHookDetach(_targetHandle);
            
            IsHooked = false;
            _trampolineHandle = IntPtr.Zero;
        }
    }
}