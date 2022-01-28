#pragma once
#include <string>

class Game
{
public:
	static char* ApplicationPath;
	static char* BasePath;
	static char* DataPath;
	static char* ApplicationPathMono;
	static char* BasePathMono;
	static char* DataPathMono;
	static bool IsIl2Cpp;

	static bool Initialize();
	static bool SetupPaths();
};