#ifdef _WIN32
#include <Windows.h>
#endif

#include "Hook.h"
#include "../Utils/Patching/PatchHelper.h"

#ifdef _WIN64
#include "../Base/MSDetours/detours_x64.h"
#elif _WIN32
#include "../Base/MSDetours/detours_x86.h"
#endif

#ifdef _WIN32
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

#ifdef __ANDROID__
void Hook::Attach(void** target, void* detour)
{
	// PatchHelper::Attach(*target, detour);
}

void Hook::Detach(void** target, void* detour)
{
	// PatchHelper::Detach(*target, detour);
}
#endif