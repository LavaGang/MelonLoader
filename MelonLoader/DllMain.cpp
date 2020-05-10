#include <Windows.h>
#include "MelonLoader.h"

bool IsInitialized = false;
BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	MelonLoader::thisdll = hinstDLL;
	if (fdwReason == DLL_PROCESS_ATTACH)
	{
#ifndef DEBUG
		DisableThreadLibraryCalls(MelonLoader::thisdll);
#endif
		MelonLoader::Main();
	}
	else if (fdwReason == DLL_PROCESS_DETACH)
		FreeLibrary(MelonLoader::thisdll);
	return TRUE;
}