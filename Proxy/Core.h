#pragma once
#include <Windows.h>
#include <string>
#include <filesystem>

class Core
{
public:
	static void Load(HINSTANCE hinstDLL);

private:
	static HMODULE LoadOriginalDLL(std::string proxy_filepath, std::string proxy_filepath_no_ext);
	static std::string GetBootstrapPath(std::string BasePath);
	static bool IsUnityGame(std::string exe_filepath);
	static void Error(std::string reason, bool should_kill = false);
	static void KillItDead();
};