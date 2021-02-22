#pragma once
#include "../Base/Keystone/include/keystone/keystone.h"

class PatchHelper
{
public:
	static bool Init();
	static bool GenerateAsm(void*, unsigned char**, size_t*);
private:
	static ks_engine* ks;
};