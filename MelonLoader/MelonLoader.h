#pragma once
#include <string>
#include <Windows.h>

class MelonLoader
{
public:
	static HINSTANCE thisdll;
	static int CommandLineC;
	static char* CommandLineV[64];
	static bool IsGameIl2Cpp;
	static bool DebugMode;
	static bool QuitFix;
	static bool AG_Force_Regenerate;
	static char* ExePath;
	static char* GamePath;
	static char* DataPath;
	static char* CompanyName;
	static char* ProductName;
	static char* ForceUnhollowerVersion;
	static char* ForceUnityVersion;

	enum LoadMode
	{
		NORMAL,
		DEV,
		BOTH
	};
	static LoadMode LoadMode_Plugins;
	static LoadMode LoadMode_Mods;

	static void Main();
	static void ParseCommandLine();
	static void ReadAppInfo();
	static bool CheckOSVersion();
	static void UNLOAD();
	static void KillProcess();
	static int CountSubstring(std::string pat, std::string txt);
	static bool DirectoryExists(const char* path);
	static long GetFileSize(std::string filename);
	static int GetIntFromConstChar(const char* str, int defaultval = 0);
};