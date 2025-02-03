using Il2CppInterop.Runtime.Injection;
using MelonLoader.CoreClrUtils;
using MelonLoader.InternalUtils;
using System;
using System.Runtime.InteropServices;

namespace MelonLoader.Engine.Unity.Il2Cpp
{
    internal sealed class Il2CppDetourProvider : IDetourProvider
    {
        public IDetour Create<TDelegate>(nint original, TDelegate target) where TDelegate : Delegate
        {
            return new MelonDetour(original, target);
        }

        private sealed class MelonDetour : IDetour
        {
            private nint _detourFrom;
            private nint _originalPtr;

            private Delegate _target;
            private IntPtr _targetPtr;

            /// <summary>
            /// Original method
            /// </summary>
            public nint Target => _detourFrom;

            public nint Detour => _targetPtr;
            public nint OriginalTrampoline => _originalPtr;

            public MelonDetour(nint detourFrom, Delegate target)
            {
                _detourFrom = detourFrom;
                _target = target;

                // We have to apply immediately because we're gonna be asked for a trampoline right away
                Apply();
            }

            public unsafe void Apply()
            {
                if (_targetPtr != IntPtr.Zero)
                    return;

                _targetPtr = Marshal.GetFunctionPointerForDelegate(_target);

                var addr = _detourFrom;
                nint addrPtr = (nint)(&addr);
                BootstrapInterop.NativeHookAttachDirect(addrPtr, _targetPtr);
                NativeStackWalk.RegisterHookAddr((ulong)addrPtr, $"Il2CppInterop detour of 0x{addrPtr:X} -> 0x{_targetPtr:X}");

                _originalPtr = addr;
            }

            public unsafe void Dispose()
            {
                if (_targetPtr == IntPtr.Zero)
                    return;

                var addr = _detourFrom;
                nint addrPtr = (nint)(&addr);

                BootstrapInterop.NativeHookDetach(addrPtr, _targetPtr);
                NativeStackWalk.UnregisterHookAddr((ulong)addrPtr);

                _targetPtr = IntPtr.Zero;
                _originalPtr = IntPtr.Zero;
            }

            public T GenerateTrampoline<T>()
                where T : Delegate
            {
                if (_originalPtr == IntPtr.Zero)
                    return null;
                return Marshal.GetDelegateForFunctionPointer<T>(_originalPtr);
            }
        }
    }
}
