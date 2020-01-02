#include "Exports.h"
#include "MelonLoader.h"
#include "Console.h"

IL2CPPDomain* melonloader_get_il2cpp_domain() { return IL2CPP::Domain; }
bool melonloader_is_il2cpp_game() { return MelonLoader::IsGameIL2CPP; }
bool melonloader_is_debug_mode() { return MelonLoader::DebugMode; }
const char* melonloader_get_game_directory() { return MelonLoader::GamePath; }
void melonloader_console_writeline(const char* txt) { Console::WriteLine(txt); }