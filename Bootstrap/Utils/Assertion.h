#pragma once
#include <Windows.h>

class Assertion
{
public:
	static bool ShouldContinue;
	static void ThrowInternalFailure(const char* msg);
	static FARPROC GetExport(HMODULE mod, const char* export_name, bool internalfailure = true);
};