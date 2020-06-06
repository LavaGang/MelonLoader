#include <filesystem>
#include <fstream>
#include <iostream>
#include <sstream>
#include <memory>
#include "MelonLoader.h"
#include "Console.h"
#include "Mono.h"
#include "HookManager.h"
#include "Logger.h"
#include "ModHandler.h"
#include "UnityPlayer.h"
#pragma warning(disable:4996)

HINSTANCE MelonLoader::thisdll = NULL;
int MelonLoader::CommandLineC = NULL;
char* MelonLoader::CommandLineV[64];
bool MelonLoader::IsGameIl2Cpp = false;
bool MelonLoader::DebugMode = false;
bool MelonLoader::ConsoleEnabled = true;
bool MelonLoader::RainbowMode = false;
bool MelonLoader::RandomRainbowMode = false;
bool MelonLoader::QuitFix = false;
char* MelonLoader::ExePath = NULL;
char* MelonLoader::GamePath = NULL;
char* MelonLoader::DataPath = NULL;
char* MelonLoader::CompanyName = NULL;
char* MelonLoader::ProductName = NULL;

void MelonLoader::Main()
{
	if (CheckOSVersion())
	{
		ParseCommandLine();

		LPSTR filepath = new CHAR[MAX_PATH];
		HMODULE exe_module = GetModuleHandle(NULL);
		GetModuleFileName(exe_module, filepath, MAX_PATH);

		long exe_size = GetFileSize(filepath);
		if ((exe_size * 0.000001) > 10)
			UnityPlayer::Module = exe_module;

		std::string filepathstr = filepath;
		ExePath = new char[filepathstr.size() + 1];
		std::copy(filepathstr.begin(), filepathstr.end(), ExePath);
		ExePath[filepathstr.size()] = '\0';

		filepathstr = filepathstr.substr(0, filepathstr.find_last_of("\\/"));
		GamePath = new char[filepathstr.size() + 1];
		std::copy(filepathstr.begin(), filepathstr.end(), GamePath);
		GamePath[filepathstr.size()] = '\0';

		std::string gameassemblypath = filepathstr + "\\GameAssembly.dll";
		WIN32_FIND_DATA data;
		HANDLE h = FindFirstFile(gameassemblypath.c_str(), &data);
		if (h != INVALID_HANDLE_VALUE)
			IsGameIl2Cpp = true;

		Logger::Initialize(filepathstr);

#ifdef DEBUG
		Console::Create();
		DebugMode = true;
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
			std::string basepath = std::string();
			std::string configpath = std::string();
			if (IsGameIl2Cpp)
			{
				assemblypath = filepathstr + "\\MelonLoader\\Managed";
				basepath = filepathstr + "\\MelonLoader\\Mono";
				configpath = ndatapath + "\\il2cpp_data\\etc";
			}
			else
			{
				assemblypath = ndatapath + "\\Managed";
				std::string newbasepath = filepathstr + "\\Mono";
				h = FindFirstFile(newbasepath.c_str(), &data);
				if (h == INVALID_HANDLE_VALUE)
				{
					newbasepath = ndatapath + "\\Mono";
					h = FindFirstFile(newbasepath.c_str(), &data);
				}
				if (h == INVALID_HANDLE_VALUE)
				{
					newbasepath = filepathstr + "\\MonoBleedingEdge";
					h = FindFirstFile(newbasepath.c_str(), &data);
				}
				if (h == INVALID_HANDLE_VALUE)
				{
					newbasepath = ndatapath + "\\MonoBleedingEdge";
					h = FindFirstFile(newbasepath.c_str(), &data);
				}
				if (h != INVALID_HANDLE_VALUE)
				{
					basepath = newbasepath + "\\EmbedRuntime";
					configpath = newbasepath + "\\etc";
				}
			}
			if (!assemblypath.empty())
			{
				Mono::AssemblyPath = new char[assemblypath.size() + 1];
				std::copy(assemblypath.begin(), assemblypath.end(), Mono::AssemblyPath);
				Mono::AssemblyPath[assemblypath.size()] = '\0';
			}
			if (!basepath.empty())
			{
				Mono::BasePath = new char[basepath.size() + 1];
				std::copy(basepath.begin(), basepath.end(), Mono::BasePath);
				Mono::BasePath[basepath.size()] = '\0';
			}
			if (!configpath.empty())
			{
				Mono::ConfigPath = new char[configpath.size() + 1];
				std::copy(configpath.begin(), configpath.end(), Mono::ConfigPath);
				Mono::ConfigPath[configpath.size()] = '\0';
			}

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
}

void MelonLoader::ParseCommandLine()
{
	char* next = NULL;
	char* curchar = strtok_s(GetCommandLine(), " ", &next);
	while (curchar && (CommandLineC < 63))
	{
		CommandLineV[CommandLineC++] = curchar;
		curchar = strtok_s(0, " ", &next);
	}
	CommandLineV[CommandLineC] = 0;
	if (CommandLineC > 0)
	{
		for (int i = 0; i < CommandLineC; i++)
		{
			const char* command = CommandLineV[i];
			if (command != NULL)
			{
				if (strstr(command, "--melonloader.rainbow") != NULL)
					RainbowMode = true;
				else if (strstr(command, "--melonloader.randomrainbow") != NULL)
					RandomRainbowMode = true;
				else if (strstr(command, "--melonloader.quitfix") != NULL)
					QuitFix = true;
				else if (strstr(command, "--melonloader.maxlogs") != NULL)
					Logger::MaxLogs = GetIntFromConstChar(CommandLineV[i + 1], 10);
				else if (strstr(command, "--melonloader.hideconsole") != NULL)
					ConsoleEnabled = false;
#ifndef DEBUG
				else if (strstr(command, "--melonloader.debug") != NULL)
				{
					Console::Create();
					DebugMode = true;
				}
#endif
			}
		}
	}
}

void MelonLoader::ReadAppInfo()
{
	std::ifstream appinfofile((std::string(DataPath) + "\\app.info"));
	std::string line;
	while (std::getline(appinfofile, line, '\n'))
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
	appinfofile.close();
}

bool MelonLoader::CheckOSVersion()
{
	OSVERSIONINFO info;
	ZeroMemory(&info, sizeof(OSVERSIONINFO));
	info.dwOSVersionInfoSize = sizeof(OSVERSIONINFO);
	GetVersionEx(&info);
	if ((info.dwMajorVersion < 6) || ((info.dwMajorVersion == 6) && (info.dwMinorVersion < 1)))
	{
		int result = MessageBox(NULL, "You are running on an Unsupported OS.\nWe can not offer support if there are any issues.\nContinue with MelonLoader?", "MelonLoader", MB_ICONWARNING | MB_YESNO);
		if (result == IDYES)
			return true;
		return false;
	}
	return true;
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

int MelonLoader::CountSubstring(std::string pat, std::string txt)
{
	size_t M = pat.length();
	size_t N = txt.length();
	int res = 0;
	for (int i = 0; i <= N - M; i++)
	{
		int j;
		for (j = 0; j < M; j++)
			if (txt[i + j] != pat[j])
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

long MelonLoader::GetFileSize(std::string filename)
{
	struct stat stat_buf;
	return ((stat(filename.c_str(), &stat_buf) == 0) ? stat_buf.st_size : -1);
}

int MelonLoader::GetIntFromConstChar(const char* str, int defaultval)
{
	if (str == NULL || *str == '\0')
		return defaultval;
    bool negate = (str[0] == '-');
    if ( *str == '+' || *str == '-' )
        ++str;
	if (*str == '\0')
		return defaultval;
    int result = 0;
    while(*str)
    {
		if (*str >= '0' && *str <= '9')
			result = result * 10 - (*str - '0');
		else
			return defaultval;
        ++str;
    }
    return negate ? result : -result;
}