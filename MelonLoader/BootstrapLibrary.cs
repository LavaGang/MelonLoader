using MelonLoader.Bootstrap;

namespace MelonLoader;

internal class BootstrapLibrary
{
    internal NativeHookFn NativeHookAttach { get; private set; }
    internal NativeHookFn NativeHookDetach { get; private set; }
    internal LogMsgFn LogMsg { get; private set; }
    internal LogErrorFn LogError { get; private set; }
    internal LogMelonInfoFn LogMelonInfo { get; private set; }
    internal ActionFn MonoInstallHooks { get; private set; }
    internal PtrRetFn MonoGetDomainPtr { get; private set; }
    internal PtrRetFn MonoGetRuntimeHandle { get; private set; }
}
