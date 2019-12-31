using System;

namespace NET_SDK.Reflection
{
    public class IL2CPP_Type : IL2CPP_Base
    {
        public string Name;

        internal IL2CPP_Type(IntPtr ptr) : base(ptr)
        {
            Ptr = ptr;
            Name = IL2CPP.il2cpp_type_get_name(Ptr);
        }
    }
}
