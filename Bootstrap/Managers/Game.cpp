#include <Windows.h>
#include <string>
#include <fstream>
#include <sstream>
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

bool Game::ReadInfo()
{
	ReadAppInfo();
	return ReadUnityVersion();
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

bool Game::ReadUnityVersion()
{
	const char* version = ReadUnityVersionFromFileInfo();
	if ((version == NULL) || (strstr(version, ".") == NULL))
	{
		Logger::Warning("Failed to Read Unity Version from File Info! Attempting Fallback to globalgamemanagers");
		version = ReadUnityVersionFromGlobalGameManagers();
	}
	if ((version == NULL) || (strstr(version, ".") == NULL))
	{
		Assertion::ThrowInternalFailure("Failed to Read Unity Version from File Info or globalgamemanagers!");
		return false;
	}
	std::string versionstr = version;
	UnityVersion = new char[versionstr.size() + 1];
	std::copy(versionstr.begin(), versionstr.end(), UnityVersion);
	UnityVersion[versionstr.size()] = '\0';
	return true;
}

const char* Game::ReadUnityVersionFromFileInfo()
{
	DWORD handle;
	DWORD size = GetFileVersionInfoSizeA(ApplicationPath, &handle);
	if (size == NULL)
		return NULL;
	LPSTR data = new char[size];
	if (!GetFileVersionInfoA(ApplicationPath, handle, size, data))
		return NULL;
	UINT bufsize = 0;
	VS_FIXEDFILEINFO* info = NULL;
	if (!VerQueryValueA(data, "\\", (LPVOID*)&info, &bufsize) || (bufsize == NULL))
		return NULL;
	return (std::to_string((info->dwFileVersionMS >> 16) & 0xffff)
		+ "."
		+ std::to_string((info->dwFileVersionMS >> 0) & 0xffff)
		+ "."
		+ std::to_string((info->dwFileVersionLS >> 16) & 0xffff)).c_str();
}

const char* Game::ReadUnityVersionFromGlobalGameManagers()
{
	std::string globalgamemanagerspath = std::string(DataPath) + "\\globalgamemanagers";
	if (!Core::FileExists(globalgamemanagerspath.c_str()))
		return NULL;
	std::ifstream globalgamemanagersstream(globalgamemanagerspath, std::ios::binary);
	if (!globalgamemanagersstream.is_open() || !globalgamemanagersstream.good())
		return NULL;
	std::vector<char> filedata((std::istreambuf_iterator<char>(globalgamemanagersstream)), (std::istreambuf_iterator<char>()));
	globalgamemanagersstream.close();
	std::stringstream output;
	int i = 20;
	while (filedata[i] != NULL)
	{
		char bit = filedata[i];
		if (bit == 0x66)
			break;
		output << filedata[i];
		i++;
	}
	return output.str().c_str();
}