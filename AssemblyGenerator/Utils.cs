using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace MelonLoader.AssemblyGenerator
{
    internal static class Utils
    {
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal extern static string GetGameAssemblyPath();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal extern static string GetUnityVersion();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal extern static string GetGameName();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal extern static string GetManagedDirectory();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal extern static string GetConfigDirectory();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal extern static string GetAssemblyGeneratorPath();
        //[MethodImpl(MethodImplOptions.InternalCall)]
        //internal extern static bool ForceRegeneration();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal extern static string ForceVersion_UnityDependencies();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal extern static string ForceVersion_Dumper();
        [MethodImpl(MethodImplOptions.InternalCall)]
        [return: MarshalAs(UnmanagedType.LPStr)]
        internal extern static string ForceVersion_Il2CppAssemblyUnhollower();
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern static void SetProcessId(int id);
    }
}