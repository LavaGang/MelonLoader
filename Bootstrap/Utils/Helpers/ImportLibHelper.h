#pragma once
#ifdef _WIN32
#include <Windows.h>
#elif defined(__ANDROID__)
#include <stdint.h>
#endif
class ImportLibHelper
{
public:
#ifdef _WIN32
	static FARPROC GetExport(HMODULE mod, const char* export_name);
#elif defined(__ANDROID__)
	static void* GetExport(void* mod, const char* export_name);
	static void* GetInternalExport(void* mod, const char* ref_name, uint64_t ref_lib_addr, uint64_t dest_lib_addr);
#endif
};

