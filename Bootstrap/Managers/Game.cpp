#ifdef _WIN32
#include <Windows.h>
#endif

#include <string>
#include <fstream>
#include <sstream>
#include "Game.h"
#include "../Base/Core.h"
#include "Il2Cpp.h"
#include "../Utils/Assertion.h"
#include "../Utils/Console/Logger.h"
#pragma comment(lib,"version.lib")

char* Game::ApplicationPath = NULL;
char* Game::BasePath = NULL;
char* Game::DataPath = NULL;
char* Game::Developer = NULL;
char* Game::Name = NULL;
char* Game::UnityVersion = NULL;

#ifdef PORT_DISABLE
bool Game::IsIl2Cpp = false;
#else
bool Game::IsIl2Cpp = true;
#endif


bool Game::Initialize()
{
	if (!SetupPaths())
	{
		Assertion::ThrowInternalFailure("Failed to Setup Game Paths!");
		return false;
	}

#ifdef PORT_DISABLE
	std::string GameAssemblyPath = (std::string(BasePath) + "\\GameAssembly.dll");
	if (Core::FileExists(GameAssemblyPath.c_str()))
	{
		IsIl2Cpp = true;
		Il2Cpp::GameAssemblyPath = new char[GameAssemblyPath.size() + 1];
		std::copy(GameAssemblyPath.begin(), GameAssemblyPath.end(), Il2Cpp::GameAssemblyPath);
		Il2Cpp::GameAssemblyPath[GameAssemblyPath.size()] = '\0';
	}
#endif
	return true;
}

bool Game::SetupPaths()
{
#ifdef _WIN32
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
#elif defined(__ANDROID__)
	BasePath = "/storage/emulated/0/Android/data/com.SirCoolness.PlaygroundQuestGame/files";
	DataPath = "/storage/emulated/0/Android/data/com.SirCoolness.PlaygroundQuestGame/files";
#endif
	return true;
}

#ifdef PORT_DISABLE
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
	std::string version = ReadUnityVersionFromFileInfo();
	if (version.empty() || (strstr(version.c_str(), ".") == NULL))
		version = ReadUnityVersionFromGlobalGameManagers();
	if (version.empty() || (strstr(version.c_str(), ".") == NULL))
	{
		Assertion::ThrowInternalFailure("Failed to Read Unity Version from File Info or globalgamemanagers!");
		return false;
	}
	UnityVersion = new char[version.size() + 1];
	std::copy(version.begin(), version.end(), UnityVersion);
	UnityVersion[version.size()] = '\0';
	return true;
}

std::string Game::ReadUnityVersionFromFileInfo()
{
	const char* output = Core::GetFileInfoProductVersion(ApplicationPath);
	if (output == NULL)
		return NULL;
	std::string outputstr = output;
	//Logger::Msg(outputstr.c_str());
	outputstr = outputstr.substr(0, outputstr.find_last_of('.'));
	//Logger::Msg(outputstr.c_str());
	return outputstr;
}

std::string Game::ReadUnityVersionFromGlobalGameManagers()
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
	return output.str();
}
#endif