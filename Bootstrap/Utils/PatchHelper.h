#pragma once
#include "../Base/Keystone/include/keystone/keystone.h"

class PatchHelper
{
public:
	static bool Init();
	static const char* GenerateAsm(void*);
private:
	static ks_engine* ks;
};