#include "Hooks.h"
#include "../MelonLoader.h"

il2cpp_add_internal_call_t Hook_il2cpp_add_internal_call::Original_il2cpp_add_internal_call = NULL;

void Hook_il2cpp_add_internal_call::Hook()
{
	if (Original_il2cpp_add_internal_call == NULL)
	{
		Original_il2cpp_add_internal_call = IL2CPP::il2cpp_add_internal_call;
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach(&(LPVOID&)Original_il2cpp_add_internal_call, Hooked_il2cpp_add_internal_call);
		DetourTransactionCommit();
	}
}

void Hook_il2cpp_add_internal_call::Unhook()
{
	if (Original_il2cpp_add_internal_call != NULL)
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourDetach(&(LPVOID&)Original_il2cpp_add_internal_call, Hooked_il2cpp_add_internal_call);
		DetourTransactionCommit();
		Original_il2cpp_add_internal_call = NULL;
	}
}

void Hook_il2cpp_add_internal_call::Hooked_il2cpp_add_internal_call(const char* name, void* method)
{
	if (!MelonLoader::MupotMode)
	{
		Original_il2cpp_add_internal_call(name, method);
		Mono::mono_add_internal_call(name, method);
	}
}