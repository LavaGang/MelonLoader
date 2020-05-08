#include <filesystem>
#include <fstream>
#include <iostream>
#include <sstream>
#include "MelonLoader.h"
#include "Console.h"
#include "Mono.h"
#include "HookManager.h"
#include "Logger.h"
#include "ModHandler.h"

bool MelonLoader::IsGameIl2Cpp = false;
HINSTANCE MelonLoader::thisdll = NULL;
bool MelonLoader::DebugMode = false;
bool MelonLoader::RainbowMode = false;
bool MelonLoader::RandomRainbowMode = false;
bool MelonLoader::QuitFix = false;
char* MelonLoader::GamePath = NULL;
char* MelonLoader::DataPath = NULL;
char* MelonLoader::CompanyName = NULL;
char* MelonLoader::ProductName = NULL;

void MelonLoader::Main()
{
	LPSTR filepath = new CHAR[MAX_PATH];
	GetModuleFileName(GetModuleHandle(NULL), filepath, MAX_PATH);
	std::string filepathstr = filepath;
	filepathstr = filepathstr.substr(0, filepathstr.find_last_of("\\/"));

	GamePath = new char[filepathstr.size() + 1];
	std::copy(filepathstr.begin(), filepathstr.end(), GamePath);
	GamePath[filepathstr.size()] = '\0';

	if (strstr(GetCommandLine(), "--melonloader.rainbow") != NULL)
		RainbowMode = true;
	if (strstr(GetCommandLine(), "--melonloader.randomrainbow") != NULL)
		RandomRainbowMode = true;
	if (strstr(GetCommandLine(), "--melonloader.quitfix") != NULL)
		QuitFix = true;

	std::string gameassemblypath = filepathstr + "\\GameAssembly.dll";
	WIN32_FIND_DATA data;
	HANDLE h = FindFirstFile(gameassemblypath.c_str(), &data);
	if (h != INVALID_HANDLE_VALUE)
		IsGameIl2Cpp = true;

	Logger::Initialize(filepathstr);

#ifndef _DEBUG
	if (strstr(GetCommandLine(), "--melonloader.debug") != NULL)
	{
#endif
		Console::Create();
		DebugMode = true;
#ifndef _DEBUG
	}
#endif

	std::string pdatapath = filepathstr + "\\*_Data";
	h = FindFirstFile(pdatapath.c_str(), &data);
	if (h != INVALID_HANDLE_VALUE)
	{
		char* nPtr = new char[lstrlen(data.cFileName) + 1];
		for (int i = 0; i < lstrlen(data.cFileName); i++)
			nPtr[i] = char(data.cFileName[i]);
		nPtr[lstrlen(data.cFileName)] = '\0';

		std::string ndatapath = filepathstr + "\\" + std::string(nPtr);
		DataPath = new char[ndatapath.size() + 1];
		std::copy(ndatapath.begin(), ndatapath.end(), DataPath);
		DataPath[ndatapath.size()] = '\0';

		std::string assemblypath = std::string();
		if (IsGameIl2Cpp)
		{
			assemblypath = filepathstr + "\\MelonLoader\\Managed";
			std::string configpath = ndatapath + "\\il2cpp_data\\etc";
			Mono::ConfigPath = new char[configpath.size() + 1];
			std::copy(configpath.begin(), configpath.end(), Mono::ConfigPath);
			Mono::ConfigPath[configpath.size()] = '\0';
		}
		else
			assemblypath = ndatapath + "\\Managed";
		Mono::AssemblyPath = new char[assemblypath.size() + 1];
		std::copy(assemblypath.begin(), assemblypath.end(), Mono::AssemblyPath);
		Mono::AssemblyPath[assemblypath.size()] = '\0';

		ReadAppInfo();

		if (IsGameIl2Cpp)
		{
			if (Mono::Load() && Mono::Setup())
				HookManager::LoadLibraryW_Hook();
		}
		else
			HookManager::LoadLibraryW_Hook();
	}
}

void MelonLoader::ReadAppInfo()
{
	std::ifstream appinfofile((std::string(DataPath) + "\\app.info"));
	std::stringstream filebuffer;
	filebuffer << appinfofile.rdbuf();
	appinfofile.close();
	std::string line;
	while (std::getline(filebuffer, line, '\n'))
	{
		if (CompanyName == NULL)
		{
			CompanyName = new char[line.size() + 1];
			std::copy(line.begin(), line.end(), CompanyName);
			CompanyName[line.size()] = '\0';
		}
		else
		{
			ProductName = new char[line.size() + 1];
			std::copy(line.begin(), line.end(), ProductName);
			ProductName[line.size()] = '\0';
		}
	}
}

void MelonLoader::UNLOAD(bool alt)
{
	if (!alt)
		ModHandler::OnApplicationQuit();
	HookManager::UnhookAll();
	if (!alt)
		Logger::Log("UNLOADED!");
	Logger::Stop();
}

void MelonLoader::KillProcess()
{
	HANDLE hProcess = GetCurrentProcess();
	if (hProcess != NULL)
	{
		TerminateProcess(hProcess, NULL);
		CloseHandle(hProcess);
	}
}

bool MelonLoader::Is64bit()
{
	// To-Do
	return true;
}

int MelonLoader::CountSubstring(std::string pat, std::string txt)
{
	size_t M = pat.length();
	size_t N = txt.length();
	int res = 0;
	for (int i = 0; i <= N - M; i++)
	{
		int j;
		for (j = 0; j < M; j++)
			if (txt[(int)i + j] != pat[j])
				break;
		if (j == M)
		{
			res++;
			j = 0;
		}
	}
	return res;
}

bool MelonLoader::DirectoryExists(const char* path)
{
	struct stat info;
	if (stat(path, &info) != NULL)
		return false;
	if (info.st_mode & S_IFDIR)
		return true;
	return false;
}