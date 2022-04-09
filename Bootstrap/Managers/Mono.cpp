#include <string>
#include "Mono.h"
#include "Game.h"
#include "Hook.h"
#include "..\Utils\Assertion.h"
#include "..\Utils\CommandLine.h"
#include "..\Utils\Debug.h"
#include "..\Utils\Encoding.h"
#include "..\Core.h"
#include "InternalCalls.h"
#include "BaseAssembly.h"
#include "Il2Cpp.h"
#include "../Utils/Logging/Logger.h"
#include "../Utils/Il2CppAssemblyGenerator.h"

const char* Mono::LibNames[] = { "mono", "mono-2.0-bdwgc", "mono-2.0-sgen", "mono-2.0-boehm" };
const char* Mono::FolderNames[] = { "Mono", "MonoBleedingEdge", "MonoBleedingEdge.x86", "MonoBleedingEdge.x64" };
char* Mono::LibPath = NULL;
char* Mono::BasePath = NULL;
char* Mono::ManagedPath = NULL;
char* Mono::ManagedPathMono = NULL;
char* Mono::ConfigPath = NULL;
char* Mono::ConfigPathMono = NULL;
char* Mono::MonoConfigPathMono = NULL;
HMODULE Mono::Module = NULL;
HMODULE Mono::PosixHelper = NULL;
Mono::Domain* Mono::domain = NULL;
bool Mono::IsOldMono = false;
Mono::Method* Mono::ToStringMethod = NULL;
Mono::Method* Mono::Mono_PreStart = NULL;
Mono::Method* Mono::Mono_Start = NULL;
Mono::Method* Mono::AssemblyManager_Resolve = NULL;
Mono::Method* Mono::AssemblyManager_LoadInfo = NULL;

#define MONODEF(fn) Mono::Exports::fn##_t Mono::Exports::fn = NULL;

MONODEF(mono_jit_init)
MONODEF(mono_jit_init_version)
MONODEF(mono_jit_parse_options)
MONODEF(mono_set_assemblies_path)
MONODEF(mono_assembly_setrootdir)
MONODEF(mono_set_config_dir)
MONODEF(mono_runtime_set_main_args)
MONODEF(mono_thread_set_main)
MONODEF(mono_thread_current)
MONODEF(mono_domain_set_config)
MONODEF(mono_add_internal_call)
MONODEF(mono_lookup_internal_call)
MONODEF(mono_runtime_invoke)
MONODEF(mono_method_get_name)
MONODEF(mono_domain_assembly_open)
MONODEF(mono_assembly_get_image)
MONODEF(mono_class_from_name)
MONODEF(mono_class_get_method_from_name)
MONODEF(mono_string_to_utf8)
MONODEF(mono_string_new)
MONODEF(mono_object_get_class)
MONODEF(mono_object_to_string)
MONODEF(mono_property_get_get_method)
MONODEF(mono_free)
MONODEF(g_free)

MONODEF(mono_raise_exception)
MONODEF(mono_get_exception_bad_image_format)
MONODEF(mono_image_get_name);
MONODEF(mono_image_open_full)
MONODEF(mono_image_open_from_data_full)
MONODEF(mono_image_close)
MONODEF(mono_image_get_table_rows)
MONODEF(mono_metadata_decode_table_row_col)
MONODEF(mono_array_addr_with_size)
MONODEF(mono_array_length)
MONODEF(mono_metadata_string_heap)
MONODEF(mono_class_get_name)

MONODEF(mono_debug_init)
MONODEF(mono_debug_domain_create)

MONODEF(mono_install_assembly_preload_hook)
MONODEF(mono_install_assembly_search_hook)
MONODEF(mono_install_assembly_load_hook)
MONODEF(mono_assembly_get_object)

#undef MONODEF

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
	Debug::Msg("Loading Mono Library...");
	Module = LoadLibraryA(LibPath);
	if (Module == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Load Mono Library!");
		return false;
	}

	PosixHelper = LoadLibraryA((std::string(BasePath) + "\\MonoPosixHelper.dll").c_str());
	if ((PosixHelper == NULL) && !IsOldMono)
	{
		Assertion::ThrowInternalFailure("Failed to Load Mono Posix Helper!");
		return false;
	}
	return Exports::Initialize();
}

std::string Mono::CheckLibName(std::string base_path, std::string folder_name, std::string lib_name)
{
	std::string new_path = base_path + "\\" + folder_name;
	if (!Core::DirectoryExists(new_path.c_str()))
		return std::string();

	std::string lib_path = new_path + "\\" + lib_name + ".dll";
	if (!Core::FileExists(lib_path.c_str()))
		return std::string();

	return lib_path;
}

std::string Mono::CheckFolderName(std::string folder_name)
{
	std::string MonoLibPath = std::string();

	for (int z = 0; z < (sizeof(LibNames) / sizeof(LibNames[0])); z++)
	{
		if (Game::IsIl2Cpp)
		{
			MonoLibPath = CheckLibName(Core::BasePath, ("MelonLoader\\Dependencies\\" + folder_name), LibNames[z]);
			if (!MonoLibPath.empty())
				break;
		}
		else
		{
			MonoLibPath = CheckLibName(Game::BasePath, folder_name, LibNames[z]);
			if (!MonoLibPath.empty())
				break;
			MonoLibPath = CheckLibName(Game::BasePath, (folder_name + "\\EmbedRuntime"), LibNames[z]);
			if (!MonoLibPath.empty())
				break;

			MonoLibPath = CheckLibName(Game::DataPath, folder_name, LibNames[z]);
			if (!MonoLibPath.empty())
				break;
			MonoLibPath = CheckLibName(Game::DataPath, (folder_name + "\\EmbedRuntime"), LibNames[z]);
			if (!MonoLibPath.empty())
				break;
		}
	}

	return MonoLibPath;
}

bool Mono::SetupPaths()
{
	std::string MonoLibPath = std::string();
	for (int i = 0; i < (sizeof(FolderNames) / sizeof(FolderNames[0])); i++)
	{
		MonoLibPath = CheckFolderName(FolderNames[i]);
		if (!MonoLibPath.empty())
			break;
	}
	if (MonoLibPath.empty())
	{
		Assertion::ThrowInternalFailure("Failed to Find Mono Library!");
		return false;
	}

#define MONO_STR(s) ((s ## Mono) = Encoding::OsToUtf8((s)))

	LibPath = new char[MonoLibPath.size() + 1];
	std::copy(MonoLibPath.begin(), MonoLibPath.end(), LibPath);
	LibPath[MonoLibPath.size()] = '\0';

	std::string MonoDir = MonoLibPath.substr(0, MonoLibPath.find_last_of("\\/"));
	BasePath = new char[MonoDir.size() + 1];
	std::copy(MonoDir.begin(), MonoDir.end(), BasePath);
	BasePath[MonoDir.size()] = '\0';

	if (Game::IsIl2Cpp)
	{
		std::string ManagedPathStr = (std::string(Core::BasePath) + "\\MelonLoader\\Managed");
		ManagedPath = new char[ManagedPathStr.size() + 1];
		std::copy(ManagedPathStr.begin(), ManagedPathStr.end(), ManagedPath);
		ManagedPath[ManagedPathStr.size()] = '\0';

		std::string ConfigPathStr = (std::string(Game::DataPath) + "\\il2cpp_data\\etc");
		ConfigPath = new char[ConfigPathStr.size() + 1];
		std::copy(ConfigPathStr.begin(), ConfigPathStr.end(), ConfigPath);
		ConfigPath[ConfigPathStr.size()] = '\0';

		std::string MonoConfigPathStr = (MonoDir + "\\etc");
		MonoConfigPathMono = new char[MonoConfigPathStr.size() + 1];
		std::copy(MonoConfigPathStr.begin(), MonoConfigPathStr.end(), MonoConfigPathMono);
		MonoConfigPathMono[MonoConfigPathStr.size()] = '\0';

		MonoConfigPathMono = Encoding::OsToUtf8(MonoConfigPathMono);

		MONO_STR(ManagedPath);
		MONO_STR(ConfigPath);

		return true;
	}

	std::string ManagedPathStr = (std::string(Game::DataPath) + "\\Managed");
	ManagedPath = new char[ManagedPathStr.size() + 1];
	std::copy(ManagedPathStr.begin(), ManagedPathStr.end(), ManagedPath);
	ManagedPath[ManagedPathStr.size()] = '\0';

	std::string ConfigPathStr = MonoDir;
	if (strstr(ConfigPathStr.c_str(), "EmbedRuntime"))
		ConfigPathStr = ConfigPathStr.substr(0, ConfigPathStr.find_last_of("\\/"));
	ConfigPathStr = (ConfigPathStr + "\\etc");
	ConfigPath = new char[ConfigPathStr.size() + 1];
	std::copy(ConfigPathStr.begin(), ConfigPathStr.end(), ConfigPath);
	ConfigPath[ConfigPathStr.size()] = '\0';

	MonoConfigPathMono = Encoding::OsToUtf8(ConfigPath);

	MONO_STR(ManagedPath);
	MONO_STR(ConfigPath);

#undef MONO_STR
	return true;
}

void Mono::InstallAssemblyHooks()
{
	Exports::mono_install_assembly_preload_hook(Hooks::AssemblyPreLoad, NULL);
	Exports::mono_install_assembly_search_hook(Hooks::AssemblySearch, NULL);
	Exports::mono_install_assembly_load_hook(Hooks::AssemblyLoad, NULL);
}

void Mono::CreateDomain(const char* name)
{
	if (domain != NULL)
		return;
	Debug::Msg("Creating Mono Domain...");

	Exports::mono_set_assemblies_path(ManagedPathMono);
	Exports::mono_assembly_setrootdir(ManagedPathMono);
	Exports::mono_set_config_dir(MonoConfigPathMono);

	if (!IsOldMono)
		Exports::mono_runtime_set_main_args(CommandLine::argc, CommandLine::argvMono);

	if (Debug::Enabled)
	{
		if (IsOldMono)
			Mono::ParseEnvOption("DNSPY_UNITY_DBG");
		else
			Mono::ParseEnvOption("DNSPY_UNITY_DBG2");
		Exports::mono_debug_init(MONO_DEBUG_FORMAT_MONO);
	}

	domain = Exports::mono_jit_init(name);

	if (Debug::Enabled && (Exports::mono_debug_domain_create != NULL))
		Exports::mono_debug_domain_create(domain);

	Exports::mono_thread_set_main(Exports::mono_thread_current());
	if (!IsOldMono)
	{
		Debug::Msg("Setting Mono Domain Config...");
		Exports::mono_domain_set_config(domain, Game::BasePathMono, name);
	}
}

void Mono::AddInternalCall(const char* name, void* method)
{
	Debug::Msg(name);
	Exports::mono_add_internal_call(name, method);
}

void Mono::Free(void* ptr)
{
	if (IsOldMono)
		Exports::g_free(ptr);
	else
		Exports::mono_free(ptr);
}

bool Mono::Exports::Initialize()
{
	Debug::Msg("Initializing Mono Exports...");

	#define MONODEF(fn) fn = (fn##_t) Assertion::GetExport(Module, #fn);
	#define MONODEF_NOINTERNALFAILURE(fn) fn = (fn##_t) Assertion::GetExport(Module, #fn, false);

	MONODEF(mono_jit_init)
	MONODEF(mono_jit_parse_options)
	MONODEF(mono_thread_set_main)
	MONODEF(mono_thread_current)
	MONODEF(mono_add_internal_call)
	MONODEF(mono_lookup_internal_call)
	MONODEF(mono_runtime_invoke)
	MONODEF(mono_method_get_name)
	MONODEF(mono_domain_assembly_open)
	MONODEF(mono_assembly_get_image)
	MONODEF(mono_class_from_name)
	MONODEF(mono_class_get_name)
	MONODEF(mono_class_get_method_from_name)
	MONODEF(mono_string_to_utf8)
	MONODEF(mono_string_new)
	MONODEF(mono_object_get_class)
	MONODEF(mono_property_get_get_method)
	MONODEF(mono_image_get_name)

	MONODEF(mono_install_assembly_preload_hook)
	MONODEF(mono_install_assembly_search_hook)
	MONODEF(mono_install_assembly_load_hook)
	MONODEF(mono_assembly_get_object)

	MONODEF(mono_debug_init)
	if (Debug::Enabled)
	{
		if (Mono::IsOldMono)
			MONODEF_NOINTERNALFAILURE(mono_debug_domain_create)
		else
			MONODEF(mono_debug_domain_create)
	}

	if (!IsOldMono)
	{
		MONODEF(mono_domain_set_config)
		MONODEF(mono_free)
		MONODEF(mono_object_to_string)
	}
	else
		MONODEF(g_free)

	if (Game::IsIl2Cpp)
	{
		MONODEF(mono_set_assemblies_path)
		MONODEF(mono_assembly_setrootdir)
		MONODEF(mono_set_config_dir)

		if (!IsOldMono)
			MONODEF(mono_runtime_set_main_args)

		MONODEF(mono_raise_exception)
		MONODEF(mono_get_exception_bad_image_format)
		MONODEF(mono_image_open_full)
		MONODEF(mono_image_open_from_data_full)
		MONODEF(mono_image_close)
		MONODEF(mono_image_get_table_rows)
		MONODEF(mono_metadata_decode_table_row_col)
		MONODEF(mono_array_addr_with_size)
		MONODEF(mono_array_length)
		MONODEF(mono_metadata_string_heap)
	}
	else
		MONODEF(mono_jit_init_version)

	#undef MONODEF

	return Assertion::ShouldContinue;
}

Mono::String* Mono::ExceptionToString(Object* obj)
{
	if (!IsOldMono)
		return Exports::mono_object_to_string(obj, NULL);

	if (ToStringMethod == NULL)
	{
		Assembly* mscorlib = Exports::mono_domain_assembly_open(domain, "mscorlib");
		Image* mscorlib_image = Exports::mono_assembly_get_image(mscorlib);
		Class* object_class = Exports::mono_class_from_name(mscorlib_image, "System", "Exception");
		ToStringMethod = Exports::mono_class_get_method_from_name(object_class, "ToString", NULL);
	}
	if (ToStringMethod == NULL)
		return NULL;

	return (String*)Exports::mono_runtime_invoke(ToStringMethod, obj, NULL, NULL);
}

void Mono::LogException(Mono::Object* exceptionObject, bool shouldThrow)
{
	if (exceptionObject == NULL)
		return;
	String* returnstr = ExceptionToString(exceptionObject);
	if (returnstr == NULL)
		return;
	const char* returnstrc = Exports::mono_string_to_utf8(returnstr);
	if (returnstrc == NULL)
		return;
	Logger::QuickLog(returnstrc, Error);
	Free(returnstr);
}

void Mono::ParseEnvOption(const char* name)
{
	const int size = 1024;
	char env[size];
	DWORD length = GetEnvironmentVariableA(name, env, size);
	if (length > 0 && length < size)
	{
		char* options[1] = { env };
		Exports::mono_jit_parse_options(1, options);
	}
}

void Mono::Preload()
{
	if (Game::IsIl2Cpp || !Mono::IsOldMono)
		return;

	std::string PreloadAssemblyPath = std::string(Core::BasePath) + "\\MelonLoader\\Dependencies\\SupportModules\\Preload.dll";
	if (!Game::IsIl2Cpp && !Core::FileExists(PreloadAssemblyPath.c_str()))
	{
		Assertion::ThrowInternalFailure("Preload.dll Does Not Exist!");
		return;
	}

	Debug::Msg("Initializing Preload Assembly...");
	Mono::Assembly* assembly = Mono::Exports::mono_domain_assembly_open(Mono::domain, PreloadAssemblyPath.c_str());
	if (assembly == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Open Preload Assembly!");
		return;
	}
	Mono::Image* image = Mono::Exports::mono_assembly_get_image(assembly);
	if (image == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get Image from Preload Assembly!");
		return;
	}
	Mono::Class* klass = Mono::Exports::mono_class_from_name(image, "MelonLoader.Support", "Preload");
	if (image == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get Class from Preload Image!");
		return;
	}
	Mono::Method* initialize = Mono::Exports::mono_class_get_method_from_name(klass, "Initialize", NULL);
	if (initialize == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get Initialize Method from Preload Class!");
		return;
	}
	Mono::Object* exObj = NULL;
	Mono::Exports::mono_runtime_invoke(initialize, NULL, NULL, &exObj);
}

bool Mono::InvokeInitialize() 
{
	Preload();
	Debug::Msg("Initializing Base Assembly...");
	Mono::Assembly* assembly = Mono::Exports::mono_domain_assembly_open(Mono::domain, BaseAssembly::PathMono);
	if (assembly == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Open Mono Assembly!");
		return false;
	}
	Mono::Image* image = Mono::Exports::mono_assembly_get_image(assembly);
	if (image == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get Image from Mono Assembly!");
		return false;
	}
	Mono::Class* klass = Mono::Exports::mono_class_from_name(image, "MelonLoader", "Core");
	if (image == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get MelonLoader.Core from Mono Image!");
		return false;
	}
	Mono::Method* Mono_Initialize = Mono::Exports::mono_class_get_method_from_name(klass, "Initialize", NULL);
	if (Mono_Initialize == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get MelonLoader.Core.Initialize!");
		return false;
	}
	Mono_PreStart = Mono::Exports::mono_class_get_method_from_name(klass, "PreStart", NULL);
	if (Mono_PreStart == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get MelonLoader.Core.PreStart!");
		return false;
	}
	Mono_Start = Mono::Exports::mono_class_get_method_from_name(klass, "Start", NULL);
	if (Mono_Start == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get MelonLoader.Core.Start!");
		return false;
	}

	Mono::Class* klass_AssemblyManager = Mono::Exports::mono_class_from_name(image, "MelonLoader.MonoInternals.ResolveInternals", "AssemblyManager");
	if (klass_AssemblyManager == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get MelonLoader.MonoInternals.ResolveInternals.AssemblyManager from Mono Image!");
		return false;
	}
	AssemblyManager_Resolve = Mono::Exports::mono_class_get_method_from_name(klass_AssemblyManager, "Resolve", 6);
	if (AssemblyManager_Resolve == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get MelonLoader.MonoInternals.ResolveInternals.AssemblyManager.Resolve!");
		return false;
	}
	AssemblyManager_LoadInfo = Mono::Exports::mono_class_get_method_from_name(klass_AssemblyManager, "LoadInfo", 1);
	if (AssemblyManager_LoadInfo == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get MelonLoader.MonoInternals.ResolveInternals.AssemblyManager.LoadInfo!");
		return false;
	}

	Logger::WriteSpacer();

	Mono::Object* exObj = NULL;
	Mono::Object* result = Mono::Exports::mono_runtime_invoke(Mono_Initialize, NULL, NULL, &exObj);
	if (exObj != NULL)
	{
		Mono::LogException(exObj);
		Assertion::ThrowInternalFailure("Failed to Invoke Initialize Method!");
		return false;
	}
	int returnval = *(int*)((char*)result + 0x8);
	Debug::Msg(("Return Value = " + std::to_string(returnval)).c_str());
	if (Debug::Enabled)
		Logger::WriteSpacer();
	return (returnval == 0);
}

bool Mono::InvokePreStart()
{
	if (Mono_PreStart == NULL)
		return false;
	Debug::Msg("Pre-Starting Base Assembly...");
	Logger::WriteSpacer();
	Mono::Object* exObj = NULL;
	Mono::Object* result = Mono::Exports::mono_runtime_invoke(Mono_PreStart, NULL, NULL, &exObj);
	if (exObj != NULL)
	{
		Mono::LogException(exObj);
		Assertion::ThrowInternalFailure("Failed to Invoke PreStart Method!");
	}
	int returnval = *(int*)((char*)result + 0x8);
	if (Game::IsIl2Cpp)
		Il2CppAssemblyGenerator::Cleanup();
	Debug::Msg(("Return Value = " + std::to_string(returnval)).c_str());
	if (Debug::Enabled)
		Logger::WriteSpacer();
	return (returnval == 0);
}

void Mono::InvokeStart()
{
	if (Mono_Start == NULL)
		return;
	Debug::Msg("Starting Base Assembly...");
	Logger::WriteSpacer();
	Mono::Object* exObj = NULL;
	Mono::Object* result = Mono::Exports::mono_runtime_invoke(Mono_Start, NULL, NULL, &exObj);
	if (exObj != NULL)
	{
		Mono::LogException(exObj);
		Assertion::ThrowInternalFailure("Failed to Invoke Start Method!");
	}
	int returnval = *(int*)((char*)result + 0x8);
	Debug::Msg(("Return Value = " + std::to_string(returnval)).c_str());
	if (Debug::Enabled)
		Logger::WriteSpacer();
}

Mono::Domain* Mono::Hooks::mono_jit_init_version(const char* name, const char* version)
{
	Console::SetHandles();
	Debug::Msg("Detaching Hook from mono_jit_init_version...");
	Hook::Detach(&(LPVOID&)Exports::mono_jit_init_version, mono_jit_init_version);
	Debug::Msg("Creating Mono Domain...");

	if (Debug::Enabled)
	{
		if (IsOldMono)
			Mono::ParseEnvOption("DNSPY_UNITY_DBG");
		else
			Mono::ParseEnvOption("DNSPY_UNITY_DBG2");
		Exports::mono_debug_init(MONO_DEBUG_FORMAT_MONO);
	}

	domain = Exports::mono_jit_init_version(name, version);

	if (Debug::Enabled && (Exports::mono_debug_domain_create != NULL))
		Exports::mono_debug_domain_create(domain);

	Exports::mono_thread_set_main(Exports::mono_thread_current());
	if (!IsOldMono)
	{
		Debug::Msg("Setting Mono Domain Config...");
		Exports::mono_domain_set_config(domain, Game::BasePathMono, name);
	}
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
	//if (method_name != NULL)
	//	Debug::Msg(method_name);
	if ((strstr(method_name, "Internal_ActiveSceneChanged") != NULL) ||
		(strstr(method_name, "UnityEngine.ISerializationCallbackReceiver.OnAfterDeserialize") != NULL)
		|| (IsOldMono && (
			(strstr(method_name, "Awake") != NULL)
			|| (strstr(method_name, "DoSendMouseEvents") != NULL))))
	{
		Debug::Msg("Detaching Hook from mono_runtime_invoke...");
		Hook::Detach(&(LPVOID&)Exports::mono_runtime_invoke, mono_runtime_invoke);
		if (BaseAssembly::PreStart())
			BaseAssembly::Start();
	}
	return Exports::mono_runtime_invoke(method, obj, params, exec);
}

Mono::Assembly* Mono::Hooks::AssemblyPreLoad(AssemblyName* aname, char** assemblies_path, void* user_data) { return AssemblyResolve(aname, user_data, true); }
Mono::Assembly* Mono::Hooks::AssemblySearch(AssemblyName* aname, void* user_data) { return AssemblyResolve(aname, user_data, false); }
Mono::Assembly* Mono::Hooks::AssemblyResolve(AssemblyName* aname, void* user_data, bool is_preload)
{
	if (Mono::AssemblyManager_Resolve == NULL)
		return NULL;

	if (aname == NULL)
		return NULL;

	String* name = Mono::Exports::mono_string_new(domain, aname->name);
	uint16_t version_major = aname->major;
	uint16_t version_minor = aname->minor;
	uint16_t version_build = aname->build;
	uint16_t version_revision = aname->revision;
	void* args[] = {
		name,
		&version_major,
		&version_minor,
		&version_build,
		&version_revision,
		&is_preload
	};

	Mono::Object* exObj = NULL;
	Mono::Object* result = Mono::Exports::mono_runtime_invoke(Mono::AssemblyManager_Resolve, NULL, args, &exObj);
	if (exObj != NULL)
	{
		Mono::LogException(exObj);
		Assertion::ThrowInternalFailure("Failed to Invoke MelonLoader.MonoInternals.ResolveInternals.AssemblyManager.Resolve!");
	}

	if (result != NULL)
		return ((Mono::ReflectionAssembly*)result)->assembly;
	return NULL;
}

void Mono::Hooks::AssemblyLoad(Assembly* assembly, void* user_data)
{
	if (Mono::AssemblyManager_LoadInfo == NULL)
		return;

	if (assembly == NULL)
		return;

	ReflectionAssembly* reflectionAssembly = Exports::mono_assembly_get_object(Mono::domain, assembly);
	if (reflectionAssembly == NULL)
		return;

	void* args[] = {
		reflectionAssembly
	};

	Mono::Object* exObj = NULL;
	Mono::Object* result = Mono::Exports::mono_runtime_invoke(Mono::AssemblyManager_LoadInfo, NULL, args, &exObj);
	if (exObj != NULL)
	{
		Mono::LogException(exObj);
		Assertion::ThrowInternalFailure("Failed to Invoke MelonLoader.MonoInternals.ResolveInternals.AssemblyManager.Load!");
	}
}