#pragma once
#include "IL2CPP.h"

extern "C"
{
	__declspec(dllexport) IL2CPPDomain* __stdcall melonloader_get_il2cpp_domain();
	__declspec(dllexport) bool __stdcall melonloader_is_il2cpp_game();
	__declspec(dllexport) const char* __stdcall melonloader_getcommandline();
	__declspec(dllexport) const char* __stdcall melonloader_getgamepath();
	__declspec(dllexport) bool __stdcall melonloader_is_debug_mode();
}