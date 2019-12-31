using System;
using System.Runtime.InteropServices;

namespace NET_SDK.Reflection
{
    public class IL2CPP_Field : IL2CPP_Base
    {
        public string Name;
        private IL2CPP_BindingFlags Flags;
        private IL2CPP_Type ReturnType;

        internal IL2CPP_Field(IntPtr ptr) : base(ptr)
        {
            Ptr = ptr;
            Name = Marshal.PtrToStringAnsi(IL2CPP.il2cpp_field_get_name(Ptr));
            uint flags = 0;
            Flags = (IL2CPP_BindingFlags)IL2CPP.il2cpp_field_get_flags(Ptr, ref flags);
            ReturnType = new IL2CPP_Type(IL2CPP.il2cpp_field_get_type(Ptr));
        }

        public IL2CPP_BindingFlags GetFlags() => Flags;
        public bool HasFlag(IL2CPP_BindingFlags flag) => ((GetFlags() & flag) != 0);

        public IL2CPP_Type GetReturnType() => ReturnType;

        unsafe public IL2CPP_Object GetValue() => GetValue(IntPtr.Zero);
        unsafe public IL2CPP_Object GetValue(IntPtr obj)
        {
            IntPtr returnval = IntPtr.Zero;
            if (HasFlag(IL2CPP_BindingFlags.FIELD_STATIC))
                returnval = IL2CPP.il2cpp_field_get_value_object(Ptr, IntPtr.Zero);
                // IL2CPP.il2cpp_field_static_get_value(Ptr, ref returnval);
            else
                // IL2CPP.il2cpp_field_get_value(obj, Ptr, ref returnval);
                returnval = IL2CPP.il2cpp_field_get_value_object(Ptr, obj);
            if (returnval != IntPtr.Zero)
                return new IL2CPP_Object(returnval, GetReturnType());
            return null;
        }

        unsafe public void SetValue(IntPtr value) => SetValue(IntPtr.Zero, value);
        unsafe public void SetValue(IntPtr obj, IntPtr value)
        {
            if (HasFlag(IL2CPP_BindingFlags.FIELD_STATIC))
                IL2CPP.il2cpp_field_static_set_value(Ptr, value);
            else
                IL2CPP.il2cpp_field_set_value_object(obj, Ptr, value);
        }
    }
}
