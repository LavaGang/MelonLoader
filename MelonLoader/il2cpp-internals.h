#pragma once
#pragma warning( disable : 4200 )
#include <stdint.h>

struct Il2CppDomain;
struct Il2CppAssembly;
struct Il2CppImage;
struct Il2CppMethod {
	void* targetMethod;
};

typedef struct Il2CppTypeDefinition
{
	int32_t nameIndex;
	int32_t namespaceIndex;
	int32_t byvalTypeIndex;
	int32_t byrefTypeIndex;
	int32_t declaringTypeIndex;
	int32_t parentIndex;
	int32_t elementTypeIndex;
	int32_t genericContainerIndex;
	uint32_t flags;
	int32_t fieldStart;
	int32_t methodStart;
	int32_t eventStart;
	int32_t propertyStart;
	int32_t nestedTypesStart;
	int32_t interfacesStart;
	int32_t vtableStart;
	int32_t interfaceOffsetsStart;
	uint16_t method_count;
	uint16_t property_count;
	uint16_t field_count;
	uint16_t event_count;
	uint16_t nested_type_count;
	uint16_t vtable_count;
	uint16_t interfaces_count;
	uint16_t interface_offsets_count;
	uint32_t bitfield;
	uint32_t token;
} Il2CppTypeDefinition;

typedef struct VirtualInvokeData
{
	void* methodPtr;
	const void* method;
} VirtualInvokeData;

typedef enum Il2CppTypeEnum
{
	Il2Cpp_TYPE_END = 0x00,       /* End of List */
	Il2Cpp_TYPE_VOID = 0x01,
	Il2Cpp_TYPE_BOOLEAN = 0x02,
	Il2Cpp_TYPE_CHAR = 0x03,
	Il2Cpp_TYPE_I1 = 0x04,
	Il2Cpp_TYPE_U1 = 0x05,
	Il2Cpp_TYPE_I2 = 0x06,
	Il2Cpp_TYPE_U2 = 0x07,
	Il2Cpp_TYPE_I4 = 0x08,
	Il2Cpp_TYPE_U4 = 0x09,
	Il2Cpp_TYPE_I8 = 0x0a,
	Il2Cpp_TYPE_U8 = 0x0b,
	Il2Cpp_TYPE_R4 = 0x0c,
	Il2Cpp_TYPE_R8 = 0x0d,
	Il2Cpp_TYPE_STRING = 0x0e,
	Il2Cpp_TYPE_PTR = 0x0f,       /* arg: <type> token */
	Il2Cpp_TYPE_BYREF = 0x10,       /* arg: <type> token */
	Il2Cpp_TYPE_VALUETYPE = 0x11,       /* arg: <type> token */
	Il2Cpp_TYPE_CLASS = 0x12,       /* arg: <type> token */
	Il2Cpp_TYPE_VAR = 0x13,       /* Generic parameter in a generic type definition, represented as number (compressed unsigned integer) number */
	Il2Cpp_TYPE_ARRAY = 0x14,       /* type, rank, boundsCount, bound1, loCount, lo1 */
	Il2Cpp_TYPE_GENERICINST = 0x15,     /* <type> <type-arg-count> <type-1> \x{2026} <type-n> */
	Il2Cpp_TYPE_TYPEDBYREF = 0x16,
	Il2Cpp_TYPE_I = 0x18,
	Il2Cpp_TYPE_U = 0x19,
	Il2Cpp_TYPE_FNPTR = 0x1b,        /* arg: full method signature */
	Il2Cpp_TYPE_OBJECT = 0x1c,
	Il2Cpp_TYPE_SZARRAY = 0x1d,       /* 0-based one-dim-array */
	Il2Cpp_TYPE_MVAR = 0x1e,       /* Generic parameter in a generic method definition, represented as number (compressed unsigned integer)  */
	Il2Cpp_TYPE_CMOD_REQD = 0x1f,       /* arg: typedef or typeref token */
	Il2Cpp_TYPE_CMOD_OPT = 0x20,       /* optional arg: typedef or typref token */
	Il2Cpp_TYPE_INTERNAL = 0x21,       /* CLR internal type */

	Il2Cpp_TYPE_MODIFIER = 0x40,       /* Or with the following types */
	Il2Cpp_TYPE_SENTINEL = 0x41,       /* Sentinel for varargs method signature */
	Il2Cpp_TYPE_PINNED = 0x45,       /* Local var that points to pinned object */

	Il2Cpp_TYPE_ENUM = 0x55        /* an enumeration */
} Il2CppTypeEnum;

typedef struct Il2CppType
{
	union
	{
		// We have this dummy field first because pre C99 compilers (MSVC) can only initializer the first value in a union.
		void* dummy;
		int32_t klassIndex; /* for VALUETYPE and CLASS */
		const Il2CppType* type;   /* for PTR and SZARRAY */
		void* array; /* for ARRAY */
		//MonoMethodSignature *method;
		int32_t genericParameterIndex; /* for VAR and MVAR */
		void* generic_class; /* for GENERICINST */
	} data;
	unsigned int attrs : 16; /* param attributes or field flags */
	Il2CppTypeEnum type : 8;
	unsigned int num_mods : 6;  /* max 64 modifiers follow at the end */
	unsigned int byref : 1;
	unsigned int pinned : 1;  /* valid when included in a local var signature */
	//MonoCustomMod modifiers [MONO_ZERO_LEN_ARRAY]; /* this may grow */
} Il2CppType;

typedef struct Il2CppClass
{
	// The following fields are always valid for a Il2CppClass structure
	const Il2CppImage* image;
	void* gc_desc;
	const char* name;
	const char* namespaze;
	Il2CppType byval_arg;
	Il2CppType this_arg;
	Il2CppClass* element_class;
	Il2CppClass* castClass;
	Il2CppClass* declaringType;
	Il2CppClass* parent;
	void* generic_class;
	const Il2CppTypeDefinition* typeDefinition; // non-NULL for Il2CppClass's constructed from type defintions
	const void* interopData;
	Il2CppClass* klass; // hack to pretend we are a MonoVTable. Points to ourself
	// End always valid fields

	// The following fields need initialized before access. This can be done per field or as an aggregate via a call to Class::Init
	void* fields; // Initialized in SetupFields
	const void* events; // Initialized in SetupEvents
	const void* properties; // Initialized in SetupProperties
	const void** methods; // Initialized in SetupMethods
	Il2CppClass** nestedTypes; // Initialized in SetupNestedTypes
	Il2CppClass** implementedInterfaces; // Initialized in SetupInterfaces
	void* interfaceOffsets; // Initialized in Init
	void* static_fields; // Initialized in Init
	const void* rgctx_data; // Initialized in Init
	// used for fast parent checks
	Il2CppClass** typeHierarchy; // Initialized in SetupTypeHierachy
	// End initialization required fields

	void* unity_user_data;

	uint32_t initializationExceptionGCHandle;

	uint32_t cctor_started;
	uint32_t cctor_finished;
	alignas(8) size_t cctor_thread;

	// Remaining fields are always valid except where noted
	int32_t genericContainerIndex;
	uint32_t instance_size;
	uint32_t actualSize;
	uint32_t element_size;
	int32_t native_size;
	uint32_t static_fields_size;
	uint32_t thread_static_fields_size;
	int32_t thread_static_fields_offset;
	uint32_t flags;
	uint32_t token;

	uint16_t method_count; // lazily calculated for arrays, i.e. when rank > 0
	uint16_t property_count;
	uint16_t field_count;
	uint16_t event_count;
	uint16_t nested_type_count;
	uint16_t vtable_count; // lazily calculated for arrays, i.e. when rank > 0
	uint16_t interfaces_count;
	uint16_t interface_offsets_count; // lazily calculated for arrays, i.e. when rank > 0

	uint8_t typeHierarchyDepth; // Initialized in SetupTypeHierachy
	uint8_t genericRecursionDepth;
	uint8_t rank;
	uint8_t minimumAlignment; // Alignment of this type
	uint8_t naturalAligment; // Alignment of this type without accounting for packing
	uint8_t packingSize;

	// this is critical for performance of Class::InitFromCodegen. Equals to initialized && !has_initialization_error at all times.
	// Use Class::UpdateInitializedAndNoError to update
	uint8_t initialized_and_no_error : 1;

	uint8_t valuetype : 1;
	uint8_t initialized : 1;
	uint8_t enumtype : 1;
	uint8_t is_generic : 1;
	uint8_t has_references : 1;
	uint8_t init_pending : 1;
	uint8_t size_inited : 1;
	uint8_t has_finalize : 1;
	uint8_t has_cctor : 1;
	uint8_t is_blittable : 1;
	uint8_t is_import_or_windows_runtime : 1;
	uint8_t is_vtable_initialized : 1;
	uint8_t has_initialization_error : 1;
	VirtualInvokeData vtable[0];
} Il2CppClass;