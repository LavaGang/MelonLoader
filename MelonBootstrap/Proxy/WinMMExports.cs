using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MelonLoader.Bootstrap.Proxy;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Exported proxy names")]
internal static class WinMMExports
{
    [UnmanagedCallersOnly(EntryPoint = "ImplmciExecute")]
    public static void ImplmciExecute() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplCloseDriver")]
    public static void ImplCloseDriver() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplDefDriverProc")]
    public static void ImplDefDriverProc() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplDriverCallback")]
    public static void ImplDriverCallback() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplDrvGetModuleHandle")]
    public static void ImplDrvGetModuleHandle() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplGetDriverModuleHandle")]
    public static void ImplGetDriverModuleHandle() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplOpenDriver")]
    public static void ImplOpenDriver() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplPlaySound")]
    public static void ImplPlaySound() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplPlaySoundA")]
    public static void ImplPlaySoundA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplPlaySoundW")]
    public static void ImplPlaySoundW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplSendDriverMessage")]
    public static void ImplSendDriverMessage() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplWOWAppExit")]
    public static void ImplWOWAppExit() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplauxGetDevCapsA")]
    public static void ImplauxGetDevCapsA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplauxGetDevCapsW")]
    public static void ImplauxGetDevCapsW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplauxGetNumDevs")]
    public static void ImplauxGetNumDevs() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplauxGetVolume")]
    public static void ImplauxGetVolume() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplauxOutMessage")]
    public static void ImplauxOutMessage() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplauxSetVolume")]
    public static void ImplauxSetVolume() { }

    [UnmanagedCallersOnly(EntryPoint = "ImpljoyConfigChanged")]
    public static void ImpljoyConfigChanged() { }

    [UnmanagedCallersOnly(EntryPoint = "ImpljoyGetDevCapsA")]
    public static void ImpljoyGetDevCapsA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImpljoyGetDevCapsW")]
    public static void ImpljoyGetDevCapsW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImpljoyGetNumDevs")]
    public static void ImpljoyGetNumDevs() { }

    [UnmanagedCallersOnly(EntryPoint = "ImpljoyGetPos")]
    public static void ImpljoyGetPos() { }

    [UnmanagedCallersOnly(EntryPoint = "ImpljoyGetPosEx")]
    public static void ImpljoyGetPosEx() { }

    [UnmanagedCallersOnly(EntryPoint = "ImpljoyGetThreshold")]
    public static void ImpljoyGetThreshold() { }

    [UnmanagedCallersOnly(EntryPoint = "ImpljoyReleaseCapture")]
    public static void ImpljoyReleaseCapture() { }

    [UnmanagedCallersOnly(EntryPoint = "ImpljoySetCapture")]
    public static void ImpljoySetCapture() { }

    [UnmanagedCallersOnly(EntryPoint = "ImpljoySetThreshold")]
    public static void ImpljoySetThreshold() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmciDriverNotify")]
    public static void ImplmciDriverNotify() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmciDriverYield")]
    public static void ImplmciDriverYield() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmciFreeCommandResource")]
    public static void ImplmciFreeCommandResource() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmciGetCreatorTask")]
    public static void ImplmciGetCreatorTask() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmciGetDeviceIDA")]
    public static void ImplmciGetDeviceIDA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmciGetDeviceIDFromElementIDA")]
    public static void ImplmciGetDeviceIDFromElementIDA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmciGetDeviceIDFromElementIDW")]
    public static void ImplmciGetDeviceIDFromElementIDW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmciGetDeviceIDW")]
    public static void ImplmciGetDeviceIDW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmciGetDriverData")]
    public static void ImplmciGetDriverData() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmciGetErrorStringA")]
    public static void ImplmciGetErrorStringA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmciGetErrorStringW")]
    public static void ImplmciGetErrorStringW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmciGetYieldProc")]
    public static void ImplmciGetYieldProc() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmciLoadCommandResource")]
    public static void ImplmciLoadCommandResource() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmciSendCommandA")]
    public static void ImplmciSendCommandA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmciSendCommandW")]
    public static void ImplmciSendCommandW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmciSendStringA")]
    public static void ImplmciSendStringA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmciSendStringW")]
    public static void ImplmciSendStringW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmciSetDriverData")]
    public static void ImplmciSetDriverData() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmciSetYieldProc")]
    public static void ImplmciSetYieldProc() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiConnect")]
    public static void ImplmidiConnect() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiDisconnect")]
    public static void ImplmidiDisconnect() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiInAddBuffer")]
    public static void ImplmidiInAddBuffer() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiInClose")]
    public static void ImplmidiInClose() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiInGetDevCapsA")]
    public static void ImplmidiInGetDevCapsA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiInGetDevCapsW")]
    public static void ImplmidiInGetDevCapsW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiInGetErrorTextA")]
    public static void ImplmidiInGetErrorTextA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiInGetErrorTextW")]
    public static void ImplmidiInGetErrorTextW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiInGetID")]
    public static void ImplmidiInGetID() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiInGetNumDevs")]
    public static void ImplmidiInGetNumDevs() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiInMessage")]
    public static void ImplmidiInMessage() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiInOpen")]
    public static void ImplmidiInOpen() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiInPrepareHeader")]
    public static void ImplmidiInPrepareHeader() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiInReset")]
    public static void ImplmidiInReset() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiInStart")]
    public static void ImplmidiInStart() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiInStop")]
    public static void ImplmidiInStop() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiInUnprepareHeader")]
    public static void ImplmidiInUnprepareHeader() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiOutCacheDrumPatches")]
    public static void ImplmidiOutCacheDrumPatches() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiOutCachePatches")]
    public static void ImplmidiOutCachePatches() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiOutClose")]
    public static void ImplmidiOutClose() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiOutGetDevCapsA")]
    public static void ImplmidiOutGetDevCapsA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiOutGetDevCapsW")]
    public static void ImplmidiOutGetDevCapsW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiOutGetErrorTextA")]
    public static void ImplmidiOutGetErrorTextA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiOutGetErrorTextW")]
    public static void ImplmidiOutGetErrorTextW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiOutGetID")]
    public static void ImplmidiOutGetID() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiOutGetNumDevs")]
    public static void ImplmidiOutGetNumDevs() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiOutGetVolume")]
    public static void ImplmidiOutGetVolume() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiOutLongMsg")]
    public static void ImplmidiOutLongMsg() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiOutMessage")]
    public static void ImplmidiOutMessage() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiOutOpen")]
    public static void ImplmidiOutOpen() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiOutPrepareHeader")]
    public static void ImplmidiOutPrepareHeader() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiOutReset")]
    public static void ImplmidiOutReset() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiOutSetVolume")]
    public static void ImplmidiOutSetVolume() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiOutShortMsg")]
    public static void ImplmidiOutShortMsg() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiOutUnprepareHeader")]
    public static void ImplmidiOutUnprepareHeader() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiStreamClose")]
    public static void ImplmidiStreamClose() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiStreamOpen")]
    public static void ImplmidiStreamOpen() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiStreamOut")]
    public static void ImplmidiStreamOut() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiStreamPause")]
    public static void ImplmidiStreamPause() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiStreamPosition")]
    public static void ImplmidiStreamPosition() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiStreamProperty")]
    public static void ImplmidiStreamProperty() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiStreamRestart")]
    public static void ImplmidiStreamRestart() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmidiStreamStop")]
    public static void ImplmidiStreamStop() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmixerClose")]
    public static void ImplmixerClose() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmixerGetControlDetailsA")]
    public static void ImplmixerGetControlDetailsA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmixerGetControlDetailsW")]
    public static void ImplmixerGetControlDetailsW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmixerGetDevCapsA")]
    public static void ImplmixerGetDevCapsA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmixerGetDevCapsW")]
    public static void ImplmixerGetDevCapsW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmixerGetID")]
    public static void ImplmixerGetID() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmixerGetLineControlsA")]
    public static void ImplmixerGetLineControlsA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmixerGetLineControlsW")]
    public static void ImplmixerGetLineControlsW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmixerGetLineInfoA")]
    public static void ImplmixerGetLineInfoA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmixerGetLineInfoW")]
    public static void ImplmixerGetLineInfoW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmixerGetNumDevs")]
    public static void ImplmixerGetNumDevs() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmixerMessage")]
    public static void ImplmixerMessage() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmixerOpen")]
    public static void ImplmixerOpen() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmixerSetControlDetails")]
    public static void ImplmixerSetControlDetails() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmDrvInstall")]
    public static void ImplmmDrvInstall() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmGetCurrentTask")]
    public static void ImplmmGetCurrentTask() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmTaskBlock")]
    public static void ImplmmTaskBlock() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmTaskCreate")]
    public static void ImplmmTaskCreate() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmTaskSignal")]
    public static void ImplmmTaskSignal() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmTaskYield")]
    public static void ImplmmTaskYield() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioAdvance")]
    public static void ImplmmioAdvance() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioAscend")]
    public static void ImplmmioAscend() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioClose")]
    public static void ImplmmioClose() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioCreateChunk")]
    public static void ImplmmioCreateChunk() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioDescend")]
    public static void ImplmmioDescend() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioFlush")]
    public static void ImplmmioFlush() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioGetInfo")]
    public static void ImplmmioGetInfo() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioInstallIOProcA")]
    public static void ImplmmioInstallIOProcA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioInstallIOProcW")]
    public static void ImplmmioInstallIOProcW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioOpenA")]
    public static void ImplmmioOpenA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioOpenW")]
    public static void ImplmmioOpenW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioRead")]
    public static void ImplmmioRead() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioRenameA")]
    public static void ImplmmioRenameA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioRenameW")]
    public static void ImplmmioRenameW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioSeek")]
    public static void ImplmmioSeek() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioSendMessage")]
    public static void ImplmmioSendMessage() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioSetBuffer")]
    public static void ImplmmioSetBuffer() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioSetInfo")]
    public static void ImplmmioSetInfo() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioStringToFOURCCA")]
    public static void ImplmmioStringToFOURCCA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioStringToFOURCCW")]
    public static void ImplmmioStringToFOURCCW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmioWrite")]
    public static void ImplmmioWrite() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplmmsystemGetVersion")]
    public static void ImplmmsystemGetVersion() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplsndPlaySoundA")]
    public static void ImplsndPlaySoundA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplsndPlaySoundW")]
    public static void ImplsndPlaySoundW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImpltimeBeginPeriod")]
    public static void ImpltimeBeginPeriod() { }

    [UnmanagedCallersOnly(EntryPoint = "ImpltimeEndPeriod")]
    public static void ImpltimeEndPeriod() { }

    [UnmanagedCallersOnly(EntryPoint = "ImpltimeGetDevCaps")]
    public static void ImpltimeGetDevCaps() { }

    [UnmanagedCallersOnly(EntryPoint = "ImpltimeGetSystemTime")]
    public static void ImpltimeGetSystemTime() { }

    [UnmanagedCallersOnly(EntryPoint = "ImpltimeGetTime")]
    public static void ImpltimeGetTime() { }

    [UnmanagedCallersOnly(EntryPoint = "ImpltimeKillEvent")]
    public static void ImpltimeKillEvent() { }

    [UnmanagedCallersOnly(EntryPoint = "ImpltimeSetEvent")]
    public static void ImpltimeSetEvent() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveInAddBuffer")]
    public static void ImplwaveInAddBuffer() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveInClose")]
    public static void ImplwaveInClose() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveInGetDevCapsA")]
    public static void ImplwaveInGetDevCapsA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveInGetDevCapsW")]
    public static void ImplwaveInGetDevCapsW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveInGetErrorTextA")]
    public static void ImplwaveInGetErrorTextA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveInGetErrorTextW")]
    public static void ImplwaveInGetErrorTextW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveInGetID")]
    public static void ImplwaveInGetID() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveInGetNumDevs")]
    public static void ImplwaveInGetNumDevs() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveInGetPosition")]
    public static void ImplwaveInGetPosition() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveInMessage")]
    public static void ImplwaveInMessage() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveInOpen")]
    public static void ImplwaveInOpen() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveInPrepareHeader")]
    public static void ImplwaveInPrepareHeader() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveInReset")]
    public static void ImplwaveInReset() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveInStart")]
    public static void ImplwaveInStart() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveInStop")]
    public static void ImplwaveInStop() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveInUnprepareHeader")]
    public static void ImplwaveInUnprepareHeader() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutBreakLoop")]
    public static void ImplwaveOutBreakLoop() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutClose")]
    public static void ImplwaveOutClose() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutGetDevCapsA")]
    public static void ImplwaveOutGetDevCapsA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutGetDevCapsW")]
    public static void ImplwaveOutGetDevCapsW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutGetErrorTextA")]
    public static void ImplwaveOutGetErrorTextA() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutGetErrorTextW")]
    public static void ImplwaveOutGetErrorTextW() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutGetID")]
    public static void ImplwaveOutGetID() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutGetNumDevs")]
    public static void ImplwaveOutGetNumDevs() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutGetPitch")]
    public static void ImplwaveOutGetPitch() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutGetPlaybackRate")]
    public static void ImplwaveOutGetPlaybackRate() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutGetPosition")]
    public static void ImplwaveOutGetPosition() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutGetVolume")]
    public static void ImplwaveOutGetVolume() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutMessage")]
    public static void ImplwaveOutMessage() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutOpen")]
    public static void ImplwaveOutOpen() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutPause")]
    public static void ImplwaveOutPause() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutPrepareHeader")]
    public static void ImplwaveOutPrepareHeader() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutReset")]
    public static void ImplwaveOutReset() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutRestart")]
    public static void ImplwaveOutRestart() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutSetPitch")]
    public static void ImplwaveOutSetPitch() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutSetPlaybackRate")]
    public static void ImplwaveOutSetPlaybackRate() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutSetVolume")]
    public static void ImplwaveOutSetVolume() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutUnprepareHeader")]
    public static void ImplwaveOutUnprepareHeader() { }

    [UnmanagedCallersOnly(EntryPoint = "ImplwaveOutWrite")]
    public static void ImplwaveOutWrite() { }
}