#include "Hooks.h"
#include "../MelonLoader.h"
#include "../detours/detours.h"

il2cpp_init_t Hook_il2cpp_init::Original_il2cpp_init = NULL;

void Hook_il2cpp_init::Hook()
{
	if (Original_il2cpp_init == NULL)
	{
		Original_il2cpp_init = IL2CPP::il2cpp_init;
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach(&(LPVOID&)Original_il2cpp_init, Hooked_il2cpp_init);
		DetourTransactionCommit();
	}
}

void Hook_il2cpp_init::Unhook()
{
	if (Original_il2cpp_init != NULL)
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourDetach(&(LPVOID&)Original_il2cpp_init, Hooked_il2cpp_init);
		DetourTransactionCommit();
		Original_il2cpp_init = NULL;
	}
}

IL2CPPDomain* Hook_il2cpp_init::Hooked_il2cpp_init(const char* name)
{
	IL2CPP::Domain = Original_il2cpp_init(name);
	Unhook();
	return IL2CPP::Domain;
}