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
	static void Msgf(const char* fmt, ...);
	static void vMsgf(const char* fmt, va_list args);
	static void ForceWrite(const char* txt);
	static void ForceWritef(const char* fmt, ...);
	static void vForceWritef(const char* fmt, va_list args);
	static void Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, const char* namesection, const char* txt);
	static void Internal_Msgf(Console::Color meloncolor, Console::Color txtcolor, const char* namesection, const char* fmt, ...);
	static void Internal_vMsgf(Console::Color meloncolor, Console::Color txtcolor, const char* namesection, const char* fmt, va_list args);

};