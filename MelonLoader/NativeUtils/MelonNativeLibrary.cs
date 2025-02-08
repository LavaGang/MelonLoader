using MelonLoader.InternalUtils;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MelonLoader
{
    public class MelonNativeLibrary
    {
        public readonly IntPtr Ptr;
        public MelonNativeLibrary(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(ptr));
            Ptr = ptr;
        }
        
        public static IntPtr LoadLib(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
                throw new ArgumentNullException(nameof(filepath));
            IntPtr ptr = BootstrapInterop.NativeLoadLib(filepath);
            if (ptr == IntPtr.Zero)
                throw new Exception($"Unable to Load Native Library {filepath}!");
            return ptr;
        }

        public static bool TryLoadLib(string filepath, out IntPtr result)
        {
            bool wasSuccessful = false;
            try
            {
                result = LoadLib(filepath);
                wasSuccessful = result != IntPtr.Zero;
            }
            catch { result = IntPtr.Zero; }
            return wasSuccessful;
        }

        public IntPtr GetExport(string name)
            => GetExport(Ptr, name);
        public Delegate GetExport(Type type, string name)
            => GetExport(Ptr, name).GetDelegate(type);
        public T GetExport<T>(string name) where T : Delegate
            => GetExport(Ptr, name).GetDelegate<T>();
        public void GetExport<T>(string name, out T output) where T : Delegate
            => output = GetExport(Ptr, name).GetDelegate<T>();
        public static IntPtr GetExport(IntPtr nativeLib, string name)
        {
            if (nativeLib == IntPtr.Zero)
                throw new ArgumentNullException(nameof(nativeLib));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            IntPtr returnval = BootstrapInterop.NativeGetExport(nativeLib, name);
            if (returnval == IntPtr.Zero)
                throw new Exception($"Unable to Find Native Library Export {name}!");

            return returnval;
        }

        public static bool TryGetExport(IntPtr handle, string name, out IntPtr result)
        {
            bool wasSuccessful = false;
            try
            {
                result = GetExport(handle, name);
                wasSuccessful = result != IntPtr.Zero;
            }
            catch { result = IntPtr.Zero; }
            return wasSuccessful;
        }
        
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal delegate string StringDelegate();
    }

    public class MelonNativeLibrary<T> : MelonNativeLibrary
    {
        public readonly T Instance;

        public static MelonNativeLibrary<T> Load(string libPath, bool throwOnError = true)
        {
            IntPtr libPtr = LoadLib(libPath);
            if (libPtr == IntPtr.Zero)
                throw new Exception($"Failed to load {libPath}");
            
            return new(libPtr, throwOnError);
        }

        public MelonNativeLibrary(IntPtr ptr, bool throwOnError = true) : base(ptr)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(ptr));

            Type specifiedType = typeof(T);
            if (specifiedType.IsAbstract && specifiedType.IsSealed)
                throw new Exception($"Specified Type {specifiedType.FullName} must be Non-Static!");

            Instance = (T)Activator.CreateInstance(specifiedType);

            foreach (var fieldInfo in specifiedType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (fieldInfo.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length != 0)
                    continue;

                var fieldType = fieldInfo.FieldType;
                MelonNativeLibraryImportAttribute[] nativeImportAtt = (MelonNativeLibraryImportAttribute[])fieldType.GetCustomAttributes(typeof(MelonNativeLibraryImportAttribute), false);
                if (fieldType.GetCustomAttributes(typeof(UnmanagedFunctionPointerAttribute), false).Length == 0)
                    continue;

                string exportName = fieldInfo.Name;
                if ((nativeImportAtt != null)
                    && (nativeImportAtt.Length > 0))
                    exportName = nativeImportAtt[0].Name;

                Delegate export = null;
                try
                {
                    export = GetExport(fieldType, exportName);
                }
                catch
                {
                    if (throwOnError)
                        throw;
                    continue;
                }

                fieldInfo.SetValue(Instance, export);
            }

            foreach (var propertyInfo in specifiedType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (propertyInfo.GetCustomAttributes(typeof(CompilerGeneratedAttribute), false).Length != 0)
                    continue;

                var fieldType = propertyInfo.PropertyType;
                MelonNativeLibraryImportAttribute[] nativeImportAtt = (MelonNativeLibraryImportAttribute[])fieldType.GetCustomAttributes(typeof(MelonNativeLibraryImportAttribute), false);
                if (fieldType.GetCustomAttributes(typeof(UnmanagedFunctionPointerAttribute), false).Length == 0)
                    continue;

                string exportName = propertyInfo.Name;
                if ((nativeImportAtt != null)
                    && (nativeImportAtt.Length > 0))
                    exportName = nativeImportAtt[0].Name;

                Delegate export = null;
                try
                {
                    export = GetExport(fieldType, exportName);
                }
                catch
                {
                    if (throwOnError)
                        throw;
                    continue;
                }

                propertyInfo.SetValue(Instance, export, null);
            }
        }
    }
}
