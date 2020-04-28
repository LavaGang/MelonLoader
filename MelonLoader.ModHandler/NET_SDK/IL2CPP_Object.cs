using System;

namespace NET_SDK.Reflection
{
    [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
    public class IL2CPP_Object : IL2CPP_Base
    {
        private readonly IL2CPP_Type ReturnType;
        internal IL2CPP_Object(IntPtr ptr, IL2CPP_Type returntype) : base(ptr)
        {
            Ptr = ptr;
            ReturnType = returntype;
        }

        [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
        public IL2CPP_Type GetReturnType() => ReturnType;

        [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
        public IntPtr UnboxIntPtr() => MelonLoader.Il2Cpp.il2cpp_object_unbox(Ptr);
        [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
        public unsafe void* Unbox() => UnboxIntPtr().ToPointer();
        [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
        public unsafe T UnboxValue<T>() where T : unmanaged
            => *(T*)Unbox();
        [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
        unsafe public string UnboxString() => new string((char*)Ptr.ToPointer() + 10);
    }
}