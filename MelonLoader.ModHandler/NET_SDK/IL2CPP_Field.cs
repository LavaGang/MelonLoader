using System;
using System.Runtime.InteropServices;

namespace NET_SDK.Reflection
{
    public class IL2CPP_Field : IL2CPP_Base
    {
        public readonly string Name;
        public readonly IL2CPP_BindingFlags Flags;
        public readonly IL2CPP_Type ReturnType;

        internal IL2CPP_Field(IntPtr ptr) : base(ptr)
        {
            Ptr = ptr;
            Name = Marshal.PtrToStringAnsi(MelonLoader.Il2CppImports.il2cpp_field_get_name(Ptr));
            Flags = (IL2CPP_BindingFlags)MelonLoader.Il2CppImports.il2cpp_field_get_flags(Ptr);
            ReturnType = new IL2CPP_Type(MelonLoader.Il2CppImports.il2cpp_field_get_type(Ptr));
        }

        public IL2CPP_BindingFlags GetFlags() => Flags;
        public bool HasFlag(IL2CPP_BindingFlags flag) => ((GetFlags() & flag) != 0);

        public IL2CPP_Type GetReturnType() => ReturnType;

        unsafe public IL2CPP_Object GetValue() => GetValue(IntPtr.Zero);
        unsafe public IL2CPP_Object GetValue(IntPtr obj)
        {
            IntPtr returnval;
            if (HasFlag(IL2CPP_BindingFlags.FIELD_STATIC))
                returnval = MelonLoader.Il2CppImports.il2cpp_field_get_value_object(Ptr, IntPtr.Zero);
            else
                returnval = MelonLoader.Il2CppImports.il2cpp_field_get_value_object(Ptr, obj);
            if (returnval != IntPtr.Zero)
                return new IL2CPP_Object(returnval, GetReturnType());
            return null;
        }

        unsafe public void SetValue(IntPtr value) => SetValue(IntPtr.Zero, value);
        unsafe public void SetValue(IntPtr obj, IntPtr value)
        {
            if (HasFlag(IL2CPP_BindingFlags.FIELD_STATIC))
                MelonLoader.Il2CppImports.il2cpp_field_static_set_value(Ptr, value);
            else
                MelonLoader.Il2CppImports.il2cpp_field_set_value(obj, Ptr, value);
        }
    }
}