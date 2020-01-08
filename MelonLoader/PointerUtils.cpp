#include "PointerUtils.h"
#include <cstdint>
#include <psapi.h>

#define InRange(x, a, b) (x >= a && x <= b) 
#define GetBits(x) (InRange((x & (~0x20)), 'A', 'F') ? ((x & (~0x20)) - 'A' + 0xA): (InRange(x, '0', '9') ? x - '0': 0))
#define GetByte(x) (GetBits(x[0]) << 4 | GetBits(x[1]))

uintptr_t PointerUtils::FindPattern(const uintptr_t& start_address, const uintptr_t& end_address, const char* target_pattern)
{
	const char* pattern = target_pattern;
	uintptr_t first_match = NULL;
	for (uintptr_t position = start_address; position < end_address; position++)
	{
		if (!*pattern)
			return first_match;
		const uint8_t pattern_current = *reinterpret_cast<const uint8_t*>(pattern);
		const uint8_t memory_current = *reinterpret_cast<const uint8_t*>(position);
		if ((pattern_current == '\?') || (memory_current == GetByte(pattern)))
		{
			if (!first_match)
				first_match = position;
			if (!pattern[2])
				return first_match;
			pattern += ((pattern_current != '\?') ? 3 : 2);
		}
		else
		{
			pattern = target_pattern;
			first_match = NULL;
		}
	}
	return NULL;
}

uintptr_t PointerUtils::FindPattern(HMODULE mod, const char* target_pattern)
{
	MODULEINFO module_info = { 0 };
	if (!GetModuleInformation(GetCurrentProcess(), mod, &module_info, sizeof(MODULEINFO)))
		return NULL;
	const uintptr_t start_address = uintptr_t(module_info.lpBaseOfDll);
	const uintptr_t end_address = (start_address + module_info.SizeOfImage);
	return FindPattern(start_address, end_address, target_pattern);
}

uint64_t PointerUtils::ResolvePtrOffset(uint64_t offset32Ptr, uint64_t nextInstructionPtr)
{
	uint32_t jmpOffset = *(uint32_t*)offset32Ptr;
	uint32_t valueUInt = *(uint32_t*)nextInstructionPtr;
	uint64_t delta = (nextInstructionPtr - valueUInt);
	uint32_t newPtrInt = (valueUInt + jmpOffset);
	return delta + newPtrInt;
}

uint64_t PointerUtils::ResolveRelativeInstruction(uint64_t instruction)
{
	byte opcode = *(byte*)instruction;
	if ((opcode != 0xE8) && (opcode != 0xE9))
		return NULL;
	return ResolvePtrOffset((instruction + 1), (instruction + 5));
}