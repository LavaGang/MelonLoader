#include <Windows.h>
#include "Mono.h"
#include "MelonLoader.h"
#include "Hooks/Hooks.h"
#include <iostream>
#include <string>
#include <vector>
#pragma warning( disable : 4996 )

char* Mono::AssemblyPath = NULL;
char* Mono::ConfigPath = NULL;
MonoDomain* Mono::Domain = NULL;
mono_set_assemblies_path_t Mono::mono_set_assemblies_path = NULL;
mono_set_config_dir_t Mono::mono_set_config_dir = NULL;
mono_init_t Mono::mono_init = NULL;
mono_jit_init_t Mono::mono_jit_init = NULL;
mono_jit_init_version_t Mono::mono_jit_init_version = NULL;
mono_domain_assembly_open_t Mono::mono_domain_assembly_open = NULL;
mono_assembly_get_image_t Mono::mono_assembly_get_image = NULL;
mono_class_from_name_t Mono::mono_class_from_name = NULL;
mono_class_get_method_from_name_t Mono::mono_class_get_method_from_name = NULL;
mono_runtime_invoke_t Mono::mono_runtime_invoke = NULL;
mono_thread_current_t Mono::mono_thread_current = NULL;
mono_thread_set_main_t Mono::mono_thread_set_main = NULL;
mono_add_internal_call_t Mono::mono_add_internal_call = NULL;
mono_resolve_icall_t Mono::mono_resolve_icall = NULL;
mono_class_enum_basetype_t Mono::mono_class_enum_basetype = NULL;
mono_class_get_name_t Mono::mono_class_get_name = NULL;
mono_type_get_class_t Mono::mono_type_get_class = NULL;
mono_assembly_setrootdir_t Mono::mono_assembly_setrootdir = NULL;
mono_get_corlib_t Mono::mono_get_corlib = NULL;
mono_image_get_assembly_t Mono::mono_image_get_assembly = NULL;
mono_runtime_set_main_args_t Mono::mono_runtime_set_main_args = NULL;

void Mono::Setup()
{
	mono_assembly_setrootdir = (mono_assembly_setrootdir_t)GetProcAddress(MelonLoader::MonoDLL, "mono_assembly_setrootdir");
	mono_set_assemblies_path = (mono_set_assemblies_path_t)GetProcAddress(MelonLoader::MonoDLL, "mono_set_assemblies_path");
	mono_set_config_dir = (mono_set_config_dir_t)GetProcAddress(MelonLoader::MonoDLL, "mono_set_config_dir");
	mono_init = (mono_init_t)GetProcAddress(MelonLoader::MonoDLL, "mono_init");
	mono_jit_init = (mono_jit_init_t)GetProcAddress(MelonLoader::MonoDLL, "mono_jit_init");
	mono_jit_init_version = (mono_jit_init_version_t)GetProcAddress(MelonLoader::MonoDLL, "mono_jit_init_version");
	mono_domain_assembly_open = (mono_domain_assembly_open_t)GetProcAddress(MelonLoader::MonoDLL, "mono_domain_assembly_open");
	mono_assembly_get_image = (mono_assembly_get_image_t)GetProcAddress(MelonLoader::MonoDLL, "mono_assembly_get_image");
	mono_class_from_name = (mono_class_from_name_t)GetProcAddress(MelonLoader::MonoDLL, "mono_class_from_name");
	mono_class_get_method_from_name = (mono_class_get_method_from_name_t)GetProcAddress(MelonLoader::MonoDLL, "mono_class_get_method_from_name");
	mono_runtime_invoke = (mono_runtime_invoke_t)GetProcAddress(MelonLoader::MonoDLL, "mono_runtime_invoke");
	mono_thread_current = (mono_thread_current_t)GetProcAddress(MelonLoader::MonoDLL, "mono_thread_current");
	mono_thread_set_main = (mono_thread_set_main_t)GetProcAddress(MelonLoader::MonoDLL, "mono_thread_set_main");
	mono_add_internal_call = (mono_add_internal_call_t)GetProcAddress(MelonLoader::MonoDLL, "mono_add_internal_call");
	mono_resolve_icall = (mono_resolve_icall_t)GetProcAddress(MelonLoader::MonoDLL, "mono_resolve_icall");
	mono_class_enum_basetype = (mono_class_enum_basetype_t)GetProcAddress(MelonLoader::MonoDLL, "mono_class_enum_basetype");
	mono_class_get_name = (mono_class_get_name_t)GetProcAddress(MelonLoader::MonoDLL, "mono_class_get_name");
	mono_type_get_class = (mono_type_get_class_t)GetProcAddress(MelonLoader::MonoDLL, "mono_type_get_class");
	mono_get_corlib = (mono_get_corlib_t)GetProcAddress(MelonLoader::MonoDLL, "mono_get_corlib");
	mono_image_get_assembly = (mono_image_get_assembly_t)GetProcAddress(MelonLoader::MonoDLL, "mono_image_get_assembly");
	mono_runtime_set_main_args = (mono_runtime_set_main_args_t)GetProcAddress(MelonLoader::MonoDLL, "mono_runtime_set_main_args");
}

void Mono::CreateDomain()
{
	if (Domain == NULL)
	{
		mono_set_assemblies_path(AssemblyPath);
		mono_assembly_setrootdir(AssemblyPath);
		mono_set_config_dir(ConfigPath);

		int argc = 0;
		char* argv[64];
		char* p2 = strtok(GetCommandLine(), " ");
		while (p2 && argc < 63)
		{
			argv[argc++] = p2;
			p2 = strtok(0, " ");
		}
		argv[argc] = 0;
		mono_runtime_set_main_args(argc, argv);

		Domain = mono_jit_init("MelonLoader");
	}
}