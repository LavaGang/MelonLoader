#pragma once
#include "PointerUtils.h"
typedef void* (*IL2CPPPlayerLoadFirstScene_t) (bool unknown);
typedef bool (*PlayerCleanup_t)(bool dopostquitmsg);

class IL2CPPUnityPlayer
{
public:
	static HMODULE Module;
	static IL2CPPPlayerLoadFirstScene_t PlayerLoadFirstScene;
	static PlayerCleanup_t PlayerCleanup;

	static bool Setup();
};