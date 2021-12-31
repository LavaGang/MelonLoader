using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using MelonLoader;
using MelonLoader.CompatibilityLayers;

namespace UnhollowerMini
{
    internal static unsafe class UnityInternals
    {
        private static readonly IntPtr domain;
        private static readonly List<InternalAssembly> assemblies = new List<InternalAssembly>();

        unsafe static UnityInternals()
        {
            domain = il2cpp_domain_get();

            uint assemblyCount = 0;
            IntPtr* assemblyArray = il2cpp_domain_get_assemblies(domain, ref assemblyCount);
            for (int i = 0; i < assemblyCount; ++i)
                assemblies.Add(new InternalAssembly(il2cpp_assembly_get_image(*(assemblyArray + i))));
        }

        private class InternalAssembly
        {
            public IntPtr ptr;
            public string name;

            public InternalAssembly(IntPtr ptr)
            {
                this.ptr = ptr;
                name = Marshal.PtrToStringAnsi(il2cpp_image_get_filename(this.ptr));
            }
        }
        internal static IntPtr GetClass(string assemblyname, string name_space, string classname)
        {
            MelonDebug.Msg($"GetClass {assemblyname} {name_space} {classname}");

            InternalAssembly assembly = assemblies.FirstOrDefault(a => a.name == assemblyname);
            if (assembly == null)
            {
                throw new Exception("Unable to find assembly " + assemblyname + " in il2cpp domain");
            }

            IntPtr clazz = il2cpp_class_from_name(assembly.ptr, name_space, classname);
            if (clazz == null)
            {
                throw new Exception("Unable to find class " + name_space + "." + classname + " in assembly " + assemblyname);
            }

            MelonDebug.Msg($" > 0x{(long)clazz:X}");
            return clazz;
        }

        internal static IntPtr GetMethod(IntPtr clazz, string name, string returntype, params string[] parameters)
        {
            MelonDebug.Msg($"GetMethod {returntype} {name}({string.Join(", ", parameters)})");

            IntPtr iter = IntPtr.Zero;
            IntPtr element;
            while ((element = il2cpp_class_get_methods(clazz, ref iter)) != IntPtr.Zero)
            {
                if (Marshal.PtrToStringAnsi(il2cpp_method_get_name(element)) != name)
                    continue;

                if (Marshal.PtrToStringAnsi(il2cpp_type_get_name(il2cpp_method_get_return_type(element))) != returntype)
                    continue;

                if (parameters.Length != il2cpp_method_get_param_count(element))
                    continue;

                bool hasValidParameters = true;
                for (uint i = 0; i < parameters.Length; ++i)
                {
                    if (Marshal.PtrToStringAnsi(il2cpp_type_get_name(il2cpp_method_get_param(element, i))) != parameters[i])
                    {
                        hasValidParameters = false;
                        break;
                    }
                }

                if (hasValidParameters)
                {
                    MelonDebug.Msg($" > 0x{(long)element:X}");
                    return element;
                }
            }

            Il2CppUnityTls_Module.Logger.Error($"Unable to find method {returntype} {name}({string.Join(", ", parameters)})");
            return IntPtr.Zero;
        }

        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr il2cpp_domain_get();
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr* il2cpp_domain_get_assemblies(IntPtr domain, ref uint size);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr il2cpp_assembly_get_image(IntPtr assembly);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr il2cpp_image_get_filename(IntPtr image);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr il2cpp_class_get_name(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr il2cpp_class_get_namespace(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr il2cpp_class_get_type(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr il2cpp_class_from_name(IntPtr image, string namespaze, string name);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr il2cpp_class_get_methods(IntPtr klass, ref IntPtr iter);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr il2cpp_method_get_name(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr il2cpp_type_get_name(IntPtr type);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr il2cpp_method_get_return_type(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern uint il2cpp_method_get_param_count(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr il2cpp_method_get_param(IntPtr method, uint index);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern unsafe IntPtr il2cpp_runtime_invoke(IntPtr method, IntPtr obj, void** param, ref IntPtr exc);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern IntPtr il2cpp_object_unbox(IntPtr obj);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void il2cpp_format_exception(IntPtr ex, void* message, int message_size);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        internal static extern void il2cpp_format_stack_trace(IntPtr ex, void* output, int output_size);
    }
}
