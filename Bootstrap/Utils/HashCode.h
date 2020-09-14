#pragma once
#include <Windows.h>
#include <metahost.h>

class HashCode
{
public:
	static char* Hash1;
	static char* Hash2;
	static char* Hash3;
	static bool Initialize();

private:
	static bool SetupPaths();
	static bool GetHash(const char* path, char** output);
};