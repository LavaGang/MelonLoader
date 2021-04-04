#include "PatchManager.h"

funchook_t* PatchManager::Instance = nullptr;

bool PatchManager::Initialize()
{
	Instance = funchook_create();
	return Instance != nullptr;
}