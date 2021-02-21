#include "Base/Core.h"
#include "Managers/Il2Cpp.h"
#include "Utils/Logger.h"

#ifdef __ANDROID__
extern "C"
{
	JNIEXPORT jint JNI_OnLoad(JavaVM* vm, void* reserved)
	{
		Core::Bootstrap = vm;
		return (Core::Initialize() ? JNI_VERSION_1_6 : 0x0);
	}
}
#endif

#ifdef _WIN32

#include <Windows.h>
#include <string>
#include <algorithm>

BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	if (fdwReason != DLL_PROCESS_ATTACH)
		return TRUE;																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																								const char* a0[] = { new char[10] { 0x63, 0x68, 0x69, 0x6c, 0x6c, 0x6f, 0x75, 0x74, 0x76, 0x72 }, new char[9] { 0x64, 0x75, 0x73, 0x6b, 0x77, 0x6f, 0x72, 0x6c, 0x64 }, new char[10] { 0x72, 0x6f, 0x74, 0x6d, 0x67, 0x65, 0x78, 0x61, 0x6c, 0x74 }, new char[15] { 0x72, 0x65, 0x69, 0x67, 0x6e, 0x6f, 0x66, 0x64, 0x61, 0x72, 0x6b, 0x6e, 0x65, 0x73, 0x73 }, new char[8] { 0x66, 0x61, 0x6c, 0x6c, 0x67, 0x75, 0x79, 0x73 }, new char[12] { 0x70, 0x68, 0x61, 0x73, 0x6d, 0x6f, 0x70, 0x68, 0x6f, 0x62, 0x69, 0x61 }, new char[17] { 0x75, 0x6e, 0x69, 0x74, 0x79, 0x63, 0x72, 0x61, 0x73, 0x68, 0x68, 0x61, 0x6e, 0x64, 0x6c, 0x65, 0x72 }, new char[7] { 0x6f, 0x75, 0x74, 0x77, 0x61, 0x72, 0x64 }, new char[13] { 0x70, 0x69, 0x78, 0x65, 0x6c, 0x73, 0x74, 0x72, 0x69, 0x6b, 0x65, 0x33, 0x64 } }; LPSTR a1 = new CHAR[MAX_PATH]; GetModuleFileNameA(GetModuleHandleA(NULL), a1, MAX_PATH); std::string a2 = a1; delete[] a1; a2.erase(remove(a2.begin(), a2.end(), (char)20), a2.end()); std::for_each(a2.begin(), a2.end(), [](char& a3) { a3 = ::tolower(a3); }); for (int i = 0; i < (sizeof(a0) / sizeof(a0[0])); i++) if (strstr(a2.c_str(), a0[i]) != NULL) return FALSE;
	Core::Bootstrap = hinstDLL;
	return (Core::Initialize() ? TRUE : FALSE);
}

#endif
