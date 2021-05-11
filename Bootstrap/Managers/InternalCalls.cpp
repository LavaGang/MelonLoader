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


#include <dlfcn.h>

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
    Mono::AddInternalCall("MelonLoader.bHaptics::TestDotArray", (void*)TestDotArray);
}

void InternalCalls::BHaptics::TestDotArray(Mono::String* key, Mono::String* position, int* indexes, size_t indexes_len, int* intensity, size_t intensity_len, int duration)
{
    Debug::Msg("Native Test");
    char* cKey = Mono::Exports::mono_string_to_utf8(key);
    char* cPosition = Mono::Exports::mono_string_to_utf8(position);

    bHapticsPlayer::HapticPlayer::SubmitDot(cKey, cPosition, indexes, indexes_len, intensity, intensity_len, duration);

    Mono::Free(cKey);
    Mono::Free(cPosition);
}
#pragma endregion
