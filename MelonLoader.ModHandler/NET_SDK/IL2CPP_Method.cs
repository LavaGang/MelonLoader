using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace NET_SDK.Reflection
{
    public class IL2CPP_Method : IL2CPP_Base
    {
        public readonly string Name;
        private readonly IL2CPP_BindingFlags Flags;
        private readonly IL2CPP_Type ReturnType;
        private readonly IL2CPP_Method_Parameter[] Parameters;
        internal IL2CPP_Method(IntPtr ptr) : base(ptr)
        {
            Ptr = ptr;
            Name = Marshal.PtrToStringAnsi(IL2CPP.il2cpp_method_get_name(Ptr));
            ReturnType = new IL2CPP_Type(IL2CPP.il2cpp_method_get_return_type(Ptr));
            uint flags = 0;
            Flags = (IL2CPP_BindingFlags)IL2CPP.il2cpp_method_get_flags(Ptr, ref flags);
            uint param_count = IL2CPP.il2cpp_method_get_param_count(Ptr);
            Parameters = new IL2CPP_Method_Parameter[param_count];
            for (uint i = 0; i < param_count; i++)
                Parameters[i] = new IL2CPP_Method_Parameter(IL2CPP.il2cpp_method_get_param(Ptr, i), Marshal.PtrToStringAnsi(IL2CPP.il2cpp_method_get_param_name(Ptr, i)));
        }

        public IL2CPP_BindingFlags GetFlags() => Flags;
        public bool HasFlag(IL2CPP_BindingFlags flag) => ((GetFlags() & flag) != 0);
        public IL2CPP_Type GetReturnType() => ReturnType;
        public IL2CPP_Method_Parameter[] GetParameters() => Parameters;
        public int GetParameterCount() => Parameters.Length;

        public IL2CPP_Object Invoke() => Invoke(IntPtr.Zero, new IntPtr[] { IntPtr.Zero });
        public IL2CPP_Object Invoke(IntPtr obj) => Invoke(obj, new IntPtr[] { IntPtr.Zero });
        public IL2CPP_Object Invoke(params IntPtr[] paramtbl) => Invoke(IntPtr.Zero, paramtbl);
        /// <summary>
        /// Invokes the method with the provided 'this' reference and parameters.
        /// Parameters can be an array of value types, <see cref="string"/>, and <see cref="IL2CPP_Object"/>
        /// <para>An <see cref="InvalidCastException"/> is thrown if any of the parameters are not valid types</para>
        /// <para>An <see cref="InvalidOperationException"/> is thrown if the Invoke fails</para>
        /// </summary>
        /// <param name="obj">The 'this' reference to call the method on.
        /// If this <see cref="IL2CPP_Method"/> is of a static method, provide a null for this parameter</param>
        /// <param name="paramtbl">Parameters to supply to this method. Includes the generic parameters (if there is any)</param>
        /// <returns>The resultant object from the Invoke</returns>
        /// <exception cref="InvalidCastException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public IL2CPP_Object Invoke(IL2CPP_Object obj, params object[] paramtbl) => Invoke(obj, IL2CPP.ObjectArrayToIntPtrArray(paramtbl));
        public IL2CPP_Object Invoke(IL2CPP_Object obj) => Invoke(obj.Ptr, new IntPtr[] { IntPtr.Zero });
        public IL2CPP_Object Invoke(params IL2CPP_Object[] paramtbl) => Invoke(IntPtr.Zero, IL2CPP.IL2CPPObjectArrayToIntPtrArray(paramtbl));
        public IL2CPP_Object Invoke(IntPtr obj, params IL2CPP_Object[] paramtbl) => Invoke(obj, IL2CPP.IL2CPPObjectArrayToIntPtrArray(paramtbl));
        public IL2CPP_Object Invoke(IL2CPP_Object obj, params IntPtr[] paramtbl) => Invoke(obj.Ptr, paramtbl);
        /// <summary>
        /// Invokes the method with the provided 'this' reference and parameters.
        /// <para>An <see cref="InvalidOperationException"/> is thrown if the Invoke fails</para>
        /// </summary>
        /// <param name="obj">The 'this' reference to call the method on.
        /// If this <see cref="IL2CPP_Method"/> is of a static method, provide a null for this parameter</param>
        /// <param name="paramtbl">Parameters to supply to this method. Includes the generic parameters (if there are any)</param>
        /// <returns>The resultant object from the Invoke</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public IL2CPP_Object Invoke(IntPtr obj, params IntPtr[] paramtbl)
        {
            IntPtr returnval = IL2CPP.InvokeMethod(Ptr, obj, paramtbl);
            if (returnval == IntPtr.Zero)
                return null;
            return new IL2CPP_Object(returnval, GetReturnType());
        }
    }

    public class IL2CPP_Method_Parameter : IL2CPP_Base
    {
        public string Name { get; private set; }
        internal IL2CPP_Method_Parameter(IntPtr ptr, string name) : base(ptr)
        {
            Ptr = ptr;
            Name = name;
        }
    }
}