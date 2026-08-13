using Il2CppInterop.Common.Attributes;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using System;

namespace UnityEngine;

[InjectedType]
internal partial struct BlittableArrayWrapper
{
    enum UpdateFlags
    {
        NoUpdateNeeded,
        SizeChanged,
        DataIsNativePointer,
        DataIsNativeOwnedMemory,
        DataIsEmpty,
        DataIsNull
    }

    Pointer<Il2CppSystem.Void> data;
    Il2CppSystem.Int32 size;
    Il2CppSystem.Int32 updateFlags;

    public readonly unsafe void Unmarshal<T>(ref T[] array) where T : struct
    {
        switch ((UpdateFlags)(int)updateFlags)
        {
            case UpdateFlags.SizeChanged:
            case UpdateFlags.DataIsNativePointer:
                array = new Span<T>((void*)data, size).ToArray();
                break;
            case UpdateFlags.DataIsNativeOwnedMemory:
                array = new Span<T>(BindingsAllocator.GetNativeOwnedDataPointer((void*)data), size).ToArray();
                BindingsAllocator.FreeNativeOwnedMemory((void*)data);
                break;
            case UpdateFlags.DataIsEmpty:
                array = [];
                break;
            case UpdateFlags.DataIsNull:
                array = null;
                break;
        }
    }

    private static class BindingsAllocator
    {
        public struct NativeOwnedMemory
        {
            public unsafe void* data;
        }

        static BindingsAllocator()
        {
            FreeNativeOwnedMemoryDelegateField = RuntimeInvoke.ResolveICall<FreeNativeOwnedMemoryDelegate>("UnityEngine.Bindings.BindingsAllocator::FreeNativeOwnedMemory");
        }

        public unsafe static void FreeNativeOwnedMemory(void* ptr) => FreeNativeOwnedMemoryDelegateField((Pointer<Il2CppSystem.Void>)ptr);

        public unsafe static void* GetNativeOwnedDataPointer(void* ptr)
        {
            return ((NativeOwnedMemory*)ptr)->data;
        }

        delegate void FreeNativeOwnedMemoryDelegate(Pointer<Il2CppSystem.Void> ptr);
        static FreeNativeOwnedMemoryDelegate FreeNativeOwnedMemoryDelegateField;
    }
}
