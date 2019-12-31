#include "Hooks.h"
#include "../MelonLoader.h"
#include "../detours/detours.h"
#include <iostream>

LoadLibraryW_t Hook_LoadLibraryW::Original_LoadLibraryW = NULL;

void Hook_LoadLibraryW::Hook()
{
	if (Original_LoadLibraryW == NULL)
	{
		Original_LoadLibraryW = LoadLibraryW;
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach(&(LPVOID&)Original_LoadLibraryW, Hooked_LoadLibraryW);
		DetourTransactionCommit();
	}
}

void Hook_LoadLibraryW::Unhook()
{
	if (Original_LoadLibraryW != NULL)
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourDetach(&(LPVOID&)Original_LoadLibraryW, Hooked_LoadLibraryW);
		DetourTransactionCommit();
		Original_LoadLibraryW = NULL;
	}
}

HMODULE __stdcall Hook_LoadLibraryW::Hooked_LoadLibraryW(LPCWSTR lpLibFileName)
{
	HMODULE lib = Original_LoadLibraryW(lpLibFileName);
	if (wcsstr(lpLibFileName, L"GameAssembly.dll"))
	{
		char path[MAX_PATH];
		if (GetModuleFileName(lib, path, sizeof(path)) != NULL)
		{
			std::string pathstr = path;
			MelonLoader::GamePath = pathstr.substr(0, pathstr.find_last_of("\\/")).c_str();
			std::string filepath = pathstr.substr(0, (pathstr.find_last_of("\\/") + 1));
			std::string datapath = filepath;
			datapath += "*_Data";
			WIN32_FIND_DATA data;
			HANDLE h = FindFirstFile(datapath.c_str(), &data);
			if (h != INVALID_HANDLE_VALUE)
			{
				char* nPtr = new char[lstrlen(data.cFileName) + 1];
				for (int i = 0; i < lstrlen(data.cFileName); i++)
					nPtr[i] = char(data.cFileName[i]);
				nPtr[lstrlen(data.cFileName)] = '\0';

				MelonLoader::IsGameIL2CPP = true;
				MelonLoader::LoadMono();

				std::string assembly_path = filepath + "MelonLoader\\Managed";
				std::string config_path = filepath + std::string(nPtr) + "\\il2cpp_data\\etc";
				Mono::CreateDomain(assembly_path.c_str(), config_path.c_str());
			}
		}

		MelonLoader::GameAssemblyDLL = lib;
		IL2CPP::Setup();

		Hook_il2cpp_init::Hook();
		Hook_il2cpp_add_internal_call::Hook();
		Hook_MetadataLoader_LoadMetadataFile::Hook();
		Hook_MetadataCache_GetTypeInfoFromTypeDefinitionIndex::Hook();
	}
	else if (wcsstr(lpLibFileName, L"dxgi.dll") != NULL)
	{
		Unhook();
		MelonLoader::ModHandler();
	}
	return lib;
}