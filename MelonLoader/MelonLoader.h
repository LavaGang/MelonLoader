#pragma once
#include <string>
#include <Windows.h>

class MelonLoader
{
public:
	static HINSTANCE thisdll;
	static bool IsGameIl2Cpp;
	static bool DebugMode;
	static bool RainbowMode;
	static bool RandomRainbowMode;
	static bool QuitFix;
	static char* GamePath;
	static char* DataPath;
	static char* CompanyName;
	static char* ProductName;
	static char* UnityVersion;
	static char* GameVersion;

	static void Main();
	static void ReadAppInfo();
	static void UNLOAD(bool alt = false);
	static bool Is64bit();
	static int CountSubstring(std::string pat, std::string txt);
	static bool DirectoryExists(const char* path);
	static void KillProcess();
};