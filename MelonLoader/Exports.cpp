#include "Exports.h"
#include "MelonLoader.h"

IL2CPPDomain* melonloader_get_il2cpp_domain() { return IL2CPP::Domain; }
bool melonloader_is_il2cpp_game() { return MelonLoader::IsGameIL2CPP; }
const char* melonloader_getcommandline() { return GetCommandLine(); }