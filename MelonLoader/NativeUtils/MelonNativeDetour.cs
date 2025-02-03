using System;

namespace MelonLoader.NativeUtils
{
    public class MelonNativeDetour<T> where T : Delegate
    {
        #region Private Members

        private IntPtr _targetHandle;
        private IntPtr _detourHandle;
        private MelonNativeHook<T> _hook;

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

        public T Trampoline { get => _hook.Trampoline; }
        public IntPtr TrampolineHandle { get => _hook.TrampolineHandle; }

        #endregion

        #region Constructors

        public MelonNativeDetour(T target, T detour) : this(target.GetFunctionPointer(), detour.GetFunctionPointer(), true) { }
        public MelonNativeDetour(T target, T detour, bool autoAttach) : this(target.GetFunctionPointer(), detour.GetFunctionPointer(), autoAttach) { }

        public MelonNativeDetour(IntPtr target, T detour) : this(target, detour.GetFunctionPointer(), true) { }
        public MelonNativeDetour(IntPtr target, T detour, bool autoAttach) : this(target, detour.GetFunctionPointer(), autoAttach) { }

        public MelonNativeDetour(IntPtr target, IntPtr detour) : this(target, detour, true) { }
        public MelonNativeDetour(IntPtr target, IntPtr detour, bool autoAttach)
        {
            if (target == IntPtr.Zero)
                throw new ArgumentNullException(nameof(target));

            if (detour == IntPtr.Zero)
                throw new ArgumentNullException(nameof(detour));

            _targetHandle = target;
            _detourHandle = detour;

            _hook = new MelonNativeHook<T>(_targetHandle, _detourHandle);

            if (autoAttach)
                Attach();
        }

        #endregion

        #region Public Methods

        public void Attach()
        {
            if (IsAttached)
                return;

            if (_hook == null)
                throw new NullReferenceException("The Native Detour's Hook has not been set!");
            _hook.Attach();

            IsAttached = true;
        }

        public void Detach()
        {
            if (!IsAttached)
                return;

            if (_hook == null)
                throw new NullReferenceException("The Native Detour's Hook has not been set!");
            _hook.Detach();

            IsAttached = false;
        }

        #endregion
    }
}
