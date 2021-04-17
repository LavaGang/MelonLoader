#pragma once
#ifdef _WIN32
#include <Windows.h>
#endif

class Assertion
{
public:
	static bool ShouldContinue;
	// dont kill the application if something fails
	static bool DontDie;
	static void ThrowInternalFailure(const char* msg);
};
