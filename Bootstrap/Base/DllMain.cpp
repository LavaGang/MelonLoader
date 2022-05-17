#include "../Core.h"

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	if (fdwReason != DLL_PROCESS_ATTACH)
		return TRUE;
	DisableThreadLibraryCalls(hinstDLL);
	Core::Initialize(hinstDLL);
	return TRUE;
}