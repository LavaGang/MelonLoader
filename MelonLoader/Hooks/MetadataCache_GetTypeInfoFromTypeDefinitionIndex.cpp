#include "Hooks.h"
#include "../MelonLoader.h"
#include "../detours/detours.h"
#include <iostream>

MetadataCache_GetTypeInfoFromTypeDefinitionIndex_t Hook_MetadataCache_GetTypeInfoFromTypeDefinitionIndex::Original_MetadataCache_GetTypeInfoFromTypeDefinitionIndex = NULL;

void Hook_MetadataCache_GetTypeInfoFromTypeDefinitionIndex::Hook()
{
	if (Original_MetadataCache_GetTypeInfoFromTypeDefinitionIndex == NULL)
	{
		Original_MetadataCache_GetTypeInfoFromTypeDefinitionIndex = Il2Cpp::MetadataCache_GetTypeInfoFromTypeDefinitionIndex;
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
	if ((index < 0) || (static_cast<uint32_t>(index) >= (Il2Cpp::s_GlobalMetadataHeader->typeDefinitionsCount / sizeof(Il2CppTypeDefinition))))
		return NULL;

	Il2CppClass* ReturnClass = (*(Il2CppClass***)(Il2Cpp::s_TypeInfoDefinitionTable))[index];
	if (ReturnClass == NULL)
	{
		ReturnClass = Il2Cpp::MetadataCache_FromTypeDefinition(index);
		(*(Il2CppClass***)(Il2Cpp::s_TypeInfoDefinitionTable))[index] = ReturnClass;
	}

	return ReturnClass;
}