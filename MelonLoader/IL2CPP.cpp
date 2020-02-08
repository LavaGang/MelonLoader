#include <Windows.h>
#include "MelonLoader.h"
#include "Il2Cpp.h"
#include "PointerUtils.h"

Il2CppDomain* IL2CPP::Domain = NULL;
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
il2cpp_class_init_t IL2CPP::il2cpp_class_init = NULL;

bool IL2CPP::Setup()
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
	il2cpp_class_init = (il2cpp_class_init_t)GetProcAddress(MelonLoader::GameAssemblyDLL, "il2cpp_class_init");

	return true;
}