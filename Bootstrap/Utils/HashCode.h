#ifndef PORT_DISABLE
#pragma once
#include <Windows.h>
#include <metahost.h>
#include <sstream>

class HashCode
{
public:
	static DWORD Hash;
	static char* Path_SM_Il2Cpp;
	static char* Path_SM_Mono;
	static char* Path_SM_Mono_Pre2017;
	static char* Path_SM_Mono_Pre5;
	static bool Initialize();
	static bool SetupPaths();

private:
	static void AddHash(const char* path);
};
#endif