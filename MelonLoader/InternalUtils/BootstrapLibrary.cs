using MelonLoader.Bootstrap;

namespace MelonLoader.InternalUtils;

internal class BootstrapLibrary
{
    internal NativeHookFn NativeHookAttach { get; private set; }
    internal NativeHookFn NativeHookDetach { get; private set; }
    internal LogMsgFn LogMsg { get; private set; }
    internal LogErrorFn LogError { get; private set; }
    internal LogMelonInfoFn LogMelonInfo { get; private set; }
    internal BoolRetFn IsConsoleOpen { get; private set; }
    internal GetLoaderConfigFn GetLoaderConfig { get; private set; }
}
