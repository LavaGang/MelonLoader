using System.Runtime.InteropServices;

namespace MelonLoader.AssemblyGenerator
{
    internal static class Logger
    {
        [DllImport(@"MelonLoader\\Dependencies\\Bootstrap.dll", EntryPoint = "Msg", CallingConvention = CallingConvention.StdCall)]
        internal static extern void Msg(string txt);
        [DllImport(@"MelonLoader\\Dependencies\\Bootstrap.dll", EntryPoint = "Warning", CallingConvention = CallingConvention.StdCall)]
        internal static extern void Warning(string txt);
        [DllImport(@"MelonLoader\\Dependencies\\Bootstrap.dll", EntryPoint = "Error", CallingConvention = CallingConvention.StdCall)]
        internal static extern void Error(string txt);
    }
}