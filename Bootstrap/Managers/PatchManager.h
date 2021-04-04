#pragma once
#include "../Base/funchook/include/funchook.h"

class PatchManager
{
public:
	static funchook_t* Instance;
	static bool Initialize();
};

