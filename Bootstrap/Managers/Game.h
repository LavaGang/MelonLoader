#pragma once
#include <string>

class Game
{
public:
	static char* ApplicationPath;
	static char* BasePath;
	static char* DataPath;
	static char* Developer;
	static char* Name;
	static char* UnityVersion;
	static bool IsIl2Cpp;

	static bool Initialize();
	static bool SetupPaths();
	static bool ReadInfo();
private:

	static void ReadAppInfo();
	static bool ReadUnityVersion();
	static std::string ReadUnityVersionFromFileInfo();
	static std::string ReadUnityVersionFromGlobalGameManagers();
};