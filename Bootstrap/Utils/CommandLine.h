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
	//static IniFile* iniFile;
	static void Read();
	//static void ReadIniFile();
	
private:
	static int GetIntFromConstChar(const char* str, int defaultval);
};