#pragma once
struct MonoDomain;
struct MonoAssembly;
struct MonoImage;
struct MonoMethod;
struct MonoClass;
struct MonoObject;
struct MonoThread;

typedef void (*mono_set_assemblies_path_t) (const char* path);
typedef void (*mono_set_config_dir_t) (const char* path);
typedef void (*mono_assembly_setrootdir_t) (const char* path);
typedef MonoDomain* (*mono_init_t) (const char* name);
typedef MonoDomain* (*mono_jit_init_t) (const char* name);
typedef MonoDomain* (*mono_jit_init_version_t) (const char* name, const char* version);
typedef MonoAssembly* (*mono_domain_assembly_open_t) (MonoDomain* domain, const char* path);
typedef MonoImage* (*mono_assembly_get_image_t) (MonoAssembly* assembly);
typedef MonoClass* (*mono_class_from_name_t) (MonoImage* image, const char* name_space, const char* name);
typedef MonoMethod* (*mono_class_get_method_from_name_t) (MonoClass* klass, const char* name, int param_count);
typedef MonoObject* (*mono_runtime_invoke_t) (MonoMethod* method, void* obj, void** params, MonoObject** exec);
typedef MonoThread* (*mono_thread_current_t)();
typedef void (*mono_thread_set_main_t)(MonoThread* thread);
typedef void (*mono_add_internal_call_t) (const char* name, void* method);
typedef void* (*mono_resolve_icall_t) (const char* name);
typedef void* (*mono_class_enum_basetype_t) (void* klass);
typedef const char* (*mono_class_get_name_t) (void* klass);
typedef void* (*mono_type_get_class_t) (void* type);
typedef void* (*mono_get_corlib_t) ();
typedef void* (*mono_image_get_assembly_t) (void* image);
typedef int (*mono_runtime_set_main_args_t) (int argc, char* argv[]);
class Mono
{
public:
	static char* AssemblyPath;
	static char* ConfigPath;
	static MonoDomain* Domain;
	static mono_set_assemblies_path_t mono_set_assemblies_path;
	static mono_set_config_dir_t mono_set_config_dir;
	static mono_assembly_setrootdir_t mono_assembly_setrootdir;
	static mono_init_t mono_init;
	static mono_jit_init_t mono_jit_init;
	static mono_jit_init_version_t mono_jit_init_version;
	static mono_domain_assembly_open_t mono_domain_assembly_open;
	static mono_assembly_get_image_t mono_assembly_get_image;
	static mono_class_from_name_t mono_class_from_name;
	static mono_class_get_method_from_name_t mono_class_get_method_from_name;
	static mono_runtime_invoke_t mono_runtime_invoke;
	static mono_thread_current_t mono_thread_current;
	static mono_thread_set_main_t mono_thread_set_main;
	static mono_add_internal_call_t mono_add_internal_call;
	static mono_resolve_icall_t mono_resolve_icall;
	static mono_class_enum_basetype_t mono_class_enum_basetype;
	static mono_class_get_name_t mono_class_get_name;
	static mono_type_get_class_t mono_type_get_class;
	static mono_get_corlib_t mono_get_corlib;
	static mono_image_get_assembly_t mono_image_get_assembly;
	static mono_runtime_set_main_args_t mono_runtime_set_main_args;

	static void Setup();
	static void CreateDomain();
};
