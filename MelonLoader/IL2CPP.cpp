#include <Windows.h>
#include "MelonLoader.h"
#include "Il2Cpp.h"
#include "PatternSearch.h"
#include <iostream>

Il2CppDomain* Il2Cpp::Domain = NULL;
il2cpp_init_t Il2Cpp::il2cpp_init = NULL;
il2cpp_add_internal_call_t Il2Cpp::il2cpp_add_internal_call = NULL;
il2cpp_custom_attrs_free_t Il2Cpp::il2cpp_custom_attrs_free = NULL;
il2cpp_resolve_icall_t Il2Cpp::il2cpp_resolve_icall = NULL;
il2cpp_class_enum_basetype_t Il2Cpp::il2cpp_class_enum_basetype = NULL;
il2cpp_class_from_system_type_t Il2Cpp::il2cpp_class_from_system_type = NULL;
il2cpp_type_get_name_t Il2Cpp::il2cpp_type_get_name = NULL;
il2cpp_class_get_name_t Il2Cpp::il2cpp_class_get_name = NULL;
il2cpp_class_get_namespace_t Il2Cpp::il2cpp_class_get_namespace = NULL;
il2cpp_class_get_assemblyname_t Il2Cpp::il2cpp_class_get_assemblyname = NULL;
il2cpp_class_from_name_t Il2Cpp::il2cpp_class_from_name = NULL;
il2cpp_class_get_method_from_name_t Il2Cpp::il2cpp_class_get_method_from_name = NULL;
il2cpp_image_get_class_t Il2Cpp::il2cpp_image_get_class = NULL;
il2cpp_type_get_class_or_element_class_t Il2Cpp::il2cpp_type_get_class_or_element_class = NULL;
il2cpp_domain_get_assemblies_t Il2Cpp::il2cpp_domain_get_assemblies = NULL;
il2cpp_assembly_get_image_t Il2Cpp::il2cpp_assembly_get_image = NULL;
MetadataCache_GetTypeInfoFromTypeDefinitionIndex_t Il2Cpp::MetadataCache_GetTypeInfoFromTypeDefinitionIndex = NULL;
MetadataCache_FromTypeDefinition_t Il2Cpp::MetadataCache_FromTypeDefinition = NULL;
MetadataLoader_LoadMetadataFile_t Il2Cpp::MetadataLoader_LoadMetadataFile = NULL;
Il2CppGlobalMetadataHeader* Il2Cpp::s_GlobalMetadataHeader = NULL;
uintptr_t Il2Cpp::s_TypeInfoDefinitionTable = NULL;

uint64_t resolvePtrOffset(uint64_t offset32Ptr, uint64_t nextInstructionPtr)
{
	uint32_t jmpOffset = *(uint32_t*)offset32Ptr;
	uint32_t valueUInt = *(uint32_t*)nextInstructionPtr;
	uint64_t delta = nextInstructionPtr - valueUInt;
	uint32_t newPtrInt = valueUInt + jmpOffset;
	return delta + newPtrInt;
}

uint64_t resolveRelativeInstruction(uint64_t instruction) {
	byte opcode = *(byte*)instruction;
	if (opcode != 0xE8 && opcode != 0xE9)
		return 0;

	return resolvePtrOffset(instruction + 1, instruction + 5); // CALL: E8 [rel32] / JMP: E9 [rel32]
}

void Il2Cpp::Setup()
{
	il2cpp_init = (il2cpp_init_t)GetProcAddress(MelonLoader::GameAssemblyDLL, "il2cpp_init");
	il2cpp_add_internal_call = (il2cpp_add_internal_call_t)GetProcAddress(MelonLoader::GameAssemblyDLL, "il2cpp_add_internal_call");
	il2cpp_custom_attrs_free = (il2cpp_custom_attrs_free_t)GetProcAddress(MelonLoader::GameAssemblyDLL, "il2cpp_custom_attrs_free");
	il2cpp_resolve_icall = (il2cpp_resolve_icall_t)GetProcAddress(MelonLoader::GameAssemblyDLL, "il2cpp_resolve_icall");
	il2cpp_class_enum_basetype = (il2cpp_class_enum_basetype_t)GetProcAddress(MelonLoader::GameAssemblyDLL, "il2cpp_class_enum_basetype");
	il2cpp_class_from_system_type = (il2cpp_class_from_system_type_t)GetProcAddress(MelonLoader::GameAssemblyDLL, "il2cpp_class_from_system_type");
	il2cpp_type_get_name = (il2cpp_type_get_name_t)GetProcAddress(MelonLoader::GameAssemblyDLL, "il2cpp_type_get_name");
	il2cpp_class_get_name = (il2cpp_class_get_name_t)GetProcAddress(MelonLoader::GameAssemblyDLL, "il2cpp_class_get_name");
	il2cpp_class_get_namespace = (il2cpp_class_get_namespace_t)GetProcAddress(MelonLoader::GameAssemblyDLL, "il2cpp_class_get_namespace");
	il2cpp_class_get_assemblyname = (il2cpp_class_get_assemblyname_t)GetProcAddress(MelonLoader::GameAssemblyDLL, "il2cpp_class_get_assemblyname");
	il2cpp_class_from_name = (il2cpp_class_from_name_t)GetProcAddress(MelonLoader::GameAssemblyDLL, "il2cpp_class_from_name");
	il2cpp_class_get_method_from_name = (il2cpp_class_get_method_from_name_t)GetProcAddress(MelonLoader::GameAssemblyDLL, "il2cpp_class_get_method_from_name");
	il2cpp_image_get_class = (il2cpp_image_get_class_t)GetProcAddress(MelonLoader::GameAssemblyDLL, "il2cpp_image_get_class");
	il2cpp_type_get_class_or_element_class = (il2cpp_type_get_class_or_element_class_t)GetProcAddress(MelonLoader::GameAssemblyDLL, "il2cpp_type_get_class_or_element_class");
	il2cpp_domain_get_assemblies = (il2cpp_domain_get_assemblies_t)GetProcAddress(MelonLoader::GameAssemblyDLL, "il2cpp_domain_get_assemblies");
	il2cpp_assembly_get_image = (il2cpp_assembly_get_image_t)GetProcAddress(MelonLoader::GameAssemblyDLL, "il2cpp_assembly_get_image");
	
	// VRChat | Pistol Whip | Boneworks
	MetadataCache_GetTypeInfoFromTypeDefinitionIndex = (MetadataCache_GetTypeInfoFromTypeDefinitionIndex_t)PatternSearch::FindPattern(MelonLoader::GameAssemblyDLL, "40 57 48 83 EC 30 48 C7 44 24 ? ? ? ? ? 48 89 5C 24 ? 48 89 74 24 ? 48 63 F9 83");
	// Audica
	if (MetadataCache_GetTypeInfoFromTypeDefinitionIndex == NULL)
		MetadataCache_GetTypeInfoFromTypeDefinitionIndex = (MetadataCache_GetTypeInfoFromTypeDefinitionIndex_t)PatternSearch::FindPattern(MelonLoader::GameAssemblyDLL, "40 57 48 83 EC 30 48 C7 44 24 ? ? ? ? ? 48 89 5C 24 ? 48 63 F9 83 FF FF 75 04 33");

	// VRChat | Boneworks
	MetadataCache_FromTypeDefinition = (MetadataCache_FromTypeDefinition_t)PatternSearch::FindPattern(MelonLoader::GameAssemblyDLL, "40 53 41 54 41 56 48 83 EC 40 48 8B 05 ? ? ? ? 48 89 74 24 ? 48 89 7C 24 ? 48 63 F9 B9");
	// Audica | Pistol Whip
	if (MetadataCache_FromTypeDefinition == NULL)
		MetadataCache_FromTypeDefinition = (MetadataCache_FromTypeDefinition_t)PatternSearch::FindPattern(MelonLoader::GameAssemblyDLL, "48 89 5C 24 ? 48 89 6C 24 ? 48 89 74 24 ? 48 89 7C 24 ? 41 56 48 83 EC 20 48 8B 05 ? ? ? ? 48 63 F1 B9");

	
	// VRChat
	MetadataLoader_LoadMetadataFile = (MetadataLoader_LoadMetadataFile_t)PatternSearch::FindPattern(MelonLoader::GameAssemblyDLL, "40 55 56 57 41 54 41 55 41 56 41 57 48 8D 6C 24 ? 48 81 EC ? ? ? ? 48 C7 45 ? ? ? ? ? 48 89 9C 24 ? ? ? ? 4C 8B F1 33");
	// Boneworks
	if (MetadataLoader_LoadMetadataFile == NULL)
		MetadataLoader_LoadMetadataFile = (MetadataLoader_LoadMetadataFile_t)PatternSearch::FindPattern(MelonLoader::GameAssemblyDLL, "48 8B C4 55 48 8D 68 A1 48 81 EC ? ? ? ? 48 C7 45 ? ? ? ? ? 48 89 58 08 48 89 78 18");
	// Audica
	if (MetadataLoader_LoadMetadataFile == NULL)
		MetadataLoader_LoadMetadataFile = (MetadataLoader_LoadMetadataFile_t)PatternSearch::FindPattern(MelonLoader::GameAssemblyDLL, "40 57 41 54 41 55 41 56 41 57 48 83 EC 30 48 C7 44 24 ? ? ? ? ? 48 89 5C 24 ? 48 89 6C 24 ? 48 89 74 24 ? 49 8B E8 4C");
	// Pistol Whip
	if (MetadataLoader_LoadMetadataFile == NULL)
		MetadataLoader_LoadMetadataFile = (MetadataLoader_LoadMetadataFile_t)PatternSearch::FindPattern(MelonLoader::GameAssemblyDLL, "40 55 56 57 41 54 41 55 41 56 41 57 48 8D 6C 24 ? 48 81 EC ? ? ? ? 48 C7 45 ? ? ? ? ? 48 89 9C 24 ? ? ? ? 4C 8B F1 45 33 FF 44 89 7D 77 48 8D 4D FF E8");


	// VRChat | Boneworks
	uintptr_t movInstructionPtr = PatternSearch::FindPattern(MelonLoader::GameAssemblyDLL, "48 89 05 ? ? ? ? 48 8B 05 ? ? ? ? 48 63 48 34 48 B8 ? ? ? ? ? ? ? ? 48 F7 E1 48 8B CA 48 C1 E9 04 BA ? ? ? ? E8 ? ? ? ? 48 89 05");
	// Pistol Whip
	if (movInstructionPtr == NULL)
		movInstructionPtr = PatternSearch::FindPattern(MelonLoader::GameAssemblyDLL, "48 89 05 ? ? ? ? 48 8B 05 ? ? ? ? 48 63 48 34 48 C1 E9 05 BA ? ? ? ? E8 ? ? ? ? 48 89 05 ? ? ? ? 48 8B 05");
	// Audica
	if (movInstructionPtr == NULL)
		movInstructionPtr = PatternSearch::FindPattern(MelonLoader::GameAssemblyDLL, "48 89 05 ? ? ? ? 48 8B 05 ? ? ? ? 48 63 48 34 48 B8 ? ? ? ? ? ? ? ? 48 F7 E1 48 8B CA 48 C1 E9 04 BA ? ? ? ? FF 15 ? ? ? ? 48 89 05");
	s_TypeInfoDefinitionTable = resolvePtrOffset(movInstructionPtr + 3, movInstructionPtr + 7);
}