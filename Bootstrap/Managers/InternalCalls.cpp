#include "InternalCalls.h"
#include "../Utils/Debug.h"
#include "../Utils/Logger.h"
#include "Game.h"
#include "Hook.h"
#include "../Utils/Assertion.h"
#include "../Base/Core.h"
#include "../Utils/HashCode.h"
#include "Il2Cpp.h"
#include "../Utils/AssemblyGenerator.h"

void InternalCalls::Initialize()
{
	Debug::Msg("Initializing Internal Calls...");
	MelonCore::AddInternalCalls();
	MelonLogger::AddInternalCalls();
	MelonUtils::AddInternalCalls();
	MelonHandler::AddInternalCalls();
	MelonDebug::AddInternalCalls();
	SupportModules::AddInternalCalls();
	AssemblyGenerator_Logger::AddInternalCalls();
	AssemblyGenerator_Utils::AddInternalCalls();
}

#pragma region MelonCore
bool InternalCalls::MelonCore::QuitFix() { return Core::QuitFix; }
void InternalCalls::MelonCore::AddInternalCalls() { Mono::AddInternalCall("MelonLoader.Core::QuitFix", QuitFix); }
#pragma endregion

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
Mono::String* InternalCalls::MelonUtils::GetHashCode() { return Mono::Exports::mono_string_new(Mono::domain, HashCode::Hash.c_str()); }
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
	Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetHashCode", GetHashCode);
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
	Mono::AddInternalCall("MelonLoader.MelonDebug::IsEnabled", IsEnabled);
	Mono::AddInternalCall("MelonLoader.MelonDebug::Internal_Msg", Internal_Msg);
}
#pragma endregion

#pragma region SupportModules
Mono::String* InternalCalls::SupportModules::GetVersionStrWithGameName(Mono::String* GameVersion) { return Mono::Exports::mono_string_new(Mono::domain, Core::GetVersionStrWithGameName(GameVersion != NULL ? Mono::Exports::mono_string_to_utf8(GameVersion) : NULL)); }
void InternalCalls::SupportModules::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.Support.Preload::GetManagedDirectory", MelonUtils::GetManagedDirectory);
	Mono::AddInternalCall("MelonLoader.Support.Main::GetVersionStrWithGameName", GetVersionStrWithGameName);
}
#pragma endregion

#pragma region AssemblyGenerator_Logger
void InternalCalls::AssemblyGenerator_Logger::Msg(Mono::String* txt)
{
	if (txt == NULL) return;
	auto str = Mono::Exports::mono_string_to_utf8(txt);
	Logger::Msg(str);
	Mono::Free(str);
}
void InternalCalls::AssemblyGenerator_Logger::Warning(Mono::String* txt)
{
	if (txt == NULL) return;
	auto str = Mono::Exports::mono_string_to_utf8(txt);
	Logger::Warning(str);
	Mono::Free(str);
}
void InternalCalls::AssemblyGenerator_Logger::Error(Mono::String* txt)
{
	if (txt == NULL) return;
	auto str = Mono::Exports::mono_string_to_utf8(txt);
	Logger::Error(str);
	Mono::Free(str);
}
void InternalCalls::AssemblyGenerator_Logger::Debug_Msg(Mono::String* txt)
{
	if (txt == NULL) return;
	auto str = Mono::Exports::mono_string_to_utf8(txt);
	Debug::Msg(str);
	Mono::Free(str);
}

void InternalCalls::AssemblyGenerator_Logger::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.AssemblyGenerator.Logger::Msg", Msg);
	Mono::AddInternalCall("MelonLoader.AssemblyGenerator.Logger::Warning", Warning);
	Mono::AddInternalCall("MelonLoader.AssemblyGenerator.Logger::Error", Error);
	Mono::AddInternalCall("MelonLoader.AssemblyGenerator.Logger::Debug_Msg", Debug_Msg);
}
#pragma endregion

#pragma region AssemblyGenerator_Utils
Mono::String* InternalCalls::AssemblyGenerator_Utils::GetGameAssemblyPath() { return Mono::Exports::mono_string_new(Mono::domain, Il2Cpp::GameAssemblyPath); }
Mono::String* InternalCalls::AssemblyGenerator_Utils::GetConfigDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Mono::ConfigPath); }
Mono::String* InternalCalls::AssemblyGenerator_Utils::GetAssemblyGeneratorPath() { return Mono::Exports::mono_string_new(Mono::domain, AssemblyGenerator::Path); }
bool InternalCalls::AssemblyGenerator_Utils::ForceRegeneration() { return AssemblyGenerator::ForceRegeneration; }
Mono::String* InternalCalls::AssemblyGenerator_Utils::ForceVersion_UnityDependencies() { return ((AssemblyGenerator::ForceVersion_UnityDependencies != NULL) ? Mono::Exports::mono_string_new(Mono::domain, AssemblyGenerator::ForceVersion_UnityDependencies) : NULL); }
Mono::String* InternalCalls::AssemblyGenerator_Utils::ForceVersion_Dumper() { return ((AssemblyGenerator::ForceVersion_Dumper != NULL) ? Mono::Exports::mono_string_new(Mono::domain, AssemblyGenerator::ForceVersion_Dumper) : NULL); }
Mono::String* InternalCalls::AssemblyGenerator_Utils::ForceVersion_Il2CppAssemblyUnhollower() { return ((AssemblyGenerator::ForceVersion_Il2CppAssemblyUnhollower != NULL) ? Mono::Exports::mono_string_new(Mono::domain, AssemblyGenerator::ForceVersion_Il2CppAssemblyUnhollower) : NULL); }
void InternalCalls::AssemblyGenerator_Utils::SetProcessId(int id) { AssemblyGenerator::ProcessId = id; }

void InternalCalls::AssemblyGenerator_Utils::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.AssemblyGenerator.Utils::GetGameAssemblyPath", GetGameAssemblyPath);
	Mono::AddInternalCall("MelonLoader.AssemblyGenerator.Utils::GetUnityVersion", InternalCalls::MelonUtils::GetUnityVersion);
	Mono::AddInternalCall("MelonLoader.AssemblyGenerator.Utils::GetGameName", InternalCalls::MelonUtils::GetGameName);
	Mono::AddInternalCall("MelonLoader.AssemblyGenerator.Utils::GetManagedDirectory", InternalCalls::MelonUtils::GetManagedDirectory);
	Mono::AddInternalCall("MelonLoader.AssemblyGenerator.Utils::GetConfigDirectory", GetConfigDirectory);
	Mono::AddInternalCall("MelonLoader.AssemblyGenerator.Utils::GetAssemblyGeneratorPath", GetAssemblyGeneratorPath);
	Mono::AddInternalCall("MelonLoader.AssemblyGenerator.Utils::ForceRegeneration", ForceRegeneration);
	Mono::AddInternalCall("MelonLoader.AssemblyGenerator.Utils::ForceVersion_UnityDependencies", ForceVersion_UnityDependencies);
	Mono::AddInternalCall("MelonLoader.AssemblyGenerator.Utils::ForceVersion_Dumper", ForceVersion_Dumper);
	Mono::AddInternalCall("MelonLoader.AssemblyGenerator.Utils::ForceVersion_Il2CppAssemblyUnhollower", ForceVersion_Il2CppAssemblyUnhollower);
	Mono::AddInternalCall("MelonLoader.AssemblyGenerator.Utils::SetProcessId", SetProcessId);
}
#pragma endregion