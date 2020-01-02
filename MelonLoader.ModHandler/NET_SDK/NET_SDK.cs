using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NET_SDK.Reflection;
#pragma warning disable 0649

namespace NET_SDK
{
    public class SDK
    {
        private static IntPtr Domain;
        private static List<IL2CPP_Assembly> AssemblyList = new List<IL2CPP_Assembly>();

        internal static void Initialize()
        {
            Domain = MelonLoader.Imports.melonloader_get_il2cpp_domain();

            uint assembly_count = 0;
            IntPtr assemblies = IL2CPP.il2cpp_domain_get_assemblies(Domain, ref assembly_count);
            IntPtr[] assembliesarr = IL2CPP.IntPtrToStructureArray<IntPtr>(assemblies, assembly_count);
            foreach (IntPtr assembly in assembliesarr)
                if (assembly != IntPtr.Zero)
                    AssemblyList.Add(new IL2CPP_Assembly(IL2CPP.il2cpp_assembly_get_image(assembly)));
        }

        private static IL2CPP_Class UnityAction = null;
        unsafe public static IntPtr Create_UnityAction(IntPtr function)
        {
            if (UnityAction == null)
                UnityAction = SDK.GetClass("UnityEngine.Events.UnityAction");
            if (UnityAction == null)
                return IntPtr.Zero;
            var obj = IL2CPP.il2cpp_object_new(UnityAction.Ptr);
            var o = Marshal.AllocHGlobal(8);
            *(IntPtr*)o = function;
            *((IntPtr*)obj + 2) = function;
            *((IntPtr*)obj + 4) = IntPtr.Zero;
            *((IntPtr*)obj + 5) = o;
            return obj;
        }

        public static IL2CPP_Assembly[] GetAssemblies() => AssemblyList.ToArray();
        public static IL2CPP_Assembly GetAssembly(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            IL2CPP_Assembly returnval = null;
            foreach (IL2CPP_Assembly asm in GetAssemblies())
            {
                if (asm.Name.Equals(name))
                {
                    returnval = asm;
                    break;
                }
            }
            return returnval;
        }

        public static IL2CPP_Class GetClass(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;
            IL2CPP_Class klass = null;
            foreach (IL2CPP_Assembly asm in GetAssemblies())
            {
                klass = asm.GetClass(ptr);
                if (klass != null)
                    return klass;
            }
            return klass;
        }

        public static IL2CPP_Class GetClass(string fullname)
        {
            if (string.IsNullOrEmpty(fullname))
                return null;
            string name;
            string name_space = string.Empty;
            int idx = fullname.LastIndexOf('.');
            if (idx != -1)
            {
                name_space = fullname.Substring(0, idx);
                name = fullname.Substring(idx + 1);
            }
            else
                name = fullname;
            return GetClass(name_space, name);
        }
        /// <summary>
        /// Gets the <see cref="IL2CPP_Class"/> from the provided namespace name and qualified name.
        /// </summary>
        /// <param name="nameSpace">The namespace the class is in</param>
        /// <param name="qualifiedName">The qualified name the class has (private classes should be separated by /)</param>
        /// <exception cref="InvalidOperationException">Thrown when the class could not be found</exception>
        /// <returns>The <see cref="IL2CPP_Class"/> found</returns>
        public static IL2CPP_Class GetClass(string nameSpace, string qualifiedName)
        {
            IL2CPP_Class ret;
            foreach (IL2CPP_Assembly asm in GetAssemblies())
            {
                ret = asm.GetClass(qualifiedName, nameSpace);
                if (ret != null)
                    return ret;
            }
            throw new InvalidOperationException($"NET_SDK: GetClass: Could not find namespace: {nameSpace} class: {qualifiedName}!");
        }
    }

    public static class IL2CPP
    {
        public static T[] IntPtrToStructureArray<T>(IntPtr ptr, uint len)
        {
            IntPtr iter = ptr;
            T[] arr = new T[len];
            for (uint i = 0; i < len; i++)
            {
                arr[i] = (T)Marshal.PtrToStructure(iter, typeof(T));
                iter += Marshal.SizeOf(typeof(T));
            }
            return arr;
        }
        unsafe public static IntPtr[] IntPtrToArray(IntPtr ptr)
        {
            long length = *((long*)ptr + 3);
            IntPtr[] result = new IntPtr[length];
            for (int i = 0; i < length; i++)
                result[i] = *(IntPtr*)((IntPtr)((long*)ptr + 4) + i * IntPtr.Size);
            return result;
        }
        unsafe public static string IntPtrToString(IntPtr ptr) => new string((char*)ptr.ToPointer() + 10); 
        public static IntPtr StringToIntPtr(string str) => il2cpp_string_new(str);
        /// <summary>
        /// Converts an array of value types, <see cref="string"/>, and <see cref="IL2CPP_Object"/> to an array of <see cref="IntPtr"/>
        /// Returns null if the input array is null
        /// <para>Throws an <see cref="InvalidCastException"/> if any of the arguments are not a valid type</para>
        /// </summary>
        /// <param name="objtbl">The object array to convert</param>
        /// <returns>The resulting <see cref="IntPtr"/> array</returns>
        /// <exception cref="InvalidCastException"></exception>
        public static IntPtr[] ObjectArrayToIntPtrArray(object[] objtbl)
        {
            if (objtbl == null)
                return null;
            IntPtr[] returntbl = new IntPtr[objtbl.Length];
            IntPtr temp;
            for (int i = 0; i < objtbl.Length; i++)
            {
                var o = objtbl[i];
                var t = o.GetType();
                if (t.IsPrimitive || t.IsValueType || t.IsEnum)
                    // Box this object
                    temp = ObjectToIntPtr(o);
                else if (o is IL2CPP_Object)
                    temp = (o as IL2CPP_Object).Ptr;
                else if (o is string)
                    temp = StringToIntPtr(o as string);
                else
                    // We don't know how to handle objects that aren't value types or IL2CPP_Objects!
                    // Throw an exception here until this is resolved
                    throw new InvalidCastException($"SDK: ObjectArrayToIntPtrArray: Cannot convert type: {t.FullName}. Please use ONLY value types and IL2CPP_Objects!");
                returntbl[i] = temp;
            }
            return returntbl;
        }
        unsafe public static IntPtr ObjectToIntPtr(object obj)
        {
            if (obj == null) return IntPtr.Zero;
            TypedReference reference = __makeref(obj);
            TypedReference* pRef = &reference;
            return (IntPtr)pRef;
        }

        public static IntPtr[] IL2CPPObjectArrayToIntPtrArray(IL2CPP_Object[] objtbl)
        {
            IntPtr[] arr = new IntPtr[objtbl.Length];
            for (uint i = 0; i < objtbl.Length; i++)
                arr[i] = objtbl[i].Ptr;
            return arr;
        }
        unsafe public static IL2CPP_Object ObjectToIL2CPPObject(IntPtr obj, IntPtr klass)
        {
            if (klass != IntPtr.Zero)
            {
                IntPtr objptr = il2cpp_value_box(klass, obj);
                if (objptr != IntPtr.Zero)
                    return new IL2CPP_Object(objptr, new IL2CPP_Type(il2cpp_class_get_type(klass)));
            }
            return null;
        }
        unsafe public static IL2CPP_Object ObjectToIL2CPPObject(IntPtr obj, string klass)
        {
            IL2CPP_Class klassobj = SDK.GetClass(klass);
            if (klassobj != null)
                return ObjectToIL2CPPObject(obj, klassobj.Ptr);
            return null;
        }

        public static IL2CPP_Class SystemTypeToIL2CPPClass(Type type)
        {
            if (type != null)
                return SDK.GetClass(type.Namespace + "." + type.Name);
            return null;
        }

        unsafe public static IntPtr InvokeMethod(IntPtr method, IntPtr obj, params IntPtr[] paramtbl)
        {
            if (method == IntPtr.Zero)
                return IntPtr.Zero;
            IntPtr[] intPtrArray;
            IntPtr returnval = IntPtr.Zero;
            intPtrArray = ((paramtbl != null) ? paramtbl : new IntPtr[0]);
            IntPtr intPtr = Marshal.AllocHGlobal(intPtrArray.Length * sizeof(void*));
            try
            {
                void** pointerArray = (void**)intPtr.ToPointer();
                for (int i = 0; i < intPtrArray.Length; i++)
                    pointerArray[i] = intPtrArray[i].ToPointer();
                IntPtr exp = IntPtr.Zero;
                returnval = IL2CPP.il2cpp_runtime_invoke(method, obj, pointerArray, ref exp);
                if (exp.ToInt32() != 0)
                {
                    // There was an exception! Handle it somehow here
                    string message = "";
                    var ptr = Marshal.AllocHGlobal(4097);
                    try
                    {
                        // 4096 is the largest length of an exception. If not, this can be changed.
                        // TODO: Make this a constant/predefine instead of a magic number
                        il2cpp_format_exception(exp, ptr, 4096);
                        message = Marshal.PtrToStringAnsi(ptr);
                        throw new InvalidOperationException($"Invoke failed with exception: {message}");
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ptr);
                    }
                    throw new InvalidOperationException($"Invoke failed! Could not get exception from invoke failure!");
                }
            }
            finally
            {
                Marshal.FreeHGlobal(intPtr);
            }
            return returnval;
        }

        // IL2CPP Functions
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_init(IntPtr domain_name);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_init_utf16(IntPtr domain_name);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_shutdown();
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_set_config_dir(IntPtr config_path);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_set_data_dir(IntPtr data_path);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_set_temp_dir(IntPtr temp_path);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_set_commandline_arguments(int argc, IntPtr argv, IntPtr basedir);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_set_commandline_arguments_utf16(int argc, IntPtr argv, IntPtr basedir);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_set_config_utf16(IntPtr executablePath);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_set_config(IntPtr executablePath);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_set_memory_callbacks(IntPtr callbacks);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_get_corlib();
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_add_internal_call(IntPtr name, IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_resolve_icall(IntPtr name);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_alloc(uint size);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_free(IntPtr ptr);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_array_class_get(IntPtr element_class, uint rank);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_array_length(IntPtr array);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_array_get_byte_length(IntPtr array);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_array_new(IntPtr elementTypeInfo, ulong length);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_array_new_specific(IntPtr arrayTypeInfo, ulong length);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_array_new_full(IntPtr array_class, ref ulong lengths, ref ulong lower_bounds);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_bounded_array_class_get(IntPtr element_class, uint rank, bool bounded);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int il2cpp_array_element_size(IntPtr array_class);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_assembly_get_image(IntPtr assembly);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_enum_basetype(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_class_is_generic(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_class_is_inflated(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_class_is_assignable_from(IntPtr klass, IntPtr oklass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_class_is_subclass_of(IntPtr klass, IntPtr klassc, bool check_interfaces);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_class_has_parent(IntPtr klass, IntPtr klassc);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_from_il2cpp_type(IntPtr type);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_from_name(IntPtr image, IntPtr namespaze, IntPtr name);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_from_system_type(IntPtr type);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_element_class(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_events(IntPtr klass, ref IntPtr iter);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_fields(IntPtr klass, ref IntPtr iter);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_nested_types(IntPtr klass, ref IntPtr iter);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_interfaces(IntPtr klass, ref IntPtr iter);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_properties(IntPtr klass, ref IntPtr iter);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_property_from_name(IntPtr klass, IntPtr name);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_field_from_name(IntPtr klass, IntPtr name);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_methods(IntPtr klass, ref IntPtr iter);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_method_from_name(IntPtr klass, IntPtr name, int argsCount);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_name(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_namespace(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_parent(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_declaring_type(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int il2cpp_class_instance_size(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_class_num_fields(IntPtr enumKlass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_class_is_valuetype(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int il2cpp_class_value_size(IntPtr klass, ref uint align);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_class_is_blittable(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int il2cpp_class_get_flags(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_class_is_abstract(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_class_is_interface(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int il2cpp_class_array_element_size(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_from_type(IntPtr type);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_type(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_class_get_type_token(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_class_has_attribute(IntPtr klass, IntPtr attr_class);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_class_has_references(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_class_is_enum(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_image(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_assemblyname(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int il2cpp_class_get_rank(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_class_get_bitmap_size(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_class_get_bitmap(IntPtr klass, ref uint bitmap);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_stats_dump_to_file(IntPtr path);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static ulong il2cpp_stats_get_value(IL2CPP_Stat stat);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_domain_get();
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_domain_assembly_open(IntPtr domain, IntPtr name);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_domain_get_assemblies(IntPtr domain, ref uint size);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_exception_from_name_msg(IntPtr image, IntPtr name_space, IntPtr name, IntPtr msg);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_get_exception_argument_null(IntPtr arg);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_format_exception(IntPtr ex, IntPtr message, int message_size);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_format_stack_trace(IntPtr ex, IntPtr output, int output_size);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_unhandled_exception(IntPtr ex);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int il2cpp_field_get_flags(IntPtr field);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_field_get_name(IntPtr field);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_field_get_parent(IntPtr field);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_field_get_offset(IntPtr field);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_field_get_type(IntPtr field);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_field_get_value(IntPtr obj, IntPtr field, ref IntPtr value);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_field_get_value_object(IntPtr field, IntPtr obj);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_field_has_attribute(IntPtr field, IntPtr attr_class);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_field_set_value(IntPtr obj, IntPtr field, IntPtr value);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_field_static_get_value(IntPtr field, ref IntPtr value);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_field_static_set_value(IntPtr field, IntPtr value);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_field_set_value_object(IntPtr instance, IntPtr field, IntPtr value);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_gc_collect(int maxGenerations);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int il2cpp_gc_collect_a_little();
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_gc_disable();
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_gc_enable();
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_gc_is_disabled();
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static long il2cpp_gc_get_used_size();
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static long il2cpp_gc_get_heap_size();
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_gc_wbarrier_set_field(IntPtr obj, out IntPtr targetAddress, IntPtr gcObj);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_gchandle_new(IntPtr obj, bool pinned);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_gchandle_new_weakref(IntPtr obj, bool track_resurrection);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_gchandle_get_target(uint gchandle);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_gchandle_free(uint gchandle);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_unity_liveness_calculation_begin(IntPtr filter, int max_object_count, IntPtr callback, IntPtr userdata, IntPtr onWorldStarted, IntPtr onWorldStopped);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_unity_liveness_calculation_end(IntPtr state);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_unity_liveness_calculation_from_root(IntPtr root, IntPtr state);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_unity_liveness_calculation_from_statics(IntPtr state);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_method_get_return_type(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_method_get_declaring_type(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_method_get_name(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_method_get_from_reflection(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_method_get_object(IntPtr method, IntPtr refclass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_method_is_generic(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_method_is_inflated(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_method_is_instance(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_method_get_param_count(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_method_get_param(IntPtr method, uint index);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_method_get_class(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_method_has_attribute(IntPtr method, IntPtr attr_class);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_method_get_flags(IntPtr method, ref uint iflags);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_method_get_token(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_method_get_param_name(IntPtr method, uint index);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_profiler_install(IntPtr prof, IntPtr shutdown_callback);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_profiler_set_events(IL2CPP_ProfileFlags events);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_profiler_install_enter_leave(IntPtr enter, IntPtr fleave);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_profiler_install_allocation(IntPtr callback);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_profiler_install_gc(IntPtr callback, IntPtr heap_resize_callback);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_profiler_install_fileio(IntPtr callback);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_profiler_install_thread(IntPtr start, IntPtr end);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_property_get_flags(IntPtr prop);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_property_get_get_method(IntPtr prop);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_property_get_set_method(IntPtr prop);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_property_get_name(IntPtr prop);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_property_get_parent(IntPtr prop);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_object_get_class(IntPtr obj);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_object_get_size(IntPtr obj);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_object_get_virtual_method(IntPtr obj, IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_object_new(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_object_unbox(IntPtr obj);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_value_box(IntPtr klass, IntPtr data);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_monitor_enter(IntPtr obj);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_monitor_try_enter(IntPtr obj, uint timeout);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_monitor_exit(IntPtr obj);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_monitor_pulse(IntPtr obj);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_monitor_pulse_all(IntPtr obj);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_monitor_wait(IntPtr obj);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_monitor_try_wait(IntPtr obj, uint timeout);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        unsafe public extern static IntPtr il2cpp_runtime_invoke(IntPtr method, IntPtr obj, void** param, ref IntPtr exc);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        // param can be of Il2CppObject*
        unsafe public extern static IntPtr il2cpp_runtime_invoke_convert_args(IntPtr method, IntPtr obj, void** param, int paramCount, ref IntPtr exc);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_runtime_class_init(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_runtime_object_init(IntPtr obj);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_runtime_object_init_exception(IntPtr obj, ref IntPtr exc);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_runtime_unhandled_exception_policy_set(IL2CPP_RuntimeUnhandledExceptionPolicy value);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int il2cpp_string_length(IntPtr str);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        unsafe public extern static char* il2cpp_string_chars(IntPtr str);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_string_new(string str);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_string_new_len(string str, uint length);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_string_new_utf16(string text, int len);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_string_new_wrapper(string str);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_string_intern(string str);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_string_is_interned(string str);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_thread_current();
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_thread_attach(IntPtr domain);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_thread_detach(IntPtr thread);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        unsafe public extern static void** il2cpp_thread_get_all_attached_threads(ref uint size);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_is_vm_thread(IntPtr thread);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_current_thread_walk_frame_stack(IntPtr func, IntPtr user_data);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_thread_walk_frame_stack(IntPtr thread, IntPtr func, IntPtr user_data);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_current_thread_get_top_frame(IntPtr frame);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_thread_get_top_frame(IntPtr thread, IntPtr frame);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_current_thread_get_frame_at(int offset, IntPtr frame);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_thread_get_frame_at(IntPtr thread, int offset, IntPtr frame);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int il2cpp_current_thread_get_stack_depth();
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int il2cpp_thread_get_stack_depth(IntPtr thread);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_type_get_object(IntPtr type);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static int il2cpp_type_get_type(IntPtr type);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_type_get_class_or_element_class(IntPtr type);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static string il2cpp_type_get_name(IntPtr type);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_type_is_byref(IntPtr type);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_type_get_attrs(IntPtr type);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_type_equals(IntPtr type, IntPtr otherType);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_type_get_assembly_qualified_name(IntPtr type);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_image_get_assembly(IntPtr image);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_image_get_name(IntPtr image);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_image_get_filename(IntPtr image);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_image_get_entry_point(IntPtr image);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_image_get_class_count(IntPtr image);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_image_get_class(IntPtr image, uint index);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_capture_memory_snapshot();
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_free_captured_memory_snapshot(IntPtr snapshot);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_set_find_plugin_callback(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_register_log_callback(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_debugger_set_agent_options(IntPtr options);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_is_debugger_attached();
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        unsafe public extern static void il2cpp_unity_install_unitytls_interface(void* unitytlsInterfaceStruct);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_custom_attrs_from_class(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_custom_attrs_from_method(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_custom_attrs_get_attr(IntPtr ainfo, IntPtr attr_klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static bool il2cpp_custom_attrs_has_attr(IntPtr ainfo, IntPtr attr_klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_custom_attrs_construct(IntPtr cinfo);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static void il2cpp_custom_attrs_free(IntPtr ainfo);
    }
}