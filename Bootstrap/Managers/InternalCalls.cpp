#ifdef PORT_DISABLE
#include "InternalCalls.h"
#include "../Utils/Debug.h"
#include "../Utils/Logger.h"
#include "Game.h"
#include "Hook.h"
#include "../Utils/Assertion.h"
#include "../Base/Core.h"

void InternalCalls::Initialize()
{
	Debug::Msg("Initializing Internal Calls...");
	MelonCore::AddInternalCalls();
	MelonLogger::AddInternalCalls();
	MelonUtils::AddInternalCalls();
	MelonHandler::AddInternalCalls();
	MelonDebug::AddInternalCalls();
}

#pragma region MelonCore
bool InternalCalls::MelonCore::QuitFix() { return Core::QuitFix; }
void InternalCalls::MelonCore::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.Core::QuitFix", QuitFix);
}
#pragma endregion

#pragma region MelonLogger
void InternalCalls::MelonLogger::Internal_Msg(Console::Color color, Mono::String* namesection, Mono::String* txt)
{
	auto nsStr = namesection != NULL ? Mono::Exports::mono_string_to_utf8(namesection) : NULL;
	auto txtStr = Mono::Exports::mono_string_to_utf8(txt);
	Logger::Internal_Msg(color, nsStr, txtStr);
	if (nsStr != NULL) Mono::Free(nsStr);
	Mono::Free(txtStr);
}

void InternalCalls::MelonLogger::Internal_PrintModName(Console::Color color, Mono::String* name, Mono::String* version)
{
	auto nameStr = Mono::Exports::mono_string_to_utf8(name);
	auto versionStr = Mono::Exports::mono_string_to_utf8(version);
	Logger::Internal_PrintModName(color, nameStr, versionStr);
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
void InternalCalls::MelonLogger::Flush() { Logger::Flush(); Console::Flush(); }
void InternalCalls::MelonLogger::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.MelonLogger::Internal_PrintModName", Internal_PrintModName);
	Mono::AddInternalCall("MelonLoader.MelonLogger::Internal_Msg", Internal_Msg);
	Mono::AddInternalCall("MelonLoader.MelonLogger::Internal_Warning", Internal_Warning);
	Mono::AddInternalCall("MelonLoader.MelonLogger::Internal_Error", Internal_Error);
	Mono::AddInternalCall("MelonLoader.MelonLogger::ThrowInternalFailure", ThrowInternalFailure);
	Mono::AddInternalCall("MelonLoader.MelonLogger::WriteSpacer", WriteSpacer);
	Mono::AddInternalCall("MelonLoader.MelonLogger::Flush", Flush);
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
Mono::String* InternalCalls::MelonUtils::GetGameName() { return Mono::Exports::mono_string_new(Mono::domain, Game::Name); }
Mono::String* InternalCalls::MelonUtils::GetGameDeveloper() { return Mono::Exports::mono_string_new(Mono::domain, Game::Developer); }
Mono::String* InternalCalls::MelonUtils::GetGameDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Game::BasePath); }
Mono::String* InternalCalls::MelonUtils::GetGameDataDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Game::DataPath); }
Mono::String* InternalCalls::MelonUtils::GetUnityVersion() { return Mono::Exports::mono_string_new(Mono::domain, Game::UnityVersion); }
Mono::String* InternalCalls::MelonUtils::GetManagedDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Mono::ManagedPath); }

void InternalCalls::MelonUtils::SCT(Mono::String* title)
{
	if (title == NULL) return;
	auto str = Mono::Exports::mono_string_to_utf8(title);
	Console::SetTitle(str);
	Mono::Free(str);
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

void InternalCalls::MelonUtils::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.MelonUtils::IsGame32Bit", IsGame32Bit);
	Mono::AddInternalCall("MelonLoader.MelonUtils::IsGameIl2Cpp", IsGameIl2Cpp);
	Mono::AddInternalCall("MelonLoader.MelonUtils::IsOldMono", IsOldMono);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetApplicationPath", GetApplicationPath);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetGameDataDirectory", GetGameDataDirectory);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetUnityVersion", GetUnityVersion);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetManagedDirectory", GetManagedDirectory);
	Mono::AddInternalCall("MelonLoader.MelonUtils::SetConsoleTitle", SCT);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetFileProductName", GetFileProductName);
	Mono::AddInternalCall("MelonLoader.MelonUtils::NativeHookAttach", Hook::Attach);
	Mono::AddInternalCall("MelonLoader.MelonUtils::NativeHookDetach", Hook::Detach);

	Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetGameName", GetGameName);
	Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetGameDeveloper", GetGameDeveloper);
	Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetGameDirectory", GetGameDirectory);

}
#pragma endregion

#pragma region MelonHandler
InternalCalls::MelonHandler::LoadMode InternalCalls::MelonHandler::LoadModeForPlugins = InternalCalls::MelonHandler::LoadMode::NORMAL;
InternalCalls::MelonHandler::LoadMode InternalCalls::MelonHandler::LoadModeForMods = InternalCalls::MelonHandler::LoadMode::NORMAL;
InternalCalls::MelonHandler::LoadMode InternalCalls::MelonHandler::GetLoadModeForPlugins() { return LoadModeForPlugins; }
InternalCalls::MelonHandler::LoadMode InternalCalls::MelonHandler::GetLoadModeForMods() { return LoadModeForMods; }
void InternalCalls::MelonHandler::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.MelonHandler::GetLoadModeForPlugins", GetLoadModeForPlugins);
	Mono::AddInternalCall("MelonLoader.MelonHandler::GetLoadModeForMods", GetLoadModeForMods);
}
#pragma endregion

#pragma region MelonDebug
bool InternalCalls::MelonDebug::IsEnabled() { return Debug::Enabled; }
void InternalCalls::MelonDebug::Internal_Msg(Console::Color color, Mono::String* namesection, Mono::String* txt)
{
	auto nsStr = namesection != NULL ? Mono::Exports::mono_string_to_utf8(namesection) : NULL;
	auto txtStr = Mono::Exports::mono_string_to_utf8(txt);
	Debug::Internal_Msg(color, nsStr, txtStr);
	if (nsStr != NULL) Mono::Free(nsStr);
	Mono::Free(txtStr);
}
void InternalCalls::MelonDebug::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.MelonDebug::IsEnabled", IsEnabled);
	Mono::AddInternalCall("MelonLoader.MelonDebug::Internal_Msg", Internal_Msg);
}
#pragma endregion
#endif