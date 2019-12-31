#include <Windows.h>
#include "MelonLoader.h"
#include "IL2CPP.h"
#include "PatternSearch.h"

IL2CPPDomain* IL2CPP::Domain = NULL;
il2cpp_init_t IL2CPP::il2cpp_init = NULL;
il2cpp_add_internal_call_t IL2CPP::il2cpp_add_internal_call = NULL;
il2cpp_custom_attrs_free_t IL2CPP::il2cpp_custom_attrs_free = NULL;
il2cpp_resolve_icall_t IL2CPP::il2cpp_resolve_icall = NULL;
il2cpp_class_enum_basetype_t IL2CPP::il2cpp_class_enum_basetype = NULL;
il2cpp_class_from_system_type_t IL2CPP::il2cpp_class_from_system_type = NULL;
il2cpp_type_get_name_t IL2CPP::il2cpp_type_get_name = NULL;
il2cpp_class_get_name_t IL2CPP::il2cpp_class_get_name = NULL;
il2cpp_class_get_namespace_t IL2CPP::il2cpp_class_get_namespace = NULL;
il2cpp_class_get_assemblyname_t IL2CPP::il2cpp_class_get_assemblyname = NULL;
il2cpp_class_from_name_t IL2CPP::il2cpp_class_from_name = NULL;
il2cpp_class_get_method_from_name_t IL2CPP::il2cpp_class_get_method_from_name = NULL;
il2cpp_image_get_class_t IL2CPP::il2cpp_image_get_class = NULL;
il2cpp_type_get_class_or_element_class_t IL2CPP::il2cpp_type_get_class_or_element_class = NULL;
il2cpp_domain_get_assemblies_t IL2CPP::il2cpp_domain_get_assemblies = NULL;
il2cpp_assembly_get_image_t IL2CPP::il2cpp_assembly_get_image = NULL;
MetadataCache_GetTypeInfoFromTypeDefinitionIndex_t IL2CPP::MetadataCache_GetTypeInfoFromTypeDefinitionIndex = NULL;
MetadataLoader_LoadMetadataFile_t IL2CPP::MetadataLoader_LoadMetadataFile = NULL;
MetadataCache_FromTypeDefinition_t IL2CPP::MetadataCache_FromTypeDefinition = NULL;
Assembly_Load_t IL2CPP::Assembly_Load = NULL;
Il2CppGlobalMetadataHeader* IL2CPP::s_GlobalMetadataHeader = NULL;

void IL2CPP::Setup()
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
	MetadataCache_GetTypeInfoFromTypeDefinitionIndex = (MetadataCache_GetTypeInfoFromTypeDefinitionIndex_t)PatternSearch::FindPattern(MelonLoader::GameAssemblyDLL, "40 57 48 83 EC 30 48 C7 44 24 ? ? ? ? ? 48 89 5C 24 ? 48 89 74 24 ? 48 63 F9 83");
	MetadataLoader_LoadMetadataFile = (MetadataLoader_LoadMetadataFile_t)PatternSearch::FindPattern(MelonLoader::GameAssemblyDLL, "40 55 56 57 41 54 41 55 41 56 41 57 48 8D 6C 24 ? 48 81 EC ? ? ? ? 48 C7 45 ? ? ? ? ? 48 89 9C 24 ? ? ? ? 4C 8B F1 33");
	MetadataCache_FromTypeDefinition = (MetadataCache_FromTypeDefinition_t)PatternSearch::FindPattern(MelonLoader::GameAssemblyDLL, "40 53 41 54 41 56 48 83 EC 40 48 8B 05 ? ? ? ? 48 89 74 24 ? 48 89 7C 24");
	Assembly_Load = (Assembly_Load_t)PatternSearch::FindPattern(MelonLoader::GameAssemblyDLL, "48 89 5C 24 ? 48 89 6C 24 ? 48 89 74 24 ? 57 48 83 EC 20 48 C7 C6 ? ? ? ? 48 8B F9");
}