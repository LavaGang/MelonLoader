#pragma once
#include "PointerUtils.h"

typedef bool (*SingleAppInstance_FindOtherInstance_t) (LPARAM lParam);
typedef void* (*MonoPlayerLoadFirstScene_t) (bool unknown);

class MonoUnityPlayer
{
public:
	static HMODULE Module;
	static voidfunc_t UnityMain;
	static SingleAppInstance_FindOtherInstance_t SingleAppInstance_FindOtherInstance;
	static MonoPlayerLoadFirstScene_t PlayerLoadFirstScene;

	static bool Load();
	static bool Setup();
};