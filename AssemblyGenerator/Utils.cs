using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MelonLoader.AssemblyGenerator
{
    internal static class Utils
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal extern static string GetConfigDirectory();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal extern static string GetAssemblyGeneratorPath();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal extern static string GetGameAssemblyPath();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void SetProcessId(int id);
    }
}