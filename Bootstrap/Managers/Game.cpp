#include <Windows.h>
#include <string>
#include <fstream>
#include <sstream>
#include "Game.h"
#include "../Core.h"
#include "Il2Cpp.h"
#include "../Utils/Assertion.h"
#include "../Utils/Logging/Logger.h"
#include "../Utils/Encoding.h"
#pragma comment(lib,"version.lib")

char* Game::ApplicationPath = NULL;
char* Game::BasePath = NULL;
char* Game::DataPath = NULL;
char* Game::ApplicationPathMono = NULL;
char* Game::BasePathMono = NULL;
char* Game::DataPathMono = NULL;
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

		Il2Cpp::GameAssemblyPathMono = Encoding::OsToUtf8(Il2Cpp::GameAssemblyPath);
	}

	std::string UnityPlayerPath = (std::string(BasePath) + "\\UnityPlayer.dll");
	Il2Cpp::UnityPlayerPath = new char[UnityPlayerPath.size() + 1];
	std::copy(UnityPlayerPath.begin(), UnityPlayerPath.end(), Il2Cpp::UnityPlayerPath);
	Il2Cpp::UnityPlayerPath[UnityPlayerPath.size()] = '\0';

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

#define MONO_STR(s) ((s ## Mono) = Encoding::OsToUtf8((s)))
	MONO_STR(ApplicationPath);
	MONO_STR(BasePath);
	MONO_STR(DataPath);
#undef MONO_STR

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
		Logger::QuickLog("app.info DOES NOT EXIST! Defaulting to UNKNOWN for Company and Product Names", Warning);
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
	Console::SetDefaultTitleWithGameName();
}

bool Game::ReadUnityVersion()
{
	std::string version = ReadUnityVersionFromFileInfo();
	if (version.empty() || (strstr(version.c_str(), ".") == NULL))
		version = ReadUnityVersionFromGlobalGameManagers();
	if (version.empty() || (strstr(version.c_str(), ".") == NULL))
		version = ReadUnityVersionFromMainData();
	if (version.empty() || (strstr(version.c_str(), ".") == NULL))
	{
		Assertion::ThrowInternalFailure("Failed to Read Unity Version from UnityPlayer.dll, globalgamemanagers, or mainData!");
		return false;
	}
	UnityVersion = new char[version.size() + 1];
	std::copy(version.begin(), version.end(), UnityVersion);
	UnityVersion[version.size()] = '\0';
	return true;
}

std::string Game::ReadUnityVersionFromFileInfo()
{
	const char* output = Core::GetFileInfoProductVersion(Il2Cpp::UnityPlayerPath);
	if (output == NULL)
		return std::string();
	std::string outputstr = output;
	outputstr = outputstr.substr(0, outputstr.find_last_of('.'));
	return outputstr;
}

std::string Game::ReadUnityVersionFromGlobalGameManagers()
{
	std::string globalgamemanagerspath = std::string(DataPath) + "\\globalgamemanagers";
	if (!Core::FileExists(globalgamemanagerspath.c_str()))
		return std::string();
	std::ifstream globalgamemanagersstream(globalgamemanagerspath, std::ios::binary);
	if (!globalgamemanagersstream.is_open() || !globalgamemanagersstream.good())
		return std::string();
	std::vector<char> filedata((std::istreambuf_iterator<char>(globalgamemanagersstream)), (std::istreambuf_iterator<char>()));
	globalgamemanagersstream.close();
	std::stringstream output;
	int i = 22;
	while (filedata[i] != NULL)
	{
		char bit = filedata[i];
		if (bit == 0x00)
			break;
		output << filedata[i];
		i++;
	}
	return output.str();
}

std::string Game::ReadUnityVersionFromMainData()
{
	std::string maindatapath = std::string(DataPath) + "\\mainData";
	if (!Core::FileExists(maindatapath.c_str()))
		return std::string();
	std::ifstream maindatastream(maindatapath, std::ios::binary);
	if (!maindatastream.is_open() || !maindatastream.good())
		return std::string();
	std::vector<char> filedata((std::istreambuf_iterator<char>(maindatastream)), (std::istreambuf_iterator<char>()));
	maindatastream.close();
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
	return output.str();
}