#include "Exports.h"
#include "MelonLoader.h"
#include "Console.h"

Il2CppDomain* melonloader_get_il2cpp_domain() { return IL2CPP::Domain; }
bool melonloader_is_il2cpp_game() { return MelonLoader::IsGameIl2Cpp; }
bool melonloader_is_debug_mode() { return MelonLoader::DebugMode; }
bool melonloader_is_mupot_mode() { return MelonLoader::MupotMode; }
const char* melonloader_game_directory() { return MelonLoader::GamePath; }
void melonloader_console_writeline(const char* txt) { Console::WriteLine(txt); }
void melonloader_detour(Il2CppMethod* target, void* detour) { MelonLoader::Detour(target, detour); }
void melonloader_undetour(Il2CppMethod* target, void* detour) { MelonLoader::UnDetour(target, detour); }