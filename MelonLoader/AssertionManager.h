#pragma once
#include <string>
#include <Windows.h>

class AssertionManager
{
public:
	static bool Result;

	static void Start(const char* filename, const char* position);
	static void Decide(void* thing, const char* name);
	static HMODULE LoadLib(const char* name, const char* filepath);
	static HMODULE GetModuleHandlePtr(const char* name);
	static FARPROC GetExport(HMODULE mod, const char* export_name);
	static uintptr_t FindPattern(HMODULE mod, const char* name, const char* target_pattern);

private:
	static const char* FileName;
	static const char* Position;
};