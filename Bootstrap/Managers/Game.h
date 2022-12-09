#pragma once

class Game
{
public:
	static char* ApplicationPath;
	static char* BasePath;
	static char* DataPath;
	static char* BasePathMono;
	static bool IsIl2Cpp;

	static bool Initialize();
	static bool SetupPaths();
};