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
		MelonLoader::IsGameIl2Cpp = true;
		MelonLoader::LoadMono();
		Mono::CreateDomain();

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