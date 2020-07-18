#include "Exports.h"
#include "MelonLoader.h"
#include "Il2Cpp.h"
#include "Mono.h"
#include "HookManager.h"
#include "Logger.h"

void Log(MonoString* txt) { Logger::Log(Mono::mono_string_to_utf8(txt)); }
void LogColor(MonoString* txt, ConsoleColor color) { Logger::Log(Mono::mono_string_to_utf8(txt), color); }
void LogWarning(MonoString* namesection, MonoString* txt) { Logger::LogWarning(Mono::mono_string_to_utf8(namesection), Mono::mono_string_to_utf8(txt)); }
void LogError(MonoString* namesection, MonoString* txt) { Logger::LogError(Mono::mono_string_to_utf8(namesection), Mono::mono_string_to_utf8(txt)); }
void LogDLLError(MonoString* namesection, MonoString* msg) { Logger::LogDLLError(Mono::mono_string_to_utf8(namesection), Mono::mono_string_to_utf8(msg)); }
void LogDLLStatus(ModHandler_DLLStatus type) { Logger::LogDLLStatus(type); }
bool IsIl2CppGame() { return MelonLoader::IsGameIl2Cpp; }
bool IsDebugMode() { return MelonLoader::DebugMode; }
bool IsConsoleEnabled() { return Console::Enabled; }
bool ShouldShowGameLogs() { return Console::ShouldShowGameLogs; }
MonoString* GetGameDirectory() { return Mono::mono_string_new(Mono::Domain, MelonLoader::GamePath); }
MonoString* GetGameDataDirectory() { return Mono::mono_string_new(Mono::Domain, MelonLoader::DataPath); }
void Hook(Il2CppMethod* target, void* detour) { HookManager::Hook(target, detour); }
void Unhook(Il2CppMethod* target, void* detour) { HookManager::Unhook(target, detour); }
bool IsOldMono() { return Mono::IsOldMono; }
MonoString* GetCompanyName() { return Mono::mono_string_new(Mono::Domain, ((MelonLoader::CompanyName == NULL) ? "UNKNOWN" : MelonLoader::CompanyName)); }
MonoString* GetProductName() { return Mono::mono_string_new(Mono::Domain, ((MelonLoader::ProductName == NULL) ? "UNKNOWN" : MelonLoader::ProductName)); }
MonoString* GetAssemblyDirectory() { return Mono::mono_string_new(Mono::Domain, Mono::AssemblyPath); }
MonoString* GetMonoConfigDirectory() { return Mono::mono_string_new(Mono::Domain, Mono::ConfigPath); }
MonoString* GetExePath() { return Mono::mono_string_new(Mono::Domain, MelonLoader::ExePath); }
bool IsQuitFix() { return MelonLoader::QuitFix; }
bool IsDevModsOnly() { return MelonLoader::DevModsOnly; }
bool IsDevPluginsOnly() { return MelonLoader::DevPluginsOnly; }
bool AG_Force_Regenerate() { return MelonLoader::AG_Force_Regenerate; }
MonoString* AG_Force_Version_Unhollower() { if (MelonLoader::ForceUnhollowerVersion != NULL) return Mono::mono_string_new(Mono::Domain, MelonLoader::ForceUnhollowerVersion); return NULL; }
void SetTitleForConsole(MonoString* txt) { Console::SetTitle(Mono::mono_string_to_utf8(txt)); }

void Exports::AddInternalCalls()
{
	Mono::mono_add_internal_call("MelonLoader.Imports::UNLOAD_MELONLOADER", MelonLoader::UNLOAD);
	Mono::mono_add_internal_call("MelonLoader.Imports::IsIl2CppGame", IsIl2CppGame);
	Mono::mono_add_internal_call("MelonLoader.Imports::IsDebugMode", IsDebugMode);
	Mono::mono_add_internal_call("MelonLoader.Imports::IsConsoleEnabled", IsConsoleEnabled);
	Mono::mono_add_internal_call("MelonLoader.Imports::GetGameDirectory", GetGameDirectory);
	Mono::mono_add_internal_call("MelonLoader.Imports::GetGameDataDirectory", GetGameDataDirectory);
	Mono::mono_add_internal_call("MelonLoader.Imports::GetAssemblyDirectory", GetAssemblyDirectory);
	Mono::mono_add_internal_call("MelonLoader.Imports::GetMonoConfigDirectory", GetMonoConfigDirectory);
	Mono::mono_add_internal_call("MelonLoader.Imports::Hook", Hook);
	Mono::mono_add_internal_call("MelonLoader.Imports::Unhook", Unhook);
	Mono::mono_add_internal_call("MelonLoader.Imports::IsOldMono", IsOldMono);
	Mono::mono_add_internal_call("MelonLoader.Imports::GetCompanyName", GetCompanyName);
	Mono::mono_add_internal_call("MelonLoader.Imports::GetProductName", GetProductName);
	Mono::mono_add_internal_call("MelonLoader.Imports::GetExePath", GetExePath);
	Mono::mono_add_internal_call("MelonLoader.Imports::IsQuitFix", IsQuitFix);
	Mono::mono_add_internal_call("MelonLoader.Imports::IsDevModsOnly", IsDevModsOnly);
	Mono::mono_add_internal_call("MelonLoader.Imports::IsDevPluginsOnly", IsDevPluginsOnly);
	Mono::mono_add_internal_call("MelonLoader.Imports::AG_Force_Regenerate", AG_Force_Regenerate);
	Mono::mono_add_internal_call("MelonLoader.Imports::AG_Force_Version_Unhollower", AG_Force_Version_Unhollower);

	Mono::mono_add_internal_call("MelonLoader.Console::Allocate", Console::Create);
	Mono::mono_add_internal_call("MelonLoader.Console::SetTitle", SetTitleForConsole);
	Mono::mono_add_internal_call("MelonLoader.Console::SetColor", Console::SetColor);
	//Mono::mono_add_internal_call("MelonLoader.Console::ShouldShowGameLogs", ShouldShowGameLogs);

	Mono::mono_add_internal_call("MelonLoader.MelonModLogger::Native_Log", Log);
	Mono::mono_add_internal_call("MelonLoader.MelonModLogger::Native_LogColor", LogColor);
	Mono::mono_add_internal_call("MelonLoader.MelonModLogger::Native_LogWarning", LogWarning);
	Mono::mono_add_internal_call("MelonLoader.MelonModLogger::Native_LogError", LogError);
	Mono::mono_add_internal_call("MelonLoader.MelonModLogger::Native_LogDLLError", LogDLLError);
	Mono::mono_add_internal_call("MelonLoader.MelonModLogger::Native_LogDLLStatus", LogDLLStatus);
}