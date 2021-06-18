#pragma once
#ifdef _WIN32
#include <Windows.h>
#endif
#include <stdarg.h>

class Assertion
{
public:
	static bool ShouldContinue;
	// dont kill the application if something fails
	static bool DontDie;
	// return false for that extra syntax sugar
	static bool ThrowInternalFailure(const char* msg);
	static bool ThrowInternalFailuref(const char* fmt, ...);
	static bool vThrowInternalFailuref(const char* fmt, va_list args);
};
