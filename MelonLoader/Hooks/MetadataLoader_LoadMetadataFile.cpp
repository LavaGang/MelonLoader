#include "Hooks.h"
#include "../MelonLoader.h"
#include "../detours/detours.h"
#include <iostream>

MetadataLoader_LoadMetadataFile_t Hook_MetadataLoader_LoadMetadataFile::Original_MetadataLoader_LoadMetadataFile = NULL;

void Hook_MetadataLoader_LoadMetadataFile::Hook()
{
	if (Original_MetadataLoader_LoadMetadataFile == NULL)
	{
		Original_MetadataLoader_LoadMetadataFile = IL2CPP::MetadataLoader_LoadMetadataFile;
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach(&(LPVOID&)Original_MetadataLoader_LoadMetadataFile, Hooked_MetadataLoader_LoadMetadataFile);
		DetourTransactionCommit();
	}
}

void Hook_MetadataLoader_LoadMetadataFile::Unhook()
{
	if (Original_MetadataLoader_LoadMetadataFile != NULL)
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourDetach(&(LPVOID&)Original_MetadataLoader_LoadMetadataFile, Hooked_MetadataLoader_LoadMetadataFile);
		DetourTransactionCommit();
		Original_MetadataLoader_LoadMetadataFile = NULL;
	}
}

Il2CppGlobalMetadataHeader* Hook_MetadataLoader_LoadMetadataFile::Hooked_MetadataLoader_LoadMetadataFile(const char* fileName)
{
	IL2CPP::s_GlobalMetadataHeader = Original_MetadataLoader_LoadMetadataFile(fileName);
	return IL2CPP::s_GlobalMetadataHeader;
}