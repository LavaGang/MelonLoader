#include "MelonLoader.h"
#include "IL2CPPUnityPlayer.h"
#include "Console.h"

IL2CPPPlayerLoadFirstScene_t IL2CPPUnityPlayer::PlayerLoadFirstScene = NULL;

bool IL2CPPUnityPlayer::Setup()
{
	PlayerLoadFirstScene = (IL2CPPPlayerLoadFirstScene_t)PointerUtils::FindPattern(MelonLoader::IL2CPPUnityPlayerDLL, "40 53 48 83 EC 20 0F B6 D9 E8 ? ? ? ? 48 8B C8 E8 ? ? ? ? E8 ? ? ? ? 48 8D 88 ? ? ? ? E8 ? ? ? ? E8 ? ? ? ? 48 85 C0 74 2E E8");

	return true;
}