#include <string>
#include <fstream>
#include <sstream>
#include "Game.h"
#include "../Base/Core.h"
#include "Il2Cpp.h"
#include "../Utils/Assertion.h"
#include "../Utils/Console/Logger.h"
#include "../Utils/Encoding.h"
#include "../Utils/Console/Debug.h"
#include "AndroidData.h"
#include <string.h>
#pragma comment(lib,"version.lib")

#ifdef _WIN32
#include <Windows.h>
#elif defined(__ANDROID__)
#include <android/asset_manager.h>
#include <android/asset_manager_jni.h>
#endif

char* Game::ApplicationPath = NULL;
char* Game::BasePath = NULL;
char* Game::DataPath = NULL;
char* Game::ApplicationPathMono = NULL;
char* Game::BasePathMono = NULL;
char* Game::DataPathMono = NULL;
char* Game::Package = NULL;
char* Game::Developer = NULL;
char* Game::Name = NULL;
char* Game::UnityVersion = NULL;
bool Game::IsIl2Cpp = false;
bool Game::FirstRun = true;

bool Game::Initialize()
{
	if (!SetupPaths())
	{
		Assertion::ThrowInternalFailure("Failed to Setup Game Paths!");
		return false;
	}
#if _WIN32
	std::string GameAssemblyPath = (std::string(BasePath) + "\\GameAssembly.dll");
	std::string UnityPlayerPath = (std::string(BasePath) + "\\UnityPlayer.dll");
	if (Core::FileExists(GameAssemblyPath.c_str()))
	{
		IsIl2Cpp = true;
		Il2Cpp::GameAssemblyPath = new char[GameAssemblyPath.size() + 1];
		std::copy(GameAssemblyPath.begin(), GameAssemblyPath.end(), Il2Cpp::GameAssemblyPath);
		Il2Cpp::GameAssemblyPath[GameAssemblyPath.size()] = '\0';

		Il2Cpp::GameAssemblyPathMono = Encoding::OsToUtf8(Il2Cpp::GameAssemblyPath);
	}
	Il2Cpp::UnityPlayerPath = new char[UnityPlayerPath.size() + 1];
	std::copy(UnityPlayerPath.begin(), UnityPlayerPath.end(), Il2Cpp::UnityPlayerPath);
	Il2Cpp::UnityPlayerPath[UnityPlayerPath.size()] = '\0';
#else 
	IsIl2Cpp = true;
#endif
	return true;
}

bool Game::SetupPaths()
{
#if _WIN32
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

#elif defined(__ANDROID__)
	size_t BaseDirLen = strlen(AndroidData::BaseDataDir);
	size_t AppNameLen = strlen(AndroidData::AppName);
	size_t AppPathLen = BaseDirLen + AppNameLen + 2;
	ApplicationPath = (char*)malloc(AppPathLen);

	memcpy(ApplicationPath, AndroidData::BaseDataDir, BaseDirLen);
	memcpy(ApplicationPath + BaseDirLen + 1, AndroidData::AppName, AppNameLen);

	ApplicationPath[BaseDirLen] = '/';
	ApplicationPath[AppPathLen - 1] = '\0';

	size_t PathLen = strlen(AndroidData::DataDir);
	BasePath = (char*)malloc(PathLen + 1);
	DataPath = (char*)malloc(PathLen + 1);

	memcpy(BasePath, AndroidData::DataDir, PathLen);
	memcpy(DataPath, AndroidData::DataDir, PathLen);

	BasePath[PathLen] = '\0';
	DataPath[PathLen] = '\0';

	Debug::Msg(ApplicationPath);
	Debug::Msg(BasePath);
	Debug::Msg(DataPath);
#endif

	return true;
}

bool Game::ReadInfo()
{
	ReadAppInfo();
	return ReadUnityVersion();
}

void Game::ReadAppInfo()
{
#ifndef PORT_DISABLE
	std::string appinfopath = std::string(DataPath) + "\\app.info";
	if (!Core::FileExists(appinfopath.c_str()))
	{
		if (FirstRun)
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
	FirstRun = false;
	Console::SetDefaultTitleWithGameName();
#elif defined(__ANDROID__)
	size_t AppNameLen = strlen(AndroidData::AppName);
	Package = (char*)malloc(AppNameLen + 1);
	memcpy(Package, AndroidData::AppName, AppNameLen);
	Package[AppNameLen] = '\0';

	Debug::Msgf("Package:   %s", Package);

	std::string PackageStr = std::string(Package);
	size_t DeveloperStart = PackageStr.find('.');
	if (DeveloperStart == std::string::npos)
		return;

	size_t NameStart = PackageStr.find('.', DeveloperStart + 1);
	if (NameStart == std::string::npos)
		return;

	size_t NameEnd = PackageStr.find('.', NameStart + 1);
	if (NameEnd == std::string::npos)
		NameEnd = PackageStr.size();
	
	std::string DeveloperStr = PackageStr.substr(DeveloperStart + 1, NameStart - DeveloperStart - 1);
	Developer = new char[DeveloperStr.size() + 1];
	std::copy(DeveloperStr.begin(), DeveloperStr.end(), Developer);
	Developer[DeveloperStr.size()] = '\0';
	
	std::string NameStr = PackageStr.substr(NameStart + 1, NameEnd - NameStart);
	Name = new char[NameStr.size() + 1];
	std::copy(NameStr.begin(), NameStr.end(), Name);
	Name[NameStr.size()] = '\0';
	
	Debug::Msgf("Developer: %s", Developer);
	Debug::Msgf("Name:      %s", Name);
#endif
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
		Assertion::ThrowInternalFailure("Failed to Read Unity Version from File Info or globalgamemanagers!");
		return false;
	}

	UnityVersion = new char[version.size() + 1];
	std::copy(version.begin(), version.end(), UnityVersion);
	UnityVersion[version.size()] = '\0';

	Debug::Msgf("Unity:     %s", UnityVersion);

	return true;
}

std::string Game::ReadUnityVersionFromFileInfo()
{
#ifdef _WIN32
	const char* output = Core::GetFileInfoProductVersion(ApplicationPath);
	if (output == NULL)
		return std::string();
	std::string outputstr = output;
	outputstr = outputstr.substr(0, outputstr.find_last_of('.'));
	return outputstr;
#else 
	return std::string();
#endif
}

// TODO: load asset manager globally
std::string Game::ReadUnityVersionFromGlobalGameManagers()
{
#ifdef _WIN32
#define STARTING_INDEX 22
	std::string globalgamemanagerspath = std::string(DataPath) + "\\globalgamemanagers";
	if (!Core::FileExists(globalgamemanagerspath.c_str()))
		return std::string();
	std::ifstream globalgamemanagersstream(globalgamemanagerspath, std::ios::binary);
	if (!globalgamemanagersstream.is_open() || !globalgamemanagersstream.good())
		return std::string();
	std::vector<char> filedata((std::istreambuf_iterator<char>(globalgamemanagersstream)), (std::istreambuf_iterator<char>()));
	globalgamemanagersstream.close();
#elif defined(__ANDROID__)
#define STARTING_INDEX 0x14
	jclass jCore = Core::Env->FindClass("com/melonloader/Core");
	if (jCore == NULL)
		return std::string();

	jmethodID mid = Core::Env->GetStaticMethodID(jCore, "GetAssetManager", "()Landroid/content/res/AssetManager;");
	if (mid == NULL)
		return std::string();

	jobject jAM = Core::Env->CallStaticObjectMethod(jCore, mid);
	AAssetManager* am = AAssetManager_fromJava(Core::Env, jAM);
	if (am == NULL)
		return std::string();

	AAsset* asset = AAssetManager_open(am, "bin/Data/globalgamemanagers", AASSET_MODE_BUFFER);
	if (asset == NULL)
		return std::string();
	
	const char* filedata = (const char*)AAsset_getBuffer(asset);
	if (filedata == NULL)
		return std::string();
#endif

	std::stringstream output;
	int i = STARTING_INDEX;
#undef STARTING_INDEX
	
	while (filedata[i] != NULL)
	{
		char bit = filedata[i];
		if (bit == 0x00 || bit == 0x66)
			break;
		output << filedata[i];
		i++;
	}

#ifdef __ANDROID__
	AAsset_close(asset);
#endif

	return output.str();
}

std::string Game::ReadUnityVersionFromMainData()
{
#ifdef _WIN32
#define STARTING_INDEX 20
	std::string maindatapath = std::string(DataPath) + "\\mainData";
	if (!Core::FileExists(maindatapath.c_str()))
		return std::string();
	std::ifstream maindatastream(maindatapath, std::ios::binary);
	if (!maindatastream.is_open() || !maindatastream.good())
		return std::string();
	std::vector<char> filedata((std::istreambuf_iterator<char>(maindatastream)), (std::istreambuf_iterator<char>()));
	maindatastream.close();
#elif defined(__ANDROID__)
#define STARTING_INDEX 0x12
	jclass jCore = Core::Env->FindClass("com/melonloader/Core");
	if (jCore == NULL)
		return std::string();

	jmethodID mid = Core::Env->GetStaticMethodID(jCore, "GetAssetManager", "()Landroid/content/res/AssetManager;");
	if (mid == NULL)
		return std::string();

	jobject jAM = Core::Env->CallStaticObjectMethod(jCore, mid);
	AAssetManager* am = AAssetManager_fromJava(Core::Env, jAM);
	if (am == NULL)
		return std::string();

	AAsset* asset = AAssetManager_open(am, "bin/Data/data.unity3d", AASSET_MODE_BUFFER);
	if (asset == NULL)
		return std::string();

	const char* filedata = (const char*)AAsset_getBuffer(asset);
	if (filedata == NULL)
		return std::string();
#endif
	std::stringstream output;
	int i = STARTING_INDEX;
#undef STARTING_INDEX
	
	while (filedata[i] != NULL)
	{
		char bit = filedata[i];
		if (bit == 0x0 || bit == 0x66)
			break;
		output << filedata[i];
		i++;
	}

#ifdef __ANDROID__
	AAsset_close(asset);
#endif
	
	return output.str();
}