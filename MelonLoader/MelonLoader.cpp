#include <Windows.h>
#include <string>
#include <filesystem>
#include "MelonLoader.h"
#include "Console.h"
#include "Mono.h"
#include "IL2CPP.h"
#include "Hooks/Hooks.h"

bool MelonLoader::IsGameIL2CPP = false;
HMODULE MelonLoader::MonoDLL = NULL;
HMODULE MelonLoader::GameAssemblyDLL = NULL;
HINSTANCE MelonLoader::thisdll = NULL;
MonoImage* MelonLoader::ModHandlerImage = NULL;
bool MelonLoader::DebugMode = false;
const char* MelonLoader::GamePath = NULL;

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

	Hook_LoadLibraryW::Hook();
}

void MelonLoader::ApplicationQuit()
{
	if (ModHandlerImage != NULL)
	{
		MonoClass* klass = Mono::mono_class_from_name(ModHandlerImage, "MelonLoader", "Main");
		if (klass != NULL)
		{
			MonoMethod* method = Mono::mono_class_get_method_from_name(klass, "OnApplicationQuit", NULL);
			if (method != NULL)
				Mono::mono_runtime_invoke(method, NULL, NULL, NULL);
		}
	}
}

void MelonLoader::ModHandler()
{
	if (Mono::Domain != NULL)
	{
		MonoAssembly* assembly = Mono::mono_domain_assembly_open(Mono::Domain, "MelonLoader\\MelonLoader.ModHandler.dll");
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
						ModHandlerImage = image;
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
	MonoDLL = LoadLibrary("MelonLoader\\Mono\\mono-2.0-bdwgc.dll");
	if (MonoDLL)
	{
		HMODULE MonoPosixDLL = LoadLibrary("MelonLoader\\Mono\\MonoPosixHelper.dll");
		if (MonoPosixDLL)
		{
			Mono::Setup();
			return true;
		}
		else
			MessageBox(NULL, "Failed to Load MonoPosixHelper.dll!", "MelonLoader", MB_ICONERROR | MB_OK);
	}
	else
		MessageBox(NULL, "Failed to Load mono-2.0-bdwgc.dll!", "MelonLoader", MB_ICONERROR | MB_OK);
	return false;
}