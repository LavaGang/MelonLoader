using System;
using System.Runtime.InteropServices;

namespace NET_SDK.Reflection
{
    [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
    public class IL2CPP_Property : IL2CPP_Base
    {
        public readonly string Name;
        public readonly IL2CPP_BindingFlags Flags;
        private readonly IL2CPP_Method getMethod;
        private readonly IL2CPP_Method setMethod;
        internal IL2CPP_Property(IntPtr ptr) : base(ptr)
        {
            Ptr = ptr;
            Name = Marshal.PtrToStringAnsi(MelonLoader.Il2CppImports.il2cpp_property_get_name(Ptr));
            Flags = (IL2CPP_BindingFlags)MelonLoader.Il2CppImports.il2cpp_property_get_flags(Ptr);

            IntPtr getMethodPtr = MelonLoader.Il2CppImports.il2cpp_property_get_get_method(Ptr);
            if (getMethodPtr != IntPtr.Zero)
                getMethod = new IL2CPP_Method(getMethodPtr);

            IntPtr setMethodPtr = MelonLoader.Il2CppImports.il2cpp_property_get_set_method(Ptr);
            if (setMethodPtr != IntPtr.Zero)
                setMethod = new IL2CPP_Method(setMethodPtr);
        }
        [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
        public IL2CPP_BindingFlags GetFlags() => Flags;
        [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
        public bool HasFlag(IL2CPP_BindingFlags flag) => ((GetFlags() & flag) != 0);
        [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
        public IL2CPP_Method GetGetMethod() => getMethod;
        [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
        public IL2CPP_Method GetSetMethod() => setMethod;
    }
}