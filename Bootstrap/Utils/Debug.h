#pragma once
#include "../Utils/Console.h"

class Debug
{
public:
	static bool Enabled;
	static void Msg(const char* txt);
	static void DirectWrite(const char* txt);
	static void Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, const char* namesection, const char* txt);
};