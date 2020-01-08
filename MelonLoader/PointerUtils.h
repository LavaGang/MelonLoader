#pragma once
#include <Windows.h>
#include <stdint.h>

class PointerUtils
{
public:
	static uintptr_t FindPattern(const uintptr_t& start_address, const uintptr_t& end_address, const char* target_pattern);
	static uintptr_t FindPattern(HMODULE mod, const char* target_pattern);
	static uint64_t ResolvePtrOffset(uint64_t offset32Ptr, uint64_t nextInstructionPtr);
	static uint64_t ResolveRelativeInstruction(uint64_t instruction);
};