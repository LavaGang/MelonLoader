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
private:

	static bool SetupPaths();
	static void ReadAppInfo();
	static void UnknownUnityVersion();
	static void ReadUnityVersion();
};