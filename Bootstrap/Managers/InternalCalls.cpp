#include "InternalCalls.h"
#include "../Utils/Console/Debug.h"
#include "../Utils/Console/Logger.h"
#include "Game.h"
#include "Hook.h"
#include "../Utils/Assertion.h"
#include "../Base/Core.h"

#include "Il2Cpp.h"
#include "../Utils/Helpers/ImportLibHelper.h"
#include "sys/mman.h"
#include "stdlib.h"
#include "../Utils/AssemblyUnhollower/XrefScannerBindings.h"
#include <android/log.h>
#include "bHapticsPlayer.h"

#include "../Managers/bHapticsPlayer.h"


#include <dlfcn.h>

#include "BaseAssembly.h"

void InternalCalls::Initialize()
{
	Debug::Msg("Initializing Internal Calls...");
    MelonLogger::AddInternalCalls();
    MelonUtils::AddInternalCalls();
    MelonDebug::AddInternalCalls();
    SupportModules::AddInternalCalls();
    UnhollowerIl2Cpp::AddInternalCalls();
    BHaptics::AddInternalCalls();
}

#pragma region MelonLogger
void InternalCalls::MelonLogger::Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, Mono::String* namesection, Mono::String* txt)
{
    auto nsStr = namesection != NULL ? Mono::Exports::mono_string_to_utf8(namesection) : NULL;
    auto txtStr = Mono::Exports::mono_string_to_utf8(txt);
    Logger::Internal_Msg(meloncolor, txtcolor, nsStr, txtStr);
    if (nsStr != NULL) Mono::Free(nsStr);
    Mono::Free(txtStr);
}

void InternalCalls::MelonLogger::Internal_PrintModName(Console::Color meloncolor, Mono::String* name, Mono::String* version)
{
    auto nameStr = Mono::Exports::mono_string_to_utf8(name);
    auto versionStr = Mono::Exports::mono_string_to_utf8(version);
    Logger::Internal_PrintModName(meloncolor, nameStr, versionStr);
    Mono::Free(nameStr);
    Mono::Free(versionStr);
}

void InternalCalls::MelonLogger::Internal_Warning(Mono::String* namesection, Mono::String* txt)
{
    auto nsStr = namesection != NULL ? Mono::Exports::mono_string_to_utf8(namesection) : NULL;
    auto txtStr = Mono::Exports::mono_string_to_utf8(txt);
    Logger::Internal_Warning(nsStr, txtStr);
    if (nsStr != NULL) Mono::Free(nsStr);
    Mono::Free(txtStr);
}

void InternalCalls::MelonLogger::Internal_Error(Mono::String* namesection, Mono::String* txt)
{
    auto nsStr = namesection != NULL ? Mono::Exports::mono_string_to_utf8(namesection) : NULL;
    auto txtStr = Mono::Exports::mono_string_to_utf8(txt);
    Logger::Internal_Error(nsStr, txtStr);
    if (nsStr != NULL) Mono::Free(nsStr);
    Mono::Free(txtStr);
}

void InternalCalls::MelonLogger::ThrowInternalFailure(Mono::String* msg)
{
    auto str = Mono::Exports::mono_string_to_utf8(msg);
    Assertion::ThrowInternalFailure(str);
    Mono::Free(str);
}

void InternalCalls::MelonLogger::WriteSpacer() { Logger::WriteSpacer(); }
void InternalCalls::MelonLogger::Flush() { 
#ifdef PORT_DISABLE
    Logger::Flush(); Console::Flush(); 
#endif
}
void InternalCalls::MelonLogger::AddInternalCalls()
{
    Mono::AddInternalCall("MelonLoader.MelonLogger::Internal_PrintModName", (void*)Internal_PrintModName);
    Mono::AddInternalCall("MelonLoader.MelonLogger::Internal_Msg", (void*)Internal_Msg);
    Mono::AddInternalCall("MelonLoader.MelonLogger::Internal_Warning", (void*)Internal_Warning);
    Mono::AddInternalCall("MelonLoader.MelonLogger::Internal_Error", (void*)Internal_Error);
    Mono::AddInternalCall("MelonLoader.MelonLogger::ThrowInternalFailure", (void*)ThrowInternalFailure);
    Mono::AddInternalCall("MelonLoader.MelonLogger::WriteSpacer", (void*)WriteSpacer);
    Mono::AddInternalCall("MelonLoader.MelonLogger::Flush", (void*)Flush);
}
#pragma endregion

#pragma region MelonUtils
bool InternalCalls::MelonUtils::IsGame32Bit()
{
#ifdef _WIN64
    return false;
#else
    return true;
#endif
}
bool InternalCalls::MelonUtils::IsGameIl2Cpp() { return Game::IsIl2Cpp; }
bool InternalCalls::MelonUtils::IsOldMono() { return Mono::IsOldMono; }
Mono::String* InternalCalls::MelonUtils::GetApplicationPath() { return Mono::Exports::mono_string_new(Mono::domain, Game::ApplicationPath); }
Mono::String* InternalCalls::MelonUtils::GetGamePackage() { return Mono::Exports::mono_string_new(Mono::domain, Game::Package); }
Mono::String* InternalCalls::MelonUtils::GetGameName() { return Mono::Exports::mono_string_new(Mono::domain, Game::Name); }
Mono::String* InternalCalls::MelonUtils::GetGameDeveloper() { return Mono::Exports::mono_string_new(Mono::domain, Game::Developer); }
Mono::String* InternalCalls::MelonUtils::GetGameDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Game::BasePath); }
Mono::String* InternalCalls::MelonUtils::GetGameDataDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Game::DataPath); }
Mono::String* InternalCalls::MelonUtils::GetUnityVersion() { return Mono::Exports::mono_string_new(Mono::domain, Game::UnityVersion); }
Mono::String* InternalCalls::MelonUtils::GetManagedDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Mono::ManagedPath); }
Mono::String* InternalCalls::MelonUtils::GetMainAssemblyLoc() { return Mono::Exports::mono_string_new(Mono::domain, Il2Cpp::LibPath); }
#ifdef PORT_DISABLE
Mono::String* InternalCalls::MelonUtils::GetHashCode() { return Mono::Exports::mono_string_new(Mono::domain, HashCode::Hash.c_str()); }
#else 
Mono::String* InternalCalls::MelonUtils::GetHashCode() { return Mono::Exports::mono_string_new(Mono::domain, "Placeholder Hash"); }
#endif
void InternalCalls::MelonUtils::SCT(Mono::String* title)
{
#ifdef PORT_DISABLE
    if (title == NULL) return;
    auto str = Mono::Exports::mono_string_to_utf8(title);
    Console::SetTitle(str);
    Mono::Free(str);
#else 
    return;
#endif
}
Mono::String* InternalCalls::MelonUtils::GetFileProductName(Mono::String* filepath)
{
    char* filepathstr = Mono::Exports::mono_string_to_utf8(filepath);
    if (filepathstr == NULL)
        return NULL;
    const char* info = Core::GetFileInfoProductName(filepathstr);
    Mono::Free(filepathstr);
    if (info == NULL)
        return NULL;
    return Mono::Exports::mono_string_new(Mono::domain, info);
}

void InternalCalls::MelonUtils::GetStaticSettings(StaticSettings::Settings_t &settings)
{
    memcpy(&settings, &StaticSettings::Settings, sizeof(StaticSettings::Settings_t));
}

void InternalCalls::MelonUtils::AddInternalCalls()
{
    Mono::AddInternalCall("MelonLoader.MelonUtils::IsGame32Bit", (void*)IsGame32Bit);
    Mono::AddInternalCall("MelonLoader.MelonUtils::IsGameIl2Cpp", (void*)IsGameIl2Cpp);
    Mono::AddInternalCall("MelonLoader.MelonUtils::IsOldMono", (void*)IsOldMono);
    Mono::AddInternalCall("MelonLoader.MelonUtils::GetApplicationPath", (void*)GetApplicationPath);
    Mono::AddInternalCall("MelonLoader.MelonUtils::GetGameDataDirectory", (void*)GetGameDataDirectory);
    Mono::AddInternalCall("MelonLoader.MelonUtils::GetMainAssemblyLoc", (void*)GetMainAssemblyLoc);
    Mono::AddInternalCall("MelonLoader.MelonUtils::GetUnityVersion", (void*)GetUnityVersion);
    Mono::AddInternalCall("MelonLoader.MelonUtils::GetManagedDirectory", (void*)GetManagedDirectory);
    Mono::AddInternalCall("MelonLoader.MelonUtils::SetConsoleTitle", (void*)SCT);
    Mono::AddInternalCall("MelonLoader.MelonUtils::GetFileProductName", (void*)GetFileProductName);
    Mono::AddInternalCall("MelonLoader.MelonUtils::NativeHookAttach", (void*)Hook::Attach);
    Mono::AddInternalCall("MelonLoader.MelonUtils::NativeHookDetach", (void*)Hook::Detach);

    Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetGamePackage", (void*)GetGamePackage);
    Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetGameName", (void*)GetGameName);
    Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetGameDeveloper", (void*)GetGameDeveloper);
    Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetGameDirectory", (void*)GetGameDirectory);
    Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetHashCode", (void*)GetHashCode);
    
    Mono::AddInternalCall("MelonLoader.MelonUtils::GetStaticSettings", (void*)GetStaticSettings);
}
#pragma endregion

#pragma region MelonDebug
void InternalCalls::MelonDebug::Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, Mono::String* namesection, Mono::String* txt)
{
    auto nsStr = namesection != NULL ? Mono::Exports::mono_string_to_utf8(namesection) : NULL;
    auto txtStr = Mono::Exports::mono_string_to_utf8(txt);
    Debug::Internal_Msg(meloncolor, txtcolor, nsStr, txtStr);
    if (nsStr != NULL) Mono::Free(nsStr);
    Mono::Free(txtStr);
}
void InternalCalls::MelonDebug::AddInternalCalls()
{
    Mono::AddInternalCall("MelonLoader.MelonDebug::Internal_Msg", (void*)Internal_Msg);
}
#pragma endregion

#pragma region SupportModules
void InternalCalls::SupportModules::SetDefaultConsoleTitleWithGameName(Mono::String* GameVersion) { 
#ifdef PORT_DISABLE
    Console::SetDefaultTitleWithGameName(GameVersion != NULL ? Mono::Exports::mono_string_to_utf8(GameVersion) : NULL);
#endif
}
void InternalCalls::SupportModules::AddInternalCalls()
{
    Mono::AddInternalCall("MelonLoader.Support.Preload::GetManagedDirectory", (void*)MelonUtils::GetManagedDirectory);
    Mono::AddInternalCall("MelonLoader.Support.Main::SetDefaultConsoleTitleWithGameName", (void*)SetDefaultConsoleTitleWithGameName);
}
#pragma endregion

#pragma region UnhollowerIl2Cpp
void InternalCalls::UnhollowerIl2Cpp::AddInternalCalls()
{
    Mono::AddInternalCall("UnhollowerRuntimeLib.ClassInjector::GetProcAddress", (void*)GetProcAddress);
    Mono::AddInternalCall("UnhollowerRuntimeLib.ClassInjector::LoadLibrary", (void*)LoadLibrary);

    Mono::AddInternalCall("UnhollowerRuntimeLib.XrefScans.CSHelper::GetAsmLoc", (void*)GetAsmLoc);
    Mono::AddInternalCall("UnhollowerRuntimeLib.XrefScans.CSHelper::CleanupDisasm_Native", (void*)CleanupDisasm);

    Mono::AddInternalCall("UnhollowerRuntimeLib.XrefScans.XrefScanner::XrefScanImpl_Native", (void*)XrefScannerBindings::XrefScanner::XrefScanImplNative);

    Mono::AddInternalCall("UnhollowerRuntimeLib.XrefScans.XrefScannerLowLevel::JumpTargetsImpl_Native", (void*)XrefScannerBindings::XrefScannerLowLevel::JumpTargetsImpl);

    Mono::AddInternalCall("UnhollowerRuntimeLib.XrefScans.XrefScanUtilFinder::FindLastRcxReadAddressBeforeCallTo_Native", (void*)XrefScannerBindings::XrefScanUtilFinder::FindLastRcxReadAddressBeforeCallTo);
    Mono::AddInternalCall("UnhollowerRuntimeLib.XrefScans.XrefScanUtilFinder::FindByteWriteTargetRightAfterCallTo_Native", (void*)XrefScannerBindings::XrefScanUtilFinder::FindByteWriteTargetRightAfterCallTo);
}

void* InternalCalls::UnhollowerIl2Cpp::GetProcAddress(void* hModule, Mono::String* procName)
{
    char* parsedSym = Mono::Exports::mono_string_to_utf8(procName);
    void* res = dlsym(hModule, parsedSym);
    Mono::Free(parsedSym);
    return res;
}

void* InternalCalls::UnhollowerIl2Cpp::LoadLibrary(Mono::String* lpFileName)
{
    char* parsedLib = Mono::Exports::mono_string_to_utf8(lpFileName);
    Debug::Msg(parsedLib);
    if (strcmp(parsedLib, "GameAssembly.dll") == 0)
        return Il2Cpp::Handle;

    return dlopen(parsedLib, RTLD_NOW | RTLD_GLOBAL);
}

void* InternalCalls::UnhollowerIl2Cpp::GetAsmLoc()
{
    return Il2Cpp::MemLoc;
}

void InternalCalls::UnhollowerIl2Cpp::CleanupDisasm()
{
    Debug::Msg("CleanupDisasm not implemented");
}
#pragma endregion

#pragma region bHaptics

void InternalCalls::BHaptics::AddInternalCalls()
{
    Mono::Class* playerKlass = Mono::Exports::mono_class_from_name(BaseAssembly::Image, "MelonLoader", "bHaptics_NativeLibrary");
    Mono::Class* connectionManagerKlass = Mono::Exports::mono_class_from_name(BaseAssembly::Image, "MelonLoader.ConnectionManager", "ConnectionManager");
    if (playerKlass == NULL || connectionManagerKlass == NULL)
    {
        Assertion::ThrowInternalFailure("Failed to Get Class from Mono Image!");
        return;
    }
    
    Mono_Invoke_OnChange = Mono::Exports::mono_class_get_method_from_name(playerKlass, "Invoke_OnChange", NULL);
    
    Mono_Invoke_OnDeviceUpdate = Mono::Exports::mono_class_get_method_from_name(connectionManagerKlass, "Invoke_OnDeviceUpdate", NULL);
    Mono_Invoke_OnScanStatusChange = Mono::Exports::mono_class_get_method_from_name(connectionManagerKlass, "Invoke_OnScanStatusChange", NULL);
    Mono_Invoke_OnChangeResponse = Mono::Exports::mono_class_get_method_from_name(connectionManagerKlass, "Invoke_OnChangeResponse", NULL);
    Mono_Invoke_OnConnect = Mono::Exports::mono_class_get_method_from_name(connectionManagerKlass, "Invoke_OnConnect", NULL);
    Mono_Invoke_OnDisconnect = Mono::Exports::mono_class_get_method_from_name(connectionManagerKlass, "Invoke_OnDisconnect", NULL);
    if (
        Mono_Invoke_OnChange == NULL ||
        Mono_Invoke_OnDeviceUpdate == NULL ||
        Mono_Invoke_OnScanStatusChange == NULL ||
        Mono_Invoke_OnChangeResponse == NULL ||
        Mono_Invoke_OnConnect == NULL ||
        Mono_Invoke_OnDisconnect == NULL
        )
    {
        Assertion::ThrowInternalFailure("Failed to Get Method from Class!");
        return;
    }
    
    // native parser
    Mono::AddInternalCall("MelonLoader.NativeParser::ReleaseAddress", (void*)ReleaseAddress);

    // player
    Mono::AddInternalCall("MelonLoader.bHaptics_NativeLibrary::TurnOff", (void*)TurnOff);
    Mono::AddInternalCall("MelonLoader.bHaptics_NativeLibrary::TurnOffAll", (void*)TurnOffAll);
    Mono::AddInternalCall("MelonLoader.bHaptics_NativeLibrary::RegisterProject", (void*)RegisterProject);
    Mono::AddInternalCall("MelonLoader.bHaptics_NativeLibrary::RegisterProjectReflected", (void*)RegisterProjectReflected);
    Mono::AddInternalCall("MelonLoader.bHaptics_NativeLibrary::SubmitRegistered", (void*)SubmitRegistered);
    Mono::AddInternalCall("MelonLoader.bHaptics_NativeLibrary::SubmitRegisteredWithTime", (void*)SubmitRegisteredWithTime);
    Mono::AddInternalCall("MelonLoader.bHaptics_NativeLibrary::IsRegistered", (void*)IsRegistered);
    Mono::AddInternalCall("MelonLoader.bHaptics_NativeLibrary::IsPlaying", (void*)IsPlaying);
    Mono::AddInternalCall("MelonLoader.bHaptics_NativeLibrary::IsAnythingPlaying", (void*)IsAnythingPlaying);
    Mono::AddInternalCall("MelonLoader.bHaptics_NativeLibrary::Internal_SubmitDot", (void*)Internal_SubmitDot);
    Mono::AddInternalCall("MelonLoader.bHaptics_NativeLibrary::Internal_SubmitPath", (void*)Internal_SubmitPath);
    Mono::AddInternalCall("MelonLoader.bHaptics_NativeLibrary::Internal_GetPositionStatus", (void*)Internal_GetPositionStatus);

    // connection manager
    Mono::AddInternalCall("MelonLoader.ConnectionManager.ConnectionManager::Scan", (void*)Scan);
    Mono::AddInternalCall("MelonLoader.ConnectionManager.ConnectionManager::StopScan", (void*)StopScan);
    Mono::AddInternalCall("MelonLoader.ConnectionManager.ConnectionManager::RefreshPairingInfo", (void*)RefreshPairingInfo);
    Mono::AddInternalCall("MelonLoader.ConnectionManager.ConnectionManager::Unpair", (void*)Unpair);
    Mono::AddInternalCall("MelonLoader.ConnectionManager.ConnectionManager::UnpairAll", (void*)UnpairAll);
    Mono::AddInternalCall("MelonLoader.ConnectionManager.ConnectionManager::TogglePosition", (void*)TogglePosition);
    Mono::AddInternalCall("MelonLoader.ConnectionManager.ConnectionManager::Ping", (void*)Ping);
    Mono::AddInternalCall("MelonLoader.ConnectionManager.ConnectionManager::PingAll", (void*)PingAll);
    Mono::AddInternalCall("MelonLoader.ConnectionManager.ConnectionManager::IsDeviceConnected", (void*)IsDeviceConnected);
    Mono::AddInternalCall("MelonLoader.ConnectionManager.ConnectionManager::Internal_Pair", (void*)Internal_Pair);
    Mono::AddInternalCall("MelonLoader.ConnectionManager.ConnectionManager::Internal_PairPositioned", (void*)Internal_PairPositioned);
    Mono::AddInternalCall("MelonLoader.ConnectionManager.ConnectionManager::Internal_GetIsScanning", (void*)Internal_GetIsScanning);
    Mono::AddInternalCall("MelonLoader.ConnectionManager.ConnectionManager::Internal_ChangePosition", (void*)Internal_ChangePosition);
    Mono::AddInternalCall("MelonLoader.ConnectionManager.ConnectionManager::Internal_SetMotor", (void*)Internal_SetMotor);
    Mono::AddInternalCall("MelonLoader.ConnectionManager.ConnectionManager::Internal_GetDeviceList", (void*)Internal_GetDeviceList);

    // device
    Mono::AddInternalCall("MelonLoader.Utils.bHapticsExtra.BhapticsDevice::Internal_IsPing", (void*)Internal_IsPing);
    Mono::AddInternalCall("MelonLoader.Utils.bHapticsExtra.BhapticsDevice::Internal_IsPaired", (void*)Internal_IsPaired);
    Mono::AddInternalCall("MelonLoader.Utils.bHapticsExtra.BhapticsDevice::Internal_GetConnectFailCount", (void*)Internal_GetConnectFailCount);
    Mono::AddInternalCall("MelonLoader.Utils.bHapticsExtra.BhapticsDevice::Internal_GetRssi", (void*)Internal_GetRssi);
    Mono::AddInternalCall("MelonLoader.Utils.bHapticsExtra.BhapticsDevice::Internal_GetConnectionStatus", (void*)Internal_GetConnectionStatus);
    Mono::AddInternalCall("MelonLoader.Utils.bHapticsExtra.BhapticsDevice::Internal_GetPosition", (void*)Internal_GetPosition);
    Mono::AddInternalCall("MelonLoader.Utils.bHapticsExtra.BhapticsDevice::Internal_GetAddress", (void*)Internal_GetAddress);
    Mono::AddInternalCall("MelonLoader.Utils.bHapticsExtra.BhapticsDevice::Internal_GetDeviceName", (void*)Internal_GetDeviceName);
    Mono::AddInternalCall("MelonLoader.Utils.bHapticsExtra.BhapticsDevice::Internal_GetBattery", (void*)Internal_GetBattery);
    Mono::AddInternalCall("MelonLoader.Utils.bHapticsExtra.BhapticsDevice::Internal_GetType", (void*)Internal_GetType);
    Mono::AddInternalCall("MelonLoader.Utils.bHapticsExtra.BhapticsDevice::Internal_GetLastBytes", (void*)Internal_GetLastBytes);
    Mono::AddInternalCall("MelonLoader.Utils.bHapticsExtra.BhapticsDevice::Internal_GetLastScannedTime", (void*)Internal_GetLastScannedTime);
    Mono::AddInternalCall("MelonLoader.Utils.bHapticsExtra.BhapticsDevice::Internal_ToString", (void*)Internal_ToString);
}

intptr_t InternalCalls::BHaptics::ConvertJavaToMonoDeviceList(std::vector<jobject>& jDeviceList)
{
    char** resCData = new char*[jDeviceList.size()];

    for (size_t i = 0; i < jDeviceList.size(); i++)
    {
        const char* address = bHapticsPlayer::BhapticsDevice::GetAddress(jDeviceList[i]);

        resCData[i] = (char*)malloc(strlen(address));
        strcpy(resCData[i], address);
    }
    
    int64_t* res = new int64_t[2] { jDeviceList.size(), (int64_t)resCData };

    return (intptr_t)res;
}

int InternalCalls::BHaptics::ReleaseAddress(intptr_t* address)
{
    delete[] address;
    return 0;
}

#pragma region Actions

void InternalCalls::BHaptics::Invoke_OnChange()
{
    Mono::Object* exObj = NULL;
    Mono::Exports::mono_runtime_invoke(Mono_Invoke_OnChange, NULL, NULL, &exObj);

    if (exObj != NULL)
        Mono::LogException(exObj);
}

void InternalCalls::BHaptics::Invoke_OnDeviceUpdate(std::vector<jobject> deviceArr)
{
    void** args = new void*[1] { (void*)ConvertJavaToMonoDeviceList(deviceArr) };
    
    Mono::Object* exObj = NULL;
    Mono::Exports::mono_runtime_invoke(Mono_Invoke_OnConnect, NULL, args, &exObj);

    if (exObj != NULL)
        Mono::LogException(exObj);
}

void InternalCalls::BHaptics::Invoke_OnScanStatusChange(bool isScanning)
{
    void** args = new void*[1] { &isScanning };
    
    Mono::Object* exObj = NULL;
    Mono::Exports::mono_runtime_invoke(Mono_Invoke_OnScanStatusChange, NULL, args, &exObj);

    if (exObj != NULL)
        Mono::LogException(exObj);
}

void InternalCalls::BHaptics::Invoke_OnChangeResponse()
{
    Mono::Object* exObj = NULL;
    Mono::Exports::mono_runtime_invoke(Mono_Invoke_OnChangeResponse, NULL, NULL, &exObj);

    if (exObj != NULL)
        Mono::LogException(exObj);
}

void InternalCalls::BHaptics::Invoke_OnConnect(const char* address)
{
    void** args = new void*[1] { Mono::Exports::mono_string_new(Mono::domain, address) };
    
    Mono::Object* exObj = NULL;
    Mono::Exports::mono_runtime_invoke(Mono_Invoke_OnConnect, NULL, args, &exObj);

    if (exObj != NULL)
        Mono::LogException(exObj);
}

void InternalCalls::BHaptics::Invoke_OnDisconnect(const char* address)
{
    void** args = new void*[1] { Mono::Exports::mono_string_new(Mono::domain, address) };
    
    Mono::Object* exObj = NULL;
    Mono::Exports::mono_runtime_invoke(Mono_Invoke_OnDisconnect, NULL, args, &exObj);

    if (exObj != NULL)
        Mono::LogException(exObj);
}

#pragma endregion 

#pragma region HapticPlayer

void InternalCalls::BHaptics::TurnOff(Mono::String* key)
{
    char* cKey = Mono::Exports::mono_string_to_utf8(key);
    
    Debug::Msgf("InternalCalls::BHaptics::TurnOff %s", cKey);
    bHapticsPlayer::HapticPlayer::TurnOff(cKey);
    
    Mono::Free(cKey);
}

void InternalCalls::BHaptics::TurnOffAll()
{
    Debug::Msgf("InternalCalls::BHaptics::TurnOffAll");
    bHapticsPlayer::HapticPlayer::TurnOffAll();
}

void InternalCalls::BHaptics::RegisterProject(Mono::String* key, Mono::String* contents)
{
    char* cKey = Mono::Exports::mono_string_to_utf8(key);
    char* cContents = Mono::Exports::mono_string_to_utf8(contents);
    
    Debug::Msgf("InternalCalls::BHaptics::RegisterProject %s", cKey);
    bHapticsPlayer::HapticPlayer::RegisterProject(cKey, cContents);
    
    Mono::Free(cKey);
    Mono::Free(cContents);
}

void InternalCalls::BHaptics::RegisterProjectReflected(Mono::String* key, Mono::String* contents)
{
    char* cKey = Mono::Exports::mono_string_to_utf8(key);
    char* cContents = Mono::Exports::mono_string_to_utf8(contents);
    
    Debug::Msgf("InternalCalls::BHaptics::RegisterProjectReflected %s", cKey);
    bHapticsPlayer::HapticPlayer::RegisterProjectReflected(cKey, cContents);
    
    Mono::Free(cKey);
    Mono::Free(cContents);
}

void InternalCalls::BHaptics::SubmitRegistered(Mono::String* key, Mono::String* altKey, float intensity, float duration,
    float offsetAngleX, float offsetY)
{
    char* cKey = Mono::Exports::mono_string_to_utf8(key);
    char* cAltKey = Mono::Exports::mono_string_to_utf8(altKey);
    
    Debug::Msgf("InternalCalls::BHaptics::SubmitRegistered %s %s (%f, %f, %f, %f)", cKey, cAltKey, intensity, duration, offsetAngleX, offsetY);
    bHapticsPlayer::HapticPlayer::SubmitRegistered(cKey, cAltKey, intensity, duration, offsetAngleX, offsetY);
    
    Mono::Free(cKey);
    Mono::Free(cAltKey);
}

void InternalCalls::BHaptics::SubmitRegisteredWithTime(Mono::String* key, int startTime)
{
    char* cKey = Mono::Exports::mono_string_to_utf8(key);
    
    Debug::Msgf("InternalCalls::BHaptics::SubmitRegisteredWithTime %s %d", cKey, startTime);
    bHapticsPlayer::HapticPlayer::SubmitRegisteredWithTime(cKey, startTime);
    
    Mono::Free(cKey);
}

bool InternalCalls::BHaptics::IsRegistered(Mono::String* key)
{
    char* cKey = Mono::Exports::mono_string_to_utf8(key);
    
    Debug::Msgf("InternalCalls::BHaptics::IsRegistered %s", cKey);
    auto res = bHapticsPlayer::HapticPlayer::IsRegistered(cKey);
    
    Mono::Free(cKey);

    return res;
}

bool InternalCalls::BHaptics::IsPlaying(Mono::String* key)
{
    char* cKey = Mono::Exports::mono_string_to_utf8(key);
    
    Debug::Msgf("InternalCalls::BHaptics::IsPlaying %s", cKey);
    auto res = bHapticsPlayer::HapticPlayer::IsPlaying(cKey);
    
    Mono::Free(cKey);

    return res;
}

bool InternalCalls::BHaptics::IsAnythingPlaying()
{
    Debug::Msgf("InternalCalls::BHaptics::IsAnythingPlaying");
    return bHapticsPlayer::HapticPlayer::IsAnythingPlaying();
}

void InternalCalls::BHaptics::Internal_SubmitDot(Mono::String* key, Mono::String* position, int* indexes,
    int* intensities, int* sizes, int duration)
{
    char* cKey = Mono::Exports::mono_string_to_utf8(key);
    char* cPosition = Mono::Exports::mono_string_to_utf8(position);

    size_t indexes_size = sizes[0];
    size_t intensities_size = sizes[1];
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_SubmitDot %s %s (%d, %d)", cKey, cPosition, indexes_size, intensities_size);
    bHapticsPlayer::HapticPlayer::SubmitDot(cKey, cPosition, indexes, indexes_size, intensities, intensities_size, duration);
    
    Mono::Free(cKey);
    Mono::Free(cPosition);
}

void InternalCalls::BHaptics::Internal_SubmitPath(Mono::String* key, Mono::String* position, float* x, float* y,
    int* intensities, int* sizes, int duration)
{
    char* cKey = Mono::Exports::mono_string_to_utf8(key);
    char* cPosition = Mono::Exports::mono_string_to_utf8(position);

    size_t x_size = sizes[0];
    size_t y_size = sizes[1];
    size_t intensities_size = sizes[2];
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_SubmitPath %s %s (%d, %d, %d)", cKey, cPosition, x_size, y_size, intensities_size);
    bHapticsPlayer::HapticPlayer::SubmitPath(cKey, cPosition, x, x_size, y, y_size, intensities, intensities_size, duration);
    
    Mono::Free(cKey);
    Mono::Free(cPosition);
}

intptr_t InternalCalls::BHaptics::Internal_GetPositionStatus(Mono::String* position)
{
    char* cPosition = Mono::Exports::mono_string_to_utf8(position);
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_GetPositionStatus %s", cPosition);
    auto resData = bHapticsPlayer::HapticPlayer::GetPositionStatus(cPosition);
    
    Mono::Free(cPosition);

    char* resCData = new char[resData.size()];
    std::copy(resData.begin(), resData.end(), resCData);
    
    int64_t* res = new int64_t[2] { resData.size(), (int64_t)resCData };

    return (intptr_t)res;
}

#pragma endregion

#pragma region BhapticsManager

void InternalCalls::BHaptics::Scan()
{
    Debug::Msgf("InternalCalls::BHaptics::Scan");
    bHapticsPlayer::BhapticsManager::Scan();
}

void InternalCalls::BHaptics::StopScan()
{
    Debug::Msgf("InternalCalls::BHaptics::StopScan");
    bHapticsPlayer::BhapticsManager::StopScan();
}

void InternalCalls::BHaptics::RefreshPairingInfo()
{
    Debug::Msgf("InternalCalls::BHaptics::RefreshPairingInfo");
    bHapticsPlayer::BhapticsManager::RefreshPairingInfo();
}

void InternalCalls::BHaptics::Unpair(Mono::String* address)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    
    Debug::Msgf("InternalCalls::BHaptics::Unpair %s", cAddress);
    bHapticsPlayer::BhapticsManager::Unpair(cAddress);
    
    Mono::Free(cAddress);
}

void InternalCalls::BHaptics::UnpairAll()
{
    Debug::Msgf("InternalCalls::BHaptics::UnpairAll");
    bHapticsPlayer::BhapticsManager::UnpairAll();
}

void InternalCalls::BHaptics::TogglePosition(Mono::String* address)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    
    Debug::Msgf("InternalCalls::BHaptics::TogglePosition %s", cAddress);
    bHapticsPlayer::BhapticsManager::TogglePosition(cAddress);
    
    Mono::Free(cAddress);
}

void InternalCalls::BHaptics::Ping(Mono::String* address)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    
    Debug::Msgf("InternalCalls::BHaptics::Ping %s", cAddress);
    bHapticsPlayer::BhapticsManager::Ping(cAddress);
    
    Mono::Free(cAddress);
}

void InternalCalls::BHaptics::PingAll()
{
    Debug::Msgf("InternalCalls::BHaptics::PingAll");
    bHapticsPlayer::BhapticsManager::PingAll();
}

bool InternalCalls::BHaptics::IsDeviceConnected(Mono::String* address)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    
    Debug::Msgf("InternalCalls::BHaptics::IsDeviceConnected %s", cAddress);
    auto res = bHapticsPlayer::BhapticsManager::IsDeviceConnected(cAddress);
    
    Mono::Free(cAddress);

    return res;
}

void InternalCalls::BHaptics::Internal_Pair(Mono::String* address)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_Pair %s", cAddress);
    bHapticsPlayer::BhapticsManager::Pair(cAddress);
    
    Mono::Free(cAddress);
}

void InternalCalls::BHaptics::Internal_PairPositioned(Mono::String* address, Mono::String* position)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    char* cPosition = Mono::Exports::mono_string_to_utf8(position);
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_PairPositioned %s %s", cAddress, cPosition);
    bHapticsPlayer::BhapticsManager::PairPositioned(cAddress, cPosition);
    
    Mono::Free(cAddress);
    Mono::Free(cPosition);
}

bool InternalCalls::BHaptics::Internal_GetIsScanning()
{
    Debug::Msgf("InternalCalls::BHaptics::Internal_GetIsScanning");
    return bHapticsPlayer::BhapticsManager::IsScanning();
}

void InternalCalls::BHaptics::Internal_ChangePosition(Mono::String* address, Mono::String* position)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    char* cPosition = Mono::Exports::mono_string_to_utf8(position);
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_ChangePosition %s %s", cAddress, cPosition);
    bHapticsPlayer::BhapticsManager::ChangePosition(cAddress, cPosition);
    
    Mono::Free(cAddress);
    Mono::Free(cPosition);
}

void InternalCalls::BHaptics::Internal_SetMotor(Mono::String* address, char* bytes, size_t size)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_SetMotor %s %d", cAddress, size);
    bHapticsPlayer::BhapticsManager::SetMotor(cAddress, bytes, size);
    
    Mono::Free(cAddress);
}

intptr_t InternalCalls::BHaptics::Internal_GetDeviceList()
{
    Debug::Msgf("InternalCalls::BHaptics::Internal_GetDeviceList");
    auto resData = bHapticsPlayer::BhapticsManager::GetDeviceList();
    return ConvertJavaToMonoDeviceList(resData);
}

#pragma endregion 

#pragma region BhapticsDevice

bool InternalCalls::BHaptics::Internal_IsPing(Mono::String* address)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_IsPing %s", cAddress);
    auto res = bHapticsPlayer::BhapticsDevice::IsPing(bHapticsPlayer::BhapticsDevice::GetDevice(cAddress));
    
    Mono::Free(cAddress);

    return res;
}

bool InternalCalls::BHaptics::Internal_IsPaired(Mono::String* address)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_IsPaired %s", cAddress);
    auto res = bHapticsPlayer::BhapticsDevice::IsPaired(bHapticsPlayer::BhapticsDevice::GetDevice(cAddress));
    
    Mono::Free(cAddress);

    return res;
}

int InternalCalls::BHaptics::Internal_GetConnectFailCount(Mono::String* address)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_GetConnectFailCount %s", cAddress);
    auto res = bHapticsPlayer::BhapticsDevice::GetConnectFailedCount(bHapticsPlayer::BhapticsDevice::GetDevice(cAddress));
    
    Mono::Free(cAddress);

    return res;
}

int InternalCalls::BHaptics::Internal_GetRssi(Mono::String* address)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_GetRssi %s", cAddress);
    auto res = bHapticsPlayer::BhapticsDevice::GetRSSI(bHapticsPlayer::BhapticsDevice::GetDevice(cAddress));
    
    Mono::Free(cAddress);

    return res;
}

Mono::String* InternalCalls::BHaptics::Internal_GetConnectionStatus(Mono::String* address)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_GetConnectionStatus %s", cAddress);
    auto res = bHapticsPlayer::BhapticsDevice::GetConnectionStatus(bHapticsPlayer::BhapticsDevice::GetDevice(cAddress));
    
    Mono::Free(cAddress);
    
    return Mono::Exports::mono_string_new(Mono::domain, res);
}

Mono::String* InternalCalls::BHaptics::Internal_GetPosition(Mono::String* address)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_GetPosition %s", cAddress);
    auto res = bHapticsPlayer::BhapticsDevice::GetPosition(bHapticsPlayer::BhapticsDevice::GetDevice(cAddress));
    
    Mono::Free(cAddress);
    
    return Mono::Exports::mono_string_new(Mono::domain, res);
}

Mono::String* InternalCalls::BHaptics::Internal_GetAddress(Mono::String* address)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_GetAddress %s", cAddress);
    auto res = bHapticsPlayer::BhapticsDevice::GetAddress(bHapticsPlayer::BhapticsDevice::GetDevice(cAddress));
    
    Mono::Free(cAddress);
    
    return Mono::Exports::mono_string_new(Mono::domain, res);
}

Mono::String* InternalCalls::BHaptics::Internal_GetDeviceName(Mono::String* address)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_GetDeviceName %s", cAddress);
    auto res = bHapticsPlayer::BhapticsDevice::GetDeviceName(bHapticsPlayer::BhapticsDevice::GetDevice(cAddress));
    
    Mono::Free(cAddress);
    
    return Mono::Exports::mono_string_new(Mono::domain, res);
}

int InternalCalls::BHaptics::Internal_GetBattery(Mono::String* address)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_GetBattery %s", cAddress);
    auto res = bHapticsPlayer::BhapticsDevice::GetBattery(bHapticsPlayer::BhapticsDevice::GetDevice(cAddress));
    
    Mono::Free(cAddress);

    return res;
}

Mono::String* InternalCalls::BHaptics::Internal_GetType(Mono::String* address)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_GetType %s", cAddress);
    auto res = bHapticsPlayer::BhapticsDevice::GetType(bHapticsPlayer::BhapticsDevice::GetDevice(cAddress));
    
    Mono::Free(cAddress);
    
    return Mono::Exports::mono_string_new(Mono::domain, res);
}

intptr_t InternalCalls::BHaptics::Internal_GetLastBytes(Mono::String* address)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_GetLastBytes %s", cAddress);
    auto resData = bHapticsPlayer::BhapticsDevice::GetLastBytes(bHapticsPlayer::BhapticsDevice::GetDevice(cAddress));
    
    Mono::Free(cAddress);

    char* resCData = new char[resData.size()];
    std::copy(resData.begin(), resData.end(), resCData);
    
    int64_t* res = new int64_t[2] { resData.size(), (int64_t)resCData };

    return (intptr_t)res;
}

int64_t InternalCalls::BHaptics::Internal_GetLastScannedTime(Mono::String* address)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_GetLastScannedTime %s", cAddress);
    auto res = bHapticsPlayer::BhapticsDevice::GetLastScannedTime(bHapticsPlayer::BhapticsDevice::GetDevice(cAddress));
    
    Mono::Free(cAddress);

    return res;
}

Mono::String* InternalCalls::BHaptics::Internal_ToString(Mono::String* address)
{
    char* cAddress = Mono::Exports::mono_string_to_utf8(address);
    
    Debug::Msgf("InternalCalls::BHaptics::Internal_ToString %s", cAddress);
    auto res = bHapticsPlayer::BhapticsDevice::ToString(bHapticsPlayer::BhapticsDevice::GetDevice(cAddress));
    
    Mono::Free(cAddress);
    
    return Mono::Exports::mono_string_new(Mono::domain, res);
}

#pragma endregion

#pragma endregion
