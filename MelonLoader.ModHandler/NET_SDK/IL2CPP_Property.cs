using System;
using System.Runtime.InteropServices;

namespace NET_SDK.Reflection
{
    public class IL2CPP_Property : IL2CPP_Base
    {
        public string Name;
        private IL2CPP_BindingFlags Flags;
        private IL2CPP_Method getMethod;
        private IL2CPP_Method setMethod;

        internal IL2CPP_Property(IntPtr ptr) : base(ptr)
        {
            Ptr = ptr;
            Name = Marshal.PtrToStringAnsi(IL2CPP.il2cpp_property_get_name(Ptr));
            Flags = (IL2CPP_BindingFlags)IL2CPP.il2cpp_property_get_flags(Ptr);

            IntPtr getMethodPtr = IL2CPP.il2cpp_property_get_get_method(Ptr);
            if (getMethodPtr != IntPtr.Zero)
                getMethod = new IL2CPP_Method(getMethodPtr);

            IntPtr setMethodPtr = IL2CPP.il2cpp_property_get_set_method(Ptr);
            if (setMethodPtr != IntPtr.Zero)
                setMethod = new IL2CPP_Method(setMethodPtr);
        }

        public IL2CPP_BindingFlags GetFlags() => Flags;
        public bool HasFlag(IL2CPP_BindingFlags flag) => ((GetFlags() & flag) != 0);

        public IL2CPP_Method GetGetMethod() => getMethod;
        public IL2CPP_Method GetSetMethod() => setMethod;
    }
}