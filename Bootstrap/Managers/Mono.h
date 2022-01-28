#pragma once
#include <Windows.h>
#include <string>

class Mono
{
public:
	struct Thread;
	struct Domain;
	struct Assembly;
	struct Image;
	struct Class;
	struct Method;
	struct Property;
	struct Object
	{
		void* vtable;
		void* synchronisation;
	};
	struct String;

	struct ReflectionAssembly
	{
		Object object;
		Assembly* assembly;
		/* CAS related */
		Object* evidence;	/* Evidence */
	};

	struct MarshalByRefObject
	{
		Object obj;
		Object* identity;
	};

	/* This is a copy of System.AppDomain */
	struct AppDomain
	{
		MarshalByRefObject mbr;
		Domain* data;
	};

	struct AssemblyName
	{
		const char* name;
		const char* culture;
		const char* hash_value;
		const char* public_key;
		// string of 16 hex chars + 1 NULL
		char public_key_token[17];
		uint32_t hash_alg;
		uint32_t hash_len;
		uint32_t flags;
		uint16_t major, minor, build, revision, arch;
	};

	static HMODULE Module;
	static Domain* domain;
	static bool IsOldMono;
	static char* LibPath;
	static char* ManagedPath;
	static char* ManagedPathMono;
	static char* ConfigPath;
	static char* ConfigPathMono;
	static char* MonoConfigPathMono;

	static bool Initialize();
	static bool Load();
	static bool SetupPaths();
	static void InstallAssemblyHooks();
	static void CreateDomain(const char* name);
	static void AddInternalCall(const char* name, void* method);
	static String* ExceptionToString(Object* obj);
	static void LogException(Object* exceptionObject, bool shouldThrow = false);
	static void Free(void* ptr);
	static std::string CheckFolderName(std::string folder_name);
	static std::string CheckLibName(std::string base_path, std::string folder_name, std::string lib_name);

	typedef Assembly* (*AssemblyPreLoadFunc) (AssemblyName* aname, char** assemblies_path, void* user_data);
	typedef Assembly* (*AssemblySearchFunc) (AssemblyName* aname, void* user_data);
	typedef void (*AssemblyLoadFunc) (Assembly* assembly, void* user_data);

	typedef enum
	{
		MONO_DEBUG_FORMAT_NONE,
		MONO_DEBUG_FORMAT_MONO,
		/* Deprecated, the mdb debugger is not longer supported. */
		MONO_DEBUG_FORMAT_DEBUGGER
	} MonoDebugFormat;

	typedef enum
	{
		MONO_IMAGE_OK,
		MONO_IMAGE_ERROR_ERRNO,
		MONO_IMAGE_MISSING_ASSEMBLYREF,
		MONO_IMAGE_IMAGE_INVALID
	} MonoImageOpenStatus;

	typedef enum
	{
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
		MONO_TABLE_MEMBERREF,
		/* 0xa */
		MONO_TABLE_CONSTANT,
		MONO_TABLE_CUSTOMATTRIBUTE,
		MONO_TABLE_FIELDMARSHAL,
		MONO_TABLE_DECLSECURITY,
		MONO_TABLE_CLASSLAYOUT,
		MONO_TABLE_FIELDLAYOUT,
		/* 0x10 */
		MONO_TABLE_STANDALONESIG,
		MONO_TABLE_EVENTMAP,
		MONO_TABLE_EVENT_POINTER,
		MONO_TABLE_EVENT,
		MONO_TABLE_PROPERTYMAP,
		MONO_TABLE_PROPERTY_POINTER,
		MONO_TABLE_PROPERTY,
		MONO_TABLE_METHODSEMANTICS,
		MONO_TABLE_METHODIMPL,
		MONO_TABLE_MODULEREF,
		/* 0x1a */
		MONO_TABLE_TYPESPEC,
		MONO_TABLE_IMPLMAP,
		MONO_TABLE_FIELDRVA,
		MONO_TABLE_UNUSED6,
		MONO_TABLE_UNUSED7,
		MONO_TABLE_ASSEMBLY,
		/* 0x20 */
		MONO_TABLE_ASSEMBLYPROCESSOR,
		MONO_TABLE_ASSEMBLYOS,
		MONO_TABLE_ASSEMBLYREF,
		MONO_TABLE_ASSEMBLYREFPROCESSOR,
		MONO_TABLE_ASSEMBLYREFOS,
		MONO_TABLE_FILE,
		MONO_TABLE_EXPORTEDTYPE,
		MONO_TABLE_MANIFESTRESOURCE,
		MONO_TABLE_NESTEDCLASS,
		MONO_TABLE_GENERICPARAM,
		/* 0x2a */
		MONO_TABLE_METHODSPEC,
		MONO_TABLE_GENERICPARAMCONSTRAINT,
		MONO_TABLE_UNUSED8,
		MONO_TABLE_UNUSED9,
		MONO_TABLE_UNUSED10,
		/* Portable PDB tables */
		MONO_TABLE_DOCUMENT,
		/* 0x30 */
		MONO_TABLE_METHODBODY,
		MONO_TABLE_LOCALSCOPE,
		MONO_TABLE_LOCALVARIABLE,
		MONO_TABLE_LOCALCONSTANT,
		MONO_TABLE_IMPORTSCOPE,
		MONO_TABLE_STATEMACHINEMETHOD,
		MONO_TABLE_CUSTOMDEBUGINFORMATION

		#define MONO_TABLE_LAST MONO_TABLE_CUSTOMDEBUGINFORMATION
		#define MONO_TABLE_NUM (MONO_TABLE_LAST + 1)
	} MonoMetaTableEnum;

	enum
	{
		MONO_FIELD_FLAGS,
		MONO_FIELD_NAME,
		MONO_FIELD_SIGNATURE,
		MONO_FIELD_SIZE
	};

	enum
	{
		MONO_METHOD_RVA,
		MONO_METHOD_IMPLFLAGS,
		MONO_METHOD_FLAGS,
		MONO_METHOD_NAME,
		MONO_METHOD_SIGNATURE,
		MONO_METHOD_PARAMLIST,
		MONO_METHOD_SIZE
	};

	enum
	{
		MONO_TYPEDEF_FLAGS,
		MONO_TYPEDEF_NAME,
		MONO_TYPEDEF_NAMESPACE,
		MONO_TYPEDEF_EXTENDS,
		MONO_TYPEDEF_FIELD_LIST,
		MONO_TYPEDEF_METHOD_LIST,
		MONO_TYPEDEF_SIZE
	};

	enum
	{
		MONO_TYPEREF_SCOPE,
		MONO_TYPEREF_NAME,
		MONO_TYPEREF_NAMESPACE,
		MONO_TYPEREF_SIZE
	};

	class Exports
	{
	public:
		static bool Initialize();
		
		#define MONODEF(rt, fn, args) typedef rt (* fn##_t) args; static fn##_t fn;

		MONODEF(Domain*, mono_jit_init, (const char* name))
		MONODEF(Domain*, mono_jit_init_version, (const char* name, const char* version))
		MONODEF(void*, mono_jit_parse_options, (int argc, char* argv[]))
		MONODEF(void, mono_set_assemblies_path, (const char* path))
		MONODEF(void, mono_assembly_setrootdir, (const char* path))
		MONODEF(void, mono_set_config_dir, (const char* path))
		MONODEF(int, mono_runtime_set_main_args, (int argc, char* argv[]))
		MONODEF(Thread*, mono_thread_current, ())
		MONODEF(void, mono_thread_set_main, (Thread* thread))
		MONODEF(void, mono_domain_set_config, (Domain* domain, const char* configpath, const char* filename))
		MONODEF(void, mono_add_internal_call, (const char* name, void* method))
		MONODEF(void*, mono_lookup_internal_call, (Method* method))
		MONODEF(Object*, mono_runtime_invoke, (Method* method, Object* obj, void** params, Object** exec))
		MONODEF(const char*, mono_method_get_name, (Method* method))
		MONODEF(Assembly*, mono_domain_assembly_open, (Domain* domain, const char* path))
		MONODEF(Image*, mono_assembly_get_image, (Assembly* assembly))
		MONODEF(Class*, mono_class_from_name, (Image* image, const char* name_space, const char* name))
		MONODEF(Method*, mono_class_get_method_from_name, (Class* klass, const char* name, int param_count))
		MONODEF(char*, mono_string_to_utf8, (String* str))
		MONODEF(String*, mono_string_new, (Domain* domain, const char* str))
		MONODEF(Class*, mono_object_get_class, (Object* obj))
		MONODEF(String*, mono_object_to_string, (Object* obj, Object** exec))
		MONODEF(Method*, mono_property_get_get_method, (Property* prop))
		MONODEF(void, mono_free, (void* ptr))
		MONODEF(void, g_free, (void* ptr))
		
		MONODEF(void, mono_raise_exception, (Object *ex))
		MONODEF(Object*, mono_get_exception_bad_image_format, (const char *msg))
		MONODEF(const char*, mono_image_get_name, (Image* image))
			
        MONODEF(Image*, mono_image_open_full, (const char *path, MonoImageOpenStatus* status, bool refonly))
        MONODEF(Image*, mono_image_open_from_data_full, (const char *data, unsigned int size, bool need_copy, MonoImageOpenStatus* status, bool refonly))
        MONODEF(void, mono_image_close, (Image* image))
        MONODEF(int, mono_image_get_table_rows, (Image *image, int table_id))
		MONODEF(unsigned int, mono_metadata_decode_table_row_col, (Image *image, int table,int idx, unsigned int col))
		MONODEF(char*, mono_array_addr_with_size, (Object *array, int size, uintptr_t idx))
		MONODEF(uintptr_t, mono_array_length, (Object *array))
		MONODEF(const char*, mono_metadata_string_heap, (Image *meta, unsigned int table_index))
		MONODEF(const char*, mono_class_get_name, (Class *klass))

		MONODEF(void, mono_debug_init, (MonoDebugFormat format))
		MONODEF(void, mono_debug_domain_create, (Domain* domain))

		MONODEF(void, mono_install_assembly_preload_hook, (AssemblyPreLoadFunc func, void* user_data))
		MONODEF(void, mono_install_assembly_search_hook, (AssemblySearchFunc func, void* user_data))
		MONODEF(void, mono_install_assembly_load_hook, (AssemblyLoadFunc func, void* user_data))
		MONODEF(ReflectionAssembly*, mono_assembly_get_object, (Domain* domain, Assembly* assembly))
		
		#undef MONODEF
	};

	class Hooks
	{
	public:
		static Domain* mono_jit_init_version(const char* name, const char* version);
		static Object* mono_runtime_invoke(Method* method, Object* obj, void** params, Object** exec);

		static Assembly* AssemblyPreLoad(AssemblyName* aname, char** assemblies_path, void* user_data);
		static Assembly* AssemblySearch(AssemblyName* aname, void* user_data);
		static Assembly* AssemblyResolve(AssemblyName* aname, void* user_data, bool is_preload);
		static void AssemblyLoad(Assembly* assembly, void* user_data);
	};

private:
	static char* BasePath;
	static const char* LibNames[];
	static const char* FolderNames[];
	static HMODULE PosixHelper;
	static Method* ToStringMethod;

	static void ParseEnvOption(const char* name);
};