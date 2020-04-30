#include "Exports.h"
#include "MelonLoader.h"
#include "IL2CPP.h"
#include "Mono.h"
#include "HookManager.h"
#include "Logger.h"

void UNLOAD_MELONLOADER() { MelonLoader::UNLOAD(); }
void Logger_Log(MonoString* txt) { Logger::Log(Mono::mono_string_to_utf8(txt)); }
void Logger_LogColor(MonoString* txt, ConsoleColor color) { Logger::Log(Mono::mono_string_to_utf8(txt), color); }
void Logger_LogError(MonoString* namesection, MonoString* txt) { Logger::LogError(Mono::mono_string_to_utf8(namesection), Mono::mono_string_to_utf8(txt)); }
void Logger_LogModError(MonoString* namesection, MonoString* msg) { Logger::LogModError(Mono::mono_string_to_utf8(namesection), Mono::mono_string_to_utf8(msg)); }
void Logger_LogModStatus(int type) { Logger::LogModStatus(type); }
Il2CppDomain* GetIl2CppDomain() { return IL2CPP::Domain; }
bool IsIl2CppGame() { return MelonLoader::IsGameIl2Cpp; }
bool IsDebugMode() { return MelonLoader::DebugMode; }
bool IsRainbowMode() { return MelonLoader::RainbowMode; }
bool IsRandomRainbowMode() { return MelonLoader::RandomRainbowMode; }
const char* GetGameDirectory() { return MelonLoader::GamePath; }
void Hook(Il2CppMethod* target, void* detour) { HookManager::Hook(target, detour); }
void Unhook(Il2CppMethod* target, void* detour) { HookManager::Unhook(target, detour); }

void Exports::AddInternalCalls()
{
	Mono::mono_add_internal_call("MelonLoader.Imports::UNLOAD_MELONLOADER", UNLOAD_MELONLOADER);
	Mono::mono_add_internal_call("MelonLoader.Imports::Logger_Log", Logger_Log);
	Mono::mono_add_internal_call("MelonLoader.Imports::Logger_LogColor", Logger_LogColor);
	Mono::mono_add_internal_call("MelonLoader.Imports::Logger_LogError", Logger_LogError);
	Mono::mono_add_internal_call("MelonLoader.Imports::Logger_LogModError", Logger_LogModError);
	Mono::mono_add_internal_call("MelonLoader.Imports::Logger_LogModStatus", Logger_LogModStatus);
	Mono::mono_add_internal_call("MelonLoader.Imports::GetIl2CppDomain", GetIl2CppDomain);
	Mono::mono_add_internal_call("MelonLoader.Imports::IsIl2CppGame", IsIl2CppGame);
	Mono::mono_add_internal_call("MelonLoader.Imports::IsDebugMode", IsDebugMode);
	Mono::mono_add_internal_call("MelonLoader.Imports::IsRainbowMode", IsRainbowMode);
	Mono::mono_add_internal_call("MelonLoader.Imports::IsRandomRainbowMode", IsRandomRainbowMode);
	Mono::mono_add_internal_call("MelonLoader.Imports::GetGameDirectory", GetGameDirectory);
	Mono::mono_add_internal_call("MelonLoader.Imports::Hook", Hook);
	Mono::mono_add_internal_call("MelonLoader.Imports::Unhook", Unhook);
	Mono::mono_add_internal_call("MelonLoader.Imports::AllocConsole", AllocConsole);
	Mono::mono_add_internal_call("MelonLoader.Imports::SetForegroundWindow", SetForegroundWindow);
	Mono::mono_add_internal_call("MelonLoader.Imports::GetConsoleWindow", GetConsoleWindow);

	if (MelonLoader::IsGameIl2Cpp)
		IL2CPP::AddInternalCalls();
}