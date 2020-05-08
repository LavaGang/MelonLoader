#pragma once
#include "PointerUtils.h"

typedef void* (*PlayerLoadFirstScene_t) (bool unknown);
typedef bool (*PlayerCleanup_t)(bool dopostquitmsg);
typedef void (*EndOfFrameCallbacks_DequeAll_t)();

class UnityPlayer
{
public:
	static HMODULE Module;
	static PlayerLoadFirstScene_t PlayerLoadFirstScene;
	static PlayerCleanup_t PlayerCleanup;
	static EndOfFrameCallbacks_DequeAll_t EndOfFrameCallbacks_DequeAll;

	static bool Load();
	static bool Setup();
};