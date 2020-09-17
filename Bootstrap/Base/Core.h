#pragma once
#include <Windows.h>
#include <chrono>

class Core
{
public:
	static HINSTANCE Bootstrap;
	static char* Path;
	static const char* Version;
	static bool QuitFix;
	static bool Initialize();
	static bool DirectoryExists(const char* path);
	static bool FileExists(const char* path);
	static void GetLocalTime(std::chrono::system_clock::time_point* now, std::chrono::milliseconds* ms, std::tm* bt);
	static void WelcomeMessage();
	static void KillCurrentProcess();
private:
	static const char* GetOSVersion();
	static bool OSVersionCheck();
};