#include <Windows.h>
#include "MelonLoader.h"

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	MelonLoader::thisdll = hinstDLL;
	if (fdwReason == DLL_PROCESS_ATTACH)
		MelonLoader::Main();
	else if (fdwReason == DLL_PROCESS_DETACH)
	{
		MelonLoader::ApplicationQuit();
		FreeLibrary(MelonLoader::thisdll);
	}
	return TRUE;
}
