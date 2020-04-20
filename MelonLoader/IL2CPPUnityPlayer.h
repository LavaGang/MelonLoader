#pragma once
#include "PointerUtils.h"
typedef void* (*IL2CPPPlayerLoadFirstScene_t) (bool unknown);
typedef bool (*PlayerCleanup_t)(bool dopostquitmsg);
typedef __int64 (*BaseBehaviourManager_CommonUpdate_t) (void* behaviour_manager);
//typedef void (*GUIManager_DoGUIEvent_t) (void* __0, void* __1, bool __2);
typedef void (*EndOfFrameCallbacks_DequeAll_t)();

class IL2CPPUnityPlayer
{
public:
	static HMODULE Module;
	static IL2CPPPlayerLoadFirstScene_t PlayerLoadFirstScene;
	static PlayerCleanup_t PlayerCleanup;
	static BaseBehaviourManager_CommonUpdate_t BaseBehaviourManager_Update;
	static BaseBehaviourManager_CommonUpdate_t BaseBehaviourManager_FixedUpdate;
	static BaseBehaviourManager_CommonUpdate_t BaseBehaviourManager_LateUpdate;
	//static GUIManager_DoGUIEvent_t GUIManager_DoGUIEvent;
	static EndOfFrameCallbacks_DequeAll_t EndOfFrameCallbacks_DequeAll;

	static bool Setup();
};