// Just proxied functions in this file
use proxygen_macros::forward;

#[forward]
#[export_name="CloseDriver"]
pub extern "C" fn CloseDriver() {}

#[forward]
#[export_name="DefDriverProc"]
pub extern "C" fn DefDriverProc() {}

#[forward]
#[export_name="DllCanUnloadNow"]
pub extern "C" fn DllCanUnloadNow() {}

#[forward]
#[export_name="DllGetClassObject"]
pub extern "C" fn DllGetClassObject() {}

#[forward]
#[export_name="DllRegisterServer"]
pub extern "C" fn DllRegisterServer() {}

#[forward]
#[export_name="DllUnregisterServer"]
pub extern "C" fn DllUnregisterServer() {}

#[forward]
#[export_name="DriverCallback"]
pub extern "C" fn DriverCallback() {}

#[forward]
#[export_name="DrvClose"]
pub extern "C" fn DrvClose() {}

#[forward]
#[export_name="DrvDefDriverProc"]
pub extern "C" fn DrvDefDriverProc() {}

#[forward]
#[export_name="DrvGetModuleHandle"]
pub extern "C" fn DrvGetModuleHandle() {}

#[forward]
#[export_name="DrvOpen"]
pub extern "C" fn DrvOpen() {}

#[forward]
#[export_name="DrvOpenA"]
pub extern "C" fn DrvOpenA() {}

#[forward]
#[export_name="DrvSendMessage"]
pub extern "C" fn DrvSendMessage() {}

#[forward]
#[export_name="GetDriverFlags"]
pub extern "C" fn GetDriverFlags() {}

#[forward]
#[export_name="GetDriverModuleHandle"]
pub extern "C" fn GetDriverModuleHandle() {}

#[forward]
#[export_name="GetFileVersionInfoA"]
pub extern "C" fn GetFileVersionInfoA() {}

#[forward]
#[export_name="GetFileVersionInfoExA"]
pub extern "C" fn GetFileVersionInfoExA() {}

#[forward]
#[export_name="GetFileVersionInfoExW"]
pub extern "C" fn GetFileVersionInfoExW() {}

#[forward]
#[export_name="GetFileVersionInfoSizeA"]
pub extern "C" fn GetFileVersionInfoSizeA() {}

#[forward]
#[export_name="GetFileVersionInfoSizeExA"]
pub extern "C" fn GetFileVersionInfoSizeExA() {}

#[forward]
#[export_name="GetFileVersionInfoSizeExW"]
pub extern "C" fn GetFileVersionInfoSizeExW() {}

#[forward]
#[export_name="GetFileVersionInfoSizeW"]
pub extern "C" fn GetFileVersionInfoSizeW() {}

#[forward]
#[export_name="GetFileVersionInfoW"]
pub extern "C" fn GetFileVersionInfoW() {}

#[forward]
#[export_name="OpenDriver"]
pub extern "C" fn OpenDriver() {}

#[forward]
#[export_name="OpenDriverA"]
pub extern "C" fn OpenDriverA() {}

#[forward]
#[export_name="PlaySound"]
pub extern "C" fn PlaySound() {}

#[forward]
#[export_name="PlaySoundA"]
pub extern "C" fn PlaySoundA() {}

#[forward]
#[export_name="PlaySoundW"]
pub extern "C" fn PlaySoundW() {}

#[forward]
#[export_name="SendDriverMessage"]
pub extern "C" fn SendDriverMessage() {}

#[forward]
#[export_name="VerFindFileA"]
pub extern "C" fn VerFindFileA() {}

#[forward]
#[export_name="VerFindFileW"]
pub extern "C" fn VerFindFileW() {}

#[forward]
#[export_name="VerInstallFileA"]
pub extern "C" fn VerInstallFileA() {}

#[forward]
#[export_name="VerInstallFileW"]
pub extern "C" fn VerInstallFileW() {}

#[forward]
#[export_name="VerLanguageNameA"]
pub extern "C" fn VerLanguageNameA() {}

#[forward]
#[export_name="VerLanguageNameW"]
pub extern "C" fn VerLanguageNameW() {}

#[forward]
#[export_name="VerQueryValueA"]
pub extern "C" fn VerQueryValueA() {}

#[forward]
#[export_name="VerQueryValueW"]
pub extern "C" fn VerQueryValueW() {}

#[forward]
#[export_name="WinHttpAddRequestHeaders"]
pub extern "C" fn WinHttpAddRequestHeaders() {}

#[forward]
#[export_name="WinHttpCheckPlatform"]
pub extern "C" fn WinHttpCheckPlatform() {}

#[forward]
#[export_name="WinHttpCloseHandle"]
pub extern "C" fn WinHttpCloseHandle() {}

#[forward]
#[export_name="WinHttpConnect"]
pub extern "C" fn WinHttpConnect() {}

#[forward]
#[export_name="WinHttpCrackUrl"]
pub extern "C" fn WinHttpCrackUrl() {}

#[forward]
#[export_name="WinHttpCreateProxyResolver"]
pub extern "C" fn WinHttpCreateProxyResolver() {}

#[forward]
#[export_name="WinHttpCreateUrl"]
pub extern "C" fn WinHttpCreateUrl() {}

#[forward]
#[export_name="WinHttpDetectAutoProxyConfigUrl"]
pub extern "C" fn WinHttpDetectAutoProxyConfigUrl() {}

#[forward]
#[export_name="WinHttpFreeProxyResult"]
pub extern "C" fn WinHttpFreeProxyResult() {}

#[forward]
#[export_name="WinHttpFreeProxyResultEx"]
pub extern "C" fn WinHttpFreeProxyResultEx() {}

#[forward]
#[export_name="WinHttpFreeProxySettings"]
pub extern "C" fn WinHttpFreeProxySettings() {}

#[forward]
#[export_name="WinHttpGetDefaultProxyConfiguration"]
pub extern "C" fn WinHttpGetDefaultProxyConfiguration() {}

#[forward]
#[export_name="WinHttpGetIEProxyConfigForCurrentUser"]
pub extern "C" fn WinHttpGetIEProxyConfigForCurrentUser() {}

#[forward]
#[export_name="WinHttpGetProxyForUrl"]
pub extern "C" fn WinHttpGetProxyForUrl() {}

#[forward]
#[export_name="WinHttpGetProxyForUrlEx"]
pub extern "C" fn WinHttpGetProxyForUrlEx() {}

#[forward]
#[export_name="WinHttpGetProxyForUrlEx2"]
pub extern "C" fn WinHttpGetProxyForUrlEx2() {}

#[forward]
#[export_name="WinHttpGetProxyResult"]
pub extern "C" fn WinHttpGetProxyResult() {}

#[forward]
#[export_name="WinHttpGetProxyResultEx"]
pub extern "C" fn WinHttpGetProxyResultEx() {}

#[forward]
#[export_name="WinHttpGetProxySettingsVersion"]
pub extern "C" fn WinHttpGetProxySettingsVersion() {}

#[forward]
#[export_name="WinHttpOpen"]
pub extern "C" fn WinHttpOpen() {}

#[forward]
#[export_name="WinHttpOpenRequest"]
pub extern "C" fn WinHttpOpenRequest() {}

#[forward]
#[export_name="WinHttpQueryAuthSchemes"]
pub extern "C" fn WinHttpQueryAuthSchemes() {}

#[forward]
#[export_name="WinHttpQueryDataAvailable"]
pub extern "C" fn WinHttpQueryDataAvailable() {}

#[forward]
#[export_name="WinHttpQueryHeaders"]
pub extern "C" fn WinHttpQueryHeaders() {}

#[forward]
#[export_name="WinHttpQueryOption"]
pub extern "C" fn WinHttpQueryOption() {}

#[forward]
#[export_name="WinHttpReadData"]
pub extern "C" fn WinHttpReadData() {}

#[forward]
#[export_name="WinHttpReadProxySettings"]
pub extern "C" fn WinHttpReadProxySettings() {}

#[forward]
#[export_name="WinHttpReceiveResponse"]
pub extern "C" fn WinHttpReceiveResponse() {}

#[forward]
#[export_name="WinHttpResetAutoProxy"]
pub extern "C" fn WinHttpResetAutoProxy() {}

#[forward]
#[export_name="WinHttpSendRequest"]
pub extern "C" fn WinHttpSendRequest() {}

#[forward]
#[export_name="WinHttpSetCredentials"]
pub extern "C" fn WinHttpSetCredentials() {}

#[forward]
#[export_name="WinHttpSetDefaultProxyConfiguration"]
pub extern "C" fn WinHttpSetDefaultProxyConfiguration() {}

#[forward]
#[export_name="WinHttpSetOption"]
pub extern "C" fn WinHttpSetOption() {}

#[forward]
#[export_name="WinHttpSetStatusCallback"]
pub extern "C" fn WinHttpSetStatusCallback() {}

#[forward]
#[export_name="WinHttpSetTimeouts"]
pub extern "C" fn WinHttpSetTimeouts() {}

#[forward]
#[export_name="WinHttpTimeFromSystemTime"]
pub extern "C" fn WinHttpTimeFromSystemTime() {}

#[forward]
#[export_name="WinHttpTimeToSystemTime"]
pub extern "C" fn WinHttpTimeToSystemTime() {}

#[forward]
#[export_name="WinHttpWebSocketClose"]
pub extern "C" fn WinHttpWebSocketClose() {}

#[forward]
#[export_name="WinHttpWebSocketCompleteUpgrade"]
pub extern "C" fn WinHttpWebSocketCompleteUpgrade() {}

#[forward]
#[export_name="WinHttpWebSocketQueryCloseStatus"]
pub extern "C" fn WinHttpWebSocketQueryCloseStatus() {}

#[forward]
#[export_name="WinHttpWebSocketReceive"]
pub extern "C" fn WinHttpWebSocketReceive() {}

#[forward]
#[export_name="WinHttpWebSocketSend"]
pub extern "C" fn WinHttpWebSocketSend() {}

#[forward]
#[export_name="WinHttpWebSocketShutdown"]
pub extern "C" fn WinHttpWebSocketShutdown() {}

#[forward]
#[export_name="WinHttpWriteData"]
pub extern "C" fn WinHttpWriteData() {}

#[forward]
#[export_name="WinHttpWriteProxySettings"]
pub extern "C" fn WinHttpWriteProxySettings() {}

#[forward]
#[export_name="auxGetDevCapsA"]
pub extern "C" fn auxGetDevCapsA() {}

#[forward]
#[export_name="auxGetDevCapsW"]
pub extern "C" fn auxGetDevCapsW() {}

#[forward]
#[export_name="auxGetNumDevs"]
pub extern "C" fn auxGetNumDevs() {}

#[forward]
#[export_name="auxGetVolume"]
pub extern "C" fn auxGetVolume() {}

#[forward]
#[export_name="auxOutMessage"]
pub extern "C" fn auxOutMessage() {}

#[forward]
#[export_name="auxSetVolume"]
pub extern "C" fn auxSetVolume() {}

#[forward]
#[export_name="joyConfigChanged"]
pub extern "C" fn joyConfigChanged() {}

#[forward]
#[export_name="joyGetDevCapsA"]
pub extern "C" fn joyGetDevCapsA() {}

#[forward]
#[export_name="joyGetDevCapsW"]
pub extern "C" fn joyGetDevCapsW() {}

#[forward]
#[export_name="joyGetNumDevs"]
pub extern "C" fn joyGetNumDevs() {}

#[forward]
#[export_name="joyGetPos"]
pub extern "C" fn joyGetPos() {}

#[forward]
#[export_name="joyGetPosEx"]
pub extern "C" fn joyGetPosEx() {}

#[forward]
#[export_name="joyGetThreshold"]
pub extern "C" fn joyGetThreshold() {}

#[forward]
#[export_name="joyReleaseCapture"]
pub extern "C" fn joyReleaseCapture() {}

#[forward]
#[export_name="joySetCapture"]
pub extern "C" fn joySetCapture() {}

#[forward]
#[export_name="joySetThreshold"]
pub extern "C" fn joySetThreshold() {}

#[forward]
#[export_name="mciDriverNotify"]
pub extern "C" fn mciDriverNotify() {}

#[forward]
#[export_name="mciDriverYield"]
pub extern "C" fn mciDriverYield() {}

#[forward]
#[export_name="mciExecute"]
pub extern "C" fn mciExecute() {}

#[forward]
#[export_name="mciFreeCommandResource"]
pub extern "C" fn mciFreeCommandResource() {}

#[forward]
#[export_name="mciGetCreatorTask"]
pub extern "C" fn mciGetCreatorTask() {}

#[forward]
#[export_name="mciGetDeviceIDA"]
pub extern "C" fn mciGetDeviceIDA() {}

#[forward]
#[export_name="mciGetDeviceIDFromElementIDA"]
pub extern "C" fn mciGetDeviceIDFromElementIDA() {}

#[forward]
#[export_name="mciGetDeviceIDFromElementIDW"]
pub extern "C" fn mciGetDeviceIDFromElementIDW() {}

#[forward]
#[export_name="mciGetDeviceIDW"]
pub extern "C" fn mciGetDeviceIDW() {}

#[forward]
#[export_name="mciGetDriverData"]
pub extern "C" fn mciGetDriverData() {}

#[forward]
#[export_name="mciGetErrorStringA"]
pub extern "C" fn mciGetErrorStringA() {}

#[forward]
#[export_name="mciGetErrorStringW"]
pub extern "C" fn mciGetErrorStringW() {}

#[forward]
#[export_name="mciGetYieldProc"]
pub extern "C" fn mciGetYieldProc() {}

#[forward]
#[export_name="mciLoadCommandResource"]
pub extern "C" fn mciLoadCommandResource() {}

#[forward]
#[export_name="mciSendCommandA"]
pub extern "C" fn mciSendCommandA() {}

#[forward]
#[export_name="mciSendCommandW"]
pub extern "C" fn mciSendCommandW() {}

#[forward]
#[export_name="mciSendStringA"]
pub extern "C" fn mciSendStringA() {}

#[forward]
#[export_name="mciSendStringW"]
pub extern "C" fn mciSendStringW() {}

#[forward]
#[export_name="mciSetDriverData"]
pub extern "C" fn mciSetDriverData() {}

#[forward]
#[export_name="mciSetYieldProc"]
pub extern "C" fn mciSetYieldProc() {}

#[forward]
#[export_name="midiConnect"]
pub extern "C" fn midiConnect() {}

#[forward]
#[export_name="midiDisconnect"]
pub extern "C" fn midiDisconnect() {}

#[forward]
#[export_name="midiInAddBuffer"]
pub extern "C" fn midiInAddBuffer() {}

#[forward]
#[export_name="midiInClose"]
pub extern "C" fn midiInClose() {}

#[forward]
#[export_name="midiInGetDevCapsA"]
pub extern "C" fn midiInGetDevCapsA() {}

#[forward]
#[export_name="midiInGetDevCapsW"]
pub extern "C" fn midiInGetDevCapsW() {}

#[forward]
#[export_name="midiInGetErrorTextA"]
pub extern "C" fn midiInGetErrorTextA() {}

#[forward]
#[export_name="midiInGetErrorTextW"]
pub extern "C" fn midiInGetErrorTextW() {}

#[forward]
#[export_name="midiInGetID"]
pub extern "C" fn midiInGetID() {}

#[forward]
#[export_name="midiInGetNumDevs"]
pub extern "C" fn midiInGetNumDevs() {}

#[forward]
#[export_name="midiInMessage"]
pub extern "C" fn midiInMessage() {}

#[forward]
#[export_name="midiInOpen"]
pub extern "C" fn midiInOpen() {}

#[forward]
#[export_name="midiInPrepareHeader"]
pub extern "C" fn midiInPrepareHeader() {}

#[forward]
#[export_name="midiInReset"]
pub extern "C" fn midiInReset() {}

#[forward]
#[export_name="midiInStart"]
pub extern "C" fn midiInStart() {}

#[forward]
#[export_name="midiInStop"]
pub extern "C" fn midiInStop() {}

#[forward]
#[export_name="midiInUnprepareHeader"]
pub extern "C" fn midiInUnprepareHeader() {}

#[forward]
#[export_name="midiOutCacheDrumPatches"]
pub extern "C" fn midiOutCacheDrumPatches() {}

#[forward]
#[export_name="midiOutCachePatches"]
pub extern "C" fn midiOutCachePatches() {}

#[forward]
#[export_name="midiOutClose"]
pub extern "C" fn midiOutClose() {}

#[forward]
#[export_name="midiOutGetDevCapsA"]
pub extern "C" fn midiOutGetDevCapsA() {}

#[forward]
#[export_name="midiOutGetDevCapsW"]
pub extern "C" fn midiOutGetDevCapsW() {}

#[forward]
#[export_name="midiOutGetErrorTextA"]
pub extern "C" fn midiOutGetErrorTextA() {}

#[forward]
#[export_name="midiOutGetErrorTextW"]
pub extern "C" fn midiOutGetErrorTextW() {}

#[forward]
#[export_name="midiOutGetID"]
pub extern "C" fn midiOutGetID() {}

#[forward]
#[export_name="midiOutGetNumDevs"]
pub extern "C" fn midiOutGetNumDevs() {}

#[forward]
#[export_name="midiOutGetVolume"]
pub extern "C" fn midiOutGetVolume() {}

#[forward]
#[export_name="midiOutLongMsg"]
pub extern "C" fn midiOutLongMsg() {}

#[forward]
#[export_name="midiOutMessage"]
pub extern "C" fn midiOutMessage() {}

#[forward]
#[export_name="midiOutOpen"]
pub extern "C" fn midiOutOpen() {}

#[forward]
#[export_name="midiOutPrepareHeader"]
pub extern "C" fn midiOutPrepareHeader() {}

#[forward]
#[export_name="midiOutReset"]
pub extern "C" fn midiOutReset() {}

#[forward]
#[export_name="midiOutSetVolume"]
pub extern "C" fn midiOutSetVolume() {}

#[forward]
#[export_name="midiOutShortMsg"]
pub extern "C" fn midiOutShortMsg() {}

#[forward]
#[export_name="midiOutUnprepareHeader"]
pub extern "C" fn midiOutUnprepareHeader() {}

#[forward]
#[export_name="midiStreamClose"]
pub extern "C" fn midiStreamClose() {}

#[forward]
#[export_name="midiStreamOpen"]
pub extern "C" fn midiStreamOpen() {}

#[forward]
#[export_name="midiStreamOut"]
pub extern "C" fn midiStreamOut() {}

#[forward]
#[export_name="midiStreamPause"]
pub extern "C" fn midiStreamPause() {}

#[forward]
#[export_name="midiStreamPosition"]
pub extern "C" fn midiStreamPosition() {}

#[forward]
#[export_name="midiStreamProperty"]
pub extern "C" fn midiStreamProperty() {}

#[forward]
#[export_name="midiStreamRestart"]
pub extern "C" fn midiStreamRestart() {}

#[forward]
#[export_name="midiStreamStop"]
pub extern "C" fn midiStreamStop() {}

#[forward]
#[export_name="mixerClose"]
pub extern "C" fn mixerClose() {}

#[forward]
#[export_name="mixerGetControlDetailsA"]
pub extern "C" fn mixerGetControlDetailsA() {}

#[forward]
#[export_name="mixerGetControlDetailsW"]
pub extern "C" fn mixerGetControlDetailsW() {}

#[forward]
#[export_name="mixerGetDevCapsA"]
pub extern "C" fn mixerGetDevCapsA() {}

#[forward]
#[export_name="mixerGetDevCapsW"]
pub extern "C" fn mixerGetDevCapsW() {}

#[forward]
#[export_name="mixerGetID"]
pub extern "C" fn mixerGetID() {}

#[forward]
#[export_name="mixerGetLineControlsA"]
pub extern "C" fn mixerGetLineControlsA() {}

#[forward]
#[export_name="mixerGetLineControlsW"]
pub extern "C" fn mixerGetLineControlsW() {}

#[forward]
#[export_name="mixerGetLineInfoA"]
pub extern "C" fn mixerGetLineInfoA() {}

#[forward]
#[export_name="mixerGetLineInfoW"]
pub extern "C" fn mixerGetLineInfoW() {}

#[forward]
#[export_name="mixerGetNumDevs"]
pub extern "C" fn mixerGetNumDevs() {}

#[forward]
#[export_name="mixerMessage"]
pub extern "C" fn mixerMessage() {}

#[forward]
#[export_name="mixerOpen"]
pub extern "C" fn mixerOpen() {}

#[forward]
#[export_name="mixerSetControlDetails"]
pub extern "C" fn mixerSetControlDetails() {}

#[forward]
#[export_name="mmGetCurrentTask"]
pub extern "C" fn mmGetCurrentTask() {}

#[forward]
#[export_name="mmTaskBlock"]
pub extern "C" fn mmTaskBlock() {}

#[forward]
#[export_name="mmTaskCreate"]
pub extern "C" fn mmTaskCreate() {}

#[forward]
#[export_name="mmTaskSignal"]
pub extern "C" fn mmTaskSignal() {}

#[forward]
#[export_name="mmTaskYield"]
pub extern "C" fn mmTaskYield() {}

#[forward]
#[export_name="mmioAdvance"]
pub extern "C" fn mmioAdvance() {}

#[forward]
#[export_name="mmioAscend"]
pub extern "C" fn mmioAscend() {}

#[forward]
#[export_name="mmioClose"]
pub extern "C" fn mmioClose() {}

#[forward]
#[export_name="mmioCreateChunk"]
pub extern "C" fn mmioCreateChunk() {}

#[forward]
#[export_name="mmioDescend"]
pub extern "C" fn mmioDescend() {}

#[forward]
#[export_name="mmioFlush"]
pub extern "C" fn mmioFlush() {}

#[forward]
#[export_name="mmioGetInfo"]
pub extern "C" fn mmioGetInfo() {}

#[forward]
#[export_name="mmioInstallIOProc16"]
pub extern "C" fn mmioInstallIOProc16() {}

#[forward]
#[export_name="mmioInstallIOProcA"]
pub extern "C" fn mmioInstallIOProcA() {}

#[forward]
#[export_name="mmioInstallIOProcW"]
pub extern "C" fn mmioInstallIOProcW() {}

#[forward]
#[export_name="mmioOpenA"]
pub extern "C" fn mmioOpenA() {}

#[forward]
#[export_name="mmioOpenW"]
pub extern "C" fn mmioOpenW() {}

#[forward]
#[export_name="mmioRead"]
pub extern "C" fn mmioRead() {}

#[forward]
#[export_name="mmioRenameA"]
pub extern "C" fn mmioRenameA() {}

#[forward]
#[export_name="mmioRenameW"]
pub extern "C" fn mmioRenameW() {}

#[forward]
#[export_name="mmioSeek"]
pub extern "C" fn mmioSeek() {}

#[forward]
#[export_name="mmioSendMessage"]
pub extern "C" fn mmioSendMessage() {}

#[forward]
#[export_name="mmioSetBuffer"]
pub extern "C" fn mmioSetBuffer() {}

#[forward]
#[export_name="mmioSetInfo"]
pub extern "C" fn mmioSetInfo() {}

#[forward]
#[export_name="mmioStringToFOURCCA"]
pub extern "C" fn mmioStringToFOURCCA() {}

#[forward]
#[export_name="mmioStringToFOURCCW"]
pub extern "C" fn mmioStringToFOURCCW() {}

#[forward]
#[export_name="mmioWrite"]
pub extern "C" fn mmioWrite() {}

#[forward]
#[export_name="mmsystemGetVersion"]
pub extern "C" fn mmsystemGetVersion() {}

#[forward]
#[export_name="sndPlaySoundA"]
pub extern "C" fn sndPlaySoundA() {}

#[forward]
#[export_name="sndPlaySoundW"]
pub extern "C" fn sndPlaySoundW() {}

#[forward]
#[export_name="timeBeginPeriod"]
pub extern "C" fn timeBeginPeriod() {}

#[forward]
#[export_name="timeEndPeriod"]
pub extern "C" fn timeEndPeriod() {}

#[forward]
#[export_name="timeGetDevCaps"]
pub extern "C" fn timeGetDevCaps() {}

#[forward]
#[export_name="timeGetSystemTime"]
pub extern "C" fn timeGetSystemTime() {}

#[forward]
#[export_name="timeGetTime"]
pub extern "C" fn timeGetTime() {}

#[forward]
#[export_name="timeKillEvent"]
pub extern "C" fn timeKillEvent() {}

#[forward]
#[export_name="timeSetEvent"]
pub extern "C" fn timeSetEvent() {}

#[forward]
#[export_name="waveInAddBuffer"]
pub extern "C" fn waveInAddBuffer() {}

#[forward]
#[export_name="waveInClose"]
pub extern "C" fn waveInClose() {}

#[forward]
#[export_name="waveInGetDevCapsA"]
pub extern "C" fn waveInGetDevCapsA() {}

#[forward]
#[export_name="waveInGetDevCapsW"]
pub extern "C" fn waveInGetDevCapsW() {}

#[forward]
#[export_name="waveInGetErrorTextA"]
pub extern "C" fn waveInGetErrorTextA() {}

#[forward]
#[export_name="waveInGetErrorTextW"]
pub extern "C" fn waveInGetErrorTextW() {}

#[forward]
#[export_name="waveInGetID"]
pub extern "C" fn waveInGetID() {}

#[forward]
#[export_name="waveInGetNumDevs"]
pub extern "C" fn waveInGetNumDevs() {}

#[forward]
#[export_name="waveInGetPosition"]
pub extern "C" fn waveInGetPosition() {}

#[forward]
#[export_name="waveInMessage"]
pub extern "C" fn waveInMessage() {}

#[forward]
#[export_name="waveInOpen"]
pub extern "C" fn waveInOpen() {}

#[forward]
#[export_name="waveInPrepareHeader"]
pub extern "C" fn waveInPrepareHeader() {}

#[forward]
#[export_name="waveInReset"]
pub extern "C" fn waveInReset() {}

#[forward]
#[export_name="waveInStart"]
pub extern "C" fn waveInStart() {}

#[forward]
#[export_name="waveInStop"]
pub extern "C" fn waveInStop() {}

#[forward]
#[export_name="waveInUnprepareHeader"]
pub extern "C" fn waveInUnprepareHeader() {}

#[forward]
#[export_name="waveOutBreakLoop"]
pub extern "C" fn waveOutBreakLoop() {}

#[forward]
#[export_name="waveOutClose"]
pub extern "C" fn waveOutClose() {}

#[forward]
#[export_name="waveOutGetDevCapsA"]
pub extern "C" fn waveOutGetDevCapsA() {}

#[forward]
#[export_name="waveOutGetDevCapsW"]
pub extern "C" fn waveOutGetDevCapsW() {}

#[forward]
#[export_name="waveOutGetErrorTextA"]
pub extern "C" fn waveOutGetErrorTextA() {}

#[forward]
#[export_name="waveOutGetErrorTextW"]
pub extern "C" fn waveOutGetErrorTextW() {}

#[forward]
#[export_name="waveOutGetID"]
pub extern "C" fn waveOutGetID() {}

#[forward]
#[export_name="waveOutGetNumDevs"]
pub extern "C" fn waveOutGetNumDevs() {}

#[forward]
#[export_name="waveOutGetPitch"]
pub extern "C" fn waveOutGetPitch() {}

#[forward]
#[export_name="waveOutGetPlaybackRate"]
pub extern "C" fn waveOutGetPlaybackRate() {}

#[forward]
#[export_name="waveOutGetPosition"]
pub extern "C" fn waveOutGetPosition() {}

#[forward]
#[export_name="waveOutGetVolume"]
pub extern "C" fn waveOutGetVolume() {}

#[forward]
#[export_name="waveOutMessage"]
pub extern "C" fn waveOutMessage() {}

#[forward]
#[export_name="waveOutOpen"]
pub extern "C" fn waveOutOpen() {}

#[forward]
#[export_name="waveOutPause"]
pub extern "C" fn waveOutPause() {}

#[forward]
#[export_name="waveOutPrepareHeader"]
pub extern "C" fn waveOutPrepareHeader() {}

#[forward]
#[export_name="waveOutReset"]
pub extern "C" fn waveOutReset() {}

#[forward]
#[export_name="waveOutRestart"]
pub extern "C" fn waveOutRestart() {}

#[forward]
#[export_name="waveOutSetPitch"]
pub extern "C" fn waveOutSetPitch() {}

#[forward]
#[export_name="waveOutSetPlaybackRate"]
pub extern "C" fn waveOutSetPlaybackRate() {}

#[forward]
#[export_name="waveOutSetVolume"]
pub extern "C" fn waveOutSetVolume() {}

#[forward]
#[export_name="waveOutUnprepareHeader"]
pub extern "C" fn waveOutUnprepareHeader() {}

#[forward]
#[export_name="waveOutWrite"]
pub extern "C" fn waveOutWrite() {}

