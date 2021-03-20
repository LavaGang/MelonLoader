#pragma once
#ifdef _WIN32
#include <Windows.h>
#endif

class Assertion
{
public:
	static bool ShouldContinue;
	static void ThrowInternalFailure(const char* msg);
};
