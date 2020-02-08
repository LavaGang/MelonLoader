#pragma once
#include <stdint.h>
#include "il2cpp-internals.h"

typedef Il2CppDomain* (*il2cpp_init_t) (const char* name);
typedef void (*il2cpp_add_internal_call_t) (const char* name, void* method);
typedef void (*il2cpp_custom_attrs_free_t) (void* attrptr);
typedef void* (*il2cpp_resolve_icall_t) (const char* name);
typedef Il2CppType* (*il2cpp_class_enum_basetype_t) (Il2CppClass* klass);
typedef Il2CppClass* (*il2cpp_class_from_system_type_t) (void* type);
typedef const char* (*il2cpp_type_get_name_t) (Il2CppType* type);
typedef const char* (*il2cpp_class_get_name_t) (Il2CppClass* klass);
typedef const char* (*il2cpp_class_get_namespace_t) (Il2CppClass* klass);
typedef const char* (*il2cpp_class_get_assemblyname_t) (Il2CppClass* klass);
typedef Il2CppClass* (*il2cpp_class_from_name_t) (Il2CppImage* image, const char* name_space, const char* name);
typedef Il2CppMethod* (*il2cpp_class_get_method_from_name_t) (Il2CppClass* klass, const char* name, int argsCount);
typedef Il2CppClass* (*il2cpp_image_get_class_t) (Il2CppImage* image, int index);
typedef Il2CppClass* (*il2cpp_type_get_class_or_element_class_t) (Il2CppType* type);
typedef Il2CppAssembly** (*il2cpp_domain_get_assemblies_t) (Il2CppDomain* domain, size_t* size);
typedef Il2CppImage* (*il2cpp_assembly_get_image_t) (Il2CppAssembly* assembly);
typedef bool (*il2cpp_class_init_t) (Il2CppClass* klass);

class IL2CPP
{
public:
	static Il2CppDomain* Domain;
	static il2cpp_init_t il2cpp_init;
	static il2cpp_add_internal_call_t il2cpp_add_internal_call;
	static il2cpp_custom_attrs_free_t il2cpp_custom_attrs_free;
	static il2cpp_resolve_icall_t il2cpp_resolve_icall;
	static il2cpp_class_enum_basetype_t il2cpp_class_enum_basetype;
	static il2cpp_class_from_system_type_t il2cpp_class_from_system_type;
	static il2cpp_type_get_name_t il2cpp_type_get_name;
	static il2cpp_class_get_name_t il2cpp_class_get_name;
	static il2cpp_class_get_namespace_t il2cpp_class_get_namespace;
	static il2cpp_class_get_assemblyname_t il2cpp_class_get_assemblyname;
	static il2cpp_class_from_name_t il2cpp_class_from_name;
	static il2cpp_class_get_method_from_name_t il2cpp_class_get_method_from_name;
	static il2cpp_image_get_class_t il2cpp_image_get_class;
	static il2cpp_type_get_class_or_element_class_t il2cpp_type_get_class_or_element_class;
	static il2cpp_domain_get_assemblies_t il2cpp_domain_get_assemblies;
	static il2cpp_assembly_get_image_t il2cpp_assembly_get_image;
	static il2cpp_class_init_t il2cpp_class_init;

	static bool Setup();
};