#include "pch.h"
#include "Patcher.h"

#include <android/log.h>



#include "Utils/Debug.h"
#include "Utils/Logger.h"
#include "Utils/PatchHelper.h"

Patcher::Patcher(void* fnPtr, void* patchPtr)
{
	this->fnPtr = fnPtr;
	this->patchPtr = patchPtr;

	compiled = false;
	applied = false;
	compiledPatch = nullptr;
	instance = nullptr;
	
	Compile();
}

void Patcher::ApplyPatch()
{
	if (applied || !compiled)
		return;

	applied = true;
	instance->Apply();
	
	if (Debug::Enabled)
	{
		__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "[%p] Applying Patch", fnPtr);
		int value;
		memcpy(&value, (const void*)fnPtr, 2);
		__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "[%p] Current Instructions", value);
	}
}

void Patcher::ClearPatch()
{
	if (!applied)
		return;

	applied = false;
	instance->Reset();
	
	if (Debug::Enabled)
	{
		__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "[%p] Removing Patch", fnPtr);
		int value;
		memcpy(&value, (const void*)fnPtr, 2);
		__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "[%p] Current Instructions", value);
	}
}

void Patcher::Compile()
{
	if (applied)
		ClearPatch();
	
	compiled = PatchHelper::GenerateAsm(patchPtr, (unsigned char**)&compiledPatch, &compiledSize);

	if (compiled)
		instance = Patch::Setup(fnPtr, compiledPatch, compiledSize);
}

