#include "Hooks.h"
#include "../MelonLoader.h"
#include "../detours/detours.h"
#include <iostream>
#include "../Console.h"

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

Il2CppClass* Hook_MetadataCache_GetTypeInfoFromTypeDefinitionIndex::Hooked_MetadataCache_GetTypeInfoFromTypeDefinitionIndex(int index)
{
	if ((*(Il2CppClass***)(IL2CPP::s_TypeInfoDefinitionTable)) == NULL)
		return NULL;
	if ((index < 0) || (static_cast<uint32_t>(index) >= (IL2CPP::s_GlobalMetadataHeader->typeDefinitionsCount / sizeof(Il2CppTypeDefinition))))
		index = 0;
	Il2CppClass* ReturnClass = (*(Il2CppClass***)(IL2CPP::s_TypeInfoDefinitionTable))[index];
	if (ReturnClass == NULL)
	{
		ReturnClass = IL2CPP::MetadataCache_FromTypeDefinition(index);
		(*(Il2CppClass***)(IL2CPP::s_TypeInfoDefinitionTable))[index] = ReturnClass;
	}
	return ReturnClass;
}