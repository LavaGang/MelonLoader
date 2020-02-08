#pragma once
#include "PointerUtils.h"
typedef bool (*SingleAppInstance_FindOtherInstance_t) (LPARAM lParam);
typedef void* (*IL2CPPPlayerLoadFirstScene_t) (bool unknown);

class IL2CPPUnityPlayer
{
public:
	static IL2CPPPlayerLoadFirstScene_t PlayerLoadFirstScene;

	static bool Setup();
};