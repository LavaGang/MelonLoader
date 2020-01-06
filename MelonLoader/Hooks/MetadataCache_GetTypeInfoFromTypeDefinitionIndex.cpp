#include "Hooks.h"
#include "../MelonLoader.h"
#include "../detours/detours.h"
#include <iostream>

MetadataCache_GetTypeInfoFromTypeDefinitionIndex_t Hook_MetadataCache_GetTypeInfoFromTypeDefinitionIndex::Original_MetadataCache_GetTypeInfoFromTypeDefinitionIndex = NULL;

void Hook_MetadataCache_GetTypeInfoFromTypeDefinitionIndex::Hook()
{
	if (Original_MetadataCache_GetTypeInfoFromTypeDefinitionIndex == NULL)
	{
		Original_MetadataCache_GetTypeInfoFromTypeDefinitionIndex = IL2CPP::MetadataCache_GetTypeInfoFromTypeDefinitionIndex;
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach(&(LPVOID&)Original_MetadataCache_GetTypeInfoFromTypeDefinitionIndex, Hooked_MetadataCache_GetTypeInfoFromTypeDefinitionIndex);
		DetourTransactionCommit();
	}
}

void Hook_MetadataCache_GetTypeInfoFromTypeDefinitionIndex::Unhook()
{
	if (Original_MetadataCache_GetTypeInfoFromTypeDefinitionIndex != NULL)
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourDetach(&(LPVOID&)Original_MetadataCache_GetTypeInfoFromTypeDefinitionIndex, Hooked_MetadataCache_GetTypeInfoFromTypeDefinitionIndex);
		DetourTransactionCommit();
		Original_MetadataCache_GetTypeInfoFromTypeDefinitionIndex = NULL;
	}
}

void* Hook_MetadataCache_GetTypeInfoFromTypeDefinitionIndex::Hooked_MetadataCache_GetTypeInfoFromTypeDefinitionIndex(int index)
{
	if ((index < 0) || (static_cast<uint32_t>(index) >= (IL2CPP::s_GlobalMetadataHeader->typeDefinitionsCount / sizeof(Il2CppTypeDefinition))))
		return NULL;

	/*
	if (!s_TypeInfoDefinitionTable[index])
    {
        il2cpp::os::FastAutoLock lock(&g_MetadataLock);
        if (!s_TypeInfoDefinitionTable[index])
            s_TypeInfoDefinitionTable[index] = FromTypeDefinition(index);
    }
    return s_TypeInfoDefinitionTable[index];
	*/

	return Original_MetadataCache_GetTypeInfoFromTypeDefinitionIndex(index);
}