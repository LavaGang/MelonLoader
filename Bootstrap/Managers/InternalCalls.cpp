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
void InternalCalls::MelonLogger::Internal_Msg(Mono::String* namesection, Mono::String* txt) { Logger::Internal_Msg(((namesection != NULL) ? Mono::Exports::mono_string_to_utf8(namesection) : NULL), Mono::Exports::mono_string_to_utf8(txt)); }
void InternalCalls::MelonLogger::Internal_Warning(Mono::String* namesection, Mono::String* txt) { Logger::Internal_Warning(((namesection != NULL) ? Mono::Exports::mono_string_to_utf8(namesection) : NULL), Mono::Exports::mono_string_to_utf8(txt)); }
void InternalCalls::MelonLogger::Internal_Error(Mono::String* namesection, Mono::String* txt) { Logger::Internal_Error(((namesection != NULL) ? Mono::Exports::mono_string_to_utf8(namesection) : NULL), Mono::Exports::mono_string_to_utf8(txt)); }
void InternalCalls::MelonLogger::ThrowInternalFailure(Mono::String* msg) { Assertion::ThrowInternalFailure(Mono::Exports::mono_string_to_utf8(msg)); }
void InternalCalls::MelonLogger::WriteSpacer() { Logger::WriteSpacer(); }
void InternalCalls::MelonLogger::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.MelonLogger::Internal_Msg", Internal_Msg);
	Mono::AddInternalCall("MelonLoader.MelonLogger::Internal_Warning", Internal_Warning);
	Mono::AddInternalCall("MelonLoader.MelonLogger::Internal_Error", Internal_Error);
	Mono::AddInternalCall("MelonLoader.MelonLogger::ThrowInternalFailure", ThrowInternalFailure);
	Mono::AddInternalCall("MelonLoader.MelonLogger::WriteSpacer", WriteSpacer);
}
#pragma endregion

#pragma region MelonUtils
bool InternalCalls::MelonUtils::IsGameIl2Cpp() { return Game::IsIl2Cpp; }
bool InternalCalls::MelonUtils::IsOldMono() { return Mono::IsOldMono; }
Mono::String* InternalCalls::MelonUtils::GetApplicationPath() { return Mono::Exports::mono_string_new(Mono::domain, Game::ApplicationPath); }
Mono::String* InternalCalls::MelonUtils::GetGameName() { return Mono::Exports::mono_string_new(Mono::domain, Game::Name); }
Mono::String* InternalCalls::MelonUtils::GetGameDeveloper() { return Mono::Exports::mono_string_new(Mono::domain, Game::Developer); }
Mono::String* InternalCalls::MelonUtils::GetGameDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Game::BasePath); }
Mono::String* InternalCalls::MelonUtils::GetGameDataDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Game::DataPath); }
Mono::String* InternalCalls::MelonUtils::GetUnityVersion() { return Mono::Exports::mono_string_new(Mono::domain, Game::UnityVersion); }
Mono::String* InternalCalls::MelonUtils::GetManagedDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Mono::ManagedPath); }
void InternalCalls::MelonUtils::SCT(Mono::String* title) { if (title == NULL) return; Console::SetTitle(Mono::Exports::mono_string_to_utf8(title)); }
void InternalCalls::MelonUtils::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.MelonUtils::IsGameIl2Cpp", IsGameIl2Cpp);
	Mono::AddInternalCall("MelonLoader.MelonUtils::IsOldMono", IsOldMono);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetApplicationPath", GetApplicationPath);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetGameName", GetGameName);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetGameDeveloper", GetGameDeveloper);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetGameDirectory", GetGameDirectory);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetGameDataDirectory", GetGameDataDirectory);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetUnityVersion", GetUnityVersion);
	Mono::AddInternalCall("MelonLoader.MelonUtils::GetManagedDirectory", GetManagedDirectory);
	Mono::AddInternalCall("MelonLoader.MelonUtils::SetConsoleTitle", SCT);
	Mono::AddInternalCall("MelonLoader.MelonUtils::NativeHookAttach", Hook::Attach);
	Mono::AddInternalCall("MelonLoader.MelonUtils::NativeHookDetach", Hook::Detach);
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
void InternalCalls::MelonDebug::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.MelonDebug::IsEnabled", IsEnabled);
}
#pragma endregion