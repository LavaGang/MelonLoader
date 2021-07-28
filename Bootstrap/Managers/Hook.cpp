#ifdef _WIN32
#include <Windows.h>
#endif

#include <funchook.h>

#include "Hook.h"
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
std::unordered_map<void**, Hook::FunchookDef*> Hook::HookMap;

void Hook::Attach(void** target, void* detour)
{
	int rv;
	
	if (HookMap.find(target) == HookMap.end())
	{
		HookMap[target] = (Hook::FunchookDef*)malloc(sizeof(Hook::FunchookDef));
		HookMap[target]->original = *target;
		HookMap[target]->handle = funchook_create();
		rv = funchook_prepare(HookMap[target]->handle, target, detour);
		if (rv != 0)
		{
			Logger::Error("Failed to prepare hook");
			return;
		}
	}
	
	rv = funchook_install(HookMap[target]->handle, 0);
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

	void* reset = HookMap[target]->original;

	rv = funchook_uninstall(HookMap[target]->handle, 0);
	if (rv != 0)
	{
		Logger::Error("Failed to uninstall hook");
		return;
	}

	*target = reset;

	return;
}
#endif