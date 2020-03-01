#include "Hooks.h"
#include "../MelonLoader.h"

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

Il2CppDomain* Hook_il2cpp_init::Hooked_il2cpp_init(const char* name)
{
	IL2CPP::Domain = Original_il2cpp_init(name);
	if (MelonLoader::MupotMode && MonoUnityPlayer::Load() && MonoUnityPlayer::Setup())
	{
		Hook_PlayerLoadFirstScene::Hook();
		Hook_SingleAppInstance_FindOtherInstance::Hook();
		MonoUnityPlayer::UnityMain();
	}
	Unhook();
	return IL2CPP::Domain;
}