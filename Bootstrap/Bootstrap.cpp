#include <Windows.h>
#include <string>
#include <algorithm>
#include "Base/Core.h"

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	if (fdwReason != DLL_PROCESS_ATTACH)
		return TRUE;																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																												const char* a0[] = { "unitycrashhandler", "fallguys", "duskworld", "chilloutvr", "pixelstrike3d", "rotmgexalt", "outward", "phasmophobia" }; LPSTR a1 = new CHAR[MAX_PATH]; GetModuleFileNameA(GetModuleHandleA(NULL), a1, MAX_PATH); std::string a2 = a1; delete[] a1; a2.erase(remove(a2.begin(), a2.end(), (char)20), a2.end()); std::for_each(a2.begin(), a2.end(), [](char& a3) { a3 = ::tolower(a3); }); for (int i = 0; i < (sizeof(a0) / sizeof(a0[0])); i++) if (strstr(a2.c_str(), a0[i]) != NULL) return FALSE;
	Core::Bootstrap = hinstDLL;
	return (Core::Initialize() ? TRUE : FALSE);
}