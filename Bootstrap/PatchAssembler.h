#pragma once
#include "Base/Keystone/include/keystone/keystone.h"

class PatchAssembler
{
public:
	static void Init();
	static void Free();
	static ks_engine* ks;
};

