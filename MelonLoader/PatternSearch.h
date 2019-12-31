#pragma once
#include <Windows.h>

class PatternSearch
{
public:
	static uintptr_t FindPattern(const uintptr_t& start_address, const uintptr_t& end_address, const char* target_pattern);
	static uintptr_t FindPattern(HMODULE mod, const char* target_pattern);
};