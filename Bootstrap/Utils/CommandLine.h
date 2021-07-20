#pragma once
#include "IniFile.h"
#include "Console.h"

class CommandLine
{
public:
	static int argc;
	static char* argv[64];
	static char* argvMono[64];
	static void Read();
	
private:
	static int GetIntFromConstChar(const char* str, int defaultval);
};