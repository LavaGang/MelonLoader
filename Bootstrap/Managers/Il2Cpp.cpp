#include "Il2Cpp.h"
#include "Game.h"
#include "../Utils/Assertion.h"
#include "../Utils/AssemblyGenerator.h"
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
Il2Cpp::Exports::il2cpp_unity_install_unitytls_interface_t Il2Cpp::Exports::il2cpp_unity_install_unitytls_interface = NULL;

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

void Il2Cpp::CallInstallUnityTLSInterface()
{
	Debug::Msg("Trying to find InstallUnityTlsInterface");
	HMODULE mod = LoadLibraryA(Il2Cpp::UnityPlayerPath);
	// Unity 2018
	std::vector<uintptr_t> installTLSCandidates = PointerUtils::FindAllPattern(mod,
#ifdef _WIN64
		"48 8B 0D ? ? ? ? 48 85 C9 0F 85 DC 01 00 00 48 8B 05 ? ? ? ? 48 8D 0D ? ? ? ? 48 89 05 ? ? ? ? 48 8B 05 ? ? ? ? 48 89 05"
#else
		"A1 ? ? ? ? 85 C0 0F 85 68 01 00 00 A1 ? ? ? ? A3 ? ? ? ? A1 ? ? ? ? A3 ? ? ? ? A1 ? ? ? ? A3 ? ? ? ? A1 ? ? ? ? A3 ? ? ? ? B8 ? ? ? ? C7 05"
#endif
	);
	if (installTLSCandidates.size() == 0) // Unity 2019+
		installTLSCandidates = PointerUtils::FindAllPattern(mod,
#ifdef _WIN64
			"48 8B 0D ? ? ? ? 48 8B 15 ? ? ? ? 48 85 C9 0F 85 DC 01 00 00 48 8B 05 ? ? ? ? 48 8D 0D ? ? ? ? 48 89 05 ? ? ? ? 48 8B 05 ? ? ? ? 48 89 05"
#else
			"A1 ? ? ? ? 8B 0D ? ? ? ? 85 C0 0F 85 68 01 00 00 A1 ? ? ? ? A3 ? ? ? ? A1 ? ? ? ? A3 ? ? ? ? A1 ? ? ? ? A3 ? ? ? ? A1 ? ? ? ? A3 ? ? ? ? B8 ? ? ? ? C7 05"
#endif
		);
	if (installTLSCandidates.size() == 0)
	{
		Debug::Msg("InstallUnityTlsInterface was not found!");
		return;
	}
	for (auto i = installTLSCandidates.begin(); i != installTLSCandidates.end(); ++i)
	{
		if (!*i || *i & 0xF) continue;
		Debug::Msg("Calling InstallUnityTlsInterface");
		((void (*) (void)) * i)();
		break;
	}
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
	Il2Cpp::CallInstallUnityTLSInterface();
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
		BaseAssembly::Start();
	}
	return Exports::il2cpp_runtime_invoke(method, obj, params, exec);
}

void Il2Cpp::Hooks::il2cpp_unity_install_unitytls_interface(void* unitytlsInterfaceStruct)
{
	Exports::il2cpp_unity_install_unitytls_interface(unitytlsInterfaceStruct);
	if (!UnityTLSInterfaceStruct && unitytlsInterfaceStruct &&
		!(*(long long*)(unitytlsInterfaceStruct) & ~0xFF))
		UnityTLSInterfaceStruct = unitytlsInterfaceStruct;
}