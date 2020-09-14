#include <Windows.h>
#include <string>
#include <fstream>
#include "Game.h"
#include "../Base/Core.h"
#include "Il2Cpp.h"
#include "../Utils/Assertion.h"
#include "../Utils/Logger.h"
#pragma comment(lib,"version.lib")

char* Game::ApplicationPath = NULL;
char* Game::BasePath = NULL;
char* Game::DataPath = NULL;
char* Game::Developer = NULL;
char* Game::Name = NULL;
char* Game::UnityVersion = NULL;
bool Game::IsIl2Cpp = false;

bool Game::Initialize()
{
	if (!SetupPaths())
	{
		Assertion::ThrowInternalFailure("Failed to Setup Game Paths!");
		return false;
	}
	ReadAppInfo();
	ReadUnityVersion();
	std::string GameAssemblyPath = (std::string(BasePath) + "\\GameAssembly.dll");
	if (Core::FileExists(GameAssemblyPath.c_str()))
	{
		IsIl2Cpp = true;
		Il2Cpp::GameAssemblyPath = new char[GameAssemblyPath.size() + 1];
		std::copy(GameAssemblyPath.begin(), GameAssemblyPath.end(), Il2Cpp::GameAssemblyPath);
		Il2Cpp::GameAssemblyPath[GameAssemblyPath.size()] = '\0';
	}
	return true;
}

bool Game::SetupPaths()
{
	LPSTR filepathstr = new CHAR[MAX_PATH];
	HMODULE exe_module = GetModuleHandleA(NULL);
	GetModuleFileNameA(exe_module, filepathstr, MAX_PATH);
	std::string filepath = filepathstr;
	delete[] filepathstr;

	ApplicationPath = new char[filepath.size() + 1];
	std::copy(filepath.begin(), filepath.end(), ApplicationPath);
	ApplicationPath[filepath.size()] = '\0';
	
	std::string BasePathStr = filepath.substr(0, filepath.find_last_of("\\/"));
	BasePath = new char[BasePathStr.size() + 1];
	std::copy(BasePathStr.begin(), BasePathStr.end(), BasePath);
	BasePath[BasePathStr.size()] = '\0';

	std::string DataPathStr = (filepath.substr(0, filepath.find_last_of(".")) + "_Data");
	DataPath = new char[DataPathStr.size() + 1];
	std::copy(DataPathStr.begin(), DataPathStr.end(), DataPath);
	DataPath[DataPathStr.size()] = '\0';

	return true;
}

void Game::ReadAppInfo()
{
	std::string appinfopath = std::string(DataPath) + "\\app.info";
	if (!Core::FileExists(appinfopath.c_str()))
	{
		Logger::Warning("app.info DOES NOT EXIST! Defaulting to UNKNOWN for Company and Product Names");
		std::string unknown = "UNKNOWN";
		Developer = new char[unknown.size() + 1];
		std::copy(unknown.begin(), unknown.end(), Developer);
		Developer[unknown.size()] = '\0';
		Name = new char[unknown.size() + 1];
		std::copy(unknown.begin(), unknown.end(), Name);
		Name[unknown.size()] = '\0';
		return;
	}
	std::ifstream appinfofile(appinfopath);
	std::string line;
	while (std::getline(appinfofile, line, '\n'))
		if (Developer == NULL)
		{
			Developer = new char[line.size() + 1];
			std::copy(line.begin(), line.end(), Developer);
			Developer[line.size()] = '\0';
		}
		else
		{
			Name = new char[line.size() + 1];
			std::copy(line.begin(), line.end(), Name);
			Name[line.size()] = '\0';
		}
	appinfofile.close();
}

void Game::UnknownUnityVersion()
{
	Logger::Warning("Defaulting to UNKNOWN for Unity Version");
	std::string unknown = "UNKNOWN";
	UnityVersion = new char[unknown.size() + 1];
	std::copy(unknown.begin(), unknown.end(), UnityVersion);
	UnityVersion[unknown.size()] = '\0';
}

void Game::ReadUnityVersion()
{
	DWORD handle;
	DWORD size = GetFileVersionInfoSizeA(ApplicationPath, &handle);
	if (size == NULL)
	{
		UnknownUnityVersion();
		return;
	}
	LPSTR data = new char[size];
	if (!GetFileVersionInfoA(ApplicationPath, handle, size, data))
	{
		UnknownUnityVersion();
		return;
	}
	UINT bufsize = 0;
	LPBYTE buf = NULL;
	if (!VerQueryValueA(data, "\\", (VOID FAR * FAR*) &buf, &bufsize) || (bufsize == NULL))
	{
		UnknownUnityVersion();
		return;
	}
	VS_FIXEDFILEINFO* info = (VS_FIXEDFILEINFO*)buf;
	if (info->dwSignature != 0xfeef04bd)
	{
		UnknownUnityVersion();
		return;
	}
	std::string output_version = std::to_string((info->dwFileVersionMS >> 16) & 0xffff) + "." + std::to_string((info->dwFileVersionMS >> 0) & 0xffff) + "." + std::to_string((info->dwFileVersionLS >> 16) & 0xffff);
	UnityVersion = new char[output_version.size() + 1];
	std::copy(output_version.begin(), output_version.end(), UnityVersion);
	UnityVersion[output_version.size()] = '\0';
}