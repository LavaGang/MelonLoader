#pragma once
#include "../Utils/Console.h"

#ifdef _WIN32
#define __USE_COUT
#endif

class Debug
{
public:
	static bool Enabled;
	static void Msg(const char* txt);
	static void ForceWrite(const char* txt);
	static void Internal_Msg(Console::Color color, const char* namesection, const char* txt);
};