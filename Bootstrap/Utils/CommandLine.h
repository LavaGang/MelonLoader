#pragma once
#include "IniFile.h"
#include "Console.h"

class CommandLine
{
public:
	static int argc;
	//static char** argvMono;
	static char* argv[64];
	static char* argvMono[64];
	static IniFile* iniFile;
	static void Read();
	static void ReadIniFile();
	static const char* GetPrefix() { return ((Console::Mode == Console::DisplayMode::LEMON) ? "LemonLoader" : "MelonLoader"); }
	static const char* AddPrefixToLaunchOption(const char* ending) { return (std::string("--") + ((Console::Mode == Console::DisplayMode::LEMON) ? "lemonloader" : "melonloader") + "." + ending).c_str(); }

private:
	static int GetIntFromConstChar(const char* str, int defaultval);
};