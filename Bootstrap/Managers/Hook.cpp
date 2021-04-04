#ifdef _WIN32
#include <Windows.h>
#endif

#include "Hook.h"
#include "../Utils/Patching/PatchHelper.h"
#include "PatchManager.h"
#include "../Base/funchook/include/funchook.h"
#include "../Utils/Console/Logger.h"

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
std::unordered_map<void**, funchook_t*> Hook::HookMap;

void Hook::Attach(void** target, void* detour)
{
	// funchook* funchook = funchook_create();
	// funchook_prepare(funchook, target, detour);
	// funchook_install(funchook, 0);
	int rv;
	
	if (HookMap.find(target) == HookMap.end())
	{
		HookMap[target] = funchook_create();
		rv = funchook_prepare(HookMap[target], target, detour);
		if (rv != 0)
		{
			Logger::Error("Failed to prepare hook");
			return;
		}
	}
	
	rv = funchook_install(HookMap[target], 0);
	if (rv != 0)
	{
		Logger::Error("Failed to install hook");
		return;
	}
	
	return;
}

void Hook::Detach(void** target, void* detour)
{
	int rv;

	if (HookMap.find(target) == HookMap.end())
	{
		Logger::Error("Hook doesn't exist");
		return;
	}

	rv = funchook_uninstall(HookMap[target], 0);
	if (rv != 0)
	{
		Logger::Error("Failed to uninstall hook");
		return;
	}

	return;
}
#endif