#pragma once

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
private:

	static void ReadAppInfo();
	static bool ReadUnityVersion();
	static const char* ReadUnityVersionFromFileInfo();
	static const char* ReadUnityVersionFromGlobalGameManagers();
};