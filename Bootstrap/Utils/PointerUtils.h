#pragma once
#include <Windows.h>
#include <stdint.h>
#include <vector>

class PointerUtils
{
public:
	static uintptr_t FindPattern(const uintptr_t& start_address, const uintptr_t& end_address, const char* target_pattern);
	static uintptr_t FindPattern(HMODULE mod, const char* target_pattern);
	static uintptr_t FindPattern(HMODULE mod, const char* start_address_pattern, const char* target_pattern);
	static std::vector<uintptr_t> FindAllPattern(HMODULE mod, const char* target_pattern);
	static std::vector<uintptr_t> FindAllPattern(uintptr_t start_address, uintptr_t end_address, const char* target_pattern);
	static uint64_t ResolvePtrOffset(uint64_t offset32Ptr, uint64_t nextInstructionPtr);
	static uint64_t ResolvePtrOffsetFromInstruction(uint64_t instruction, uint64_t start, uint64_t end);
	static uint64_t ResolvePtrOffsetFromInstructionPattern(HMODULE mod, const char* pattern, uint64_t start, uint64_t end);
};