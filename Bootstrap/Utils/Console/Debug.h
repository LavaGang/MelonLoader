#pragma once
#include "Console.h"
#include "../Console/Console.h"

#ifdef _WIN32
#define __USE_COUT
#endif

class Debug
{
public:
	static bool Enabled;
	static void Msg(const char* txt);
	static void ForceWrite(const char* txt);
	static void Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, const char* namesection, const char* txt);
};