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
typedef Il2CppClass* (*MetadataCache_GetTypeInfoFromTypeDefinitionIndex_t) (int index);
typedef Il2CppClass* (*MetadataCache_FromTypeDefinition_t) (int index);

typedef struct Il2CppGlobalMetadataHeader
{
	int32_t sanity;
	int32_t version;
	int32_t stringLiteralOffset;
	int32_t stringLiteralCount;
	int32_t stringLiteralDataOffset;
	int32_t stringLiteralDataCount;
	int32_t stringOffset;
	int32_t stringCount;
	int32_t eventsOffset;
	int32_t eventsCount;
	int32_t propertiesOffset;
	int32_t propertiesCount;
	int32_t methodsOffset;
	int32_t methodsCount;
	int32_t parameterDefaultValuesOffset;
	int32_t parameterDefaultValuesCount;
	int32_t fieldDefaultValuesOffset;
	int32_t fieldDefaultValuesCount;
	int32_t fieldAndParameterDefaultValueDataOffset;
	int32_t fieldAndParameterDefaultValueDataCount;
	int32_t fieldMarshaledSizesOffset;
	int32_t fieldMarshaledSizesCount;
	int32_t parametersOffset;
	int32_t parametersCount;
	int32_t fieldsOffset;
	int32_t fieldsCount;
	int32_t genericParametersOffset;
	int32_t genericParametersCount;
	int32_t genericParameterConstraintsOffset;
	int32_t genericParameterConstraintsCount;
	int32_t genericContainersOffset;
	int32_t genericContainersCount;
	int32_t nestedTypesOffset;
	int32_t nestedTypesCount;
	int32_t interfacesOffset;
	int32_t interfacesCount;
	int32_t vtableMethodsOffset;
	int32_t vtableMethodsCount;
	int32_t interfaceOffsetsOffset;
	int32_t interfaceOffsetsCount;
	int32_t typeDefinitionsOffset;
	int32_t typeDefinitionsCount;
	int32_t imagesOffset;
	int32_t imagesCount;
	int32_t assembliesOffset;
	int32_t assembliesCount;
	int32_t metadataUsageListsOffset;
	int32_t metadataUsageListsCount;
	int32_t metadataUsagePairsOffset;
	int32_t metadataUsagePairsCount;
	int32_t fieldRefsOffset;
	int32_t fieldRefsCount;
	int32_t referencedAssembliesOffset;
	int32_t referencedAssembliesCount;
	int32_t attributesInfoOffset;
	int32_t attributesInfoCount;
	int32_t attributeTypesOffset;
	int32_t attributeTypesCount;
	int32_t unresolvedVirtualCallParameterTypesOffset;
	int32_t unresolvedVirtualCallParameterTypesCount;
	int32_t unresolvedVirtualCallParameterRangesOffset;
	int32_t unresolvedVirtualCallParameterRangesCount;
	int32_t windowsRuntimeTypeNamesOffset;
	int32_t windowsRuntimeTypeNamesSize;
	int32_t exportedTypeDefinitionsOffset;
	int32_t exportedTypeDefinitionsCount;
} Il2CppGlobalMetadataHeader;
typedef Il2CppGlobalMetadataHeader* (*MetadataLoader_LoadMetadataFile_t) (const char* fileName);

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

	static MetadataCache_GetTypeInfoFromTypeDefinitionIndex_t MetadataCache_GetTypeInfoFromTypeDefinitionIndex;
	static MetadataCache_FromTypeDefinition_t MetadataCache_FromTypeDefinition;
	static MetadataLoader_LoadMetadataFile_t MetadataLoader_LoadMetadataFile;
	static Il2CppGlobalMetadataHeader* s_GlobalMetadataHeader;
	static uintptr_t s_TypeInfoDefinitionTable;

	static void Setup();
};