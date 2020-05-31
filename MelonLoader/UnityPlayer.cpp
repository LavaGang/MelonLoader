#include "UnityPlayer.h"
#include "AssertionManager.h"
#include "MelonLoader.h"
#include "Logger.h"

HMODULE UnityPlayer::Module = NULL;
PlayerLoadFirstScene_t UnityPlayer::PlayerLoadFirstScene = NULL;
PlayerCleanup_t UnityPlayer::PlayerCleanup = NULL;
EndOfFrameCallbacks_DequeAll_t UnityPlayer::EndOfFrameCallbacks_DequeAll = NULL;

bool UnityPlayer::Load()
{
	AssertionManager::Start("UnityPlayer.cpp", "UnityPlayer::Load");
	if (Module == NULL)
		Module = AssertionManager::GetModuleHandlePtr("UnityPlayer");
	return !AssertionManager::Result;
}

bool UnityPlayer::Setup()
{
	AssertionManager::Start("UnityPlayer.cpp", "UnityPlayer::Setup");

	PlayerLoadFirstScene = (PlayerLoadFirstScene_t)AssertionManager::FindBestPossiblePattern(Module, "PlayerLoadFirstScene", {
		"40 53 48 83 EC 20 0F B6 D9 E8 ? ? ? ? 48 8B C8 E8 ? ? ? ? E8 ? ? ? ? 48 8D 88 ? ? ? ? E8 ? ? ? ? E8 ? ? ? ? 48 85 C0 74 2E E8",
		"48 89 5C 24 ? 48 89 74 24 ? 57 48 83 EC 20 0F B6 F1 E8 ? ? ? ? 48 8B C8 E8 ? ? ? ? E8 ? ? ? ? 33 DB 39 98 ? ? ? ? 48 8D B8 ? ? ? ? 76 30 66 66 66 0F 1F 84 00 ? ? ? ? 48 8D",
		"48 89 5C 24 ? 57 48 83 EC 20 0F B6 F9 33 DB 8B D3 48 8D 0D ? ? ? ? E8 ? ? ? ? E8 ? ? ? ? 48 8B C8 E8 ? ? ? ? 48 8B 05 ? ? ? ? 48 85 C0 75 13 48 8D 0D ? ? ? ? E8 ? ? ? ? 48 89 05 ? ? ? ? 48 8B C8 E8",
		"48 89 5C 24 ? 48 89 6C 24 ? 48 89 74 24 ? 57 48 83 EC 20 0F B6 E9 E8 ? ? ? ? 48 8B C8 E8 ? ? ? ? E8 ? ? ? ? 33 FF 48"
	});

	PlayerCleanup = (PlayerCleanup_t)AssertionManager::FindBestPossiblePattern(Module, "PlayerCleanup", {
		"40 53 48 83 EC 60 0F B6 D9 B9 ? ? ? ? E8 ? ? ? ? 33 C9 E8 ? ? ? ? 48 85 C0 0F 84 ? ? ? ? E8 ? ? ? ? 0F 57 C0 48 C7 44 24 ? ? ? ? ? 0F 29 44",
		"40 53 48 83 EC 60 0F B6 D9 33 C9 E8 ? ? ? ? 48 85 C0 0F 84 ? ? ? ? E8 ? ? ? ? 0F 57 C0 48 C7 44 24 ? ? ? ? ? 0F 11 44 24 ? 48",
		"48 89 5C 24 08 57 48 83 EC 70 0F B6 D9 B9 02 00 00 00 E8 ? ? FF FF 33 C9 E8 ? ? AD FF 33 FF 48 85 C0 0F 84 ? ? 00 00",
		"40 53 48 83 EC 20 0F B6 D9 B9 ? ? ? ? E8 ? ? ? ? 0F B6 CB E8 ? ? ? ? 84 C0 75 0F 33 C9 E8"
	});

	if (MelonLoader::IsGameIl2Cpp)
	{
		EndOfFrameCallbacks_DequeAll = (EndOfFrameCallbacks_DequeAll_t)AssertionManager::FindBestPossiblePattern(Module, "EndOfFrameCallbacks_DequeAll", {
			"40 57 48 83 EC 20 48 8B 0D ? ? ? ? 48 63 01 48 8B 7C C1 ? 48 8B CF E8 ? ? ? ? 85 C0 75 ? 48 89 5C 24 ? 48 8B CF E8 ? ? ? ? 48 8B D8 48 8B"
		});
	}

	return !AssertionManager::Result;
}