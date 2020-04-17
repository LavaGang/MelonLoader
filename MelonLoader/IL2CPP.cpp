#include "IL2CPP.h"
#include "AssertionManager.h"

HMODULE IL2CPP::Module = NULL;
Il2CppDomain* IL2CPP::Domain = NULL;
il2cpp_init_t IL2CPP::il2cpp_init = NULL;
il2cpp_add_internal_call_t IL2CPP::il2cpp_add_internal_call = NULL;
il2cpp_resolve_icall_t IL2CPP::il2cpp_resolve_icall = NULL;
il2cpp_class_from_system_type_t IL2CPP::il2cpp_class_from_system_type = NULL;
il2cpp_type_get_name_t IL2CPP::il2cpp_type_get_name = NULL;
il2cpp_class_get_name_t IL2CPP::il2cpp_class_get_name = NULL;
il2cpp_class_get_namespace_t IL2CPP::il2cpp_class_get_namespace = NULL;
il2cpp_class_get_assemblyname_t IL2CPP::il2cpp_class_get_assemblyname = NULL;
il2cpp_class_from_name_t IL2CPP::il2cpp_class_from_name = NULL;
il2cpp_class_get_method_from_name_t IL2CPP::il2cpp_class_get_method_from_name = NULL;
il2cpp_image_get_class_t IL2CPP::il2cpp_image_get_class = NULL;
il2cpp_type_get_class_or_element_class_t IL2CPP::il2cpp_type_get_class_or_element_class = NULL;
il2cpp_assembly_get_image_t IL2CPP::il2cpp_assembly_get_image = NULL;
il2cpp_domain_assembly_open_t IL2CPP::il2cpp_domain_assembly_open = NULL;
il2cpp_runtime_invoke_t IL2CPP::il2cpp_runtime_invoke = NULL;
il2cpp_class_get_nested_types_t IL2CPP::il2cpp_class_get_nested_types = NULL;
il2cpp_class_get_type_t IL2CPP::il2cpp_class_get_type = NULL;
//MetadataLoader_LoadMetadataFile_t IL2CPP::MetadataLoader_LoadMetadataFile = NULL;
//Il2CppGlobalMetadataHeader* IL2CPP::s_GlobalMetadataHeader = NULL;

bool IL2CPP::Setup()
{
	AssertionManager::Start("IL2CPP.cpp", "IL2CPP::Setup");

	il2cpp_init = (il2cpp_init_t)AssertionManager::GetExport(Module, "il2cpp_init");
	il2cpp_add_internal_call = (il2cpp_add_internal_call_t)AssertionManager::GetExport(Module, "il2cpp_add_internal_call");
	il2cpp_resolve_icall = (il2cpp_resolve_icall_t)AssertionManager::GetExport(Module, "il2cpp_resolve_icall");
	il2cpp_class_from_system_type = (il2cpp_class_from_system_type_t)AssertionManager::GetExport(Module, "il2cpp_class_from_system_type");
	il2cpp_type_get_name = (il2cpp_type_get_name_t)AssertionManager::GetExport(Module, "il2cpp_type_get_name");
	il2cpp_class_get_name = (il2cpp_class_get_name_t)AssertionManager::GetExport(Module, "il2cpp_class_get_name");
	il2cpp_class_get_namespace = (il2cpp_class_get_namespace_t)AssertionManager::GetExport(Module, "il2cpp_class_get_namespace");
	il2cpp_class_get_assemblyname = (il2cpp_class_get_assemblyname_t)AssertionManager::GetExport(Module, "il2cpp_class_get_assemblyname");
	il2cpp_class_from_name = (il2cpp_class_from_name_t)AssertionManager::GetExport(Module, "il2cpp_class_from_name");
	il2cpp_class_get_method_from_name = (il2cpp_class_get_method_from_name_t)AssertionManager::GetExport(Module, "il2cpp_class_get_method_from_name");
	il2cpp_image_get_class = (il2cpp_image_get_class_t)AssertionManager::GetExport(Module, "il2cpp_image_get_class");
	il2cpp_type_get_class_or_element_class = (il2cpp_type_get_class_or_element_class_t)AssertionManager::GetExport(Module, "il2cpp_type_get_class_or_element_class");
	il2cpp_assembly_get_image = (il2cpp_assembly_get_image_t)AssertionManager::GetExport(Module, "il2cpp_assembly_get_image");
	il2cpp_domain_assembly_open = (il2cpp_domain_assembly_open_t)AssertionManager::GetExport(Module, "il2cpp_domain_assembly_open");
	il2cpp_runtime_invoke = (il2cpp_runtime_invoke_t)AssertionManager::GetExport(Module, "il2cpp_runtime_invoke");
	il2cpp_class_get_nested_types = (il2cpp_class_get_nested_types_t)AssertionManager::GetExport(Module, "il2cpp_class_get_nested_types");
	il2cpp_class_get_type = (il2cpp_class_get_type_t)AssertionManager::GetExport(Module, "il2cpp_class_get_type");

	/*
	// 2018.4.19f1 & 2019.2.0f1 & 2018.4.20f1
	MetadataLoader_LoadMetadataFile = (MetadataLoader_LoadMetadataFile_t)PointerUtils::FindPattern(Module, "40 55 56 57 41 54 41 55 41 56 41 57 48 8D 6C 24 ? 48 81 EC ? ? ? ? 48 C7 45 ? ? ? ? ? 48 89 9C 24 ? ? ? ? 4C 8B F1");
	if (MetadataLoader_LoadMetadataFile == NULL) // 2018.4.10f1
		MetadataLoader_LoadMetadataFile = (MetadataLoader_LoadMetadataFile_t)PointerUtils::FindPattern(Module, "48 8B C4 55 48 8D 68 A1 48 81 EC ? ? ? ? 48 C7 45 ? ? ? ? ? 48 89 58 08 48 89 78 18");
	if (MetadataLoader_LoadMetadataFile == NULL) // 2018.4.6f1
		MetadataLoader_LoadMetadataFile = (MetadataLoader_LoadMetadataFile_t)PointerUtils::FindPattern(Module, "40 57 41 54 41 55 41 56 41 57 48 83 EC 30 48 C7 44 24 ? ? ? ? ? 48 89 5C 24 ? 48 89 6C 24 ? 48 89 74 24 ? 49 8B E8 4C");
	AssertionManager::Decide(MetadataLoader_LoadMetadataFile, "MetadataLoader_LoadMetadataFile");
	*/

	return !AssertionManager::Result;
}