#include <string>
#include "Mono.h"
#include "MelonLoader.h"
#include "AssertionManager.h"
#include "PointerUtils.h"
#pragma warning( disable : 4996 )

char* Mono::AssemblyPath = NULL;
char* Mono::ConfigPath = NULL;
HMODULE Mono::Module = NULL;
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
mono_class_enum_basetype_t Mono::mono_class_enum_basetype = NULL;
mono_class_get_name_t Mono::mono_class_get_name = NULL;
mono_type_get_class_t Mono::mono_type_get_class = NULL;
mono_assembly_setrootdir_t Mono::mono_assembly_setrootdir = NULL;
mono_get_corlib_t Mono::mono_get_corlib = NULL;
mono_image_get_assembly_t Mono::mono_image_get_assembly = NULL;
mono_runtime_set_main_args_t Mono::mono_runtime_set_main_args = NULL;
mono_class_init_t Mono::mono_class_init = NULL;
mono_reflection_type_get_type_t Mono::mono_reflection_type_get_type = NULL;
mono_class_from_mono_type_t Mono::mono_class_from_mono_type = NULL;
mono_assembly_get_name_t Mono::mono_assembly_get_name = NULL;
mono_domain_set_config_t Mono::mono_domain_set_config = NULL;
mono_get_root_domain_t Mono::mono_get_root_domain = NULL;
mono_method_get_name_t Mono::mono_method_get_name = NULL;
mono_object_unbox_t Mono::mono_object_unbox = NULL;
mono_method_get_reflection_name_t Mono::mono_method_get_reflection_name = NULL;
mono_method_get_class_t Mono::mono_method_get_class = NULL;
mono_class_get_image_t Mono::mono_class_get_image = NULL;
mono_image_get_name_t Mono::mono_image_get_name = NULL;
mono_method_get_signature_t Mono::mono_method_get_signature = NULL;
mono_method_get_token_t Mono::mono_method_get_token = NULL;
mono_class_get_parent_t Mono::mono_class_get_parent = NULL;
mono_lookup_internal_call_full_t Mono::mono_lookup_internal_call_full = NULL;

bool Mono::Load()
{
	AssertionManager::Start("Mono.cpp", "Mono::Load");
		Module = AssertionManager::LoadLib("Module", (std::string(MelonLoader::GamePath) + "\\MelonLoader\\Mono\\mono-2.0-bdwgc.dll").c_str());
	if (Module)
		HMODULE MonoPosixDLL = AssertionManager::LoadLib("MonoPosixDLL", (std::string(MelonLoader::GamePath) + "\\MelonLoader\\Mono\\MonoPosixHelper.dll").c_str());
	return !AssertionManager::Result;
}

bool Mono::Setup()
{
	AssertionManager::Start("Mono.cpp", "Mono::Setup");

	mono_assembly_setrootdir = (mono_assembly_setrootdir_t)AssertionManager::GetExport(Module, "mono_assembly_setrootdir");
	mono_set_assemblies_path = (mono_set_assemblies_path_t)AssertionManager::GetExport(Module, "mono_set_assemblies_path");
	mono_set_config_dir = (mono_set_config_dir_t)AssertionManager::GetExport(Module, "mono_set_config_dir");
	mono_init = (mono_init_t)AssertionManager::GetExport(Module, "mono_init");
	mono_jit_init = (mono_jit_init_t)AssertionManager::GetExport(Module, "mono_jit_init");
	mono_jit_init_version = (mono_jit_init_version_t)AssertionManager::GetExport(Module, "mono_jit_init_version");
	mono_domain_assembly_open = (mono_domain_assembly_open_t)AssertionManager::GetExport(Module, "mono_domain_assembly_open");
	mono_assembly_get_image = (mono_assembly_get_image_t)AssertionManager::GetExport(Module, "mono_assembly_get_image");
	mono_class_from_name = (mono_class_from_name_t)AssertionManager::GetExport(Module, "mono_class_from_name");
	mono_class_get_method_from_name = (mono_class_get_method_from_name_t)AssertionManager::GetExport(Module, "mono_class_get_method_from_name");
	mono_runtime_invoke = (mono_runtime_invoke_t)AssertionManager::GetExport(Module, "mono_runtime_invoke");
	mono_thread_current = (mono_thread_current_t)AssertionManager::GetExport(Module, "mono_thread_current");
	mono_thread_set_main = (mono_thread_set_main_t)AssertionManager::GetExport(Module, "mono_thread_set_main");
	mono_add_internal_call = (mono_add_internal_call_t)AssertionManager::GetExport(Module, "mono_add_internal_call");
	mono_class_enum_basetype = (mono_class_enum_basetype_t)AssertionManager::GetExport(Module, "mono_class_enum_basetype");
	mono_class_get_name = (mono_class_get_name_t)AssertionManager::GetExport(Module, "mono_class_get_name");
	mono_type_get_class = (mono_type_get_class_t)AssertionManager::GetExport(Module, "mono_type_get_class");
	mono_get_corlib = (mono_get_corlib_t)AssertionManager::GetExport(Module, "mono_get_corlib");
	mono_image_get_assembly = (mono_image_get_assembly_t)AssertionManager::GetExport(Module, "mono_image_get_assembly");
	mono_runtime_set_main_args = (mono_runtime_set_main_args_t)AssertionManager::GetExport(Module, "mono_runtime_set_main_args");
	mono_class_init = (mono_class_init_t)AssertionManager::GetExport(Module, "mono_class_init");
	mono_reflection_type_get_type = (mono_reflection_type_get_type_t)AssertionManager::GetExport(Module, "mono_reflection_type_get_type");
	mono_class_from_mono_type = (mono_class_from_mono_type_t)AssertionManager::GetExport(Module, "mono_class_from_mono_type");
	mono_assembly_get_name = (mono_assembly_get_name_t)AssertionManager::GetExport(Module, "mono_assembly_get_name");
	mono_domain_set_config = (mono_domain_set_config_t)AssertionManager::GetExport(Module, "mono_domain_set_config");
	mono_get_root_domain = (mono_get_root_domain_t)AssertionManager::GetExport(Module, "mono_get_root_domain");
	mono_method_get_name = (mono_method_get_name_t)AssertionManager::GetExport(Module, "mono_method_get_name");
	mono_object_unbox = (mono_object_unbox_t)AssertionManager::GetExport(Module, "mono_object_unbox");
	mono_method_get_reflection_name = (mono_method_get_reflection_name_t)AssertionManager::GetExport(Module, "mono_method_get_reflection_name");
	mono_method_get_class = (mono_method_get_class_t)AssertionManager::GetExport(Module, "mono_method_get_class");
	mono_class_get_image = (mono_class_get_image_t)AssertionManager::GetExport(Module, "mono_class_get_image");
	mono_image_get_name = (mono_image_get_name_t)AssertionManager::GetExport(Module, "mono_image_get_name");
	mono_method_get_signature = (mono_method_get_signature_t)AssertionManager::GetExport(Module, "mono_method_get_signature");
	mono_method_get_token = (mono_method_get_token_t)AssertionManager::GetExport(Module, "mono_method_get_token");
	mono_class_get_parent = (mono_class_get_parent_t)AssertionManager::GetExport(Module, "mono_class_get_parent");

	mono_lookup_internal_call_full = (mono_lookup_internal_call_full_t)AssertionManager::FindPattern(Module, "mono_lookup_internal_call_full", "41 54 41 55 41 56 41 57 48 81 EC ? ? ? ? 48 8B 05 ? ? ? ? 48 33 C4 48 89 84 24 ? ? ? ? 4C 8B F2 4C 8B F9 48 85 C9 75 20 4C 8D 0D ? ? ? ? 41 B8 ? ? ? ? 48 8D 15 ? ? ? ? 48 8D 0D ? ? ? ? E8 ? ? ? ? 41 F7 47 ? ? ? ? ? 74 04 4D 8B 7F 38");

	return !AssertionManager::Result;
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
		mono_thread_set_main(mono_thread_current());
		mono_domain_set_config(Domain, MelonLoader::GamePath, "MelonLoader");
	}
}