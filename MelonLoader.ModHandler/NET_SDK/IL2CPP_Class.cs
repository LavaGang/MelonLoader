using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

namespace NET_SDK.Reflection
{
    public class IL2CPP_Class : IL2CPP_Base
    {
        public string Name;
        public string Namespace;
        private IL2CPP_BindingFlags Flags;
        private List<IL2CPP_Method> MethodList = new List<IL2CPP_Method>();
        private List<IL2CPP_Field> FieldList = new List<IL2CPP_Field>();
        private List<IL2CPP_Event> EventList = new List<IL2CPP_Event>();
        private List<IL2CPP_Class> NestedTypeList = new List<IL2CPP_Class>();
        private List<IL2CPP_Property> PropertyList = new List<IL2CPP_Property>();

        internal IL2CPP_Class(IntPtr ptr) : base(ptr)
        {
            // Setup Information
            Ptr = ptr;
            Name = Marshal.PtrToStringAnsi(IL2CPP.il2cpp_class_get_name(Ptr));
            Namespace = Marshal.PtrToStringAnsi(IL2CPP.il2cpp_class_get_namespace(Ptr));
            uint flags = 0;
            Flags = (IL2CPP_BindingFlags)IL2CPP.il2cpp_class_get_flags(Ptr, ref flags);

            // Map out Methods
            IntPtr method_iter = IntPtr.Zero;
            IntPtr method = IntPtr.Zero;
            while ((method = IL2CPP.il2cpp_class_get_methods(Ptr, ref method_iter)) != IntPtr.Zero)
                MethodList.Add(new IL2CPP_Method(method));

            // Map out Fields
            IntPtr field_iter = IntPtr.Zero;
            IntPtr field = IntPtr.Zero;
            while ((field = IL2CPP.il2cpp_class_get_fields(Ptr, ref field_iter)) != IntPtr.Zero)
                FieldList.Add(new IL2CPP_Field(field));

            // Map out Events
            IntPtr evt_iter = IntPtr.Zero;
            IntPtr evt = IntPtr.Zero;
            while ((evt = IL2CPP.il2cpp_class_get_events(Ptr, ref evt_iter)) != IntPtr.Zero)
                EventList.Add(new IL2CPP_Event(evt));

            // Map out Nested Types
            IntPtr nestedtype_iter = IntPtr.Zero;
            IntPtr nestedtype = IntPtr.Zero;
            while ((nestedtype = IL2CPP.il2cpp_class_get_nested_types(Ptr, ref nestedtype_iter)) != IntPtr.Zero)
                NestedTypeList.Add(new IL2CPP_Class(nestedtype));

            // Map out Properties
            IntPtr property_iter = IntPtr.Zero;
            IntPtr property = IntPtr.Zero;
            while ((property = IL2CPP.il2cpp_class_get_properties(Ptr, ref property_iter)) != IntPtr.Zero)
                PropertyList.Add(new IL2CPP_Property(property));
        }

        public IL2CPP_BindingFlags GetFlags() => Flags;
        public bool HasFlag(IL2CPP_BindingFlags flag) => ((GetFlags() & flag) != 0);

        // Methods
        public IL2CPP_Method[] GetMethods() => MethodList.ToArray();
        public IL2CPP_Method[] GetMethods(IL2CPP_BindingFlags flags) => GetMethods(flags, null);
        public IL2CPP_Method[] GetMethods(Func<IL2CPP_Method, bool> func) => GetMethods().Where(x => func(x)).ToArray();
        public IL2CPP_Method[] GetMethods(IL2CPP_BindingFlags flags, Func<IL2CPP_Method, bool> func) => GetMethods().Where(x => (((x.GetFlags() & flags) != 0) && func(x))).ToArray();
        public IL2CPP_Method GetMethod(string name) => GetMethod(name, null);
        public IL2CPP_Method GetMethod(string name, IL2CPP_BindingFlags flags) => GetMethod(name, flags, null);
        public IL2CPP_Method GetMethod(string name, Func<IL2CPP_Method, bool> func)
        {
            IL2CPP_Method returnval = null;
            foreach (IL2CPP_Method method in GetMethods())
            {
                if (method.Name.Equals(name) && ((func == null) || func(method)))
                {
                    returnval = method;
                    break;
                }
            }
            return returnval;
        }
        public IL2CPP_Method GetMethod(string name, IL2CPP_BindingFlags flags, Func<IL2CPP_Method, bool> func)
        {
            IL2CPP_Method returnval = null;
            foreach (IL2CPP_Method method in GetMethods())
            {
                if (method.Name.Equals(name) && method.HasFlag(flags) && ((func == null) || func(method)))
                {
                    returnval = method;
                    break;
                }
            }
            return returnval;
        }

        // Fields
        public IL2CPP_Field[] GetFields() => FieldList.ToArray();
        public IL2CPP_Field[] GetFields(IL2CPP_BindingFlags flags) => GetFields(flags, null);
        public IL2CPP_Field[] GetFields(Func<IL2CPP_Field, bool> func) => GetFields().Where(x => func(x)).ToArray();
        public IL2CPP_Field[] GetFields(IL2CPP_BindingFlags flags, Func<IL2CPP_Field, bool> func) => GetFields().Where(x => (((x.GetFlags() & flags) != 0) && func(x))).ToArray();
        public IL2CPP_Field GetField(string name) => GetField(name, null);
        public IL2CPP_Field GetField(string name, IL2CPP_BindingFlags flags) => GetField(name, flags, null);
        public IL2CPP_Field GetField(string name, Func<IL2CPP_Field, bool> func)
        {
            IL2CPP_Field returnval = null;
            foreach (IL2CPP_Field field in GetFields())
            {
                if (field.Name.Equals(name) && ((func == null) || func(field)))
                {
                    returnval = field;
                    break;
                }
            }
            return returnval;
        }
        public IL2CPP_Field GetField(string name, IL2CPP_BindingFlags flags, Func<IL2CPP_Field, bool> func)
        {
            IL2CPP_Field returnval = null;
            foreach (IL2CPP_Field field in GetFields())
            {
                if (field.Name.Equals(name) && field.HasFlag(flags) && ((func == null) || func(field)))
                {
                    returnval = field;
                    break;
                }
            }
            return returnval;
        }

        // Events
        public IL2CPP_Event[] GetEvents() => EventList.ToArray();

        // Properties
        public IL2CPP_Property[] GetProperties() => PropertyList.ToArray();
        public IL2CPP_Property[] GetProperties(IL2CPP_BindingFlags flags) => GetProperties().Where(x => ((x.GetFlags() & flags) != 0)).ToArray();
        public IL2CPP_Property GetProperty(string name)
        {
            IL2CPP_Property returnval = null;
            foreach (IL2CPP_Property prop in GetProperties())
            {
                if (prop.Name.Equals(name))
                {
                    returnval = prop;
                    break;
                }
            }
            return returnval;
        }
        public IL2CPP_Property GetProperty(string name, IL2CPP_BindingFlags flags)
        {
            IL2CPP_Property returnval = null;
            foreach (IL2CPP_Property prop in GetProperties())
            {
                if (prop.Name.Equals(name) && prop.HasFlag(flags))
                {
                    returnval = prop;
                    break;
                }
            }
            return returnval;
        }

        // Nested Types
        public IL2CPP_Class[] GetNestedTypes() => NestedTypeList.ToArray();
        public IL2CPP_Class[] GetNestedTypes(IL2CPP_BindingFlags flags) => GetNestedTypes().Where(x => ((x.GetFlags() & flags) != 0)).ToArray();
        public IL2CPP_Class GetNestedType(string name) => GetNestedType(name, null);
        public IL2CPP_Class GetNestedType(string name, IL2CPP_BindingFlags flags) => GetNestedType(name, null, flags);
        public IL2CPP_Class GetNestedType(string name, string name_space)
        {
            IL2CPP_Class returnval = null;
            foreach (IL2CPP_Class type in GetNestedTypes())
            {
                if (type.Name.Equals(name) && (string.IsNullOrEmpty(type.Namespace) || type.Namespace.Equals(name_space)))
                {
                    returnval = type;
                    break;
                }
            }
            return returnval;
        }
        public IL2CPP_Class GetNestedType(string name, string name_space, IL2CPP_BindingFlags flags)
        {
            IL2CPP_Class returnval = null;
            foreach (IL2CPP_Class type in GetNestedTypes())
            {
                if (type.Name.Equals(name) && (string.IsNullOrEmpty(type.Namespace) || type.Namespace.Equals(name_space)) && type.HasFlag(flags))
                {
                    returnval = type;
                    break;
                }
            }
            return returnval;
        }
    }
}
