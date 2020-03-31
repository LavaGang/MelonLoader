using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;

namespace NET_SDK.Reflection
{
    public class IL2CPP_Class : IL2CPP_Base
    {
        public readonly string Name;
        public readonly string Namespace;
        public readonly IL2CPP_BindingFlags Flags;
        private readonly IL2CPP_Method[] MethodList;
        private readonly IL2CPP_Field[] FieldList;
        private readonly IL2CPP_Event[] EventList;
        private readonly IL2CPP_Class[] NestedTypeList;
        private readonly IL2CPP_Property[] PropertyList;

        internal IL2CPP_Class(IntPtr ptr) : base(ptr)
        {
            // Setup Information
            Ptr = ptr;
            Name = Marshal.PtrToStringAnsi(IL2CPP.il2cpp_class_get_name(Ptr));
            Namespace = Marshal.PtrToStringAnsi(IL2CPP.il2cpp_class_get_namespace(Ptr));
            Flags = (IL2CPP_BindingFlags)IL2CPP.il2cpp_class_get_flags(Ptr);

            // Map out Methods
            IntPtr method_iter = IntPtr.Zero;
            IntPtr method;
            List<IL2CPP_Method> methodsList = new List<IL2CPP_Method>();
            while ((method = IL2CPP.il2cpp_class_get_methods(Ptr, ref method_iter)) != IntPtr.Zero)
                methodsList.Add(new IL2CPP_Method(method));
            MethodList = methodsList.ToArray();
            // Map out Fields
            IntPtr field_iter = IntPtr.Zero;
            IntPtr field;
            List<IL2CPP_Field> fieldList = new List<IL2CPP_Field>();
            while ((field = IL2CPP.il2cpp_class_get_fields(Ptr, ref field_iter)) != IntPtr.Zero)
                fieldList.Add(new IL2CPP_Field(field));
            FieldList = fieldList.ToArray();
            // Map out Events
            IntPtr evt_iter = IntPtr.Zero;
            IntPtr evt;
            List<IL2CPP_Event> eventList = new List<IL2CPP_Event>();
            while ((evt = IL2CPP.il2cpp_class_get_events(Ptr, ref evt_iter)) != IntPtr.Zero)
                eventList.Add(new IL2CPP_Event(evt));
            EventList = eventList.ToArray();
            // Map out Nested Types
            IntPtr nestedtype_iter = IntPtr.Zero;
            IntPtr nestedtype;
            List<IL2CPP_Class> nestedTypeList = new List<IL2CPP_Class>();
            while ((nestedtype = IL2CPP.il2cpp_class_get_nested_types(Ptr, ref nestedtype_iter)) != IntPtr.Zero)
                nestedTypeList.Add(new IL2CPP_Class(nestedtype));
            NestedTypeList = nestedTypeList.ToArray();
            // Map out Properties
            IntPtr property_iter = IntPtr.Zero;
            IntPtr property;
            List<IL2CPP_Property> propertyList = new List<IL2CPP_Property>();
            while ((property = IL2CPP.il2cpp_class_get_properties(Ptr, ref property_iter)) != IntPtr.Zero)
                propertyList.Add(new IL2CPP_Property(property));
            PropertyList = propertyList.ToArray();
        }

        public IL2CPP_BindingFlags GetFlags() => Flags;
        public bool HasFlag(IL2CPP_BindingFlags flag) => ((GetFlags() & flag) != 0);

        // Methods
        public IL2CPP_Method[] GetMethods() => MethodList;
        public IL2CPP_Method[] GetMethods(IL2CPP_BindingFlags flags) => GetMethods(flags, null);
        public IL2CPP_Method[] GetMethods(Func<IL2CPP_Method, bool> func) => GetMethods().Where(x => func(x)).ToArray();
        public IL2CPP_Method[] GetMethods(IL2CPP_BindingFlags flags, Func<IL2CPP_Method, bool> func) => GetMethods().Where(x => (((x.GetFlags() & flags) != 0) && func(x))).ToArray();
        public IL2CPP_Method GetMethod(string name) => GetMethod(name, null);
        public IL2CPP_Method GetMethod(string name, IL2CPP_BindingFlags flags) => GetMethod(name, flags, null);
        public IL2CPP_Method GetMethod(string name, Func<IL2CPP_Method, bool> func) => GetMethods().FirstOrDefault(m => m.Name.Equals(name) && (func == null || func(m)));
        public IL2CPP_Method GetMethod(string name, IL2CPP_BindingFlags flags, Func<IL2CPP_Method, bool> func) => GetMethods().FirstOrDefault(m => m.Name.Equals(name) && m.HasFlag(flags) && (func == null || func(m)));
        /// <summary>
        /// Gets the first method matching name and flags with the specified number of parameters
        /// </summary>
        /// <param name="name">The name to search for</param>
        /// <param name="flags">The <see cref="IL2CPP_BindingFlags"/> to match</param>
        /// <param name="argCount">The parameter count to match</param>
        /// <returns>The first matching <see cref="IL2CPP_Method"/></returns>
        public IL2CPP_Method GetMethod(string name, IL2CPP_BindingFlags flags, int argCount) => GetMethods().FirstOrDefault(m => m.Name.Equals(name) && m.HasFlag(flags) && m.GetParameterCount() == argCount);
        /// <summary>
        /// Gets the first method matching name with the specified number of parameters
        /// </summary>
        /// <param name="name">The name to search for</param>
        /// <param name="argCount">The parameter count to match</param>
        /// <returns>The first matching <see cref="IL2CPP_Method"/></returns>
        public IL2CPP_Method GetMethod(string name, int argCount) => GetMethods().FirstOrDefault(m => m.Name.Equals(name) && m.GetParameterCount() == argCount);

        // Fields
        public IL2CPP_Field[] GetFields() => FieldList;
        public IL2CPP_Field[] GetFields(IL2CPP_BindingFlags flags) => GetFields(flags, null);
        public IL2CPP_Field[] GetFields(Func<IL2CPP_Field, bool> func) => GetFields().Where(x => func(x)).ToArray();
        public IL2CPP_Field[] GetFields(IL2CPP_BindingFlags flags, Func<IL2CPP_Field, bool> func) => GetFields().Where(x => (((x.GetFlags() & flags) != 0) && func(x))).ToArray();
        public IL2CPP_Field GetField(string name) => GetField(name, null);
        public IL2CPP_Field GetField(string name, IL2CPP_BindingFlags flags) => GetField(name, flags, null);
        public IL2CPP_Field GetField(string name, Func<IL2CPP_Field, bool> func) => GetFields().FirstOrDefault(f => f.Name.Equals(name) && (func == null || func(f)));
        public IL2CPP_Field GetField(string name, IL2CPP_BindingFlags flags, Func<IL2CPP_Field, bool> func) => GetFields().FirstOrDefault(f => f.Name.Equals(name) && f.HasFlag(flags) && (func == null || func(f)));

        // Events
        public IL2CPP_Event[] GetEvents() => EventList;

        // Properties
        public IL2CPP_Property[] GetProperties() => PropertyList;
        public IL2CPP_Property[] GetProperties(IL2CPP_BindingFlags flags) => GetProperties().Where(x => ((x.GetFlags() & flags) != 0)).ToArray();
        public IL2CPP_Property GetProperty(string name) => GetProperties().FirstOrDefault(p => p.Name.Equals(name));
        public IL2CPP_Property GetProperty(string name, IL2CPP_BindingFlags flags) => GetProperties().FirstOrDefault(p => p.Name.Equals(name) && p.HasFlag(flags));

        // Nested Types
        public IL2CPP_Class[] GetNestedTypes() => NestedTypeList;
        public IL2CPP_Class[] GetNestedTypes(IL2CPP_BindingFlags flags) => GetNestedTypes().Where(x => ((x.GetFlags() & flags) != 0)).ToArray();
        public IL2CPP_Class GetNestedType(string name) => GetNestedType(name, null);
        public IL2CPP_Class GetNestedType(string name, IL2CPP_BindingFlags flags) => GetNestedType(name, null, flags);
        public IL2CPP_Class GetNestedType(string name, string name_space) => GetNestedTypes().FirstOrDefault(n => n.Name.Equals(name) && (string.IsNullOrEmpty(n.Namespace) || n.Namespace.Equals(name_space)));
        public IL2CPP_Class GetNestedType(string name, string name_space, IL2CPP_BindingFlags flags) => GetNestedTypes().FirstOrDefault(n => n.Name.Equals(name) && n.HasFlag(flags) && (string.IsNullOrEmpty(n.Namespace) || n.Namespace.Equals(name_space)));
    }
}
