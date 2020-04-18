using System;

namespace NET_SDK.Reflection
{
    public class IL2CPP_Object : IL2CPP_Base
    {
        private readonly IL2CPP_Type ReturnType;
        internal IL2CPP_Object(IntPtr ptr, IL2CPP_Type returntype) : base(ptr)
        {
            Ptr = ptr;
            ReturnType = returntype;
        }

        public IL2CPP_Type GetReturnType() => ReturnType;

        public IntPtr UnboxIntPtr() => IL2CPP.il2cpp_object_unbox(Ptr);
        public unsafe void* Unbox() => UnboxIntPtr().ToPointer();
        public unsafe T UnboxValue<T>() where T : unmanaged
            => *(T*)Unbox();
        public string UnboxString() => IL2CPP.IntPtrToString(Ptr);
    }
}