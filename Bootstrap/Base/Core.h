#pragma once
#include <Windows.h>
#include <string>
#include <algorithm>
#include <filesystem>
#include <chrono>

class Core
{
public:
	static HINSTANCE Bootstrap;
	static char* Path;
	static std::string Version;
	static bool Is_ALPHA_PreRelease;
	static bool QuitFix;
	static void Initialize(HINSTANCE hinstDLL);
	static bool DirectoryExists(const char* path);
	static bool FileExists(const char* path);
	static void GetLocalTime(std::chrono::system_clock::time_point* now, std::chrono::milliseconds* ms, std::tm* bt);
	static void WelcomeMessage();
	static void KillCurrentProcess();
	static const char* GetFileInfoProductName(const char* path);
	static const char* GetFileInfoProductVersion(const char* path);
	static const char* GetVersionStr();
	static const char* GetVersionStrWithGameName(const char* GameVersion = NULL);

private:
	static const char* GetOSVersion();
	static bool OSVersionCheck();
};