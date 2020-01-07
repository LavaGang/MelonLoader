#include "Exports.h"
#include "MelonLoader.h"
#include "Console.h"

Il2CppDomain* melonloader_get_il2cpp_domain() { return Il2Cpp::Domain; }
bool melonloader_is_il2cpp_game() { return MelonLoader::IsGameIl2Cpp; }
bool melonloader_is_debug_mode() { return MelonLoader::DebugMode; }
const char* melonloader_get_game_directory() { return MelonLoader::GamePath; }
void melonloader_console_writeline(const char* txt) { Console::WriteLine(txt); }