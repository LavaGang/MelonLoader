#include "InternalCalls.h"
#include "../Utils/Debug.h"
#include "Hook.h"
#include "../Utils/Assertion.h"

void InternalCalls::Initialize()
{
	Debug::Msg("Initializing Internal Calls...");
	MelonUtils::AddInternalCalls();
}

#pragma region MelonUtils
bool InternalCalls::MelonUtils::IsGame32Bit()
{
	#ifdef _WIN64
	return false;
	#else
	return true;
	#endif
}

Mono::String* InternalCalls::MelonUtils::GetManagedDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Mono::ManagedPathMono); }
void* InternalCalls::MelonUtils::GetLibPtr() { return Mono::Module; }
void* InternalCalls::MelonUtils::GetRootDomainPtr() { return Mono::domain; }
Mono::ReflectionAssembly* InternalCalls::MelonUtils::CastManagedAssemblyPtr(void* ptr) { return (Mono::ReflectionAssembly*)ptr; }

void InternalCalls::MelonUtils::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.MelonUtils::IsGame32Bit", IsGame32Bit);
	
	Mono::AddInternalCall("MelonLoader.BootstrapInterop::NativeHookAttach", Hook::Attach);
	Mono::AddInternalCall("MelonLoader.BootstrapInterop::NativeHookDetach", Hook::Detach);

	Mono::AddInternalCall("MelonLoader.MonoInternals.MonoLibrary::GetLibPtr", GetLibPtr);
	Mono::AddInternalCall("MelonLoader.MonoInternals.MonoLibrary::GetRootDomainPtr", GetRootDomainPtr);
	Mono::AddInternalCall("MelonLoader.MonoInternals.MonoLibrary::CastManagedAssemblyPtr", CastManagedAssemblyPtr);
	Mono::AddInternalCall("MelonLoader.MonoInternals.ResolveInternals.AssemblyManager::InstallHooks", Mono::InstallAssemblyHooks);
}
#pragma endregion


#pragma endregion