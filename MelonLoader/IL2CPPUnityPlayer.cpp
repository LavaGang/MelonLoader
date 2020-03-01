#include "MelonLoader.h"
#include "IL2CPPUnityPlayer.h"
#include "Console.h"

HMODULE IL2CPPUnityPlayer::Module = NULL;
IL2CPPPlayerLoadFirstScene_t IL2CPPUnityPlayer::PlayerLoadFirstScene = NULL;
PlayerCleanup_t IL2CPPUnityPlayer::PlayerCleanup = NULL;

bool IL2CPPUnityPlayer::Setup()
{
	PlayerLoadFirstScene = (IL2CPPPlayerLoadFirstScene_t)PointerUtils::FindPattern(Module, "40 53 48 83 EC 20 0F B6 D9 E8 ? ? ? ? 48 8B C8 E8 ? ? ? ? E8 ? ? ? ? 48 8D 88 ? ? ? ? E8 ? ? ? ? E8 ? ? ? ? 48 85 C0 74 2E E8");
	if (PlayerLoadFirstScene == NULL)
		PlayerLoadFirstScene = (IL2CPPPlayerLoadFirstScene_t)PointerUtils::FindPattern(Module, "48 89 5C 24 ? 48 89 74 24 ? 57 48 83 EC 20 0F B6 F1 E8 ? ? ? ? 48 8B C8 E8 ? ? ? ? E8 ? ? ? ? 33 DB 39 98 ? ? ? ? 48 8D B8 ? ? ? ? 76 30 66 66 66 0F 1F 84 00 ? ? ? ? 48 8D");
	if (PlayerLoadFirstScene == NULL)
		PlayerLoadFirstScene = (IL2CPPPlayerLoadFirstScene_t)PointerUtils::FindPattern(Module, "48 89 5C 24 ? 57 48 83 EC 20 0F B6 F9 33 DB 8B D3 48 8D 0D ? ? ? ? E8 ? ? ? ? E8 ? ? ? ? 48 8B C8 E8 ? ? ? ? 48 8B 05 ? ? ? ? 48 85 C0 75 13 48 8D 0D ? ? ? ? E8 ? ? ? ? 48 89 05 ? ? ? ? 48 8B C8 E8");

	PlayerCleanup = (PlayerCleanup_t)PointerUtils::FindPattern(Module, "40 53 48 83 EC 60 0F B6 D9 B9 ? ? ? ? E8 ? ? ? ? 33 C9 E8 ? ? ? ? 48 85 C0 0F 84 ? ? ? ? E8 ? ? ? ? 0F 57 C0 48 C7 44 24 ? ? ? ? ? 0F 29 44");
	if (PlayerCleanup == NULL)
		PlayerCleanup = (PlayerCleanup_t)PointerUtils::FindPattern(Module, "40 53 48 83 EC 60 0F B6 D9 33 C9 E8 ? ? ? ? 48 85 C0 0F 84 ? ? ? ? E8 ? ? ? ? 0F 57 C0 48 C7 44 24 ? ? ? ? ? 0F 11 44 24 ? 48");

	return true;
}