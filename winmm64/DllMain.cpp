#include <Windows.h>
#include "Wrapper.h"
#include "MelonLoader.h"

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	MelonLoader::thisdll = hinstDLL;
	if (fdwReason == DLL_PROCESS_ATTACH)
	{
		DisableThreadLibraryCalls(MelonLoader::thisdll);
		if (Wrapper::Initialize())
			MelonLoader::Initialize();
		else
			return FALSE;
	}
	else if (fdwReason == DLL_PROCESS_DETACH)
		FreeLibrary(MelonLoader::thisdll);
	return TRUE;
}