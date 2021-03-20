#pragma once
#include "../../Base/Liberation/Liberation.h"

class Patcher
{
public:
	Patcher(void* fnPtr, void* patchPtr);
	void ApplyPatch();
	void ClearPatch();
private:
	char* compiledPatch;
	Patch* instance;
	void* fnPtr;
	void* patchPtr;
	size_t compiledSize;
	bool compiled;
	bool applied;
	void Compile();
};

