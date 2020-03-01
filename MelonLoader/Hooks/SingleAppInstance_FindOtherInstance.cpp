#include "Hooks.h"

SingleAppInstance_FindOtherInstance_t Hook_SingleAppInstance_FindOtherInstance::Original_SingleAppInstance_FindOtherInstance = NULL;

void Hook_SingleAppInstance_FindOtherInstance::Hook()
{
	if (Original_SingleAppInstance_FindOtherInstance == NULL)
	{
		Original_SingleAppInstance_FindOtherInstance = MonoUnityPlayer::SingleAppInstance_FindOtherInstance;
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach(&(LPVOID&)Original_SingleAppInstance_FindOtherInstance, Hooked_SingleAppInstance_FindOtherInstance);
		DetourTransactionCommit();
	}
}

void Hook_SingleAppInstance_FindOtherInstance::Unhook()
{
	if (Original_SingleAppInstance_FindOtherInstance != NULL)
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourDetach(&(LPVOID&)Original_SingleAppInstance_FindOtherInstance, Hooked_SingleAppInstance_FindOtherInstance);
		DetourTransactionCommit();
		Original_SingleAppInstance_FindOtherInstance = NULL;
	}
}

bool __stdcall Hook_SingleAppInstance_FindOtherInstance::Hooked_SingleAppInstance_FindOtherInstance(LPARAM lParam) { return false; }