#include "Il2Cpp.h"
#include "Game.h"
#include "../Utils/Assertion.h"
#include "Hook.h"
#include "Mono.h"
#include "../Utils/Console.h"
#include "../Utils/Debug.h"
#include "../Utils/PointerUtils.h"
#include <string>
#include "AssemblyVerifier.h"
#include "InternalCalls.h"
#include "BaseAssembly.h"

Il2Cpp::Domain* Il2Cpp::domain = NULL;
char* Il2Cpp::GameAssemblyPath = NULL;
char* Il2Cpp::GameAssemblyPathMono = NULL;
char* Il2Cpp::UnityPlayerPath = NULL;
HMODULE Il2Cpp::Module = NULL;
void* Il2Cpp::UnityTLSInterfaceStruct = NULL;
Il2Cpp::Exports::il2cpp_init_t Il2Cpp::Exports::il2cpp_init = NULL;
Il2Cpp::Exports::il2cpp_runtime_invoke_t Il2Cpp::Exports::il2cpp_runtime_invoke = NULL;
Il2Cpp::Exports::il2cpp_method_get_name_t Il2Cpp::Exports::il2cpp_method_get_name = NULL;

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
	return Assertion::ShouldContinue;
}

Il2Cpp::Domain* Il2Cpp::Hooks::il2cpp_init(const char* name)
{
	Console::SetHandles();
	Debug::Msg("Detaching Hook from il2cpp_init...");
	Hook::Detach(&(LPVOID&)Exports::il2cpp_init, il2cpp_init);
	Mono::CreateDomain(name);
	InternalCalls::Initialize();
	// todo: check if it works/is necessary on mono games
	AssemblyVerifier::InstallHooks();
	if (BaseAssembly::Initialize())
	{
		Debug::Msg("Attaching Hook to il2cpp_runtime_invoke...");
		Hook::Attach(&(LPVOID&)Exports::il2cpp_runtime_invoke, il2cpp_runtime_invoke);
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
		if (BaseAssembly::PreStart())
			BaseAssembly::Start();
	}
	return Exports::il2cpp_runtime_invoke(method, obj, params, exec);
}