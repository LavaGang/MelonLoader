using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace MelonBootstrap.Proxy;

[SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Exported proxy names")]
internal static class WinMMExports
{
    [UnmanagedCallersOnly(EntryPoint = "mciExecute")]
    public static void mciExecute() { }

    [UnmanagedCallersOnly(EntryPoint = "CloseDriver")]
    public static void CloseDriver() { }

    [UnmanagedCallersOnly(EntryPoint = "DefDriverProc")]
    public static void DefDriverProc() { }

    [UnmanagedCallersOnly(EntryPoint = "DriverCallback")]
    public static void DriverCallback() { }

    [UnmanagedCallersOnly(EntryPoint = "DrvGetModuleHandle")]
    public static void DrvGetModuleHandle() { }

    [UnmanagedCallersOnly(EntryPoint = "GetDriverModuleHandle")]
    public static void GetDriverModuleHandle() { }

    [UnmanagedCallersOnly(EntryPoint = "OpenDriver")]
    public static void OpenDriver() { }

    [UnmanagedCallersOnly(EntryPoint = "PlaySound")]
    public static void PlaySound() { }

    [UnmanagedCallersOnly(EntryPoint = "PlaySoundA")]
    public static void PlaySoundA() { }

    [UnmanagedCallersOnly(EntryPoint = "PlaySoundW")]
    public static void PlaySoundW() { }

    [UnmanagedCallersOnly(EntryPoint = "SendDriverMessage")]
    public static void SendDriverMessage() { }

    [UnmanagedCallersOnly(EntryPoint = "WOWAppExit")]
    public static void WOWAppExit() { }

    [UnmanagedCallersOnly(EntryPoint = "auxGetDevCapsA")]
    public static void auxGetDevCapsA() { }

    [UnmanagedCallersOnly(EntryPoint = "auxGetDevCapsW")]
    public static void auxGetDevCapsW() { }

    [UnmanagedCallersOnly(EntryPoint = "auxGetNumDevs")]
    public static void auxGetNumDevs() { }

    [UnmanagedCallersOnly(EntryPoint = "auxGetVolume")]
    public static void auxGetVolume() { }

    [UnmanagedCallersOnly(EntryPoint = "auxOutMessage")]
    public static void auxOutMessage() { }

    [UnmanagedCallersOnly(EntryPoint = "auxSetVolume")]
    public static void auxSetVolume() { }

    [UnmanagedCallersOnly(EntryPoint = "joyConfigChanged")]
    public static void joyConfigChanged() { }

    [UnmanagedCallersOnly(EntryPoint = "joyGetDevCapsA")]
    public static void joyGetDevCapsA() { }

    [UnmanagedCallersOnly(EntryPoint = "joyGetDevCapsW")]
    public static void joyGetDevCapsW() { }

    [UnmanagedCallersOnly(EntryPoint = "joyGetNumDevs")]
    public static void joyGetNumDevs() { }

    [UnmanagedCallersOnly(EntryPoint = "joyGetPos")]
    public static void joyGetPos() { }

    [UnmanagedCallersOnly(EntryPoint = "joyGetPosEx")]
    public static void joyGetPosEx() { }

    [UnmanagedCallersOnly(EntryPoint = "joyGetThreshold")]
    public static void joyGetThreshold() { }

    [UnmanagedCallersOnly(EntryPoint = "joyReleaseCapture")]
    public static void joyReleaseCapture() { }

    [UnmanagedCallersOnly(EntryPoint = "joySetCapture")]
    public static void joySetCapture() { }

    [UnmanagedCallersOnly(EntryPoint = "joySetThreshold")]
    public static void joySetThreshold() { }

    [UnmanagedCallersOnly(EntryPoint = "mciDriverNotify")]
    public static void mciDriverNotify() { }

    [UnmanagedCallersOnly(EntryPoint = "mciDriverYield")]
    public static void mciDriverYield() { }

    [UnmanagedCallersOnly(EntryPoint = "mciFreeCommandResource")]
    public static void mciFreeCommandResource() { }

    [UnmanagedCallersOnly(EntryPoint = "mciGetCreatorTask")]
    public static void mciGetCreatorTask() { }

    [UnmanagedCallersOnly(EntryPoint = "mciGetDeviceIDA")]
    public static void mciGetDeviceIDA() { }

    [UnmanagedCallersOnly(EntryPoint = "mciGetDeviceIDFromElementIDA")]
    public static void mciGetDeviceIDFromElementIDA() { }

    [UnmanagedCallersOnly(EntryPoint = "mciGetDeviceIDFromElementIDW")]
    public static void mciGetDeviceIDFromElementIDW() { }

    [UnmanagedCallersOnly(EntryPoint = "mciGetDeviceIDW")]
    public static void mciGetDeviceIDW() { }

    [UnmanagedCallersOnly(EntryPoint = "mciGetDriverData")]
    public static void mciGetDriverData() { }

    [UnmanagedCallersOnly(EntryPoint = "mciGetErrorStringA")]
    public static void mciGetErrorStringA() { }

    [UnmanagedCallersOnly(EntryPoint = "mciGetErrorStringW")]
    public static void mciGetErrorStringW() { }

    [UnmanagedCallersOnly(EntryPoint = "mciGetYieldProc")]
    public static void mciGetYieldProc() { }

    [UnmanagedCallersOnly(EntryPoint = "mciLoadCommandResource")]
    public static void mciLoadCommandResource() { }

    [UnmanagedCallersOnly(EntryPoint = "mciSendCommandA")]
    public static void mciSendCommandA() { }

    [UnmanagedCallersOnly(EntryPoint = "mciSendCommandW")]
    public static void mciSendCommandW() { }

    [UnmanagedCallersOnly(EntryPoint = "mciSendStringA")]
    public static void mciSendStringA() { }

    [UnmanagedCallersOnly(EntryPoint = "mciSendStringW")]
    public static void mciSendStringW() { }

    [UnmanagedCallersOnly(EntryPoint = "mciSetDriverData")]
    public static void mciSetDriverData() { }

    [UnmanagedCallersOnly(EntryPoint = "mciSetYieldProc")]
    public static void mciSetYieldProc() { }

    [UnmanagedCallersOnly(EntryPoint = "midiConnect")]
    public static void midiConnect() { }

    [UnmanagedCallersOnly(EntryPoint = "midiDisconnect")]
    public static void midiDisconnect() { }

    [UnmanagedCallersOnly(EntryPoint = "midiInAddBuffer")]
    public static void midiInAddBuffer() { }

    [UnmanagedCallersOnly(EntryPoint = "midiInClose")]
    public static void midiInClose() { }

    [UnmanagedCallersOnly(EntryPoint = "midiInGetDevCapsA")]
    public static void midiInGetDevCapsA() { }

    [UnmanagedCallersOnly(EntryPoint = "midiInGetDevCapsW")]
    public static void midiInGetDevCapsW() { }

    [UnmanagedCallersOnly(EntryPoint = "midiInGetErrorTextA")]
    public static void midiInGetErrorTextA() { }

    [UnmanagedCallersOnly(EntryPoint = "midiInGetErrorTextW")]
    public static void midiInGetErrorTextW() { }

    [UnmanagedCallersOnly(EntryPoint = "midiInGetID")]
    public static void midiInGetID() { }

    [UnmanagedCallersOnly(EntryPoint = "midiInGetNumDevs")]
    public static void midiInGetNumDevs() { }

    [UnmanagedCallersOnly(EntryPoint = "midiInMessage")]
    public static void midiInMessage() { }

    [UnmanagedCallersOnly(EntryPoint = "midiInOpen")]
    public static void midiInOpen() { }

    [UnmanagedCallersOnly(EntryPoint = "midiInPrepareHeader")]
    public static void midiInPrepareHeader() { }

    [UnmanagedCallersOnly(EntryPoint = "midiInReset")]
    public static void midiInReset() { }

    [UnmanagedCallersOnly(EntryPoint = "midiInStart")]
    public static void midiInStart() { }

    [UnmanagedCallersOnly(EntryPoint = "midiInStop")]
    public static void midiInStop() { }

    [UnmanagedCallersOnly(EntryPoint = "midiInUnprepareHeader")]
    public static void midiInUnprepareHeader() { }

    [UnmanagedCallersOnly(EntryPoint = "midiOutCacheDrumPatches")]
    public static void midiOutCacheDrumPatches() { }

    [UnmanagedCallersOnly(EntryPoint = "midiOutCachePatches")]
    public static void midiOutCachePatches() { }

    [UnmanagedCallersOnly(EntryPoint = "midiOutClose")]
    public static void midiOutClose() { }

    [UnmanagedCallersOnly(EntryPoint = "midiOutGetDevCapsA")]
    public static void midiOutGetDevCapsA() { }

    [UnmanagedCallersOnly(EntryPoint = "midiOutGetDevCapsW")]
    public static void midiOutGetDevCapsW() { }

    [UnmanagedCallersOnly(EntryPoint = "midiOutGetErrorTextA")]
    public static void midiOutGetErrorTextA() { }

    [UnmanagedCallersOnly(EntryPoint = "midiOutGetErrorTextW")]
    public static void midiOutGetErrorTextW() { }

    [UnmanagedCallersOnly(EntryPoint = "midiOutGetID")]
    public static void midiOutGetID() { }

    [UnmanagedCallersOnly(EntryPoint = "midiOutGetNumDevs")]
    public static void midiOutGetNumDevs() { }

    [UnmanagedCallersOnly(EntryPoint = "midiOutGetVolume")]
    public static void midiOutGetVolume() { }

    [UnmanagedCallersOnly(EntryPoint = "midiOutLongMsg")]
    public static void midiOutLongMsg() { }

    [UnmanagedCallersOnly(EntryPoint = "midiOutMessage")]
    public static void midiOutMessage() { }

    [UnmanagedCallersOnly(EntryPoint = "midiOutOpen")]
    public static void midiOutOpen() { }

    [UnmanagedCallersOnly(EntryPoint = "midiOutPrepareHeader")]
    public static void midiOutPrepareHeader() { }

    [UnmanagedCallersOnly(EntryPoint = "midiOutReset")]
    public static void midiOutReset() { }

    [UnmanagedCallersOnly(EntryPoint = "midiOutSetVolume")]
    public static void midiOutSetVolume() { }

    [UnmanagedCallersOnly(EntryPoint = "midiOutShortMsg")]
    public static void midiOutShortMsg() { }

    [UnmanagedCallersOnly(EntryPoint = "midiOutUnprepareHeader")]
    public static void midiOutUnprepareHeader() { }

    [UnmanagedCallersOnly(EntryPoint = "midiStreamClose")]
    public static void midiStreamClose() { }

    [UnmanagedCallersOnly(EntryPoint = "midiStreamOpen")]
    public static void midiStreamOpen() { }

    [UnmanagedCallersOnly(EntryPoint = "midiStreamOut")]
    public static void midiStreamOut() { }

    [UnmanagedCallersOnly(EntryPoint = "midiStreamPause")]
    public static void midiStreamPause() { }

    [UnmanagedCallersOnly(EntryPoint = "midiStreamPosition")]
    public static void midiStreamPosition() { }

    [UnmanagedCallersOnly(EntryPoint = "midiStreamProperty")]
    public static void midiStreamProperty() { }

    [UnmanagedCallersOnly(EntryPoint = "midiStreamRestart")]
    public static void midiStreamRestart() { }

    [UnmanagedCallersOnly(EntryPoint = "midiStreamStop")]
    public static void midiStreamStop() { }

    [UnmanagedCallersOnly(EntryPoint = "mixerClose")]
    public static void mixerClose() { }

    [UnmanagedCallersOnly(EntryPoint = "mixerGetControlDetailsA")]
    public static void mixerGetControlDetailsA() { }

    [UnmanagedCallersOnly(EntryPoint = "mixerGetControlDetailsW")]
    public static void mixerGetControlDetailsW() { }

    [UnmanagedCallersOnly(EntryPoint = "mixerGetDevCapsA")]
    public static void mixerGetDevCapsA() { }

    [UnmanagedCallersOnly(EntryPoint = "mixerGetDevCapsW")]
    public static void mixerGetDevCapsW() { }

    [UnmanagedCallersOnly(EntryPoint = "mixerGetID")]
    public static void mixerGetID() { }

    [UnmanagedCallersOnly(EntryPoint = "mixerGetLineControlsA")]
    public static void mixerGetLineControlsA() { }

    [UnmanagedCallersOnly(EntryPoint = "mixerGetLineControlsW")]
    public static void mixerGetLineControlsW() { }

    [UnmanagedCallersOnly(EntryPoint = "mixerGetLineInfoA")]
    public static void mixerGetLineInfoA() { }

    [UnmanagedCallersOnly(EntryPoint = "mixerGetLineInfoW")]
    public static void mixerGetLineInfoW() { }

    [UnmanagedCallersOnly(EntryPoint = "mixerGetNumDevs")]
    public static void mixerGetNumDevs() { }

    [UnmanagedCallersOnly(EntryPoint = "mixerMessage")]
    public static void mixerMessage() { }

    [UnmanagedCallersOnly(EntryPoint = "mixerOpen")]
    public static void mixerOpen() { }

    [UnmanagedCallersOnly(EntryPoint = "mixerSetControlDetails")]
    public static void mixerSetControlDetails() { }

    [UnmanagedCallersOnly(EntryPoint = "mmDrvInstall")]
    public static void mmDrvInstall() { }

    [UnmanagedCallersOnly(EntryPoint = "mmGetCurrentTask")]
    public static void mmGetCurrentTask() { }

    [UnmanagedCallersOnly(EntryPoint = "mmTaskBlock")]
    public static void mmTaskBlock() { }

    [UnmanagedCallersOnly(EntryPoint = "mmTaskCreate")]
    public static void mmTaskCreate() { }

    [UnmanagedCallersOnly(EntryPoint = "mmTaskSignal")]
    public static void mmTaskSignal() { }

    [UnmanagedCallersOnly(EntryPoint = "mmTaskYield")]
    public static void mmTaskYield() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioAdvance")]
    public static void mmioAdvance() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioAscend")]
    public static void mmioAscend() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioClose")]
    public static void mmioClose() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioCreateChunk")]
    public static void mmioCreateChunk() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioDescend")]
    public static void mmioDescend() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioFlush")]
    public static void mmioFlush() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioGetInfo")]
    public static void mmioGetInfo() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioInstallIOProcA")]
    public static void mmioInstallIOProcA() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioInstallIOProcW")]
    public static void mmioInstallIOProcW() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioOpenA")]
    public static void mmioOpenA() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioOpenW")]
    public static void mmioOpenW() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioRead")]
    public static void mmioRead() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioRenameA")]
    public static void mmioRenameA() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioRenameW")]
    public static void mmioRenameW() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioSeek")]
    public static void mmioSeek() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioSendMessage")]
    public static void mmioSendMessage() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioSetBuffer")]
    public static void mmioSetBuffer() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioSetInfo")]
    public static void mmioSetInfo() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioStringToFOURCCA")]
    public static void mmioStringToFOURCCA() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioStringToFOURCCW")]
    public static void mmioStringToFOURCCW() { }

    [UnmanagedCallersOnly(EntryPoint = "mmioWrite")]
    public static void mmioWrite() { }

    [UnmanagedCallersOnly(EntryPoint = "mmsystemGetVersion")]
    public static void mmsystemGetVersion() { }

    [UnmanagedCallersOnly(EntryPoint = "sndPlaySoundA")]
    public static void sndPlaySoundA() { }

    [UnmanagedCallersOnly(EntryPoint = "sndPlaySoundW")]
    public static void sndPlaySoundW() { }

    [UnmanagedCallersOnly(EntryPoint = "timeBeginPeriod")]
    public static void timeBeginPeriod() { }

    [UnmanagedCallersOnly(EntryPoint = "timeEndPeriod")]
    public static void timeEndPeriod() { }

    [UnmanagedCallersOnly(EntryPoint = "timeGetDevCaps")]
    public static void timeGetDevCaps() { }

    [UnmanagedCallersOnly(EntryPoint = "timeGetSystemTime")]
    public static void timeGetSystemTime() { }

    [UnmanagedCallersOnly(EntryPoint = "timeGetTime")]
    public static void timeGetTime() { }

    [UnmanagedCallersOnly(EntryPoint = "timeKillEvent")]
    public static void timeKillEvent() { }

    [UnmanagedCallersOnly(EntryPoint = "timeSetEvent")]
    public static void timeSetEvent() { }

    [UnmanagedCallersOnly(EntryPoint = "waveInAddBuffer")]
    public static void waveInAddBuffer() { }

    [UnmanagedCallersOnly(EntryPoint = "waveInClose")]
    public static void waveInClose() { }

    [UnmanagedCallersOnly(EntryPoint = "waveInGetDevCapsA")]
    public static void waveInGetDevCapsA() { }

    [UnmanagedCallersOnly(EntryPoint = "waveInGetDevCapsW")]
    public static void waveInGetDevCapsW() { }

    [UnmanagedCallersOnly(EntryPoint = "waveInGetErrorTextA")]
    public static void waveInGetErrorTextA() { }

    [UnmanagedCallersOnly(EntryPoint = "waveInGetErrorTextW")]
    public static void waveInGetErrorTextW() { }

    [UnmanagedCallersOnly(EntryPoint = "waveInGetID")]
    public static void waveInGetID() { }

    [UnmanagedCallersOnly(EntryPoint = "waveInGetNumDevs")]
    public static void waveInGetNumDevs() { }

    [UnmanagedCallersOnly(EntryPoint = "waveInGetPosition")]
    public static void waveInGetPosition() { }

    [UnmanagedCallersOnly(EntryPoint = "waveInMessage")]
    public static void waveInMessage() { }

    [UnmanagedCallersOnly(EntryPoint = "waveInOpen")]
    public static void waveInOpen() { }

    [UnmanagedCallersOnly(EntryPoint = "waveInPrepareHeader")]
    public static void waveInPrepareHeader() { }

    [UnmanagedCallersOnly(EntryPoint = "waveInReset")]
    public static void waveInReset() { }

    [UnmanagedCallersOnly(EntryPoint = "waveInStart")]
    public static void waveInStart() { }

    [UnmanagedCallersOnly(EntryPoint = "waveInStop")]
    public static void waveInStop() { }

    [UnmanagedCallersOnly(EntryPoint = "waveInUnprepareHeader")]
    public static void waveInUnprepareHeader() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutBreakLoop")]
    public static void waveOutBreakLoop() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutClose")]
    public static void waveOutClose() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutGetDevCapsA")]
    public static void waveOutGetDevCapsA() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutGetDevCapsW")]
    public static void waveOutGetDevCapsW() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutGetErrorTextA")]
    public static void waveOutGetErrorTextA() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutGetErrorTextW")]
    public static void waveOutGetErrorTextW() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutGetID")]
    public static void waveOutGetID() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutGetNumDevs")]
    public static void waveOutGetNumDevs() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutGetPitch")]
    public static void waveOutGetPitch() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutGetPlaybackRate")]
    public static void waveOutGetPlaybackRate() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutGetPosition")]
    public static void waveOutGetPosition() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutGetVolume")]
    public static void waveOutGetVolume() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutMessage")]
    public static void waveOutMessage() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutOpen")]
    public static void waveOutOpen() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutPause")]
    public static void waveOutPause() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutPrepareHeader")]
    public static void waveOutPrepareHeader() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutReset")]
    public static void waveOutReset() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutRestart")]
    public static void waveOutRestart() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutSetPitch")]
    public static void waveOutSetPitch() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutSetPlaybackRate")]
    public static void waveOutSetPlaybackRate() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutSetVolume")]
    public static void waveOutSetVolume() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutUnprepareHeader")]
    public static void waveOutUnprepareHeader() { }

    [UnmanagedCallersOnly(EntryPoint = "waveOutWrite")]
    public static void waveOutWrite() { }
}