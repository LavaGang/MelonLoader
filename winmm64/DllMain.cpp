#include <Windows.h>
#include "Wrapper.h"
#include "MelonLoader.h"

HINSTANCE thisdll = NULL;

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	thisdll = hinstDLL;
	if (fdwReason == DLL_PROCESS_ATTACH)
	{
		if (Wrapper::Initialize())
			MelonLoader::Initialize();
		else
			return FALSE;
	}
	else if (fdwReason == DLL_PROCESS_DETACH)
		FreeLibrary(thisdll);
	return TRUE;
}