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
            IL2CPP_Class returnval = null;
            foreach (IL2CPP_Assembly asm in GetAssemblies())
            {
                returnval = asm.GetClass(ptr);
                if (returnval != null)
                    break;
            }
            return returnval;
        }

        public static IL2CPP_Class GetClass(string fullname)
        {
            if (string.IsNullOrEmpty(fullname))
                return null;
            string name = string.Empty;
            string name_space = string.Empty;
            int idx = fullname.LastIndexOf('.');
            if (idx != -1)
            {
                name_space = fullname.Substring(0, idx);
                name = fullname.Substring(idx + 1);
            }
            else
                name = fullname;
            IL2CPP_Class returnval = null;
            foreach (IL2CPP_Assembly asm in GetAssemblies())
            {
                if (idx != -1)
                    returnval = asm.GetClass(name, name_space);
                else
                    returnval = asm.GetClass(name);
                if (returnval != null)
                    break;
            }
            return returnval;
        }
    }

    public static class IL2CPP
    {
        public static T[] IntPtrToStructureArray<T>(IntPtr ptr, uint len) { IntPtr iter = ptr; T[] arr = new T[len]; for (uint i = 0; i < len; i++) { arr[i] = (T)Marshal.PtrToStructure(iter, typeof(T)); iter += Marshal.SizeOf(typeof(T)); } return arr; }
        unsafe public static IntPtr[] IntPtrToArray(IntPtr ptr) { long length = *((long*)ptr + 3); IntPtr[] result = new IntPtr[length]; for (int i = 0; i < length; i++) result[i] = *(IntPtr*)((IntPtr)((long*)ptr + 4) + i * IntPtr.Size); return result; }
        unsafe public static string IntPtrToString(IntPtr ptr) => new string((char*)ptr.ToPointer() + 10); 
        public static IntPtr StringToIntPtr(string str) => il2cpp_string_new(str);

        public static IntPtr[] ObjectArrayToIntPtrArray(object[] objtbl) { if (objtbl == null) return null; IntPtr[] returntbl = new IntPtr[objtbl.Length]; for (int i = 0; i < objtbl.Length; i++) returntbl[i] = ObjectToIntPtr(objtbl[i]); return returntbl; }
        unsafe public static IntPtr ObjectToIntPtr(object obj) { if (obj == null) return IntPtr.Zero; TypedReference reference = __makeref(obj); TypedReference* pRef = &reference; return (IntPtr)pRef; }

        public static IntPtr[] IL2CPPObjectArrayToIntPtrArray(IL2CPP_Object[] objtbl) { IntPtr[] arr = new IntPtr[objtbl.Length]; for (uint i = 0; i < objtbl.Length; i++) { arr[i] = objtbl[i].Ptr; }; return arr; }
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
                returnval = IL2CPP.il2cpp_runtime_invoke(method, obj, pointerArray, IntPtr.Zero);
            }
            finally
            {
                Marshal.FreeHGlobal(intPtr);
            }
            return returnval;
        }

        // IL2CPP Functions
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_domain_get_assemblies(IntPtr domain, ref uint count);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_assembly_get_image(IntPtr assembly);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_image_get_name(IntPtr image);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_image_get_class_count(IntPtr image);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_image_get_class(IntPtr image, uint index);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_name(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_namespace(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_methods(IntPtr klass, ref IntPtr iter);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_method_get_name(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_fields(IntPtr klass, ref IntPtr iter);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_events(IntPtr klass, ref IntPtr iter);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_field_get_name(IntPtr field);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_method_get_param_count(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_method_get_param(IntPtr method, uint index);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_method_get_flags(IntPtr method, ref uint flags);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_method_get_param_name(IntPtr method, uint index);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_method_get_return_type(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_field_get_flags(IntPtr field, ref uint flags);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_class_get_flags(IntPtr klass, ref uint flags);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        unsafe public extern static IntPtr il2cpp_runtime_invoke(IntPtr method, IntPtr obj, void** param, IntPtr exc);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_object_unbox(IntPtr obj);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        unsafe public extern static IntPtr il2cpp_value_box(IntPtr klass, IntPtr obj);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_nested_types(IntPtr klass, ref IntPtr iter);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_properties(IntPtr klass, ref IntPtr iter);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static uint il2cpp_property_get_flags(IntPtr property);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_property_get_get_method(IntPtr property);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_property_get_set_method(IntPtr property);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_property_get_name(IntPtr property);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_string_new(string str);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_field_get_type(IntPtr field);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_property_get_type(IntPtr prop);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static string il2cpp_type_get_name(IntPtr type);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_class_get_type(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_type_get_object(IntPtr type);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_object_new(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        unsafe public extern static void il2cpp_field_static_get_value(IntPtr field, ref IntPtr value);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        unsafe public extern static void il2cpp_field_get_value(IntPtr obj, IntPtr field, ref IntPtr value);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        unsafe public extern static IntPtr il2cpp_field_get_value_object(IntPtr field, IntPtr obj);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        unsafe public extern static void il2cpp_field_static_set_value(IntPtr field, IntPtr value);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        unsafe public extern static void il2cpp_field_set_value(IntPtr obj, IntPtr field, IntPtr value);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        unsafe public extern static void il2cpp_field_set_value_object(IntPtr obj, IntPtr field, IntPtr value);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        unsafe public extern static uint il2cpp_array_length(IntPtr arr);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public extern static IntPtr il2cpp_type_get_class_or_element_class(IntPtr type);
    }
}