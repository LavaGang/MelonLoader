#include "Hooks.h"

mono_jit_init_version_t Hook_mono_jit_init_version::Original_mono_jit_init_version = NULL;

void Hook_mono_jit_init_version::Hook()
{
	if (Original_mono_jit_init_version == NULL)
	{
		Original_mono_jit_init_version = Mono::mono_jit_init_version;
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach(&(LPVOID&)Original_mono_jit_init_version, Hooked_mono_jit_init_version);
		DetourTransactionCommit();
	}
}

void Hook_mono_jit_init_version::Unhook()
{
	if (Original_mono_jit_init_version != NULL)
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourDetach(&(LPVOID&)Original_mono_jit_init_version, Hooked_mono_jit_init_version);
		DetourTransactionCommit();
		Original_mono_jit_init_version = NULL;
	}
}

MonoDomain* Hook_mono_jit_init_version::Hooked_mono_jit_init_version(const char* name, const char* version)
{
	Mono::Domain = Original_mono_jit_init_version(name, version);
	Unhook();
	return Mono::Domain;
}