#pragma once
#include "../../Base/Keystone/include/keystone/keystone.h"
#include <map>

#include "Patcher.h"

class PatchHelper
{
public:
	static bool Init();
	static void Attach(void*, void*);
	static void Detach(void*, void*);
	static bool GenerateAsm(void*, unsigned char**, size_t*);
private:
	static ks_engine* ks;
	// static std::map<void*, Patcher*> patchMap;
};
