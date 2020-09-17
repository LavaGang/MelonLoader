using System.Runtime.InteropServices;

namespace MelonLoader.AssemblyGenerator
{
    internal static class Utils
    {
        [DllImport(@"MelonLoader\\Dependencies\\Bootstrap.dll", EntryPoint = "GetGameAssemblyPath", CallingConvention = CallingConvention.StdCall)]
        internal extern static string GetGameAssemblyPath();
        [DllImport(@"MelonLoader\\Dependencies\\Bootstrap.dll", EntryPoint = "GetUnityVersion", CallingConvention = CallingConvention.StdCall)]
        internal extern static string GetUnityVersion();
        [DllImport(@"MelonLoader\\Dependencies\\Bootstrap.dll", EntryPoint = "GetManagedDirectory", CallingConvention = CallingConvention.StdCall)]
        internal extern static string GetManagedDirectory();
        [DllImport(@"MelonLoader\\Dependencies\\Bootstrap.dll", EntryPoint = "GetConfigDirectory", CallingConvention = CallingConvention.StdCall)]
        internal extern static string GetConfigDirectory();
        [DllImport(@"MelonLoader\\Dependencies\\Bootstrap.dll", EntryPoint = "GetAssemblyGeneratorPath", CallingConvention = CallingConvention.StdCall)]
        internal extern static string GetAssemblyGeneratorPath();
        [DllImport(@"MelonLoader\\Dependencies\\Bootstrap.dll", EntryPoint = "ForceRegeneration", CallingConvention = CallingConvention.StdCall)]
        internal extern static bool ForceRegeneration();
        [DllImport(@"MelonLoader\\Dependencies\\Bootstrap.dll", EntryPoint = "ForceVersion_UnityDependencies", CallingConvention = CallingConvention.StdCall)]
        internal extern static string ForceVersion_UnityDependencies();
        [DllImport(@"MelonLoader\\Dependencies\\Bootstrap.dll", EntryPoint = "ForceVersion_Il2CppDumper", CallingConvention = CallingConvention.StdCall)]
        internal extern static string ForceVersion_Il2CppDumper();
        [DllImport(@"MelonLoader\\Dependencies\\Bootstrap.dll", EntryPoint = "ForceVersion_Il2CppAssemblyUnhollower", CallingConvention = CallingConvention.StdCall)]
        internal extern static string ForceVersion_Il2CppAssemblyUnhollower();
        [DllImport(@"MelonLoader\\Dependencies\\Bootstrap.dll", EntryPoint = "SetProcessId", CallingConvention = CallingConvention.StdCall)]
        internal extern static void SetProcessId(int id);
    }
}