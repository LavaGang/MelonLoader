#include <Windows.h>
#include <string>
#include <algorithm>

#pragma region version
extern "C" FARPROC version_OriginalFuncs[17];
FARPROC version_OriginalFuncs[17];
const char* version_ExportNames[] = { "GetFileVersionInfoA", "GetFileVersionInfoByHandle", "GetFileVersionInfoExA", "GetFileVersionInfoExW", "GetFileVersionInfoSizeA", "GetFileVersionInfoSizeExA", "GetFileVersionInfoSizeExW", "GetFileVersionInfoSizeW", "GetFileVersionInfoW", "VerFindFileA", "VerFindFileW", "VerInstallFileA", "VerInstallFileW", "VerLanguageNameA", "VerLanguageNameW", "VerQueryValueA", "VerQueryValueW" };
void LoadProxy_version(HMODULE originaldll) { for (int i = 0; i < (sizeof(version_ExportNames) / sizeof(version_ExportNames[0])); i++) version_OriginalFuncs[i] = GetProcAddress(originaldll, version_ExportNames[i]); }
#pragma endregion

#pragma region winmm
extern "C" FARPROC winmm_OriginalFuncs[181];
FARPROC winmm_OriginalFuncs[181];
const char* winmm_ExportNames[] = { "CloseDriver", "DefDriverProc", "DriverCallback", "DrvGetModuleHandle", "GetDriverModuleHandle", "OpenDriver", "PlaySound", "PlaySoundA", "PlaySoundW", "SendDriverMessage", "WOWAppExit", "auxGetDevCapsA", "auxGetDevCapsW", "auxGetNumDevs", "auxGetVolume", "auxOutMessage", "auxSetVolume", "joyConfigChanged", "joyGetDevCapsA", "joyGetDevCapsW", "joyGetNumDevs", "joyGetPos", "joyGetPosEx", "joyGetThreshold", "joyReleaseCapture", "joySetCapture", "joySetThreshold", "mciDriverNotify", "mciDriverYield", "mciExecute", "mciFreeCommandResource", "mciGetCreatorTask", "mciGetDeviceIDA", "mciGetDeviceIDFromElementIDA", "mciGetDeviceIDFromElementIDW", "mciGetDeviceIDW", "mciGetDriverData", "mciGetErrorStringA", "mciGetErrorStringW", "mciGetYieldProc", "mciLoadCommandResource", "mciSendCommandA", "mciSendCommandW", "mciSendStringA", "mciSendStringW", "mciSetDriverData", "mciSetYieldProc", "midiConnect", "midiDisconnect", "midiInAddBuffer", "midiInClose", "midiInGetDevCapsA", "midiInGetDevCapsW", "midiInGetErrorTextA", "midiInGetErrorTextW", "midiInGetID", "midiInGetNumDevs", "midiInMessage", "midiInOpen", "midiInPrepareHeader", "midiInReset", "midiInStart", "midiInStop", "midiInUnprepareHeader", "midiOutCacheDrumPatches", "midiOutCachePatches", "midiOutClose", "midiOutGetDevCapsA", "midiOutGetDevCapsW", "midiOutGetErrorTextA", "midiOutGetErrorTextW", "midiOutGetID", "midiOutGetNumDevs", "midiOutGetVolume", "midiOutLongMsg", "midiOutMessage", "midiOutOpen", "midiOutPrepareHeader", "midiOutReset", "midiOutSetVolume", "midiOutShortMsg", "midiOutUnprepareHeader", "midiStreamClose", "midiStreamOpen", "midiStreamOut", "midiStreamPause", "midiStreamPosition", "midiStreamProperty", "midiStreamRestart", "midiStreamStop", "mixerClose", "mixerGetControlDetailsA", "mixerGetControlDetailsW", "mixerGetDevCapsA", "mixerGetDevCapsW", "mixerGetID", "mixerGetLineControlsA", "mixerGetLineControlsW", "mixerGetLineInfoA", "mixerGetLineInfoW", "mixerGetNumDevs", "mixerMessage", "mixerOpen", "mixerSetControlDetails", "mmDrvInstall", "mmGetCurrentTask", "mmTaskBlock", "mmTaskCreate", "mmTaskSignal", "mmTaskYield", "mmioAdvance", "mmioAscend", "mmioClose", "mmioCreateChunk", "mmioDescend", "mmioFlush", "mmioGetInfo", "mmioInstallIOProcA", "mmioInstallIOProcW", "mmioOpenA", "mmioOpenW", "mmioRead", "mmioRenameA", "mmioRenameW", "mmioSeek", "mmioSendMessage", "mmioSetBuffer", "mmioSetInfo", "mmioStringToFOURCCA", "mmioStringToFOURCCW", "mmioWrite", "mmsystemGetVersion", "sndPlaySoundA", "sndPlaySoundW", "timeBeginPeriod", "timeEndPeriod", "timeGetDevCaps", "timeGetSystemTime", "timeGetTime", "timeKillEvent", "timeSetEvent", "waveInAddBuffer", "waveInClose", "waveInGetDevCapsA", "waveInGetDevCapsW", "waveInGetErrorTextA", "waveInGetErrorTextW", "waveInGetID", "waveInGetNumDevs", "waveInGetPosition", "waveInMessage", "waveInOpen", "waveInPrepareHeader", "waveInReset", "waveInStart", "waveInStop", "waveInUnprepareHeader", "waveOutBreakLoop", "waveOutClose", "waveOutGetDevCapsA", "waveOutGetDevCapsW", "waveOutGetErrorTextA", "waveOutGetErrorTextW", "waveOutGetID", "waveOutGetNumDevs", "waveOutGetPitch", "waveOutGetPlaybackRate", "waveOutGetPosition", "waveOutGetVolume", "waveOutMessage", "waveOutOpen", "waveOutPause", "waveOutPrepareHeader", "waveOutReset", "waveOutRestart", "waveOutSetPitch", "waveOutSetPlaybackRate", "waveOutSetVolume", "waveOutUnprepareHeader", "waveOutWrite", "ExportByOrdinal2" };
void LoadProxy_winmm(HMODULE originaldll) { for (int i = 0; i < (sizeof(winmm_ExportNames) / sizeof(winmm_ExportNames[0])); i++) winmm_OriginalFuncs[i] = GetProcAddress(originaldll, winmm_ExportNames[i]); }
#pragma endregion

#pragma region winhttp
extern "C" FARPROC winhttp_OriginalFuncs[65];
FARPROC winhttp_OriginalFuncs[65];
const char* winhttp_ExportNames[] = { "Private1", "SvchostPushServiceGlobals", "WinHttpAddRequestHeaders", "WinHttpAutoProxySvcMain", "WinHttpCheckPlatform", "WinHttpCloseHandle", "WinHttpConnect", "WinHttpConnectionDeletePolicyEntries", "WinHttpConnectionDeleteProxyInfo", "WinHttpConnectionFreeNameList", "WinHttpConnectionFreeProxyInfo", "WinHttpConnectionFreeProxyList", "WinHttpConnectionGetNameList", "WinHttpConnectionGetProxyInfo", "WinHttpConnectionGetProxyList", "WinHttpConnectionSetPolicyEntries", "WinHttpConnectionSetProxyInfo", "WinHttpConnectionUpdateIfIndexTable", "WinHttpCrackUrl", "WinHttpCreateProxyResolver", "WinHttpCreateUrl", "WinHttpDetectAutoProxyConfigUrl", "WinHttpFreeProxyResult", "WinHttpFreeProxyResultEx", "WinHttpFreeProxySettings", "WinHttpGetDefaultProxyConfiguration", "WinHttpGetIEProxyConfigForCurrentUser", "WinHttpGetProxyForUrl", "WinHttpGetProxyForUrlEx", "WinHttpGetProxyForUrlEx2", "WinHttpGetProxyForUrlHvsi", "WinHttpGetProxyResult", "WinHttpGetProxyResultEx", "WinHttpGetProxySettingsVersion", "WinHttpGetTunnelSocket", "WinHttpOpen", "WinHttpOpenRequest", "WinHttpPacJsWorkerMain", "WinHttpProbeConnectivity", "WinHttpQueryAuthSchemes", "WinHttpQueryDataAvailable", "WinHttpQueryHeaders", "WinHttpQueryOption", "WinHttpReadData", "WinHttpReadProxySettings", "WinHttpReadProxySettingsHvsi", "WinHttpReceiveResponse", "WinHttpResetAutoProxy", "WinHttpSaveProxyCredentials", "WinHttpSendRequest", "WinHttpSetCredentials", "WinHttpSetDefaultProxyConfiguration", "WinHttpSetOption", "WinHttpSetStatusCallback", "WinHttpSetTimeouts", "WinHttpTimeFromSystemTime", "WinHttpTimeToSystemTime", "WinHttpWebSocketClose", "WinHttpWebSocketCompleteUpgrade", "WinHttpWebSocketQueryCloseStatus", "WinHttpWebSocketReceive", "WinHttpWebSocketSend", "WinHttpWebSocketShutdown", "WinHttpWriteData", "WinHttpWriteProxySettings" };
void LoadProxy_winhttp(HMODULE originaldll) { for (int i = 0; i < (sizeof(winhttp_ExportNames) / sizeof(winhttp_ExportNames[0])); i++) winhttp_OriginalFuncs[i] = GetProcAddress(originaldll, winhttp_ExportNames[i]); }
#pragma endregion

void KillCurrentProcess()
{
	HANDLE current_process = GetCurrentProcess();
	if (current_process == NULL)
		return;
	TerminateProcess(current_process, NULL);
	CloseHandle(current_process);
}

const char* InvalidProcessNames[] = { "unitycrashhandler", "fallguys", "duskworld", "chilloutvr", "pixelstrike3d" };
void CheckForInvalidProcess()
{
	LPSTR filepath = new CHAR[MAX_PATH];
	GetModuleFileNameA(GetModuleHandleA(NULL), filepath, MAX_PATH);
	std::string filepathstr = filepath;
	delete[] filepath;
	filepathstr.erase(remove(filepathstr.begin(), filepathstr.end(), ' '), filepathstr.end());
	std::for_each(filepathstr.begin(), filepathstr.end(), [](char& character) { character = ::tolower(character); });
	for (int i = 0; i < (sizeof(InvalidProcessNames) / sizeof(InvalidProcessNames[0])); i++)
		if (strstr(filepathstr.c_str(), InvalidProcessNames[i]) != NULL)
			KillCurrentProcess();
}

bool LoadProxy(HINSTANCE hinstDLL)
{
	LPSTR fullpathstr = new CHAR[MAX_PATH];
	GetModuleFileNameA(hinstDLL, fullpathstr, MAX_PATH);
	std::string fullpath = fullpathstr;
	delete[] fullpathstr;
	std::string filepath = fullpath.substr(0, fullpath.find_last_of("\\/"));
	std::string filename = fullpath.substr(fullpath.find_last_of("\\/") + 1, fullpath.size());
	filename = filename.substr(0, filename.find_last_of("."));
	std::for_each(filename.begin(), filename.end(), [](char& character) { character = ::tolower(character); });
	HMODULE originaldll = LoadLibraryA((fullpath.substr(0, fullpath.find_last_of(".")) + "_original.dll").c_str());
	if (originaldll == NULL)
	{
		char* system32path = new char[MAX_PATH];
		if (GetSystemDirectoryA(system32path, MAX_PATH) == NULL)
		{
			delete[] system32path;
			MessageBoxA(NULL, "Failed to Get System32 Directory!", "MelonLoader", MB_ICONERROR | MB_OK);
			return false;
		}
		originaldll = LoadLibraryA((std::string(system32path) + "\\" + filename + ".dll").c_str());
		delete[] system32path;
	}
	if (originaldll == NULL)
	{
		MessageBoxA(NULL, ("Failed to Load " + filename + ".dll!").c_str(), "MelonLoader", MB_ICONERROR | MB_OK);
		return false;
	}
	if (strstr(filename.c_str(), "version") != NULL)
		LoadProxy_version(originaldll);
	else if (strstr(filename.c_str(), "winmm") != NULL)
		LoadProxy_winmm(originaldll);
	else if (strstr(filename.c_str(), "winhttp") != NULL)
		LoadProxy_winhttp(originaldll);
	else
		return false;
	return true;
}

void LoadBootstrap()
{
	if (strstr(GetCommandLineA(), "--no-mods") != NULL)
		return;
	HINSTANCE melonloaderdll = LoadLibraryA("MelonLoader\\Dependencies\\Bootstrap.dll");
	if (melonloaderdll == NULL)
		MessageBoxA(NULL, "Failed to Load Bootstrap.dll!", "MelonLoader", MB_ICONERROR | MB_OK);
}

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	if (fdwReason != DLL_PROCESS_ATTACH)
		return TRUE;
	CheckForInvalidProcess();
	if (!LoadProxy(hinstDLL))
		return FALSE;
	LoadBootstrap();
	return TRUE;
}