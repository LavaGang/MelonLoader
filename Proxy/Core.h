#pragma once
#include <Windows.h>
#include <string>
#include <filesystem>

class Core
{
public:
	static void Load(HINSTANCE hinstDLL);

private:
	static bool IsUnityCrashHandler(std::string exe_filename) { return (strstr(exe_filename.c_str(), "unitycrashhandler") != NULL); };
	static HMODULE LoadOriginalDLL(std::string proxy_filename, std::string proxy_filename_no_ext);
	static std::string GetBootstrapPath();
	static bool IsUnityGame(std::string exe_filename);
	static void Error(std::string reason, bool should_kill = false);
	static void KillItDead();
};