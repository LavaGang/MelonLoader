using System;
using System.Runtime.InteropServices;

namespace MelonLoader.Utils
{
    public static class MelonExtensions
    {
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

        #endregion
    }
}
