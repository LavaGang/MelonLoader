using System.Runtime.InteropServices;

namespace MelonLoader.AssemblyGenerator
{
    internal static class Utils
    {
        [DllImport("MelonLoader\\Dependencies\\Bootstrap.dll", CallingConvention = CallingConvention.StdCall)]
        internal extern static string GetGameAssemblyPath();
        [DllImport("MelonLoader\\Dependencies\\Bootstrap.dll", CallingConvention = CallingConvention.StdCall)]
        internal extern static string GetUnityVersion();
        [DllImport("MelonLoader\\Dependencies\\Bootstrap.dll", CallingConvention = CallingConvention.StdCall)]
        internal extern static string GetManagedDirectory();
        [DllImport("MelonLoader\\Dependencies\\Bootstrap.dll", CallingConvention = CallingConvention.StdCall)]
        internal extern static string GetConfigDirectory();
        [DllImport("MelonLoader\\Dependencies\\Bootstrap.dll", CallingConvention = CallingConvention.StdCall)]
        internal extern static string GetAssemblyGeneratorPath();
        [DllImport("MelonLoader\\Dependencies\\Bootstrap.dll", CallingConvention = CallingConvention.StdCall)]
        internal extern static string ForceVersion_UnityDependencies();
        [DllImport("MelonLoader\\Dependencies\\Bootstrap.dll", CallingConvention = CallingConvention.StdCall)]
        internal extern static string ForceVersion_Cpp2IL();
        [DllImport("MelonLoader\\Dependencies\\Bootstrap.dll", CallingConvention = CallingConvention.StdCall)]
        internal extern static string ForceVersion_Il2CppAssemblyUnhollower();
        [DllImport("MelonLoader\\Dependencies\\Bootstrap.dll")]
        internal extern static void SetProcessId(int id);
    }
}