using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NET_SDK.Reflection
{
    public class IL2CPP_Assembly : IL2CPP_Base
    {
        public string Name;
        private List<IL2CPP_Class> ClassList = new List<IL2CPP_Class>();

        internal IL2CPP_Assembly(IntPtr ptr) : base(ptr)
        {
            // Setup Information
            Ptr = ptr;
            Name = Path.GetFileNameWithoutExtension(Marshal.PtrToStringAnsi(IL2CPP.il2cpp_image_get_name(Ptr)));

            // Map out Classes
            uint class_count = IL2CPP.il2cpp_image_get_class_count(Ptr);
                for (uint i = 0; i < class_count; i++)
                    if (Ptr != IntPtr.Zero)
                        ClassList.Add(new IL2CPP_Class(IL2CPP.il2cpp_image_get_class(Ptr, i)));
        }

        public IL2CPP_Class[] GetClasses() => ClassList.ToArray();
        public IL2CPP_Class[] GetClasses(IL2CPP_BindingFlags flags) => GetClasses().Where(x => x.HasFlag(flags)).ToArray();
        public IL2CPP_Class GetClass(string name) => GetClass(name, null);
        public IL2CPP_Class GetClass(string name, IL2CPP_BindingFlags flags) => GetClass(name, null, flags);
        public IL2CPP_Class GetClass(string name, string name_space)
        {
            IL2CPP_Class returnval = null;
            foreach (IL2CPP_Class type in GetClasses())
            {
                if (type.Name.Equals(name) && (string.IsNullOrEmpty(type.Namespace) || type.Namespace.Equals(name_space)))
                {
                    returnval = type;
                    break;
                }
                else
                {
                    foreach (IL2CPP_Class nestedtype in type.GetNestedTypes())
                    {
                        if (nestedtype.Name.Equals(name) && (string.IsNullOrEmpty(nestedtype.Namespace) || nestedtype.Namespace.Equals(name_space)))
                        {
                            returnval = nestedtype;
                            break;
                        }
                    }
                    if (returnval != null)
                        break;
                }
            }
            return returnval;
        }
        public IL2CPP_Class GetClass(string name, string name_space, IL2CPP_BindingFlags flags)
        {
            IL2CPP_Class returnval = null;
            foreach (IL2CPP_Class type in GetClasses())
            {
                if (type.Name.Equals(name) && (string.IsNullOrEmpty(type.Namespace) || type.Namespace.Equals(name_space)) && type.HasFlag(flags))
                {
                    returnval = type;
                    break;
                }
                else
                {
                    foreach (IL2CPP_Class nestedtype in type.GetNestedTypes())
                    {
                        if (nestedtype.Name.Equals(name) && (string.IsNullOrEmpty(nestedtype.Namespace) || nestedtype.Namespace.Equals(name_space)) && nestedtype.HasFlag(flags))
                        {
                            returnval = nestedtype;
                            break;
                        }
                    }
                    if (returnval != null)
                        break;
                }
            }
            return returnval;
        }
        public IL2CPP_Class GetClass(IntPtr ptr)
        {
            IL2CPP_Class returnval = null;
            foreach (IL2CPP_Class type in GetClasses())
            {
                if (type.Ptr == ptr)
                {
                    returnval = type;
                    break;
                }
                else
                {
                    foreach (IL2CPP_Class nestedtype in type.GetNestedTypes())
                    {
                        if (nestedtype.Ptr == ptr)
                        {
                            returnval = nestedtype;
                            break;
                        }
                    }
                    if (returnval != null)
                        break;
                }
            }
            return returnval;
        }
    }
}
