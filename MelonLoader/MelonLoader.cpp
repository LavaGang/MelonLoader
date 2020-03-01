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
HINSTANCE MelonLoader::thisdll = NULL;
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

			if (Mono::Load() && Mono::Setup())
			{
				if (MupotMode)
				{
					Hook_mono_add_internal_call::Hook();
					Hook_mono_jit_init_version::Hook();
				}
			}
		}
		Hook_LoadLibraryW::Hook();
	}
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

void MelonLoader::AddGameSpecificInternalCalls()
{
	if ((Mono::Domain != NULL) && (IL2CPP::Domain))
	{
		size_t asm_count;
		Il2CppAssembly** asm_tbl = IL2CPP::il2cpp_domain_get_assemblies(IL2CPP::Domain, &asm_count);
		if (asm_count > 0)
		{
			for (size_t i = 0; i < asm_count; i++)
			{
				Il2CppAssembly* asm_ptr = asm_tbl[i];
				if (asm_ptr)
				{
					Il2CppImage* image = IL2CPP::il2cpp_assembly_get_image(asm_ptr);
					if (image)
					{
						std::string image_name = IL2CPP::il2cpp_image_get_name(image);
						if (!image_name._Starts_with("UnityEngine")
							&& !image_name._Starts_with("System")
							&& !image_name._Starts_with("Mono")
							&& !image_name._Starts_with("Unity")
							&& !image_name._Starts_with("ICSharpCode")
							&& !image_name._Starts_with("IBM")
							&& !image_name._Starts_with("I18N")
							&& !image_name._Starts_with("cscompmgd")
							&& !image_name._Starts_with("Commons")
							&& !image_name._Starts_with("Boo")
							&& !image_name._Starts_with("CustomMarshalers")
							&& !image_name._Starts_with("Microsoft")
							&& !image_name._Starts_with("mscorlib")
							&& !image_name._Starts_with("Newtonsoft")
							&& !image_name._Starts_with("Novell")
							&& !image_name._Starts_with("Oculus")
							&& !image_name._Starts_with("Steam")
							&& !image_name._Starts_with("SMDiagnostics")
							&& !image_name._Starts_with("Windows")
							&& !image_name._Starts_with("Facepunch"))
						{
							int klass_count = IL2CPP::il2cpp_image_get_class_count(image);
							if (klass_count > 0)
							{
								for (int t = 0; t < klass_count; t++)
								{
									Il2CppClass* klass = IL2CPP::il2cpp_image_get_class(image, t);
									if (klass)
									{
										std::string klass_name = IL2CPP::il2cpp_class_get_name(klass);
										std::string klass_namespace = IL2CPP::il2cpp_class_get_namespace(klass);
										std::string klass_id = ((klass_namespace.empty() ? "" : (klass_namespace + ".")) + klass_name);

										void* method_iter = NULL;
										Il2CppMethod* method = NULL;
										while ((method = IL2CPP::il2cpp_class_get_methods(klass, &method_iter)) != NULL)
										{
											std::string method_name = IL2CPP::il2cpp_method_get_name(method);
											std::string method_id = (klass_id + "::" + method_name);

											Mono::mono_add_internal_call(method_id.c_str(), method->targetMethod);
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}
}