using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NET_SDK.Reflection
{
    public class IL2CPP_Assembly : IL2CPP_Base
    {
        public readonly string Name;
        private readonly IL2CPP_Class[] ClassList;

        internal IL2CPP_Assembly(IntPtr ptr) : base(ptr)
        {
            // Setup Information
            Ptr = ptr;
            Name = Path.GetFileNameWithoutExtension(Marshal.PtrToStringAnsi(IL2CPP.il2cpp_image_get_name(Ptr)));

            // Map out Classes
            uint class_count = IL2CPP.il2cpp_image_get_class_count(Ptr);
            List<IL2CPP_Class> classList = new List<IL2CPP_Class>((int)class_count);
            for (uint i = 0; i < class_count; i++)
                if (Ptr != IntPtr.Zero)
                    classList.Add(new IL2CPP_Class(IL2CPP.il2cpp_image_get_class(Ptr, i)));
            ClassList = classList.ToArray();
        }

        public IL2CPP_Class[] GetClasses() => ClassList;
        public IL2CPP_Class[] GetClasses(IL2CPP_BindingFlags flags) => GetClasses().Where(x => x.HasFlag(flags)).ToArray();
        public IL2CPP_Class GetClass(string name) => GetClass(name, null);
        public IL2CPP_Class GetClass(string name, IL2CPP_BindingFlags flags) => GetClass(name, null, flags);
        public IL2CPP_Class GetClass(string name, string name_space)
        {
            IL2CPP_Class returnval = null;
            for (int i = 0; i < ClassList.Length; i++)
            {
                IL2CPP_Class type = ClassList[i];
                if (type.Name.Equals(name) && (string.IsNullOrEmpty(type.Namespace) || type.Namespace.Equals(name_space)))
                {
                    returnval = type;
                    break;
                }
                else
                {
                    var nestedTypes = type.GetNestedTypes();
                    for (int l = 0; l < nestedTypes.Length; l++)
                    {
                        var nestedType = nestedTypes[l];
                        if (nestedType.Name.Equals(name) && (string.IsNullOrEmpty(nestedType.Namespace) || nestedType.Namespace.Equals(name_space)))
                        {
                            returnval = nestedType;
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
            for (int i = 0; i < ClassList.Length; i++)
            {
                IL2CPP_Class type = ClassList[i];
                if (type.Name.Equals(name) && (string.IsNullOrEmpty(type.Namespace) || type.Namespace.Equals(name_space)) && type.HasFlag(flags))
                {
                    returnval = type;
                    break;
                }
                else
                {
                    var nestedTypes = type.GetNestedTypes();
                    for (int l = 0; l < nestedTypes.Length; l++)
                    {
                        var nestedType = nestedTypes[l];
                        if (nestedType.Name.Equals(name) && (string.IsNullOrEmpty(nestedType.Namespace) || nestedType.Namespace.Equals(name_space)) && nestedType.HasFlag(flags))
                        {
                            returnval = nestedType;
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
            for (int i = 0; i < ClassList.Length; i++)
            {
                IL2CPP_Class type = ClassList[i];
                if (type.Ptr == ptr)
                {
                    returnval = type;
                    break;
                }
                else
                {
                    var nestedTypes = type.GetNestedTypes();
                    for (int l = 0; l < nestedTypes.Length; l++)
                    {
                        var nestedType = nestedTypes[l];
                        if (nestedType.Ptr == ptr)
                        {
                            returnval = nestedType;
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
