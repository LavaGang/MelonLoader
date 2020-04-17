#include "IL2CPPUnityPlayer.h"
#include "AssertionManager.h"

HMODULE IL2CPPUnityPlayer::Module = NULL;
IL2CPPPlayerLoadFirstScene_t IL2CPPUnityPlayer::PlayerLoadFirstScene = NULL;
PlayerCleanup_t IL2CPPUnityPlayer::PlayerCleanup = NULL;
BaseBehaviourManager_CommonUpdate_t IL2CPPUnityPlayer::BaseBehaviourManager_Update = NULL;
BaseBehaviourManager_CommonUpdate_t IL2CPPUnityPlayer::BaseBehaviourManager_FixedUpdate = NULL;
BaseBehaviourManager_CommonUpdate_t IL2CPPUnityPlayer::BaseBehaviourManager_LateUpdate = NULL;
GUIManager_DoGUIEvent_t IL2CPPUnityPlayer::GUIManager_DoGUIEvent = NULL;

bool IL2CPPUnityPlayer::Setup()
{
	AssertionManager::Start("IL2CPPUnityPlayer.cpp", "IL2CPPUnityPlayer::Setup");

	if (PlayerLoadFirstScene == NULL)
		PlayerLoadFirstScene = (IL2CPPPlayerLoadFirstScene_t)PointerUtils::FindPattern(Module, "40 53 48 83 EC 20 0F B6 D9 E8 ? ? ? ? 48 8B C8 E8 ? ? ? ? E8 ? ? ? ? 48 8D 88 ? ? ? ? E8 ? ? ? ? E8 ? ? ? ? 48 85 C0 74 2E E8");
	if (PlayerLoadFirstScene == NULL)
		PlayerLoadFirstScene = (IL2CPPPlayerLoadFirstScene_t)PointerUtils::FindPattern(Module, "48 89 5C 24 ? 48 89 74 24 ? 57 48 83 EC 20 0F B6 F1 E8 ? ? ? ? 48 8B C8 E8 ? ? ? ? E8 ? ? ? ? 33 DB 39 98 ? ? ? ? 48 8D B8 ? ? ? ? 76 30 66 66 66 0F 1F 84 00 ? ? ? ? 48 8D");
	if (PlayerLoadFirstScene == NULL)
		PlayerLoadFirstScene = (IL2CPPPlayerLoadFirstScene_t)PointerUtils::FindPattern(Module, "48 89 5C 24 ? 57 48 83 EC 20 0F B6 F9 33 DB 8B D3 48 8D 0D ? ? ? ? E8 ? ? ? ? E8 ? ? ? ? 48 8B C8 E8 ? ? ? ? 48 8B 05 ? ? ? ? 48 85 C0 75 13 48 8D 0D ? ? ? ? E8 ? ? ? ? 48 89 05 ? ? ? ? 48 8B C8 E8");
	AssertionManager::Decide(PlayerLoadFirstScene, "PlayerLoadFirstScene");

	if (PlayerCleanup == NULL)
		PlayerCleanup = (PlayerCleanup_t)PointerUtils::FindPattern(Module, "40 53 48 83 EC 60 0F B6 D9 B9 ? ? ? ? E8 ? ? ? ? 33 C9 E8 ? ? ? ? 48 85 C0 0F 84 ? ? ? ? E8 ? ? ? ? 0F 57 C0 48 C7 44 24 ? ? ? ? ? 0F 29 44");
	if (PlayerCleanup == NULL)
		PlayerCleanup = (PlayerCleanup_t)PointerUtils::FindPattern(Module, "40 53 48 83 EC 60 0F B6 D9 33 C9 E8 ? ? ? ? 48 85 C0 0F 84 ? ? ? ? E8 ? ? ? ? 0F 57 C0 48 C7 44 24 ? ? ? ? ? 0F 11 44 24 ? 48");
	AssertionManager::Decide(PlayerCleanup, "PlayerCleanup");

	if ((BaseBehaviourManager_Update == NULL) || (BaseBehaviourManager_FixedUpdate == NULL) || (BaseBehaviourManager_LateUpdate == NULL))
	{
		std::vector<uintptr_t> BaseBehaviourManager_CommonUpdate = PointerUtils::FindAllPattern(Module, "48 89 5C 24 ? 48 89 7C 24 ? 55 48 8B EC 48 81 EC ? ? ? ? 48 8B F9 B2 01 48 8D 4D C0 E8 ? ? ? ? 48 8B CF E8 ? ? ? ? 48 8B 47 08 48");
		if (BaseBehaviourManager_CommonUpdate.size() < 1) // 2019.3.6f1
			BaseBehaviourManager_CommonUpdate = PointerUtils::FindAllPattern(Module, "48 89 5C 24 ? 48 89 7C 24 ? 55 48 8B EC 48 83 EC 60 48 8B F9 B2 01 48 8D 4D C0 E8 ? ? ? ? 48 8B CF E8 ? ? ? ? 48 8B 47 08 48");
		if (BaseBehaviourManager_CommonUpdate.size() > 0)
		{
			if ((BaseBehaviourManager_Update == NULL) && (BaseBehaviourManager_CommonUpdate[0] != NULL))
				BaseBehaviourManager_Update = (BaseBehaviourManager_CommonUpdate_t)BaseBehaviourManager_CommonUpdate[0];
			if ((BaseBehaviourManager_FixedUpdate == NULL) && (BaseBehaviourManager_CommonUpdate[1] != NULL))
				BaseBehaviourManager_FixedUpdate = (BaseBehaviourManager_CommonUpdate_t)BaseBehaviourManager_CommonUpdate[1];
			if ((BaseBehaviourManager_LateUpdate == NULL) && (BaseBehaviourManager_CommonUpdate[2] != NULL))
				BaseBehaviourManager_LateUpdate = (BaseBehaviourManager_CommonUpdate_t)BaseBehaviourManager_CommonUpdate[2];
		}
		else
			AssertionManager::ThrowError("Failed to FindAllPattern ( BaseBehaviourManager_CommonUpdate )");
	}
	AssertionManager::Decide(BaseBehaviourManager_Update, "BaseBehaviourManager_Update");
	AssertionManager::Decide(BaseBehaviourManager_FixedUpdate, "BaseBehaviourManager_FixedUpdate");
	AssertionManager::Decide(BaseBehaviourManager_LateUpdate, "BaseBehaviourManager_LateUpdate");

	if (GUIManager_DoGUIEvent == NULL)
		GUIManager_DoGUIEvent = (GUIManager_DoGUIEvent_t)PointerUtils::FindPattern(Module, "44 88 44 24 ? 48 89 54 24 ? 48 89 4C 24 ? 55 53 56 57 41 54 41 55 41 56 41 57 48 8D 6C 24 ? 48 81 EC ? ? ? ? 4C 63 72 2C 4C 8D 25 ? ? ? ? 4C 8B FA 44 89 74 24 ? 48");
	if (GUIManager_DoGUIEvent == NULL)
		GUIManager_DoGUIEvent = (GUIManager_DoGUIEvent_t)PointerUtils::FindPattern(Module, "44 88 44 24 ? 48 89 54 24 ? 48 89 4C 24 ? 55 53 56 57 41 54 41 55 41 56 41 57 48 8D 6C 24 ? 48 81 EC ? ? ? ? 48 63 5A 34 48 8D 05 ? ? ? ? 48 8B F2 89 5C 24 58 4C 8B");
	AssertionManager::Decide(GUIManager_DoGUIEvent, "GUIManager_DoGUIEvent");

	return !AssertionManager::Result;
}