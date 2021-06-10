#pragma once
#include <string>

class Game
{
public:
	static char* ApplicationPath;
	static char* BasePath;
	static char* DataPath;
	static char* ApplicationPathMono;
	static char* BasePathMono;
	static char* DataPathMono;
	static char* Developer;
	static char* Name;
	static char* UnityVersion;
	static bool IsIl2Cpp;
	static bool FirstRun;

	static bool Initialize();
	static bool SetupPaths();
	static bool ReadInfo();
private:

	static void ReadAppInfo();
	static bool ReadUnityVersion();
	static std::string ReadUnityVersionFromFileInfo();
	static std::string ReadUnityVersionFromGlobalGameManagers();
	static std::string ReadUnityVersionFromMainData();
};