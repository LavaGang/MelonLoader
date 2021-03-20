#pragma once
#ifdef _WIN32
#include <Windows.h>
#endif
class ImportLibHelper
{
public:
#ifdef _WIN32
	static FARPROC GetExport(HMODULE mod, const char* export_name);
#elif defined(__ANDROID__)
	static void* GetExport(void* mod, const char* export_name);
#endif
};

