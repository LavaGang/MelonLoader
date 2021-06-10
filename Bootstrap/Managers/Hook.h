#pragma once
#include <vector>

class Hook
{
public:
	static void Attach(void** target, void* detour);
	static void Detach(void** target, void* detour);
};