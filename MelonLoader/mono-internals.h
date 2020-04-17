#pragma once
#include <stdint.h>

struct MonoDomain;
struct MonoAssembly;
struct MonoMethod;
struct MonoThread;
struct MonoImage;

typedef enum {
	MONO_TYPE_END = 0x00,       /* End of List */
	MONO_TYPE_VOID = 0x01,
	MONO_TYPE_BOOLEAN = 0x02,
	MONO_TYPE_CHAR = 0x03,
	MONO_TYPE_I1 = 0x04,
	MONO_TYPE_U1 = 0x05,
	MONO_TYPE_I2 = 0x06,
	MONO_TYPE_U2 = 0x07,
	MONO_TYPE_I4 = 0x08,
	MONO_TYPE_U4 = 0x09,
	MONO_TYPE_I8 = 0x0a,
	MONO_TYPE_U8 = 0x0b,
	MONO_TYPE_R4 = 0x0c,
	MONO_TYPE_R8 = 0x0d,
	MONO_TYPE_STRING = 0x0e,
	MONO_TYPE_PTR = 0x0f,       /* arg: <type> token */
	MONO_TYPE_BYREF = 0x10,       /* arg: <type> token */
	MONO_TYPE_VALUETYPE = 0x11,       /* arg: <type> token */
	MONO_TYPE_CLASS = 0x12,       /* arg: <type> token */
	MONO_TYPE_VAR = 0x13,	   /* number */
	MONO_TYPE_ARRAY = 0x14,       /* type, rank, boundsCount, bound1, loCount, lo1 */
	MONO_TYPE_GENERICINST = 0x15,	   /* <type> <type-arg-count> <type-1> \x{2026} <type-n> */
	MONO_TYPE_TYPEDBYREF = 0x16,
	MONO_TYPE_I = 0x18,
	MONO_TYPE_U = 0x19,
	MONO_TYPE_FNPTR = 0x1b,	      /* arg: full method signature */
	MONO_TYPE_OBJECT = 0x1c,
	MONO_TYPE_SZARRAY = 0x1d,       /* 0-based one-dim-array */
	MONO_TYPE_MVAR = 0x1e,       /* number */
	MONO_TYPE_CMOD_REQD = 0x1f,       /* arg: typedef or typeref token */
	MONO_TYPE_CMOD_OPT = 0x20,       /* optional arg: typedef or typref token */
	MONO_TYPE_INTERNAL = 0x21,       /* CLR internal type */

	MONO_TYPE_MODIFIER = 0x40,       /* Or with the following types */
	MONO_TYPE_SENTINEL = 0x41,       /* Sentinel for varargs method signature */
	MONO_TYPE_PINNED = 0x45,       /* Local var that points to pinned object */

	MONO_TYPE_ENUM = 0x55        /* an enumeration */
} MonoTypeEnum;

typedef enum {
	MONO_TABLE_MODULE,
	MONO_TABLE_TYPEREF,
	MONO_TABLE_TYPEDEF,
	MONO_TABLE_FIELD_POINTER,
	MONO_TABLE_FIELD,
	MONO_TABLE_METHOD_POINTER,
	MONO_TABLE_METHOD,
	MONO_TABLE_PARAM_POINTER,
	MONO_TABLE_PARAM,
	MONO_TABLE_INTERFACEIMPL,
	MONO_TABLE_MEMBERREF, /* 0xa */
	MONO_TABLE_CONSTANT,
	MONO_TABLE_CUSTOMATTRIBUTE,
	MONO_TABLE_FIELDMARSHAL,
	MONO_TABLE_DECLSECURITY,
	MONO_TABLE_CLASSLAYOUT,
	MONO_TABLE_FIELDLAYOUT, /* 0x10 */
	MONO_TABLE_STANDALONESIG,
	MONO_TABLE_EVENTMAP,
	MONO_TABLE_EVENT_POINTER,
	MONO_TABLE_EVENT,
	MONO_TABLE_PROPERTYMAP,
	MONO_TABLE_PROPERTY_POINTER,
	MONO_TABLE_PROPERTY,
	MONO_TABLE_METHODSEMANTICS,
	MONO_TABLE_METHODIMPL,
	MONO_TABLE_MODULEREF, /* 0x1a */
	MONO_TABLE_TYPESPEC,
	MONO_TABLE_IMPLMAP,
	MONO_TABLE_FIELDRVA,
	MONO_TABLE_UNUSED6,
	MONO_TABLE_UNUSED7,
	MONO_TABLE_ASSEMBLY, /* 0x20 */
	MONO_TABLE_ASSEMBLYPROCESSOR,
	MONO_TABLE_ASSEMBLYOS,
	MONO_TABLE_ASSEMBLYREF,
	MONO_TABLE_ASSEMBLYREFPROCESSOR,
	MONO_TABLE_ASSEMBLYREFOS,
	MONO_TABLE_FILE,
	MONO_TABLE_EXPORTEDTYPE,
	MONO_TABLE_MANIFESTRESOURCE,
	MONO_TABLE_NESTEDCLASS,
	MONO_TABLE_GENERICPARAM, /* 0x2a */
	MONO_TABLE_METHODSPEC,
	MONO_TABLE_GENERICPARAMCONSTRAINT,
	MONO_TABLE_UNUSED8,
	MONO_TABLE_UNUSED9,
	MONO_TABLE_UNUSED10,
	/* Portable PDB tables */
	MONO_TABLE_DOCUMENT, /* 0x30 */
	MONO_TABLE_METHODBODY,
	MONO_TABLE_LOCALSCOPE,
	MONO_TABLE_LOCALVARIABLE,
	MONO_TABLE_LOCALCONSTANT,
	MONO_TABLE_IMPORTSCOPE,
	MONO_TABLE_STATEMACHINEMETHOD,
	MONO_TABLE_CUSTOMDEBUGINFORMATION
} MonoMetaTableEnum;

typedef struct {
	unsigned int required : 1;
	unsigned int token : 31;
} MonoCustomMod;

struct MonoType;
struct MonoGenericInst
{
	uint32_t type_argc : 22;	/* number of type arguments */
	uint32_t is_open : 1;	/* if this is an open type */
	MonoType* type_argv[1];
};

struct MonoGenericContext
{
	/* The instantiation corresponding to the class generic parameters */
	MonoGenericInst* class_inst;
	/* The instantiation corresponding to the method generic parameters */
	MonoGenericInst* method_inst;
};

struct MonoClass;
struct MonoGenericClass {
	MonoClass* container_class;	/* the generic type definition */
	MonoGenericContext context;	/* a context that contains the type instantiation doesn't contain any method instantiation */ /* FIXME: Only the class_inst member of "context" is ever used, so this field could be replaced with just a monogenericinst */
	uint32_t is_dynamic : 1;		/* Contains dynamic types */
	uint32_t is_tb_open : 1;		/* This is the fully open instantiation for a type_builder. Quite ugly, but it's temporary.*/
	uint32_t need_sync : 1;      /* Only if dynamic. Need to be synchronized with its container class after its finished. */
	MonoClass* cached_class;	/* if present, the MonoClass corresponding to the instantiation.  */

	/*
	 * The image set which owns this generic class. Memory owned by the generic class
	 * including cached_class should be allocated from the mempool of the image set,
	 * so it is easy to free.
	 */
	void* owner;
};

struct MonoType {
	union {
		MonoClass* klass; /* for VALUETYPE and CLASS */
		MonoType* type;   /* for PTR */
		void* array; /* for ARRAY */
		void* method;
		void* generic_param; /* for VAR and MVAR */
		MonoGenericClass* generic_class; /* for GENERICINST */
	} data;
	unsigned int attrs : 16; /* param attributes or field flags */
	MonoTypeEnum type : 8;
	unsigned int num_mods : 6;  /* max 64 modifiers follow at the end */
	unsigned int byref : 1;
	unsigned int pinned : 1;  /* valid when included in a local var signature */
	MonoCustomMod modifiers[1]; /* this may grow */
};

struct MonoPropertyBagItem {
	MonoPropertyBagItem* next;
	int tag;
};

typedef struct {
	MonoPropertyBagItem* head;
} MonoPropertyBag;

typedef struct {
	const char* data;
	uint32_t  size;
} MonoStreamHeader;

struct MonoObject {
	void* vtable;
	void* synchronisation;
};

struct MonoReflectionType {
	MonoObject object;
	MonoType* type;
};

struct MonoTableInfo {
	const char* base;
	unsigned int       rows : 24;
	unsigned int       row_size : 8;

	/*
	 * Tables contain up to 9 columns and the possible sizes of the
	 * fields in the documentation are 1, 2 and 4 bytes.  So we
	 * can encode in 2 bits the size.
	 *
	 * A 32 bit value can encode the resulting size
	 *
	 * The top eight bits encode the number of columns in the table.
	 * we only need 4, but 8 is aligned no shift required.
	 */
	uint32_t   size_bitfield;
};

struct MonoClass {
	/* element class for arrays and enum basetype for enums */
	MonoClass* element_class;
	/* used for subtype checks */
	MonoClass* cast_class;

	/* for fast subtype checks */
	MonoClass** supertypes;
	uint16_t     idepth;

	/* array dimension */
	uint8_t     rank;

	int        instance_size; /* object instance size */

	unsigned int inited : 1;

	/* A class contains static and non static data. Static data can be
	 * of the same type as the class itselfs, but it does not influence
	 * the instance size of the class. To avoid cyclic calls to
	 * mono_class_init (from mono_class_instance_size ()) we first
	 * initialise all non static fields. After that we set size_inited
	 * to 1, because we know the instance size now. After that we
	 * initialise all static fields.
	 */

	 /* ALL BITFIELDS SHOULD BE WRITTEN WHILE HOLDING THE LOADER LOCK */
	unsigned int size_inited : 1;
	unsigned int valuetype : 1; /* derives from System.ValueType */
	unsigned int enumtype : 1; /* derives from System.Enum */
	unsigned int blittable : 1; /* class is blittable */
	unsigned int unicode : 1; /* class uses unicode char when marshalled */
	unsigned int wastypebuilder : 1; /* class was created at runtime from a TypeBuilder */
	unsigned int is_array_special_interface : 1; /* gtd or ginst of once of the magic interfaces that arrays implement */

	/* next byte */
	uint8_t min_align;

	/* next byte */
	unsigned int packing_size : 4;
	unsigned int ghcimpl : 1; /* class has its own GetHashCode impl */
	unsigned int has_finalize : 1; /* class has its own Finalize impl */

	unsigned int marshalbyref : 1; /* class is a MarshalByRefObject */
	unsigned int contextbound : 1; /* class is a ContextBoundObject */

	/* next byte */
	unsigned int delegate        : 1; /* class is a Delegate */
	unsigned int gc_descr_inited : 1; /* gc_descr is initialized */
	unsigned int has_cctor : 1; /* class has a cctor */
	unsigned int has_references : 1; /* it has GC-tracked references in the instance */
	unsigned int has_static_refs : 1; /* it has static fields that are GC-tracked */
	unsigned int no_special_static_fields : 1; /* has no thread/context static fields */
	/* directly or indirectly derives from ComImport attributed class.
	 * this means we need to create a proxy for instances of this class
	 * for COM Interop. set this flag on loading so all we need is a quick check
	 * during object creation rather than having to traverse supertypes
	 */
	unsigned int is_com_object : 1;
	unsigned int nested_classes_inited : 1; /* Whenever nested_class is initialized */

	/* next byte*/
	unsigned int class_kind : 3; /* One of the values from MonoTypeKind */
	unsigned int interfaces_inited : 1; /* interfaces is initialized */
	unsigned int simd_type : 1; /* class is a simd intrinsic type */
	unsigned int has_finalize_inited : 1; /* has_finalize is initialized */
	unsigned int fields_inited : 1; /* setup_fields () has finished */
	unsigned int has_failure : 1; /* See mono_class_get_exception_data () for a MonoErrorBoxed with the details */
	unsigned int has_weak_fields : 1; /* class has weak reference fields */

	MonoClass* parent;
	MonoClass* nested_in;

	MonoImage* image;
	const char* name;
	const char* name_space;

	uint32_t    type_token;
	int        vtable_size; /* number of slots */

	uint16_t     interface_count;
	uint32_t     interface_id;        /* unique inderface id (for interfaces) */
	uint32_t     max_interface_id;

	uint16_t     interface_offsets_count;
	MonoClass** interfaces_packed;
	uint16_t* interface_offsets_packed;
	/* enabled only with small config for now: we might want to do it unconditionally */
	uint8_t * interface_bitmap;

	MonoClass** interfaces;

	union {
		int class_size; /* size of area for static fields */
		int element_size; /* for array types */
		int generic_param_token; /* for generic param types, both var and mvar */
	} sizes;

	/*
	 * Field information: Type and location from object base
	 */
	void* fields;

	MonoMethod** methods;

	/* used as the type of the this argument and when passing the arg by value */
	MonoType this_arg;
	MonoType byval_arg;

	void* gc_descr;

	void* runtime_info;

	/* Generic vtable. Initialized by a call to mono_class_setup_vtable () */
	MonoMethod** vtable;

	/* Infrequently used items. See class-accessors.c: InfrequentDataKind for what goes into here. */
	MonoPropertyBag infrequent_data;

	void* unity_user_data;
};

struct MonoMethodSignature {
	MonoType* ret;
	unsigned int  param_count;
	int           sentinelpos;
	unsigned int  generic_param_count : 16;
	unsigned int  call_convention : 6;
	unsigned int  hasthis : 1;
	unsigned int  explicit_this : 1;
	unsigned int  pinvoke : 1;
	unsigned int  is_inflated : 1;
	unsigned int  has_type_parameters : 1;
	MonoType* params[1];
};

struct MonoString;