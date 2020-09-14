#include <string>
#include "Mono.h"
#include "Game.h"
#include "Hook.h"
#include "..\Utils\Assertion.h"
#include "..\Utils\CommandLine.h"
#include "..\Utils\Debug.h"
#include "..\Base\Core.h"
#include "InternalCalls.h"
#include "BaseAssembly.h"
#include "Il2Cpp.h"
#include "../Utils/Logger.h"

const char* Mono::LibNames[] = { "mono", "mono-2.0-bdwgc", "mono-2.0-sgen", "mono-2.0-boehm" };
const char* Mono::FolderNames[] = { "Mono", "MonoBleedingEdge" };
char* Mono::BasePath = NULL;
char* Mono::ManagedPath = NULL;
char* Mono::ConfigPath = NULL;
HMODULE Mono::Module = NULL;
HMODULE Mono::PosixHelper = NULL;
Mono::Domain* Mono::domain = NULL;
bool Mono::IsOldMono = false;
Mono::Exports::mono_jit_init_t Mono::Exports::mono_jit_init = NULL;
Mono::Exports::mono_jit_init_version_t Mono::Exports::mono_jit_init_version = NULL;
Mono::Exports::mono_set_assemblies_path_t Mono::Exports::mono_set_assemblies_path = NULL;
Mono::Exports::mono_assembly_setrootdir_t Mono::Exports::mono_assembly_setrootdir = NULL;
Mono::Exports::mono_set_config_dir_t Mono::Exports::mono_set_config_dir = NULL;
Mono::Exports::mono_runtime_set_main_args_t Mono::Exports::mono_runtime_set_main_args = NULL;
Mono::Exports::mono_thread_set_main_t Mono::Exports::mono_thread_set_main = NULL;
Mono::Exports::mono_thread_current_t Mono::Exports::mono_thread_current = NULL;
Mono::Exports::mono_domain_set_config_t Mono::Exports::mono_domain_set_config = NULL;
Mono::Exports::mono_add_internal_call_t Mono::Exports::mono_add_internal_call = NULL;
Mono::Exports::mono_runtime_invoke_t Mono::Exports::mono_runtime_invoke = NULL;
Mono::Exports::mono_method_get_name_t Mono::Exports::mono_method_get_name = NULL;
Mono::Exports::mono_unity_get_unitytls_interface_t Mono::Exports::mono_unity_get_unitytls_interface = NULL;
Mono::Exports::mono_domain_assembly_open_t Mono::Exports::mono_domain_assembly_open = NULL;
Mono::Exports::mono_assembly_get_image_t Mono::Exports::mono_assembly_get_image = NULL;
Mono::Exports::mono_class_from_name_t Mono::Exports::mono_class_from_name = NULL;
Mono::Exports::mono_class_get_method_from_name_t Mono::Exports::mono_class_get_method_from_name = NULL;
Mono::Exports::mono_string_to_utf8_t Mono::Exports::mono_string_to_utf8 = NULL;
Mono::Exports::mono_string_new_t Mono::Exports::mono_string_new = NULL;
Mono::Exports::mono_object_get_class_t Mono::Exports::mono_object_get_class = NULL;
Mono::Exports::mono_class_get_property_from_name_t Mono::Exports::mono_class_get_property_from_name = NULL;
Mono::Exports::mono_property_get_get_method_t Mono::Exports::mono_property_get_get_method = NULL;

bool Mono::Initialize()
{
	Debug::Msg("Initializing Mono...");
	if (!SetupPaths())
	{
		Assertion::ThrowInternalFailure("Failed to Setup Mono Paths!");
		return false;
	}
	Debug::Msg(("Mono::BasePath = " + std::string(BasePath)).c_str());
	Debug::Msg(("Mono::ManagedPath = " + std::string(ManagedPath)).c_str());
	Debug::Msg(("Mono::ConfigPath = " + std::string(ConfigPath)).c_str());
	IsOldMono = Core::FileExists((std::string(BasePath) + "\\mono.dll").c_str());
	Debug::Msg(("Mono::IsOldMono = " + std::string((IsOldMono ? "true" : "false"))).c_str());
	return true;
}

bool Mono::Load()
{
	for (int i = 0; i < (sizeof(LibNames) / sizeof(LibNames[0])); i++)
	{
		Module = LoadLibraryA((std::string(BasePath) + "\\" + LibNames[i] + ".dll").c_str());
		if (Module != NULL)
		{
			if (i == 0)
				IsOldMono = true;
			break;
		}
	}
	if (Module == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Load Mono Library!");
		return false;
	}

	PosixHelper = LoadLibraryA((std::string(BasePath) + "\\MonoPosixHelper.dll").c_str());
	if (PosixHelper == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Load Mono Posix Helper!");
		return false;
	}
	return Exports::Initialize();
}

bool Mono::SetupPaths()
{
	std::string MonoDir = std::string();
	for (int i = 0; i < (sizeof(FolderNames) / sizeof(FolderNames[0])); i++)
	{
		std::string str_base = (std::string(Game::BasePath) + "\\" + FolderNames[i]);
		std::string str_data = (std::string(Game::DataPath) + "\\" + FolderNames[i]);
		if (Game::IsIl2Cpp)
		{
			std::string str_melon = (std::string(Game::BasePath) + "\\MelonLoader\\Dependencies\\" + FolderNames[i]);
			if (Core::DirectoryExists(str_melon.c_str()))
			{
				MonoDir = str_melon;
				break;
			}
		}
		else
		{
			if (Core::DirectoryExists(str_base.c_str()))
			{
				MonoDir = str_base;
				break;
			}
			else if (Core::DirectoryExists(str_data.c_str()))
			{
				MonoDir = str_data;
				break;
			}
		}
	}
	if (MonoDir.empty())
	{
		Assertion::ThrowInternalFailure("Failed to Find Mono Directory!");
		return false;
	}

	if (Game::IsIl2Cpp)
	{
		BasePath = new char[MonoDir.size() + 1];
		std::copy(MonoDir.begin(), MonoDir.end(), BasePath);
		BasePath[MonoDir.size()] = '\0';

		std::string ManagedPathStr = (std::string(Game::BasePath) + "\\MelonLoader\\Managed");
		ManagedPath = new char[ManagedPathStr.size() + 1];
		std::copy(ManagedPathStr.begin(), ManagedPathStr.end(), ManagedPath);
		ManagedPath[ManagedPathStr.size()] = '\0';

		std::string ConfigPathStr = (std::string(Game::DataPath) + "\\il2cpp_data\\etc");
		ConfigPath = new char[ConfigPathStr.size() + 1];
		std::copy(ConfigPathStr.begin(), ConfigPathStr.end(), ConfigPath);
		ConfigPath[ConfigPathStr.size()] = '\0';

		return true;
	}

	std::string BasePathStr = (MonoDir + "\\EmbedRuntime");
	BasePath = new char[BasePathStr.size() + 1];
	std::copy(BasePathStr.begin(), BasePathStr.end(), BasePath);
	BasePath[BasePathStr.size()] = '\0';

	std::string ManagedPathStr = (std::string(Game::DataPath) + "\\Managed");
	ManagedPath = new char[ManagedPathStr.size() + 1];
	std::copy(ManagedPathStr.begin(), ManagedPathStr.end(), ManagedPath);
	ManagedPath[ManagedPathStr.size()] = '\0';

	std::string ConfigPathStr = (MonoDir + "\\etc");
	ConfigPath = new char[ConfigPathStr.size() + 1];
	std::copy(ConfigPathStr.begin(), ConfigPathStr.end(), ConfigPath);
	ConfigPath[ConfigPathStr.size()] = '\0';

	return true;
}

void Mono::CreateDomain(const char* name)
{
	if (domain != NULL)
		return;
	Debug::Msg("Creating Mono Domain...");
	Exports::mono_set_assemblies_path(ManagedPath);
	Exports::mono_assembly_setrootdir(ManagedPath);
	Exports::mono_set_config_dir(ConfigPath);
	if (!IsOldMono)
		Exports::mono_runtime_set_main_args(CommandLine::argc, CommandLine::argv);
	domain = Exports::mono_jit_init(name);
	Exports::mono_thread_set_main(Exports::mono_thread_current());
	//if (!IsOldMono)
	//	Exports::mono_domain_set_config(domain, Game::BasePath, name);
}

void Mono::AddInternalCall(const char* name, void* method)
{
	Debug::Msg(name);
	Exports::mono_add_internal_call(name, method);
}

bool Mono::Exports::Initialize()
{
	Debug::Msg("Initializing Mono Exports...");

	mono_jit_init = (mono_jit_init_t)Assertion::GetExport(Module, "mono_jit_init");
	mono_thread_set_main = (mono_thread_set_main_t)Assertion::GetExport(Module, "mono_thread_set_main");
	mono_thread_current = (mono_thread_current_t)Assertion::GetExport(Module, "mono_thread_current");
	mono_add_internal_call = (mono_add_internal_call_t)Assertion::GetExport(Module, "mono_add_internal_call");
	mono_runtime_invoke = (mono_runtime_invoke_t)Assertion::GetExport(Module, "mono_runtime_invoke");
	mono_method_get_name = (mono_method_get_name_t)Assertion::GetExport(Module, "mono_method_get_name");
	mono_domain_assembly_open = (mono_domain_assembly_open_t)Assertion::GetExport(Module, "mono_domain_assembly_open");
	mono_assembly_get_image = (mono_assembly_get_image_t)Assertion::GetExport(Module, "mono_assembly_get_image");
	mono_class_from_name = (mono_class_from_name_t)Assertion::GetExport(Module, "mono_class_from_name");
	mono_class_get_method_from_name = (mono_class_get_method_from_name_t)Assertion::GetExport(Module, "mono_class_get_method_from_name");
	mono_string_to_utf8 = (mono_string_to_utf8_t)Assertion::GetExport(Module, "mono_string_to_utf8");
	mono_string_new = (mono_string_new_t)Assertion::GetExport(Module, "mono_string_new");
	mono_object_get_class = (mono_object_get_class_t)Assertion::GetExport(Module, "mono_object_get_class");
	mono_class_get_property_from_name = (mono_class_get_property_from_name_t)Assertion::GetExport(Module, "mono_class_get_property_from_name");
	mono_property_get_get_method = (mono_property_get_get_method_t)Assertion::GetExport(Module, "mono_property_get_get_method");

	if (!IsOldMono)
	{
		mono_domain_set_config = (mono_domain_set_config_t)Assertion::GetExport(Module, "mono_domain_set_config");
		mono_unity_get_unitytls_interface = (mono_unity_get_unitytls_interface_t)Assertion::GetExport(Module, "mono_unity_get_unitytls_interface");
	}

	if (Game::IsIl2Cpp) 
	{
		mono_set_assemblies_path = (mono_set_assemblies_path_t)Assertion::GetExport(Module, "mono_set_assemblies_path");
		mono_assembly_setrootdir = (mono_assembly_setrootdir_t)Assertion::GetExport(Module, "mono_assembly_setrootdir");
		mono_set_config_dir = (mono_set_config_dir_t)Assertion::GetExport(Module, "mono_set_config_dir");
		if (!IsOldMono)
			mono_runtime_set_main_args = (mono_runtime_set_main_args_t)Assertion::GetExport(Module, "mono_runtime_set_main_args");
	}
	else
		mono_jit_init_version = (mono_jit_init_version_t)Assertion::GetExport(Module, "mono_jit_init_version");

	return Assertion::ShouldContinue;
}

void Mono::LogException(Mono::Object* exceptionObject, bool shouldThrow)
{
	if (exceptionObject == NULL)
		return;
	Class* klass = Exports::mono_object_get_class(exceptionObject);
	if (klass == NULL)
		return;
	Property* prop = Exports::mono_class_get_property_from_name(klass, "Message");
	if (prop == NULL)
		return;
	Method* method = Exports::mono_property_get_get_method(prop);
	if (method == NULL)
		return;
	String* returnstr = (String*)Exports::mono_runtime_invoke(method, exceptionObject, NULL, NULL);
	if (returnstr == NULL)
		return;
	const char* returnstrc = Exports::mono_string_to_utf8(returnstr);
	if (returnstrc == NULL)
		return;
	Logger::Error(returnstrc);
}

Mono::Domain* Mono::Hooks::mono_jit_init_version(const char* name, const char* version)
{
	Debug::Msg("Detaching Hook from mono_jit_init_version...");
	Hook::Detach(&(LPVOID&)Exports::mono_jit_init_version, mono_jit_init_version);
	Debug::Msg("Creating Mono Domain...");
	domain = Exports::mono_jit_init_version(name, version);
	Exports::mono_thread_set_main(Exports::mono_thread_current());
	if (!IsOldMono)
		Exports::mono_domain_set_config(domain, Game::BasePath, name);
	InternalCalls::Initialize();
	if (BaseAssembly::Initialize())
	{
		Debug::Msg("Attaching Hook to mono_runtime_invoke...");
		Hook::Attach(&(LPVOID&)Exports::mono_runtime_invoke, mono_runtime_invoke);
	}
	return domain;
}

Mono::Object* Mono::Hooks::mono_runtime_invoke(Method* method, Object* obj, void** params, Object** exec)
{
	const char* method_name = Exports::mono_method_get_name(method);
	if ((strstr(method_name, "Internal_ActiveSceneChanged") != NULL) || (strstr(method_name, "UnityEngine.ISerializationCallbackReceiver.OnAfterDeserialize") != NULL))
	{
		Debug::Msg("Detaching Hook from mono_runtime_invoke...");
		Hook::Detach(&(LPVOID&)Exports::mono_runtime_invoke, mono_runtime_invoke);
		BaseAssembly::Start();
	}
	return Exports::mono_runtime_invoke(method, obj, params, exec);
}

void* Mono::Hooks::mono_unity_get_unitytls_interface() { return Il2Cpp::UnityTLSInterfaceStruct; }