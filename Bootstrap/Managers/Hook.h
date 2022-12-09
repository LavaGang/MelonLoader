#pragma once

class Hook
{
public:
	static void Attach(void** target, void* detour);
	static void Detach(void** target, void* detour);
};