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