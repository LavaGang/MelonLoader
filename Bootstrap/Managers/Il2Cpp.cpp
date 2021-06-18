#include "Il2Cpp.h"
#include "Game.h"
#include "../Utils/Assertion.h"
#include "../Utils/AssemblyGenerator.h"
#include "Hook.h"
#include "Mono.h"
#include "../Utils/Console/Console.h"
#include "../Utils/Console/Debug.h"
#include <string>
#include "AssemblyVerifier.h"
#include "InternalCalls.h"
#include "BaseAssembly.h"
#include "../Utils/Console/Logger.h"

#ifdef __ANDROID__
#include <dlfcn.h>
#include <android/log.h>
#endif
#include "../Utils/Helpers/ImportLibHelper.h"

Il2Cpp::Domain* Il2Cpp::domain = NULL;
char* Il2Cpp::GameAssemblyPath = NULL;
void* Il2Cpp::UnityTLSInterfaceStruct = NULL;

Il2Cpp::Exports::il2cpp_init_t Il2Cpp::Exports::il2cpp_init = NULL;
Il2Cpp::Exports::il2cpp_runtime_invoke_t Il2Cpp::Exports::il2cpp_runtime_invoke = NULL;
Il2Cpp::Exports::il2cpp_method_get_name_t Il2Cpp::Exports::il2cpp_method_get_name = NULL;
Il2Cpp::Exports::il2cpp_unity_install_unitytls_interface_t Il2Cpp::Exports::il2cpp_unity_install_unitytls_interface = NULL;


#ifdef _WIN32
HMODULE Il2Cpp::Module = NULL;

bool Il2Cpp::Initialize()
{
	if (!Game::IsIl2Cpp)
		return true;
	Debug::Msg("Initializing Il2Cpp...");
	Debug::Msg(("Il2Cpp::GameAssemblyPath = " + std::string(GameAssemblyPath)).c_str());
	Module = LoadLibraryA(GameAssemblyPath);
	if (Module == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Load GameAssembly!");
		return false;
	}
	return Exports::Initialize();
}

bool Il2Cpp::Exports::Initialize()
{
	Debug::Msg("Initializing Il2Cpp Exports...");
	il2cpp_init = (il2cpp_init_t)Assertion::GetExport(Module, "il2cpp_init");
	il2cpp_runtime_invoke = (il2cpp_runtime_invoke_t)Assertion::GetExport(Module, "il2cpp_runtime_invoke");
	il2cpp_method_get_name = (il2cpp_method_get_name_t)Assertion::GetExport(Module, "il2cpp_method_get_name");
	if (!Mono::IsOldMono)
		il2cpp_unity_install_unitytls_interface = (il2cpp_unity_install_unitytls_interface_t)Assertion::GetExport(Module, "il2cpp_unity_install_unitytls_interface");
	return Assertion::ShouldContinue;
}

Il2Cpp::Domain* Il2Cpp::Hooks::il2cpp_init(const char* name)
{
	if (!Debug::Enabled)
		Console::SetHandles();
	Debug::Msg("Detaching Hook from il2cpp_init...");
	Hook::Detach(&(LPVOID&)Exports::il2cpp_init, il2cpp_init);
	if (AssemblyGenerator::Initialize())
	{
		Mono::CreateDomain(name);
		InternalCalls::Initialize();
		// todo: check if it works/is necessary on mono games
		AssemblyVerifier::InstallHooks();
		if (BaseAssembly::Initialize())
		{
			Debug::Msg("Attaching Hook to il2cpp_runtime_invoke...");
			Hook::Attach(&(LPVOID&)Exports::il2cpp_runtime_invoke, il2cpp_runtime_invoke);
		}
	}
	Debug::Msg("Creating Il2Cpp Domain...");
	domain = Exports::il2cpp_init(name);
	return domain;
}

Il2Cpp::Object* Il2Cpp::Hooks::il2cpp_runtime_invoke(Method* method, Object* obj, void** params, Object** exec)
{
	const char* method_name = Exports::il2cpp_method_get_name(method);
	if (strstr(method_name, "Internal_ActiveSceneChanged") != NULL)
	{
		Debug::Msg("Detaching Hook from il2cpp_runtime_invoke...");
		Hook::Detach(&(LPVOID&)Exports::il2cpp_runtime_invoke, il2cpp_runtime_invoke);
		BaseAssembly::Start();
	}
	return Exports::il2cpp_runtime_invoke(method, obj, params, exec);
}

void Il2Cpp::Hooks::il2cpp_unity_install_unitytls_interface(void* unitytlsInterfaceStruct)
{
	Exports::il2cpp_unity_install_unitytls_interface(unitytlsInterfaceStruct);
	UnityTLSInterfaceStruct = unitytlsInterfaceStruct;
}
#elif defined(__ANDROID__)
void* Il2Cpp::Handle = NULL;
void* Il2Cpp::MemLoc = NULL;
const char* Il2Cpp::LibPath = NULL;

bool Il2Cpp::Initialize()
{
	Debug::Msg("Initializing Il2Cpp...");
	Handle = dlopen("libil2cpp.so", RTLD_NOW | RTLD_GLOBAL | RTLD_GLOBAL);

	if (Handle == nullptr)
	{
		// TODO: ASSERT ERROR
		Logger::Error(dlerror());
		return false;
	}

	Debug::Msg("Loaded Il2Cpp");
	//__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "%p", Il2Cpp::Handle);

	return Exports::Initialize();
}

bool Il2Cpp::Exports::Initialize()
{
	Debug::Msg("Initializing Il2Cpp Exports...");
	
	il2cpp_init = (il2cpp_init_t)ImportLibHelper::GetExport(Handle, "il2cpp_init");
	il2cpp_runtime_invoke = (il2cpp_runtime_invoke_t)ImportLibHelper::GetExport(Handle, "il2cpp_runtime_invoke");
	il2cpp_method_get_name = (il2cpp_method_get_name_t)ImportLibHelper::GetExport(Handle, "il2cpp_method_get_name");
	il2cpp_unity_install_unitytls_interface = (il2cpp_unity_install_unitytls_interface_t)ImportLibHelper::GetExport(Handle, "il2cpp_unity_install_unitytls_interface");
	
	Dl_info dlInfo;
	dladdr((void*)il2cpp_init, &dlInfo);
	MemLoc = dlInfo.dli_fbase;
	LibPath = dlInfo.dli_fname;

	Dl_info dlInfo1;
	dladdr((void*)il2cpp_runtime_invoke, &dlInfo1);
	
	if (MemLoc != dlInfo1.dli_fbase)
		Assertion::ThrowInternalFailure("Address mismatch");

	if (!Assertion::ShouldContinue)
	{
		Logger::Error("One or more symbols failed to load.");
	}

	return Assertion::ShouldContinue;
}

bool Il2Cpp::ApplyPatches()
{
	Debug::Msg("Applying patches for Il2CPP");

	Hook::Attach((void**)&Exports::il2cpp_init, (void*)Hooks::il2cpp_init);
	//Hook::Attach((void**)&Exports::il2cpp_unity_install_unitytls_interface, (void*)Hooks::il2cpp_unity_install_unitytls_interface);

	return true;
}

#pragma region Hooks
Il2Cpp::Domain* Il2Cpp::Hooks::il2cpp_init(const char* name)
{
	// if (!Debug::Enabled)
		// Console::SetHandles();

	if (!Mono::CheckPaths())
	{
		Logger::Error("Skipping initialization of MelonLoader");
		goto exit_early;	
	}
	
	// if (AssemblyGenerator::Initialize())
	// {
		Mono::CreateDomain(name);
		BaseAssembly::LoadAssembly();
		InternalCalls::Initialize();
		// todo: check if it works/is necessary on mono games
		//AssemblyVerifier::InstallHooks();
		if (BaseAssembly::Initialize())
		{
			Debug::Msg("Attaching Hook to il2cpp_runtime_invoke...");
			Hook::Attach((void**)&Exports::il2cpp_runtime_invoke, (void*)Hooks::il2cpp_runtime_invoke);
		} else
		{
			Debug::Msg("Base assembly failed to setup.");
		}
	// }

	domain = Exports::il2cpp_init(name);

	exit_early:	
	Debug::Msg("Detaching Hook from il2cpp_init...");
	Hook::Detach((void**)&Exports::il2cpp_init, (void*)Hooks::il2cpp_init);
	
	return domain;
}

Il2Cpp::Object* Il2Cpp::Hooks::il2cpp_runtime_invoke(Method* method, Object* obj, void** params, Object** exec)
{
	const char* method_name = Exports::il2cpp_method_get_name(method);
	if (strstr(method_name, "Internal_ActiveSceneChanged") != NULL)
	{
		Debug::Msg("Detaching Hook from il2cpp_runtime_invoke...");
		Hook::Detach((void**)&(Exports::il2cpp_runtime_invoke), (void*)il2cpp_runtime_invoke);
		if (BaseAssembly::PreStart())
			BaseAssembly::Start();
	}
	return Exports::il2cpp_runtime_invoke(method, obj, params, exec);
}

void Il2Cpp::Hooks::il2cpp_unity_install_unitytls_interface(void* unitytlsInterfaceStruct)
{
	Debug::Msg("Unity TLS");
	UnityTLSInterfaceStruct = unitytlsInterfaceStruct;
	return Exports::il2cpp_unity_install_unitytls_interface(unitytlsInterfaceStruct);
}

#pragma endregion Hooks

#endif
