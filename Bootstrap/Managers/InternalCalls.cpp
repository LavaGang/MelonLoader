#include "InternalCalls.h"
#include "../Utils/Console/Debug.h"
#include "../Utils/Console/Logger.h"
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
	Mono::AddInternalCall("MelonLoader.External.Core::QuitFix", (void*)QuitFix);
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
void InternalCalls::MelonLogger::Flush()
{
#ifdef PORT_DISABLE
	Logger::Flush();
	Console::Flush();
#endif
}
void InternalCalls::MelonLogger::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.External.Logger::Internal_PrintModName", (void*)Internal_PrintModName);
	Mono::AddInternalCall("MelonLoader.External.Logger::Internal_Msg", (void*)Internal_Msg);
	Mono::AddInternalCall("MelonLoader.External.Logger::Internal_Warning", (void*)Internal_Warning);
	Mono::AddInternalCall("MelonLoader.External.Logger::Internal_Error", (void*)Internal_Error);
	Mono::AddInternalCall("MelonLoader.External.Logger::ThrowInternalFailure", (void*)ThrowInternalFailure);
	Mono::AddInternalCall("MelonLoader.External.Logger::WriteSpacer", (void*)WriteSpacer);
	Mono::AddInternalCall("MelonLoader.External.Logger::Flush", (void*)Flush);
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
#ifdef _WIN32
	if (title == NULL) return;
	auto str = Mono::Exports::mono_string_to_utf8(title);
	Console::SetTitle(str);
	Mono::Free(str);
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

void InternalCalls::MelonUtils::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.External.Utils::IsGame32Bit", (void*)IsGame32Bit);
	Mono::AddInternalCall("MelonLoader.External.Utils::IsGameIl2Cpp", (void*)IsGameIl2Cpp);
	Mono::AddInternalCall("MelonLoader.External.Utils::IsOldMono", (void*)IsOldMono);
	Mono::AddInternalCall("MelonLoader.External.Utils::GetApplicationPath", (void*)GetApplicationPath);
	Mono::AddInternalCall("MelonLoader.External.Utils::GetGameDataDirectory", (void*)GetGameDataDirectory);
	Mono::AddInternalCall("MelonLoader.External.Utils::GetUnityVersion", (void*)GetUnityVersion);
	Mono::AddInternalCall("MelonLoader.External.Utils::GetManagedDirectory", (void*)GetManagedDirectory);
	Mono::AddInternalCall("MelonLoader.External.Utils::SetConsoleTitle", (void*)SCT);
	Mono::AddInternalCall("MelonLoader.External.Utils::GetFileProductName", (void*)GetFileProductName);
	Mono::AddInternalCall("MelonLoader.External.Utils::NativeHookAttach", (void*)Hook::Attach);
	Mono::AddInternalCall("MelonLoader.External.Utils::NativeHookDetach", (void*)Hook::Detach);
	
	Mono::AddInternalCall("MelonLoader.External.Utils::Internal_GetGameName", (void*)GetGameName);
	Mono::AddInternalCall("MelonLoader.External.Utils::Internal_GetGameDeveloper", (void*)GetGameDeveloper);
	Mono::AddInternalCall("MelonLoader.External.Utils::Internal_GetGameDirectory", (void*)GetGameDirectory);
}
#pragma endregion

#pragma region MelonHandler
InternalCalls::MelonHandler::LoadMode InternalCalls::MelonHandler::LoadModeForPlugins = InternalCalls::MelonHandler::LoadMode::NORMAL;
InternalCalls::MelonHandler::LoadMode InternalCalls::MelonHandler::LoadModeForMods = InternalCalls::MelonHandler::LoadMode::NORMAL;
InternalCalls::MelonHandler::LoadMode InternalCalls::MelonHandler::GetLoadModeForPlugins() { return LoadModeForPlugins; }
InternalCalls::MelonHandler::LoadMode InternalCalls::MelonHandler::GetLoadModeForMods() { return LoadModeForMods; }
void InternalCalls::MelonHandler::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.External.Handler::GetLoadModeForPlugins", (void*)GetLoadModeForPlugins);
	Mono::AddInternalCall("MelonLoader.External.Handler::GetLoadModeForMods", (void*)GetLoadModeForMods);
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
	Mono::AddInternalCall("MelonLoader.External.Debug::IsEnabled", (void*)IsEnabled);
	Mono::AddInternalCall("MelonLoader.External.Debug::Internal_Msg", (void*)Internal_Msg);
}
#pragma endregion
