using System.Runtime.InteropServices;

namespace MelonLoader.NativeHost;

[StructLayout(LayoutKind.Sequential)]
internal struct FunctionExchange
{
    internal nint HookAttach;
    internal nint HookDetach;
    internal nint LogMsg;
    internal nint LogError;
    internal nint LogMelonInfo;

    internal nint Start;
}