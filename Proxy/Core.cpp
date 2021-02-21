#include "Core.h"
#include <string>
#include <algorithm>
#include <filesystem>

void Core::Load(HINSTANCE hinstDLL)
{
	ApplicationCheck();

	LPSTR a1 = new CHAR[MAX_PATH];
	GetModuleFileNameA(hinstDLL, a1, MAX_PATH);
	std::filesystem::path a2(a1);
	std::string filename = a2.filename().string();
	std::for_each(filename.begin(), filename.end(), [](char& character) { character = ::tolower(character); });

	std::string filename_no_ext = filename.substr(0, filename.find_last_of("."));
	HMODULE originaldll = LoadLibraryA((filename_no_ext + "_original.dll").c_str());
	if (originaldll == NULL)
	{
		char* system32path = new char[MAX_PATH];
		if (GetSystemDirectoryA(system32path, MAX_PATH) == NULL)
		{
			delete[] system32path;
			MessageBoxA(NULL, "Failed to Get System32 Directory!", "MelonLoader", MB_ICONERROR | MB_OK);
			KillItDead();
			return;
		}
		originaldll = LoadLibraryA((std::string(system32path) + "\\" + filename).c_str());
		delete[] system32path;
	}

	if (originaldll == NULL)
	{
		MessageBoxA(NULL, ("Failed to Load " + filename + "!").c_str(), "MelonLoader", MB_ICONERROR | MB_OK);
		KillItDead();
		return;
	}

	if (strstr(filename.c_str(), "version") != NULL)
		LoadExports_version(originaldll);
	else if (strstr(filename.c_str(), "winmm") != NULL)
		LoadExports_winmm(originaldll);
	else if (strstr(filename.c_str(), "winhttp") != NULL)
		LoadExports_winhttp(originaldll);
	else
	{
		KillItDead();
		return;
	}

	if (strstr(GetCommandLineA(), "--no-mods") != NULL)
		return;
	LoadLibraryA("MelonLoader\\Dependencies\\Bootstrap.dll");
}

void Core::ApplicationCheck()
{
	LPSTR a1 = new CHAR[MAX_PATH];
	GetModuleFileNameA(GetModuleHandleA(NULL), a1, MAX_PATH);
	std::filesystem::path a2(a1);
	std::string filename = a2.filename().string();
	std::for_each(filename.begin(), filename.end(), [](char& character) { character = ::tolower(character); });
	if (strstr(filename.c_str(), "unitycrashhandler") != NULL)
		KillItDead();
}

void Core::KillItDead()
{
	HANDLE current_process = GetCurrentProcess();
	TerminateProcess(current_process, NULL);
	CloseHandle(current_process);
}