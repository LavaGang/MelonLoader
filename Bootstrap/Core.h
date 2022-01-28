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
	static char* BasePath;
	static char* BasePathMono;
	static char* Path;
	static std::string Version;
	static bool Is_ALPHA_PreRelease;
	static void Initialize(HINSTANCE hinstDLL);
	static bool DirectoryExists(const char* path);
	static bool FileExists(const char* path);
	static void GetLocalTime(std::chrono::system_clock::time_point* now, std::chrono::milliseconds* ms, std::tm* bt);
	static bool CheckPathASCII();
	static void WelcomeMessage();
	static void KillCurrentProcess();
	static const char* GetFileInfoProductName(const char* path);
	static const char* GetFileInfoProductVersion(const char* path);
	static std::string GetVersionStr();
	static std::string GetVersionStrWithGameName(const char* GameName, const char* GameVersion = NULL);
	static void SetBasePath();
	static bool IsRunningInWine() { return ((wine_get_version == NULL) ? false : true); }

private:
	static const char* GetOSVersion();
	static bool OSVersionCheck();
	typedef const char* (*wine_get_version_t) ();
	static wine_get_version_t wine_get_version;
	static void SetupWineCheck();
};