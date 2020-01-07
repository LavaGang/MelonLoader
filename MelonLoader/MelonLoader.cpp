#include <Windows.h>
#include <string>
#include <filesystem>
#include "MelonLoader.h"
#include "Console.h"
#include "Mono.h"
#include "Il2Cpp.h"
#include "Hooks/Hooks.h"
#include <iostream>

bool MelonLoader::IsGameIl2Cpp = false;
HMODULE MelonLoader::MonoDLL = NULL;
HMODULE MelonLoader::GameAssemblyDLL = NULL;
HINSTANCE MelonLoader::thisdll = NULL;
MonoAssembly* MelonLoader::ModHandlerAssembly = NULL;
bool MelonLoader::DebugMode = false;
char* MelonLoader::GamePath = NULL;

void MelonLoader::Main()
{
#ifndef _DEBUG
	if (strstr(GetCommandLine(), "--melonloader.debug") != NULL)
	{
#endif
		Console::Create();
		DebugMode = true;
#ifndef _DEBUG
	}
#endif

	LPSTR filepath = new CHAR[MAX_PATH];
	GetModuleFileName(thisdll, filepath, MAX_PATH);
	std::string filepathstr = filepath;
	filepathstr = filepathstr.substr(0, filepathstr.find_last_of("\\/"));
	filepathstr = filepathstr.substr(0, filepathstr.find_last_of("\\/"));

	GamePath = new char[filepathstr.size() + 1];
	std::copy(filepathstr.begin(), filepathstr.end(), GamePath);
	GamePath[filepathstr.size()] = '\0';

	std::string managedpath = filepathstr + "\\MelonLoader\\Managed";
	Mono::AssemblyPath = new char[managedpath.size() + 1];
	std::copy(managedpath.begin(), managedpath.end(), Mono::AssemblyPath);
	Mono::AssemblyPath[managedpath.size()] = '\0';

	std::string datapath = filepathstr + "\\*_Data";
	WIN32_FIND_DATA data;
	HANDLE h = FindFirstFile(datapath.c_str(), &data);
	if (h != INVALID_HANDLE_VALUE)
	{
		char* nPtr = new char[lstrlen(data.cFileName) + 1];
		for (int i = 0; i < lstrlen(data.cFileName); i++)
			nPtr[i] = char(data.cFileName[i]);
		nPtr[lstrlen(data.cFileName)] = '\0';

		std::string configpath = filepathstr + "\\" + std::string(nPtr) + "\\il2cpp_data\\etc";
		Mono::ConfigPath = new char[configpath.size() + 1];
		std::copy(configpath.begin(), configpath.end(), Mono::ConfigPath);
		Mono::ConfigPath[configpath.size()] = '\0';
	}

	Hook_LoadLibraryW::Hook();
}

void MelonLoader::ApplicationQuit()
{
	/*
	if (ModHandlerAssembly != NULL)
	{
		MonoImage* image = Mono::mono_assembly_get_image(ModHandlerAssembly);
		if (image != NULL)
		{
			MonoClass* klass = Mono::mono_class_from_name(image, "MelonLoader", "Main");
			if (klass != NULL)
			{
				MonoMethod* method = Mono::mono_class_get_method_from_name(klass, "OnApplicationQuit", NULL);
				if (method != NULL)
				{
					Mono::mono_runtime_invoke(method, NULL, NULL, NULL);
					ModHandlerAssembly = NULL;
				}
			}
		}
	}
	*/
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
	MonoDLL = LoadLibrary((std::string(GamePath) + "\\MelonLoader\\Mono\\mono.dll").c_str());
	if (MonoDLL)
	{
		HMODULE MonoPosixDLL = LoadLibrary((std::string(GamePath) + "\\MelonLoader\\Mono\\MonoPosixHelper.dll").c_str());
		if (MonoPosixDLL)
		{
			Mono::Setup();
			return true;
		}
		else
			MessageBox(NULL, "Failed to Load MonoPosixHelper.dll!", "MelonLoader", MB_ICONERROR | MB_OK);
	}
	else
		MessageBox(NULL, "Failed to Load mono.dll!", "MelonLoader", MB_ICONERROR | MB_OK);
	return false;
}