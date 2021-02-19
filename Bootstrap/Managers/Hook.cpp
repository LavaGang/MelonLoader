#ifndef PORT_TODO_DISABLE
#include <Windows.h>
#include "Hook.h"
#ifdef _WIN64
#include "../Base/MSDetours/detours_x64.h"
#else
#include "../Base/MSDetours/detours_x86.h"
#endif

void Hook::Attach(void** target, void* detour)
{
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourAttach(target, detour);
	DetourTransactionCommit();
}

void Hook::Detach(void** target, void* detour)
{
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourDetach(target, detour);
	DetourTransactionCommit();
}
#endif