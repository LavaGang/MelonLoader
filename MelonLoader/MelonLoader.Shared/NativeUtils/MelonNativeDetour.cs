using MelonLoader.Bootstrap;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MelonLoader.NativeUtils
{
    public class MelonNativeDetour<T> where T : Delegate
    {
        #region Private Members

        private static readonly List<Delegate> _gcProtect = new();

        private IntPtr _targetHandle;
        private IntPtr _detourHandle;
        private IntPtr _trampolineHandle;
        private T _trampoline;

        #endregion

        #region Public Members

        public bool IsAttached { get; private set; }

        public IntPtr Target
        {
            get => _targetHandle;
            private set
            {
                if (value == IntPtr.Zero)
                    throw new ArgumentNullException(nameof(value));

                _targetHandle = value;
            }
        }

        public IntPtr Detour
        {
            get => _detourHandle;
            private set
            {
                if (value == IntPtr.Zero)
                    throw new ArgumentNullException(nameof(value));

                _detourHandle = value;
            }
        }

        public T Trampoline
        {
            get => _trampoline;
            private set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                if (_trampoline != null)
                    _gcProtect.Remove(_trampoline);

                _trampoline = value;
                _gcProtect.Add(_trampoline);
            }
        }

        public IntPtr TrampolineHandle
        {
            get => _trampolineHandle;
            private set
            {
                if (value == IntPtr.Zero)
                    throw new ArgumentNullException(nameof(value));

                _trampolineHandle = value;
            }
        }

        #endregion

        #region Constructors

        public MelonNativeDetour(IntPtr target, T detour) : this(target, detour, true) { }
        public MelonNativeDetour(IntPtr target, T detour, bool autoAttach)
        {
            if (target == IntPtr.Zero)
                throw new ArgumentNullException(nameof(target));

            if (detour == null)
                throw new ArgumentNullException(nameof(detour));

            _targetHandle = target;
            _detourHandle = Marshal.GetFunctionPointerForDelegate(detour);

            if (autoAttach)
                Attach();
        }

        #endregion

        #region Public Methods

        public void Attach()
        {
            if (IsAttached)
                return;

            if (_targetHandle == IntPtr.Zero)
                throw new NullReferenceException("The Native Detour's target has not been set!");

            if (_detourHandle == IntPtr.Zero)
                throw new NullReferenceException("The Native Detour's detour has not been set!");

            _trampolineHandle = BootstrapInterop.NativeHookAttach(_targetHandle, _detourHandle);

            _trampoline = (T)Marshal.GetDelegateForFunctionPointer(_trampolineHandle, typeof(T));
            _gcProtect.Add(_trampoline);

            IsAttached = true;
        }

        public void Detach()
        {
            if (!IsAttached)
                return;

            if (_targetHandle == IntPtr.Zero)
                throw new NullReferenceException("The Native Detour's target has not been set!");

            BootstrapInterop.NativeHookDetach(_targetHandle);

            IsAttached = false;

            _gcProtect.Remove(_trampoline);
            _trampoline = null;
            _trampolineHandle = IntPtr.Zero;
        }

        #endregion
    }
}
