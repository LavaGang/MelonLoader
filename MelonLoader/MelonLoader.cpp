#include <Windows.h>
#include <string>
#include <filesystem>
#include "MelonLoader.h"
#include "Console.h"
#include "Mono.h"
#include "Il2Cpp.h"
#include "Hooks/Hooks.h"
#include "PointerUtils.h"
#include "detours/detours.h"

bool MelonLoader::IsGameIl2Cpp = false;
HMODULE MelonLoader::MonoUnityPlayerDLL = NULL;
HMODULE MelonLoader::MonoDLL = NULL;
HMODULE MelonLoader::IL2CPPUnityPlayerDLL = NULL;
HMODULE MelonLoader::GameAssemblyDLL = NULL;
HINSTANCE MelonLoader::thisdll = NULL;
MonoAssembly* MelonLoader::ModHandlerAssembly = NULL;
bool MelonLoader::DebugMode = false;
bool MelonLoader::MupotMode = false;
char* MelonLoader::GamePath = NULL;
char* MelonLoader::DataPath = NULL;

void MelonLoader::Main()
{
	LPSTR filepath = new CHAR[MAX_PATH];
	GetModuleFileName(GetModuleHandle(NULL), filepath, MAX_PATH);
	std::string filepathstr = filepath;
	filepathstr = filepathstr.substr(0, filepathstr.find_last_of("\\/"));

	GamePath = new char[filepathstr.size() + 1];
	std::copy(filepathstr.begin(), filepathstr.end(), GamePath);
	GamePath[filepathstr.size()] = '\0';

	if (strstr(GetCommandLine(), "--melonloader.mupot") != NULL)
		MupotMode = true;

	std::string gameassemblypath = filepathstr + "\\GameAssembly.dll";
	WIN32_FIND_DATA data;
	HANDLE h = FindFirstFile(gameassemblypath.c_str(), &data);
	if (h != INVALID_HANDLE_VALUE)
		IsGameIl2Cpp = true;

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

		if (IsGameIl2Cpp)
		{
			std::string configpath = ndatapath + "\\il2cpp_data\\etc";
			Mono::ConfigPath = new char[configpath.size() + 1];
			std::copy(configpath.begin(), configpath.end(), Mono::ConfigPath);
			Mono::ConfigPath[configpath.size()] = '\0';

			std::string assemblypath = filepathstr + "\\MelonLoader\\Managed";
			Mono::AssemblyPath = new char[assemblypath.size() + 1];
			std::copy(assemblypath.begin(), assemblypath.end(), Mono::AssemblyPath);
			Mono::AssemblyPath[assemblypath.size()] = '\0';

			if (MelonLoader::LoadMono() && Mono::Setup())
			{
				if (MupotMode && MelonLoader::LoadMonoUnityPlayer() && MonoUnityPlayer::Setup())
				{
					Hook_mono_add_internal_call::Hook();
					Hook_mono_jit_init_version::Hook();
					Hook_SingleAppInstance_FindOtherInstance::Hook();
				}
			}
		}

		Hook_LoadLibraryW::Hook();
	}
}

bool MelonLoader::Is64bit()
{
	//BOOL b64 = FALSE;
	//return IsWow64Process(GetCurrentProcess(), &b64) && b64;
	return true;
}

void MelonLoader::ModHandler()
{
	if (Mono::Domain != NULL)
	{
		MonoAssembly* assembly = Mono::mono_domain_assembly_open(Mono::Domain, (std::string(GamePath) + "\\MelonLoader\\MelonLoader.ModHandler.dll").c_str());
		if (assembly != NULL)
		{
			MonoImage* image = Mono::mono_assembly_get_image(assembly);
			if (image != NULL)
			{
				MonoClass* klass = Mono::mono_class_from_name(image, "MelonLoader", "Main");
				if (klass != NULL)
				{
					MonoMethod* method = Mono::mono_class_get_method_from_name(klass, "Initialize", NULL);
					if (method != NULL)
					{
						Mono::mono_runtime_invoke(method, NULL, NULL, NULL);
						ModHandlerAssembly = assembly;
					}
					else
						MessageBox(NULL, "Main", "MelonLoader", MB_ICONERROR | MB_OK);
				}
				else
					MessageBox(NULL, "Bootloader", "MelonLoader", MB_ICONERROR | MB_OK);
			}
			else
				MessageBox(NULL, "MelonLoader.ModHandler Image", "MelonLoader", MB_ICONERROR | MB_OK);
		}
		else
			MessageBox(NULL, "MelonLoader.ModHandler", "MelonLoader", MB_ICONERROR | MB_OK);
	}
}

bool MelonLoader::LoadMono()
{
	MonoDLL = LoadLibrary((std::string(GamePath) + "\\MelonLoader\\Mono\\mono-2.0-bdwgc.dll").c_str());
	if (MonoDLL)
	{
		HMODULE MonoPosixDLL = LoadLibrary((std::string(GamePath) + "\\MelonLoader\\Mono\\MonoPosixHelper.dll").c_str());
		if (MonoPosixDLL)
			return true;
		else
			MessageBox(NULL, "Failed to Load MonoPosixHelper.dll!", "MelonLoader", MB_ICONERROR | MB_OK);
	}
	else
		MessageBox(NULL, "Failed to Load mono-2.0-bdwgc.dll!", "MelonLoader", MB_ICONERROR | MB_OK);
	return false;
}

bool MelonLoader::LoadMonoUnityPlayer()
{
	if (Is64bit())
	{
		MonoUnityPlayerDLL = LoadLibrary((std::string(GamePath) + "\\MelonLoader\\Mono\\MonoUnityPlayer_x64.dll").c_str());
		if (MonoUnityPlayerDLL)
			return true;
		else
			MessageBox(NULL, "Failed to Load MonoUnityPlayer_x64.dll!", "MelonLoader", MB_ICONERROR | MB_OK);
	}
	else
	{
		MonoUnityPlayerDLL = LoadLibrary((std::string(GamePath) + "\\MelonLoader\\Mono\\MonoUnityPlayer_x86.dll").c_str());
		if (MonoUnityPlayerDLL)
			return true;
		else
			MessageBox(NULL, "Failed to Load MonoUnityPlayer_x86.dll!", "MelonLoader", MB_ICONERROR | MB_OK);
	}
	return false;
}

void MelonLoader::Detour(Il2CppMethod* target, void* detour)
{
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach(&(LPVOID&)target->targetMethod, detour);
	DetourTransactionCommit();
}

void MelonLoader::UnDetour(Il2CppMethod* target, void* detour)
{
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourDetach(&(LPVOID&)target->targetMethod, detour);
	DetourTransactionCommit();
}