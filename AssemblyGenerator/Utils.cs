using System.Runtime.InteropServices;

namespace MelonLoader.AssemblyGenerator
{
    internal static class Utils
    {
        [DllImport(@"MelonLoader\\Dependencies\\Bootstrap.dll", EntryPoint = "GetGameAssemblyPath", CallingConvention = CallingConvention.StdCall)]
        internal static extern string GetGameAssemblyPath();
        [DllImport(@"MelonLoader\\Dependencies\\Bootstrap.dll", EntryPoint = "GetUnityVersion", CallingConvention = CallingConvention.StdCall)]
        internal static extern string GetUnityVersion();
        [DllImport(@"MelonLoader\\Dependencies\\Bootstrap.dll", EntryPoint = "GetManagedDirectory", CallingConvention = CallingConvention.StdCall)]
        internal static extern string GetManagedDirectory();
        [DllImport(@"MelonLoader\\Dependencies\\Bootstrap.dll", EntryPoint = "GetConfigDirectory", CallingConvention = CallingConvention.StdCall)]
        internal static extern string GetConfigDirectory();
        [DllImport(@"MelonLoader\\Dependencies\\Bootstrap.dll", EntryPoint = "GetAssemblyGeneratorPath", CallingConvention = CallingConvention.StdCall)]
        internal extern static string GetAssemblyGeneratorPath();
    }
}
