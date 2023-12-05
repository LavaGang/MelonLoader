using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MelonLoader.Utils
{
    public static class MelonExtensions
    {
        #region Assembly

        public static IEnumerable<Type> GetValidTypes(this Assembly asm)
            => GetValidTypes(asm, null);

        public static IEnumerable<Type> GetValidTypes(this Assembly asm, Func<Type, bool> predicate)
        {
            IEnumerable<Type> returnval = Enumerable.Empty<Type>();
            try { returnval = asm.GetTypes().AsEnumerable(); }
            catch (ReflectionTypeLoadException ex)
            {
                MelonLogger.Error($"Failed to load all types in assembly {asm.FullName} due to: {ex.Message}", ex);
                returnval = ex.Types;
            }
            return returnval.Where(x => (x != null) && ((predicate == null) || predicate(x)));
        }

        #endregion

        #region Delegate

        public static IntPtr GetFunctionPointer(this Delegate del)
            => Marshal.GetFunctionPointerForDelegate(del);

        #endregion

        #region IntPtr

        public static void GetDelegate<T>(this IntPtr ptr, out T output) where T : Delegate
            => output = GetDelegate<T>(ptr);
        public static T GetDelegate<T>(this IntPtr ptr) where T : Delegate
            => GetDelegate(ptr, typeof(T)) as T;
        public static Delegate GetDelegate(this IntPtr ptr, Type type)
        {
            if (ptr == IntPtr.Zero)
                throw new ArgumentNullException(nameof(ptr));
            Delegate del = Marshal.GetDelegateForFunctionPointer(ptr, type);
            if (del == null)
                throw new Exception($"Unable to Get Delegate of Type {type.FullName} for Function Pointer!");
            return del;
        }

        public static string ToAnsiString(this IntPtr ptr)
            => Marshal.PtrToStringAnsi(ptr);

        #endregion

        #region String

        public static IntPtr ToAnsiPointer(this string str)
            => Marshal.StringToHGlobalAnsi(str);

        #endregion
    }
}
