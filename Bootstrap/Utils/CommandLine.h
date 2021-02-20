#pragma once

#ifdef PORT_DISABLE
#include "IniFile.h"

class CommandLine
{
public:
	static int argc;
	static char* argv[64];
	static IniFile* iniFile;
	static void Read();
	static void ReadIniFile();

private:
	static int GetIntFromConstChar(const char* str, int defaultval);
};
#endif