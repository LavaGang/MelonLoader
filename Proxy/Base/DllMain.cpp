#include "../Core.h"

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	if (fdwReason != DLL_PROCESS_ATTACH)
		return TRUE;

	DisableThreadLibraryCalls(hinstDLL);
	core::initialize(hinstDLL);

	return TRUE;
}