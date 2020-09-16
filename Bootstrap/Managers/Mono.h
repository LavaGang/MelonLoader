#pragma once
#include <Windows.h>


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
	struct Object;
	struct String;

	static HMODULE Module;
	static Domain* domain;
	static bool IsOldMono;
	static char* ManagedPath;
	static char* ConfigPath;
	static bool Initialize();
	static bool Load();
	static bool SetupPaths();
	static void CreateDomain(const char* name);
	static void AddInternalCall(const char* name, void* method);
	static void LogException(Object* exceptionObject, bool shouldThrow = false);

	class Exports
	{
	public:
		static bool Initialize();
		typedef Domain* (*mono_jit_init_t) (const char* name);
		static mono_jit_init_t mono_jit_init;
		typedef Domain* (*mono_jit_init_version_t) (const char* name, const char* version);
		static mono_jit_init_version_t mono_jit_init_version;
		typedef void (*mono_set_assemblies_path_t) (const char* path);
		static mono_set_assemblies_path_t mono_set_assemblies_path;
		typedef void (*mono_assembly_setrootdir_t) (const char* path);
		static mono_assembly_setrootdir_t mono_assembly_setrootdir;
		typedef void (*mono_set_config_dir_t) (const char* path);
		static mono_set_config_dir_t mono_set_config_dir;
		typedef int (*mono_runtime_set_main_args_t) (int argc, char* argv[]);
		static mono_runtime_set_main_args_t mono_runtime_set_main_args;
		typedef Thread* (*mono_thread_current_t) ();
		static mono_thread_current_t mono_thread_current;
		typedef void (*mono_thread_set_main_t) (Thread* thread);
		static mono_thread_set_main_t mono_thread_set_main;
		typedef void (*mono_domain_set_config_t) (Domain* domain, const char* configpath, const char* filename);
		static mono_domain_set_config_t mono_domain_set_config;
		typedef void (*mono_add_internal_call_t) (const char* name, void* method);
		static mono_add_internal_call_t mono_add_internal_call;
		typedef Object* (*mono_runtime_invoke_t) (Method* method, Object* obj, void** params, Object** exec);
		static mono_runtime_invoke_t mono_runtime_invoke;
		typedef const char* (*mono_method_get_name_t) (Method* method);
		static mono_method_get_name_t mono_method_get_name;
		typedef void* (*mono_unity_get_unitytls_interface_t) ();
		static mono_unity_get_unitytls_interface_t mono_unity_get_unitytls_interface;
		typedef Assembly* (*mono_domain_assembly_open_t) (Domain* domain, const char* path);
		static mono_domain_assembly_open_t mono_domain_assembly_open;
		typedef Image* (*mono_assembly_get_image_t) (Assembly* assembly);
		static mono_assembly_get_image_t mono_assembly_get_image;
		typedef Class* (*mono_class_from_name_t) (Image* image, const char* name_space, const char* name);
		static mono_class_from_name_t mono_class_from_name;
		typedef Method* (*mono_class_get_method_from_name_t) (Class* klass, const char* name, int param_count);
		static mono_class_get_method_from_name_t mono_class_get_method_from_name;
		typedef const char* (*mono_string_to_utf8_t) (String* str);
		static mono_string_to_utf8_t mono_string_to_utf8;
		typedef String* (*mono_string_new_t) (Domain* domain, const char* str);
		static mono_string_new_t mono_string_new;
		typedef Class* (*mono_object_get_class_t) (Object* obj);
		static mono_object_get_class_t mono_object_get_class;
		typedef Property* (*mono_class_get_property_from_name_t) (Class* klass, const char* name);
		static mono_class_get_property_from_name_t mono_class_get_property_from_name;
		typedef Method* (*mono_property_get_get_method_t) (Property* prop);
		static mono_property_get_get_method_t mono_property_get_get_method;
	};

	class Hooks
	{
	public:
		static Domain* mono_jit_init_version(const char* name, const char* version);
		static Object* mono_runtime_invoke(Method* method, Object* obj, void** params, Object** exec);
		static void* mono_unity_get_unitytls_interface();
	};

private:
	static char* BasePath;
	static const char* LibNames[];
	static const char* FolderNames[];
	static HMODULE PosixHelper;
};