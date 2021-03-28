#ifdef PORT_DISABLE
#pragma once
#include "../../Base/Liberation/Liberation.h"

class Patcher
{
public:
	Patcher(void* fnPtr, void* patchPtr);
	void ApplyPatch();
	void ClearPatch();
	void* patchPtr;
private:
	char* compiledPatch;
	Patch* instance;
	void* fnPtr;
	size_t compiledSize;
	bool compiled;
	bool applied;
	void Compile();
};
#endif