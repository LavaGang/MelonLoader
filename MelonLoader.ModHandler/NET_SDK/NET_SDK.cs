using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NET_SDK.Reflection;
#pragma warning disable 0649
#pragma warning disable 0618

namespace NET_SDK
{
    [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
    public static class SDK
    {
        private static IntPtr Domain;
        private static IL2CPP_Assembly[] AssemblyList;

        internal static void Initialize()
        {
            Domain = MelonLoader.Il2CppImports.il2cpp_domain_get();
            uint assembly_count = 0;
            IntPtr assemblies = MelonLoader.Il2CppImports.il2cpp_domain_get_assemblies(Domain, ref assembly_count);
            IntPtr[] assembliesarr = MelonLoader.Il2CppImports.IntPtrToStructureArray<IntPtr>(assemblies, assembly_count);
            List<IL2CPP_Assembly> assemblyList = new List<IL2CPP_Assembly>();
            for (int i = 0; i < assembliesarr.Length; i++)
            {
                IntPtr assembly = assembliesarr[i];
                if (assembly != IntPtr.Zero)
                    assemblyList.Add(new IL2CPP_Assembly(MelonLoader.Il2CppImports.il2cpp_assembly_get_image(assembly)));
            }
            AssemblyList = assemblyList.ToArray();
        }

        private static IL2CPP_Class UnityAction = null;
        [ObsoleteAttribute("This method will be removed soon. Please use Generated Assembly instead.")]
        unsafe public static IntPtr Create_UnityAction(IntPtr function)
        {
            if (UnityAction == null)
                UnityAction = SDK.GetClass("UnityEngine.Events.UnityAction");
            if (UnityAction == null)
                return IntPtr.Zero;
            var obj = MelonLoader.Il2CppImports.il2cpp_object_new(UnityAction.Ptr);
            var o = Marshal.AllocHGlobal(8);
            *(IntPtr*)o = function;
            *((IntPtr*)obj + 2) = function;
            *((IntPtr*)obj + 4) = IntPtr.Zero;
            *((IntPtr*)obj + 5) = o;
            return obj;
        }

        [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection instead.")]
        public static IL2CPP_Assembly[] GetAssemblies() => AssemblyList;
        [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection instead.")]
        public static IL2CPP_Assembly GetAssembly(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            IL2CPP_Assembly returnval = null;
            var assemblies = GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                var asm = assemblies[i];
                if (asm.Name.Equals(name))
                {
                    returnval = asm;
                    break;
                }
            }
            return returnval;
        }

        [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection instead.")]
        public static IL2CPP_Class GetClass(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                return null;
            IL2CPP_Class klass = null;
            var assemblies = GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                var asm = assemblies[i];
                klass = asm.GetClass(ptr);
                if (klass != null)
                    return klass;
            }
            return klass;
        }

        [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection instead.")]
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
        [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection instead.")]
        public static IL2CPP_Class GetClass(string nameSpace, string qualifiedName)
        {
            IL2CPP_Class ret;
            var assemblies = GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                var asm = assemblies[i];
                ret = asm.GetClass(qualifiedName, nameSpace);
                if (ret != null)
                    return ret;
            }
            throw new InvalidOperationException($"NET_SDK: GetClass: Could not find namespace: {nameSpace} class: {qualifiedName}!");
        }
    }

    [ObsoleteAttribute("This method will be removed soon. Please use normal Reflection.")]
    public static class IL2CPP
    {
        [ObsoleteAttribute("This method will be removed soon.")]
        unsafe public static string IntPtrToString(IntPtr ptr) => new string((char*)ptr.ToPointer() + 10);
        [ObsoleteAttribute("This method will be removed soon.")]
        public static IntPtr StringToIntPtr(string str) => MelonLoader.Il2CppImports.il2cpp_string_new(str);
        /// <summary>
        /// Converts an array of value types, <see cref="string"/>, and <see cref="IL2CPP_Object"/> to an array of <see cref="IntPtr"/>
        /// Returns null if the input array is null
        /// <para>Throws an <see cref="InvalidCastException"/> if any of the arguments are not a valid type</para>
        /// </summary>
        /// <param name="objtbl">The object array to convert</param>
        /// <returns>The resulting <see cref="IntPtr"/> array</returns>
        /// <exception cref="InvalidCastException"></exception>
        [ObsoleteAttribute("This method will be removed soon.")]
        public static IntPtr[] ObjectArrayToIntPtrArray(object[] objtbl)
        {
            if (objtbl == null)
                return null;
            IntPtr[] returntbl = new IntPtr[objtbl.Length];
            for (int i = 0; i < objtbl.Length; i++)
                returntbl[i] = ObjectToIntPtr(objtbl[i]);
            return returntbl;
        }

        [ObsoleteAttribute("This method will be removed soon.")]
        public static IntPtr[] IL2CPPObjectArrayToIntPtrArray(IL2CPP_Object[] objtbl)
        {
            IntPtr[] arr = new IntPtr[objtbl.Length];
            for (uint i = 0; i < objtbl.Length; i++)
                arr[i] = objtbl[i].Ptr;
            return arr;
        }

        [ObsoleteAttribute("This method will be removed soon.")]
        unsafe public static IntPtr ObjectToTypedReferenceIntPtr(object obj)
        {
            if (obj == null) return IntPtr.Zero;
            TypedReference reference = __makeref(obj);
            TypedReference* pRef = &reference;
            return (IntPtr)pRef;
        }

        [ObsoleteAttribute("This method will be removed soon.")]
        public static IntPtr ObjectToIntPtr(object obj)
        {
            if (obj != null)
            {
                var t = obj.GetType();
                if (t.IsPrimitive || t.IsValueType || t.IsEnum)
                    return ObjectToTypedReferenceIntPtr(obj);
                else if (obj is IL2CPP_Object)
                    return (obj as IL2CPP_Object).Ptr;
                else if (obj is string)
                    return StringToIntPtr(obj as string);
                else
                {
                    // To-Do
                    return ObjectToTypedReferenceIntPtr(obj);
                }
            }
            return IntPtr.Zero;
        }

        [ObsoleteAttribute("This method will be removed soon.")]
        unsafe public static IL2CPP_Object ObjectIntPtrToIL2CPPObject(IntPtr obj, IntPtr klass)
        {
            if (klass != IntPtr.Zero)
            {
                IntPtr objptr = MelonLoader.Il2CppImports.il2cpp_value_box(klass, obj);
                if (objptr != IntPtr.Zero)
                    return new IL2CPP_Object(objptr, new IL2CPP_Type(MelonLoader.Il2CppImports.il2cpp_class_get_type(klass)));
            }
            return null;
        }
        [ObsoleteAttribute("This method will be removed soon.")]
        unsafe public static IL2CPP_Object ObjectIntPtrToIL2CPPObject(IntPtr obj, string klass)
        {
            IL2CPP_Class klassobj = SDK.GetClass(klass);
            if (klassobj != null)
                return ObjectIntPtrToIL2CPPObject(obj, klassobj.Ptr);
            return null;
        }

        [ObsoleteAttribute("This method will be removed soon.")]
        public static IL2CPP_Class SystemTypeToIL2CPPClass(Type type)
        {
            if (type != null)
                return SDK.GetClass(type.Namespace, type.Name);
            return null;
        }

        [ObsoleteAttribute("This method will be removed soon.")]
        unsafe public static T[] IntPtrArrayToUnboxedValueTypeArray<T>(IntPtr arr) where T : unmanaged
        {
            IntPtr[] arr_2 = MelonLoader.Il2CppImports.IntPtrToArray(arr);
            T[] return_arr = new T[arr_2.Length];
            for (uint i = 0; i < arr_2.Length; i++)
                return_arr[i] = *(T*)MelonLoader.Il2CppImports.il2cpp_object_unbox(arr_2[i]).ToPointer();
            return return_arr;
        }

        [ObsoleteAttribute("This method will be removed soon.")]
        public static T[] IL2CPPObjectArrayToUnboxedValueTypeArray<T>(IL2CPP_Object[] arr) where T : unmanaged
        {
            T[] return_arr = new T[arr.Length];
            for (uint i = 0; i < arr.Length; i++)
                return_arr[i] = arr[i].UnboxValue<T>();
            return return_arr;
        }

        [ObsoleteAttribute("This method will be removed soon.")]
        unsafe public static T[] IL2CPPObjectToUnboxedValueTypeArray<T>(IL2CPP_Object arr) where T : unmanaged
        {
            IntPtr[] arr_2 = MelonLoader.Il2CppImports.IntPtrToArray(arr.Ptr);
            T[] return_arr = new T[arr_2.Length];
            for (uint i = 0; i < arr_2.Length; i++)
                return_arr[i] = *(T*)MelonLoader.Il2CppImports.il2cpp_object_unbox(arr_2[i]).ToPointer();
            return return_arr;
        }
    }
}