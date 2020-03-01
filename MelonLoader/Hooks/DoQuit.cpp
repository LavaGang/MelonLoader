#include "Hooks.h"
#include "../ModHandler.h"
#include "../Console.h"

PlayerCleanup_t Hook_PlayerCleanup::Original_PlayerCleanup = NULL;

void Hook_PlayerCleanup::Hook()
{
	if (Original_PlayerCleanup == NULL)
	{
		Original_PlayerCleanup = IL2CPPUnityPlayer::PlayerCleanup;
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach(&(LPVOID&)Original_PlayerCleanup, Hooked_PlayerCleanup);
		DetourTransactionCommit();
	}
}

void Hook_PlayerCleanup::Unhook()
{
	if (Original_PlayerCleanup != NULL)
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourDetach(&(LPVOID&)Original_PlayerCleanup, Hooked_PlayerCleanup);
		DetourTransactionCommit();
		Original_PlayerCleanup = NULL;
	}
}

bool Hook_PlayerCleanup::Hooked_PlayerCleanup(bool dopostquitmsg)
{
	Console::WriteLine("CALLED!");
	ModHandler::OnApplicationQuit();
	return Original_PlayerCleanup(dopostquitmsg);
}