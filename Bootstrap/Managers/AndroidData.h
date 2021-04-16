#pragma once
class AndroidData
{
public:
	static char* BaseDataDir;
	static char* AppName;
	static char* DataDir;
	static bool Initialize();
private:
	static void GetBaseDataDir();
	static void GetAppName();
	static void GetDataDir();
};

