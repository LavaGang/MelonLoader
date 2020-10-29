#pragma once
#include <Windows.h>
#include <metahost.h>

class HashCode
{
public:
	static DWORD Hash;
	static bool Initialize();
	static bool SetupPaths();

private:
	static bool AddHash(const char* path);
};