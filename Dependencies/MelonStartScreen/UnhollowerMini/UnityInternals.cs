using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace UnhollowerMini
{
    internal static unsafe class UnityInternals
    {
        private delegate void delegate_gfunc_mono_assembly_foreach(IntPtr assembly, IntPtr user_data);

        private static readonly IntPtr domain;
        private static readonly List<InternalAssembly> assemblies = new List<InternalAssembly>();

        private static readonly uint monoClassOffset = 0;

        unsafe static UnityInternals()
        {
            if (MelonUtils.IsGameIl2Cpp())
            {
                domain = il2cpp_domain_get();

                uint assemblyCount = 0;
                IntPtr* assemblyArray = il2cpp_domain_get_assemblies(domain, ref assemblyCount);
                for (int i = 0; i < assemblyCount; ++i)
                    assemblies.Add(new InternalAssembly(il2cpp_assembly_get_image(*(assemblyArray + i))));
            }
            else
            {
                domain = mono_domain_get();

                MonoClass* testclass = (MonoClass*)Marshal.AllocHGlobal(sizeof(MonoClass));
                testclass->applyZeroes();
                testclass->nested_in_0x04 = (IntPtr)0x1234;
                testclass->nested_in_0x08 = (IntPtr)0x5678;
                testclass->nested_in_0x0C = (IntPtr)0x9012;
                long returnedName = (long)mono_class_get_name((IntPtr)testclass);
                MelonDebug.Msg($"returnedName {returnedName:X}");
                Marshal.FreeHGlobal((IntPtr)testclass);
                if (returnedName == 0x1234)
                    monoClassOffset = 0;
                else if (returnedName == 0x5678)
                    monoClassOffset = (uint)IntPtr.Size * 1;
                else if (returnedName == 0x9012)
                    monoClassOffset = (uint)IntPtr.Size * 2;
                else
                    throw new Exception("Failed to find MonoClass name offset");

                MelonDebug.Msg("monoClassOffset? "  + monoClassOffset);
            }
        }

        private class InternalAssembly
        {
            public IntPtr ptr;
            public string name;

            public InternalAssembly(IntPtr ptr)
            {
                this.ptr = ptr;
                if (MelonUtils.IsGameIl2Cpp())
                {
                    name = Marshal.PtrToStringAnsi(il2cpp_image_get_filename(this.ptr));
                }
                else
                {
                    name = Marshal.PtrToStringAnsi(mono_image_get_filename(this.ptr));
                }
            }
        }

        private class InternalClass
        {
            public IntPtr ptr;
            public string name;
            public string name_space;

            public InternalClass(IntPtr ptr)
            {
                this.ptr = ptr;
                if (MelonUtils.IsGameIl2Cpp())
                {
                    name = Marshal.PtrToStringAnsi(il2cpp_class_get_name(ptr));
                    name_space = Marshal.PtrToStringAnsi(il2cpp_class_get_namespace(ptr));
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            public InternalClass(IntPtr ptr, string name, string name_space)
            {
                if (MelonUtils.IsGameIl2Cpp())
                {
                    throw new NotImplementedException();
                }
                else
                {
                    this.ptr = ptr;
                    this.name = name;
                    this.name_space = name_space;
                }
            }
        }


        internal static IntPtr GetClass(string assemblyname, string name_space, string classname)
        {
            MelonDebug.Msg($"GetClass {assemblyname} {name_space} {classname}");
            if (MelonUtils.IsGameIl2Cpp())
            {
                InternalAssembly assembly = assemblies.FirstOrDefault(a => a.name == assemblyname);
                if (assembly == null)
                {
                    throw new Exception("Unable to find assembly " + assemblyname + " in il2cpp domain");
                }

                IntPtr clazz = il2cpp_class_from_name(assembly.ptr, name_space, classname);
                if (clazz == IntPtr.Zero)
                {
                    throw new Exception("Unable to find class " + name_space + "." + classname + " in assembly " + assemblyname);
                }

                MelonDebug.Msg($" > 0x{(long)clazz:X}");
                return clazz;
            }
            else
            {
                string fullname = string.IsNullOrEmpty(name_space) ? "" : (name_space + ".") + classname;

                Assembly ass = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name + ".dll" == assemblyname);
                if (ass == null)
                {
                    throw new Exception("Unable to find assembly " + assemblyname + " in mono domain");
                }

                Type t = ass.GetType(fullname);
                if (t == null)
                {
                    throw new Exception("Unable to find class " + fullname + " in assembly " + assemblyname);
                }
                MelonDebug.Msg($" > 0x{(long)(*(IntPtr*)t.TypeHandle.Value):X}");

                return *(IntPtr*)t.TypeHandle.Value;
            }
        }

        public static IntPtr GetField(IntPtr clazz, string fieldName)
        {
            MelonDebug.Msg($"GetField {fieldName}");
            if (clazz == IntPtr.Zero)
                return IntPtr.Zero;

            var field = MelonUtils.IsGameIl2Cpp() ? il2cpp_class_get_field_from_name(clazz, fieldName) : mono_class_get_field_from_name(clazz, fieldName);
            if (field == IntPtr.Zero)
                throw new Exception($"Field {fieldName} was not found on class {Marshal.PtrToStringAnsi(MelonUtils.IsGameIl2Cpp() ? il2cpp_class_get_name(clazz) : mono_class_get_name(clazz))}");
            MelonDebug.Msg($" > 0x{(long)field:X}");
            return field;
        }

        internal static IntPtr GetMethod(IntPtr clazz, string name, string returntype, params string[] parameters)
        {
            MelonDebug.Msg($"GetMethod {returntype} {name}({string.Join(", ", parameters)})");
            if (MelonUtils.IsGameIl2Cpp())
            {
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
            }
            else
            {
                
                IntPtr iter = IntPtr.Zero;
                IntPtr element;
                while ((element = mono_class_get_methods(clazz, ref iter)) != IntPtr.Zero)
                {
                    if (Marshal.PtrToStringAnsi(mono_method_get_name(element)) != name)
                        continue;

                    IntPtr sig = mono_method_get_signature(element, IntPtr.Zero, 0);
                    if (Marshal.PtrToStringAnsi(mono_type_get_name(mono_signature_get_return_type(sig))) != returntype)
                        continue;
                    if (parameters.Length != mono_signature_get_param_count(sig))
                        continue;

                    bool hasValidParameters = true;
                    IntPtr iter2 = IntPtr.Zero;
                    IntPtr param;
                    int i = 0;
                    while ((param = mono_signature_get_params(sig, ref iter2)) != IntPtr.Zero)
                    {
                        if (Marshal.PtrToStringAnsi(mono_type_get_name(param)) != parameters[i])
                        {
                            hasValidParameters = false;
                            break;
                        }
                        ++i;
                    }

                    if (hasValidParameters)
                    {
                        MelonDebug.Msg($" > 0x{(long)element:X}");
                        return element;
                    }
                }
            }
            //MelonLogger.Error($"Unable to find method {returntype} {name}({string.Join(", ", parameters)})");
            //return IntPtr.Zero;
            throw new Exception($"Unable to find method {returntype} {name}({string.Join(", ", parameters)})");
        }

        public static IntPtr ObjectBaseToPtr(InternalObjectBase obj)
        {
            return obj?.Pointer ?? IntPtr.Zero;
        }

        public static IntPtr ObjectBaseToPtrNotNull(InternalObjectBase obj)
        {
            return obj?.Pointer ?? throw new NullReferenceException();
        }

        public static IntPtr ManagedStringToInternal(string str)
        {
            if (str == null) return IntPtr.Zero;

            fixed (char* chars = str)
                return MelonUtils.IsGameIl2Cpp() ? il2cpp_string_new_utf16(chars, str.Length) : mono_string_new_utf16(domain, chars, str.Length);
        }

        public static IntPtr ResolveICall(string signature)
        {
            MelonDebug.Msg("Resolving ICall " + signature);
            IntPtr icallPtr;
            if (MelonUtils.IsGameIl2Cpp())
                icallPtr = il2cpp_resolve_icall(signature);
            else
            {
                // We generate a fake MonoMethod + MonoMethodSignature + MonoClass struct to exploit the lookup code and force resolve our icall without the class/method being registered
                // (Slaynash: Yes this is illegal)
                MonoMethod* monoMethod = IcallToFakeMonoMethod(signature);
                icallPtr = mono_lookup_internal_call((IntPtr)monoMethod);
                DestroyFakeMonoMethod(monoMethod);
            }

            if (icallPtr == IntPtr.Zero)
            {
                //MelonLogger.Error($"ICall {signature} not resolved");
                //return IntPtr.Zero;
                throw new Exception($"ICall {signature} not resolved");
            }
            MelonDebug.Msg($" > 0x{(long)icallPtr:X}");

            return icallPtr;
        }

        public static T ResolveICall<T>(string signature) where T : Delegate
        {
            IntPtr icallPtr = ResolveICall(signature);
            return icallPtr == IntPtr.Zero ? null : (T)Marshal.GetDelegateForFunctionPointer(icallPtr, typeof(T));
        }

        private static unsafe MonoMethod* IcallToFakeMonoMethod(string icallName)
        {
            string[] typeAndMethod = icallName.Split(new[] { "::" }, StringSplitOptions.None);
            int parenthesisIndex = typeAndMethod[1].IndexOf('(');
            if (parenthesisIndex >= 0)
                typeAndMethod[1] = typeAndMethod[1].Substring(0, parenthesisIndex);
            // We add a padding to the end of each allocated memory since our structs are supposed to be bigger than the one we have here
            MonoMethod* monoMethod = (MonoMethod*)Marshal.AllocHGlobal(sizeof(MonoMethod) + 0x100);
            monoMethod->applyZeroes();
            monoMethod->klass = (MonoClass*)Marshal.AllocHGlobal(sizeof(MonoClass) + 0x100);
            monoMethod->klass->applyZeroes();
            monoMethod->name = (byte*)Marshal.StringToHGlobalAnsi(typeAndMethod[1]);
            int lastDotIndex = typeAndMethod[0].LastIndexOf('.');
            if (lastDotIndex < 0)
            {
                *(IntPtr*)((long)&monoMethod->klass->nested_in_0x08 + monoClassOffset) = Marshal.StringToHGlobalAnsi("");
                *(IntPtr*)((long)&monoMethod->klass->nested_in_0x04 + monoClassOffset) = Marshal.StringToHGlobalAnsi(typeAndMethod[0]);
            }
            else
            {
                string name_space = typeAndMethod[0].Substring(0, lastDotIndex);
                string name = typeAndMethod[0].Substring(lastDotIndex + 1);
                *(IntPtr*)((long)&monoMethod->klass->nested_in_0x08 + monoClassOffset) = Marshal.StringToHGlobalAnsi(name_space);
                *(IntPtr*)((long)&monoMethod->klass->nested_in_0x04 + monoClassOffset) = Marshal.StringToHGlobalAnsi(name);
            }

            MonoMethodSignature* monoMethodSignature = (MonoMethodSignature*)Marshal.AllocHGlobal(sizeof(MonoMethodSignature));
            monoMethodSignature->ApplyZeroes();
            monoMethod->signature = monoMethodSignature;

            return monoMethod;
        }

        private static unsafe void DestroyFakeMonoMethod(MonoMethod* monoMethod)
        {
            Marshal.FreeHGlobal((IntPtr)monoMethod->signature);
            Marshal.FreeHGlobal(*(IntPtr*)((long)&monoMethod->klass->nested_in_0x04 + monoClassOffset));
            Marshal.FreeHGlobal(*(IntPtr*)((long)&monoMethod->klass->nested_in_0x08 + monoClassOffset));
            Marshal.FreeHGlobal((IntPtr)monoMethod->klass);
            Marshal.FreeHGlobal((IntPtr)monoMethod->name);
            Marshal.FreeHGlobal((IntPtr)monoMethod);
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MonoMethod
        {
            public ushort flags;  /* method flags */
            public ushort iflags; /* method implementation flags */
            public uint token;
            public MonoClass* klass; /* To what class does this method belong */
            public MonoMethodSignature* signature;
            /* name is useful mostly for debugging */
            public byte* name;

            public IntPtr method_pointer;
            public IntPtr invoke_pointer;

            /* this is used by the inlining algorithm */
            public ushort bitfield;
            public int slot;

            internal void applyZeroes()
            {
                flags = 0;
                iflags = 0;
                token = 0;
                klass = (MonoClass*)0;
                signature = (MonoMethodSignature*)0;
                name = (byte*)0;
                method_pointer = IntPtr.Zero;
                invoke_pointer = IntPtr.Zero;
                bitfield = 0;
                slot = 0;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MonoMethodSignature
        {
            public IntPtr ret;
            public ushort param_cout;
            // ...

            internal void ApplyZeroes()
            {
                ret = (IntPtr)0;
                param_cout = 0;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MonoClass
        {
            /* element class for arrays and enum basetype for enums */
            public MonoClass* element_class;
            /* used for subtype checks */
            public MonoClass* cast_class;

            /* for fast subtype checks */
            public MonoClass** supertypes;
            public ushort idepth;

            /* array dimension */
            public byte rank; // 0x0E

            /* One of the values from MonoTypeKind */
            public byte class_kind;

            public int instance_size; // 0x10

            public uint bitfield1; // 0x14

            public byte min_align; // 0x18

            public uint bitfield2; // 0x1C

            byte exception_type;

            public MonoClass* parent; // 0x24
            public MonoClass* nested_in; // 0x28 // Should always be null

            //public IntPtr cattrs; // 0x2C
            
            //
            // Starting here (unity version from 2014+), fields will be offset by IntPtr.Size
            //

            public IntPtr nested_in_0x04; // 0x2C - 0x30
            public IntPtr nested_in_0x08; // 0x30 - 0x34
            public IntPtr nested_in_0x0C; // 0x34 - 0x38
            public IntPtr nested_in_0x10; // 0x38 - 0x3C

            // ...

            internal void applyZeroes()
            {
                element_class = (MonoClass*)0;
                cast_class = (MonoClass*)0;
                supertypes = (MonoClass**)0;
                idepth = 0;
                rank = 0;
                class_kind = 0;
                instance_size = 0;
                bitfield1 = 0;
                min_align = 0;
                bitfield2 = 0;
                exception_type = 0;
                parent = (MonoClass*)0;
                nested_in = (MonoClass*)0;
                nested_in_0x04 = (IntPtr)0;
                nested_in_0x08 = (IntPtr)0;
                nested_in_0x0C = (IntPtr)0;
                nested_in_0x10 = (IntPtr)0;
            }
        }

        private struct MonoType
        {
            public IntPtr data;
            public short attrs;
            public byte type;
            public byte bitflags;

            internal void applyZeroes()
            {
                data = (IntPtr)0;
                attrs = 0;
                type = 0;
                bitflags = 0;
            }
        }

        public static IntPtr class_get_type(IntPtr klass)
        {
            return MelonUtils.IsGameIl2Cpp() ? il2cpp_class_get_type(klass) : mono_class_get_type(klass);
        }

        public static void runtime_class_init(IntPtr klass)
        {
            if (klass == IntPtr.Zero)
                throw new ArgumentException("Class to init is null");

            if (MelonUtils.IsGameIl2Cpp()) il2cpp_runtime_class_init(klass); else mono_runtime_class_init(klass);
        }

        public static IntPtr runtime_invoke(IntPtr method, IntPtr obj, void** param, ref IntPtr exc) =>
            MelonUtils.IsGameIl2Cpp() ? il2cpp_runtime_invoke(method, obj, param, ref exc) : mono_runtime_invoke(method, obj, param, ref exc);

        public static IntPtr array_new(IntPtr elementTypeInfo, ulong length) =>
            MelonUtils.IsGameIl2Cpp() ? il2cpp_array_new(elementTypeInfo, length) : mono_array_new(domain, elementTypeInfo, length);

        public static uint array_length(IntPtr array) =>
            MelonUtils.IsGameIl2Cpp() ? il2cpp_array_length(array) : *(uint*)((long)array + IntPtr.Size * 3);

        public static uint field_get_offset(IntPtr field) =>
            MelonUtils.IsGameIl2Cpp() ? il2cpp_field_get_offset(field) : mono_field_get_offset(field);

        public static IntPtr object_unbox(IntPtr obj) =>
            MelonUtils.IsGameIl2Cpp() ? il2cpp_object_unbox(obj) : mono_object_unbox(obj);

        public static IntPtr object_new(IntPtr klass) =>
            MelonUtils.IsGameIl2Cpp() ? il2cpp_object_new(klass) : mono_object_new(domain, klass);

        public static int class_value_size(IntPtr klass, ref uint align) =>
            MelonUtils.IsGameIl2Cpp() ? il2cpp_class_value_size(klass, ref align) : mono_class_value_size(klass, ref align);

        public static uint gchandle_new(IntPtr obj, bool pinned) =>
            MelonUtils.IsGameIl2Cpp() ? il2cpp_gchandle_new(obj, pinned) : mono_gchandle_new(obj, pinned ? 1 : 0);
        public static void gchandle_free(uint gchandle)
        { if (MelonUtils.IsGameIl2Cpp()) il2cpp_gchandle_free(gchandle); else mono_gchandle_free(gchandle); }
        public static IntPtr gchandle_get_target(uint gchandle) =>
            MelonUtils.IsGameIl2Cpp() ? il2cpp_gchandle_get_target(gchandle) : mono_gchandle_get_target(gchandle);

        public static IntPtr value_box(IntPtr klass, IntPtr val) =>
            MelonUtils.IsGameIl2Cpp() ? il2cpp_value_box(klass, val) : mono_value_box(domain, klass, val);

        public static void format_exception(IntPtr ex, void* message, int message_size)
        {
            if (MelonUtils.IsGameIl2Cpp())
                il2cpp_format_exception(ex, message, message_size);
            // TODO Mono mono_format_exception
        }

        public static void format_stack_trace(IntPtr ex, void* output, int output_size)
        {
            if (MelonUtils.IsGameIl2Cpp())
                il2cpp_format_stack_trace(ex, output, output_size);
            // TODO mono_format_stack_trace
        }


        // Mono Functions
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_domain_get();

        /*
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr* mono_domain_get_assemblies(IntPtr domain, ref uint size);
        */
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void mono_assembly_foreach(delegate_gfunc_mono_assembly_foreach func, IntPtr user_data);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_assembly_get_image(IntPtr assembly);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_image_get_filename(IntPtr image);
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern uint mono_image_get_class_count(IntPtr image);
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_image_get_class(IntPtr image, uint index);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_class_get_name(IntPtr klass);
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_class_get_namespace(IntPtr klass);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_lookup_internal_call(IntPtr method);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr mono_class_get_type(IntPtr klass);


        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern unsafe IntPtr mono_runtime_invoke(IntPtr method, IntPtr obj, void** param, ref IntPtr exc);
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void mono_runtime_class_init(IntPtr klass);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_array_new(IntPtr domain, IntPtr eclass, ulong n);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern uint mono_field_get_offset(IntPtr field);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_object_unbox(IntPtr obj);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_object_new(IntPtr domain, IntPtr klass);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int mono_class_value_size(IntPtr klass, ref uint align);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern uint mono_gchandle_new(IntPtr obj, int pinned);
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void mono_gchandle_free(uint gchandle);
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_gchandle_get_target(uint gchandle);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_class_get_field_from_name(IntPtr klass, [MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_value_box(IntPtr domain, IntPtr klass, IntPtr data);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_class_get_methods(IntPtr klass, ref IntPtr iter);
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr mono_method_get_name(IntPtr method);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_type_get_name(IntPtr type);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_image_get_table_info(IntPtr image, int table_id);
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int mono_table_info_get_rows(IntPtr table);
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void mono_metadata_decode_row(IntPtr t, int idx, uint[] res, int res_size);
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_metadata_string_heap(IntPtr meta, uint index);
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_class_from_name(IntPtr image, string name_space, string name);
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_domain_try_type_resolve(IntPtr domain, string name, IntPtr typebuilder_raw);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_method_get_signature(IntPtr method, IntPtr image, uint token);
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_signature_get_return_type(IntPtr sig);
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern uint mono_signature_get_param_count(IntPtr sig);
        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_signature_get_params(IntPtr sig, ref IntPtr iter);

        [DllImport("__Internal", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr mono_string_new_utf16(IntPtr domain, char* text, int len);




        // IL2CPP Functions
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr il2cpp_domain_get();

        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr il2cpp_resolve_icall([MarshalAs(UnmanagedType.LPStr)] string name);

        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern uint il2cpp_array_length(IntPtr array);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr il2cpp_array_new(IntPtr elementTypeInfo, ulong length);
        
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr il2cpp_assembly_get_image(IntPtr assembly);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr il2cpp_class_get_field_from_name(IntPtr klass, [MarshalAs(UnmanagedType.LPStr)] string name);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr il2cpp_class_get_methods(IntPtr klass, ref IntPtr iter);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr il2cpp_class_get_name(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr il2cpp_class_get_namespace(IntPtr klass);



        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr il2cpp_class_get_type(IntPtr klass);


        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern int il2cpp_class_value_size(IntPtr klass, ref uint align);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr* il2cpp_domain_get_assemblies(IntPtr domain, ref uint size);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void il2cpp_format_exception(IntPtr ex, void* message, int message_size);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void il2cpp_format_stack_trace(IntPtr ex, void* output, int output_size);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern uint il2cpp_field_get_offset(IntPtr field);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern uint il2cpp_gchandle_new(IntPtr obj, bool pinned);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr il2cpp_gchandle_get_target(uint gchandle);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void il2cpp_gchandle_free(uint gchandle);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr il2cpp_method_get_return_type(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern uint il2cpp_method_get_param_count(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr il2cpp_method_get_param(IntPtr method, uint index);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr il2cpp_method_get_name(IntPtr method);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr il2cpp_object_new(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr il2cpp_object_unbox(IntPtr obj);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr il2cpp_value_box(IntPtr klass, IntPtr data);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern unsafe IntPtr il2cpp_runtime_invoke(IntPtr method, IntPtr obj, void** param, ref IntPtr exc);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void il2cpp_runtime_class_init(IntPtr klass);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr il2cpp_string_new_utf16(char* text, int len);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr il2cpp_type_get_name(IntPtr type);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr il2cpp_image_get_filename(IntPtr image);
        [DllImport("GameAssembly", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern IntPtr il2cpp_class_from_name(IntPtr image, string namespaze, string name);
    }
}
