#include "Il2Cpp.h"
#include "Game.h"
#include "../Utils/Assertion.h"
#include "../Utils/AssemblyGenerator.h"
#include "Hook.h"
#include "Mono.h"
#include "../Utils/Console.h"
#include "../Utils/Debug.h"
#include <string>
#include "AssemblyVerifier.h"
#include "InternalCalls.h"
#include "BaseAssembly.h"
#include "../Utils/Logger.h"

#ifdef __ANDROID__
#include <dlfcn.h>
#endif

Il2Cpp::Domain* Il2Cpp::domain = NULL;
char* Il2Cpp::GameAssemblyPath = NULL;
void* Il2Cpp::UnityTLSInterfaceStruct = NULL;
Il2Cpp::Exports::il2cpp_init_t Il2Cpp::Exports::il2cpp_init = NULL;
Il2Cpp::Exports::il2cpp_runtime_invoke_t Il2Cpp::Exports::il2cpp_runtime_invoke = NULL;
Il2Cpp::Exports::il2cpp_method_get_name_t Il2Cpp::Exports::il2cpp_method_get_name = NULL;
Il2Cpp::Exports::il2cpp_unity_install_unitytls_interface_t Il2Cpp::Exports::il2cpp_unity_install_unitytls_interface = NULL;
Il2Cpp::Exports::testFnDef Il2Cpp::Exports::test_fn = NULL;
// void* Il2Cpp::Exports::test_fn_untyped = NULL;

Patcher* Il2Cpp::Patches::test_fn = NULL;

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

bool Il2Cpp::Initialize()
{
	Debug::Msg("Initializing Il2Cpp...");
	// Handle = dlopen("libil2cpp.so", RTLD_NOW & RTLD_NODELETE & RTLD_GLOBAL);
	Handle = dlopen("libPlaygroundBootstrapper.so", RTLD_NOW & RTLD_NODELETE & RTLD_GLOBAL);

	if (Handle == nullptr)
	{
		// TODO: ASSERT ERROR
		Logger::Error(dlerror());
		return false;
	}

	Debug::Msg("Loaded Il2Cpp");

	return Exports::Initialize();
}

bool Il2Cpp::Exports::Initialize()
{
	Debug::Msg("Initializing Il2Cpp Exports...");
	
	// il2cpp_init = (il2cpp_init_t)GetExport("il2cpp_init");
	// il2cpp_runtime_invoke = (il2cpp_runtime_invoke_t)GetExport("il2cpp_runtime_invoke");
	// il2cpp_method_get_name = (il2cpp_method_get_name_t)GetExport("il2cpp_method_get_name");
	// il2cpp_unity_install_unitytls_interface = (il2cpp_unity_install_unitytls_interface_t)GetExport("il2cpp_unity_install_unitytls_interface");

	test_fn = (testFnDef)GetExport("TestExternalCall");
	// test_fn_untyped = GetExport("TestExternalCall");
	
	if (ImportError)
	{
		Logger::Error("One or more symbols failed to load.");
	}

	return !ImportError;
}

bool Il2Cpp::ApplyPatches()
{
	Patcher* test_fn_res;
	Patches::test_fn = new Patcher((void *)Exports::test_fn, (void*)Hooks::test_fn);
	Patches::test_fn->ApplyPatch();

	return true;
}

#pragma region Hooks
Il2Cpp::Domain* Il2Cpp::Hooks::il2cpp_init(const char* name)
{

}

Il2Cpp::Object* Il2Cpp::Hooks::il2cpp_runtime_invoke(Method* method, Object* obj, void** params, Object** exec)
{

}

void Il2Cpp::Hooks::il2cpp_unity_install_unitytls_interface(void* unitytlsInterfaceStruct)
{

}

void Il2Cpp::Hooks::test_fn(int value)
{
	Logger::Msg(std::to_string(value).c_str());

	Patches::test_fn->ClearPatch();
	Exports::test_fn(420);
	Patches::test_fn->ApplyPatch();
}

#pragma endregion Hooks

bool Il2Cpp::ImportError = false;

void* Il2Cpp::GetExport(const char* name)
{
	void* fnPointer = dlsym(Handle, name);

	if (fnPointer == nullptr)
	{
		ImportError = true;
		Logger::Error(dlerror());

		return nullptr;
	}

	return fnPointer;
}

#endif
