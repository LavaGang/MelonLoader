#include "MelonLoader.h"
#include "MonoUnityPlayer.h"
#include "Console.h"

voidfunc_t MonoUnityPlayer::UnityMain = NULL;
SingleAppInstance_FindOtherInstance_t MonoUnityPlayer::SingleAppInstance_FindOtherInstance = NULL;
MonoPlayerLoadFirstScene_t MonoUnityPlayer::PlayerLoadFirstScene = NULL;

bool MonoUnityPlayer::Setup()
{
	UnityMain = (voidfunc_t)GetProcAddress(MelonLoader::MonoUnityPlayerDLL, "UnityMain");
	SingleAppInstance_FindOtherInstance = (SingleAppInstance_FindOtherInstance_t)PointerUtils::FindPattern(MelonLoader::MonoUnityPlayerDLL, "40 55 57 41 54 41 56 41 57 48 83 EC 60 48 8D 6C 24 ? 83 79 58 00 48 89 5D 68 48 8B D9 48 89 75 70 75 07 32 C0 E9 ? ? ? ? 48 8B 43 18");
	PlayerLoadFirstScene = (MonoPlayerLoadFirstScene_t)PointerUtils::FindPattern(MelonLoader::MonoUnityPlayerDLL, "40 53 48 83 EC 20 0F B6 D9 E8 ? ? ? ? 48 8B C8 E8 ? ? ? ? E8 ? ? ? ? 48 8D 88 ? ? ? ? E8 ? ? ? ? E8 ? ? ? ? 48 85 C0 74 2E E8");

	return true;
}