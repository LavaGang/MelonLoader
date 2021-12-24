#include "Core.h"
#include "Exports.h"
#include "Utils/Directory.h"
#include "Utils/File.h"
#pragma warning (disable:4244)

void Core::Load(HINSTANCE hinstDLL)
{
	LPSTR filepathstr = new CHAR[MAX_PATH];
	HMODULE exe_module = GetModuleHandleA(NULL);
	GetModuleFileNameA(exe_module, filepathstr, MAX_PATH);
	std::string filepath = filepathstr;
	delete[] filepathstr;

	if (!IsUnityGame(filepath))
	{
		KillItDead();
		return;
	}

	std::string proxy_filepath = File::GetModuleFilePath(hinstDLL);
	std::for_each(proxy_filepath.begin(), proxy_filepath.end(), [](char& character) { character = ::tolower(character); });
	std::string proxy_filepath_no_ext = proxy_filepath.substr(0, proxy_filepath.find_last_of("."));

	int index = -1;
	if (!Exports::IsFileNameCompatible(proxy_filepath, index))
	{
		Error("Proxy has an Incompatible File Name!", true);
		KillItDead();
		return;
	}

	HMODULE originaldll = LoadOriginalDLL(proxy_filepath, proxy_filepath_no_ext);
	if (originaldll == NULL)
	{
		Error(("Failed to Load Original " + proxy_filepath + "!").c_str(), true);
		return;
	}

	Exports::Load(index, originaldll);

	if (strstr(GetCommandLineA(), "--no-mods") != NULL)
		return;

	std::string BasePath = filepath.substr(0, filepath.find_last_of("\\/"));
	std::string bootstrap_path = GetBootstrapPath(BasePath);
	if (bootstrap_path.empty())
		return;

	if (bootstrap_path.find('?') != std::string::npos)
	{
		Error("The base directory path contains non-ASCII characters,\nwhich are not supported by MelonLoader.\nPlease remove them and try again.");
		return;
	}

	if (!File::Exists(bootstrap_path.c_str()))
	{
		Error("Bootstrap.dll does not Exist in Base Directory!");
		return;
	}

	HMODULE bootstrap_module = LoadLibraryA(bootstrap_path.c_str());
	if (bootstrap_module == NULL)
		Error("Unable to Load Bootstrap.dll from Base Directory!");
}

HMODULE Core::LoadOriginalDLL(std::string proxy_filepath, std::string proxy_filepath_no_ext)
{
	HMODULE originaldll = LoadLibraryA((proxy_filepath_no_ext + "_original.dll").c_str());
	if (originaldll == NULL)
	{
		char* system32path = new char[MAX_PATH];
		if (GetSystemDirectoryA(system32path, MAX_PATH) == NULL)
		{
			delete[] system32path;
			Error("Failed to Get System32 Directory!");
			KillItDead();
			return NULL;
		}
		originaldll = LoadLibraryA((std::string(system32path) + "\\" + proxy_filepath).c_str());
		delete[] system32path;
	}
	return originaldll;
}

std::string Core::GetBootstrapPath(std::string BasePath)
{
	std::string defaultpath = "MelonLoader\\Dependencies\\Bootstrap.dll";
	std::string returnval = BasePath + "\\" + defaultpath;

	int argc = __argc;
	wchar_t** argv = CommandLineToArgvW(GetCommandLineW(), &argc);
	for (int i = 0; i < argc; i++)
	{
		wchar_t* arg = argv[i];
		if (arg == NULL)
			continue;
		if (wcscmp(arg, L"--melonloader.basedir") == 0)
		{
			wchar_t* arg_dir = argv[i + 1];
			if (arg_dir == NULL)
				break;
			else
			{
				std::wstring ws(arg_dir);
				returnval = std::string(ws.begin(), ws.end()) + "\\" + defaultpath;
				break;
			}
		}
	}
	LocalFree(argv);

	return returnval;
}

bool Core::IsUnityGame(std::string filepath)
{
	std::string datapath = (filepath.substr(0, filepath.find_last_of(".")) + "_Data");
	if (!Directory::Exists(datapath.c_str()))
		return false;
	if (File::Exists((datapath + "\\globalgamemanagers").c_str())
		|| File::Exists((datapath + "\\data.unity3d").c_str())
		|| File::Exists((datapath + "\\mainData").c_str()))
		return true;
	return false;
}

void Core::Error(std::string reason, bool should_kill)
{
	MessageBoxA(NULL, (reason + " " + (should_kill ? "Preventing Startup" : "Continuing without MelonLoader") + "...").c_str(), "MelonLoader", MB_ICONERROR | MB_OK);
	if (should_kill)
		KillItDead();
};

void Core::KillItDead()
{
	HANDLE current_process = GetCurrentProcess();
	TerminateProcess(current_process, NULL);
	CloseHandle(current_process);
};