#include <Windows.h>
#include "MelonLoader.h"
#include "Il2Cpp.h"
#include "PointerUtils.h"
#include "Console.h"

HMODULE IL2CPP::Module = NULL;
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
il2cpp_domain_assembly_open_t IL2CPP::il2cpp_domain_assembly_open = NULL;
il2cpp_runtime_invoke_t IL2CPP::il2cpp_runtime_invoke = NULL;
il2cpp_object_unbox_t IL2CPP::il2cpp_object_unbox = NULL;
il2cpp_string_new_t IL2CPP::il2cpp_string_new = NULL;
il2cpp_image_get_name_t IL2CPP::il2cpp_image_get_name = NULL;
il2cpp_image_get_class_count_t IL2CPP::il2cpp_image_get_class_count = NULL;
il2cpp_class_get_methods_t IL2CPP::il2cpp_class_get_methods = NULL;
il2cpp_method_get_name_t IL2CPP::il2cpp_method_get_name = NULL;

bool IL2CPP::Setup()
{
	il2cpp_init = (il2cpp_init_t)GetProcAddress(Module, "il2cpp_init");
	il2cpp_add_internal_call = (il2cpp_add_internal_call_t)GetProcAddress(Module, "il2cpp_add_internal_call");
	il2cpp_custom_attrs_free = (il2cpp_custom_attrs_free_t)GetProcAddress(Module, "il2cpp_custom_attrs_free");
	il2cpp_resolve_icall = (il2cpp_resolve_icall_t)GetProcAddress(Module, "il2cpp_resolve_icall");
	il2cpp_class_enum_basetype = (il2cpp_class_enum_basetype_t)GetProcAddress(Module, "il2cpp_class_enum_basetype");
	il2cpp_class_from_system_type = (il2cpp_class_from_system_type_t)GetProcAddress(Module, "il2cpp_class_from_system_type");
	il2cpp_type_get_name = (il2cpp_type_get_name_t)GetProcAddress(Module, "il2cpp_type_get_name");
	il2cpp_class_get_name = (il2cpp_class_get_name_t)GetProcAddress(Module, "il2cpp_class_get_name");
	il2cpp_class_get_namespace = (il2cpp_class_get_namespace_t)GetProcAddress(Module, "il2cpp_class_get_namespace");
	il2cpp_class_get_assemblyname = (il2cpp_class_get_assemblyname_t)GetProcAddress(Module, "il2cpp_class_get_assemblyname");
	il2cpp_class_from_name = (il2cpp_class_from_name_t)GetProcAddress(Module, "il2cpp_class_from_name");
	il2cpp_class_get_method_from_name = (il2cpp_class_get_method_from_name_t)GetProcAddress(Module, "il2cpp_class_get_method_from_name");
	il2cpp_image_get_class = (il2cpp_image_get_class_t)GetProcAddress(Module, "il2cpp_image_get_class");
	il2cpp_type_get_class_or_element_class = (il2cpp_type_get_class_or_element_class_t)GetProcAddress(Module, "il2cpp_type_get_class_or_element_class");
	il2cpp_domain_get_assemblies = (il2cpp_domain_get_assemblies_t)GetProcAddress(Module, "il2cpp_domain_get_assemblies");
	il2cpp_assembly_get_image = (il2cpp_assembly_get_image_t)GetProcAddress(Module, "il2cpp_assembly_get_image");
	il2cpp_class_init = (il2cpp_class_init_t)GetProcAddress(Module, "il2cpp_class_init");
	il2cpp_domain_assembly_open = (il2cpp_domain_assembly_open_t)GetProcAddress(Module, "il2cpp_domain_assembly_open");
	il2cpp_runtime_invoke = (il2cpp_runtime_invoke_t)GetProcAddress(Module, "il2cpp_runtime_invoke");
	il2cpp_object_unbox = (il2cpp_object_unbox_t)GetProcAddress(Module, "il2cpp_object_unbox");
	il2cpp_string_new = (il2cpp_string_new_t)GetProcAddress(Module, "il2cpp_string_new");
	il2cpp_image_get_name = (il2cpp_image_get_name_t)GetProcAddress(Module, "il2cpp_image_get_name");
	il2cpp_image_get_class_count = (il2cpp_image_get_class_count_t)GetProcAddress(Module, "il2cpp_image_get_class_count");
	il2cpp_class_get_methods = (il2cpp_class_get_methods_t)GetProcAddress(Module, "il2cpp_class_get_methods");
	il2cpp_method_get_name = (il2cpp_method_get_name_t)GetProcAddress(Module, "il2cpp_method_get_name");

	return true;
}

bool IL2CPP::Is64bit()
{
	// To-Do
	return true;
}