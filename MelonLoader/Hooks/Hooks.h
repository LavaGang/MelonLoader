#pragma once
#include <Windows.h>
#include "../Mono.h"
#include "../IL2CPP.h"

typedef HMODULE (__stdcall* LoadLibraryW_t) (LPCWSTR lpLibFileName);
class Hook_LoadLibraryW
{
public:
	static LoadLibraryW_t Original_LoadLibraryW;

	static void Hook();
	static void Unhook();
	static HMODULE __stdcall Hooked_LoadLibraryW(LPCWSTR lpLibFileName);
};

class Hook_il2cpp_init
{
public:
	static il2cpp_init_t Original_il2cpp_init;

	static void Hook();
	static void Unhook();
	static IL2CPPDomain* Hooked_il2cpp_init(const char* name);
};

class Hook_il2cpp_add_internal_call
{
public:
	static il2cpp_add_internal_call_t Original_il2cpp_add_internal_call;

	static void Hook();
	static void Unhook();
	static void Hooked_il2cpp_add_internal_call(const char* name, void* method);
};

class Hook_MetadataLoader_LoadMetadataFile
{
public:
	static MetadataLoader_LoadMetadataFile_t Original_MetadataLoader_LoadMetadataFile;

	static void Hook();
	static void Unhook();
	static Il2CppGlobalMetadataHeader* Hooked_MetadataLoader_LoadMetadataFile(const char* fileName);
};

class Hook_MetadataCache_GetTypeInfoFromTypeDefinitionIndex
{
public:
	static MetadataCache_GetTypeInfoFromTypeDefinitionIndex_t Original_MetadataCache_GetTypeInfoFromTypeDefinitionIndex;

	static void Hook();
	static void Unhook();
	static void* Hooked_MetadataCache_GetTypeInfoFromTypeDefinitionIndex(int index);
};