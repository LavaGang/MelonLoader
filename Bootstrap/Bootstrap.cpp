#include <Windows.h>
#include "Base/Core.h"

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	if (fdwReason != DLL_PROCESS_ATTACH)
		return TRUE;

	Core::Bootstrap = hinstDLL;
	return (Core::Initialize() ? TRUE : FALSE);
}