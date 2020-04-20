#pragma once
#include "PointerUtils.h"
typedef void* (*IL2CPPPlayerLoadFirstScene_t) (bool unknown);
typedef bool (*PlayerCleanup_t)(bool dopostquitmsg);
typedef __int64 (*BaseBehaviourManager_CommonUpdate_t) (void* behaviour_manager);
typedef void (*GUIManager_DoGUIEvent_t) (void* __0, void* __1, bool __2);
typedef bool (__fastcall *MonoBehaviour_DoGUI_t) (int a1, __int64 a2, uint32_t a3, uint32_t a4, __int64 a5, uint32_t a6);
typedef char (__fastcall *MonoBehaviourDoGUI_t) (__int64 pthis, uint32_t a1, __int64 a2, uint32_t a3);
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
	static GUIManager_DoGUIEvent_t GUIManager_DoGUIEvent;
	static MonoBehaviour_DoGUI_t MonoBehaviour_DoGUI;
	static MonoBehaviourDoGUI_t MonoBehaviourDoGUI;
	static EndOfFrameCallbacks_DequeAll_t EndOfFrameCallbacks_DequeAll;

	static bool Setup();
};