#pragma once
#include "PointerUtils.h"
#include "il2cpp-internals.h"

typedef Il2CppDomain* (*il2cpp_init_t) (const char* name);
typedef void (*il2cpp_add_internal_call_t) (const char* name, void* method);
typedef void* (*il2cpp_resolve_icall_t) (const char* name);
typedef Il2CppClass* (*il2cpp_class_from_system_type_t) (Il2CppReflectionType* type);
typedef const char* (*il2cpp_type_get_name_t) (Il2CppType* type);
typedef const char* (*il2cpp_class_get_name_t) (Il2CppClass* klass);
typedef const char* (*il2cpp_class_get_namespace_t) (Il2CppClass* klass);
typedef const char* (*il2cpp_class_get_assemblyname_t) (Il2CppClass* klass);
typedef Il2CppClass* (*il2cpp_class_from_name_t) (Il2CppImage* image, const char* name_space, const char* name);
typedef Il2CppMethod* (*il2cpp_class_get_method_from_name_t) (Il2CppClass* klass, const char* name, int argsCount);
typedef Il2CppClass* (*il2cpp_image_get_class_t) (Il2CppImage* image, int index);
typedef Il2CppClass* (*il2cpp_type_get_class_or_element_class_t) (Il2CppType* type);
typedef Il2CppImage* (*il2cpp_assembly_get_image_t) (Il2CppAssembly* assembly);
typedef Il2CppAssembly* (*il2cpp_domain_assembly_open_t) (Il2CppDomain* domain, const char* name);
typedef Il2CppObject* (*il2cpp_runtime_invoke_t) (Il2CppMethod* method, void* obj, Il2CppObject** params, int** exc);
typedef Il2CppClass* (*il2cpp_class_get_nested_types_t) (Il2CppClass* klass, void** iter);
typedef Il2CppType* (*il2cpp_class_get_type_t) (Il2CppClass* klass);
typedef Il2CppGlobalMetadataHeader* (*MetadataLoader_LoadMetadataFile_t) (const char* fileName);

class IL2CPP
{
public:
	static HMODULE Module;
	static Il2CppDomain* Domain;
	static il2cpp_init_t il2cpp_init;
	static il2cpp_add_internal_call_t il2cpp_add_internal_call;
	static il2cpp_resolve_icall_t il2cpp_resolve_icall;
	static il2cpp_class_from_system_type_t il2cpp_class_from_system_type;
	static il2cpp_type_get_name_t il2cpp_type_get_name;
	static il2cpp_class_get_name_t il2cpp_class_get_name;
	static il2cpp_class_get_namespace_t il2cpp_class_get_namespace;
	static il2cpp_class_get_assemblyname_t il2cpp_class_get_assemblyname;
	static il2cpp_class_from_name_t il2cpp_class_from_name;
	static il2cpp_class_get_method_from_name_t il2cpp_class_get_method_from_name;
	static il2cpp_image_get_class_t il2cpp_image_get_class;
	static il2cpp_type_get_class_or_element_class_t il2cpp_type_get_class_or_element_class;
	static il2cpp_assembly_get_image_t il2cpp_assembly_get_image;
	static il2cpp_domain_assembly_open_t il2cpp_domain_assembly_open;
	static il2cpp_runtime_invoke_t il2cpp_runtime_invoke;
	static il2cpp_class_get_nested_types_t il2cpp_class_get_nested_types;
	static il2cpp_class_get_type_t il2cpp_class_get_type;

	static MetadataLoader_LoadMetadataFile_t MetadataLoader_LoadMetadataFile;
	static Il2CppGlobalMetadataHeader* s_GlobalMetadataHeader;

	static bool Setup();
};