#include "MonoUnityPlayer.h"
#include "MelonLoader.h"
#include "AssertionManager.h"

HMODULE MonoUnityPlayer::Module = NULL;
voidfunc_t MonoUnityPlayer::UnityMain = NULL;
SingleAppInstance_FindOtherInstance_t MonoUnityPlayer::SingleAppInstance_FindOtherInstance = NULL;
MonoPlayerLoadFirstScene_t MonoUnityPlayer::PlayerLoadFirstScene = NULL;

bool MonoUnityPlayer::Load()
{
	AssertionManager::Start("MonoUnityPlayer.cpp", "MonoUnityPlayer::Load");

	if (MelonLoader::Is64bit())
		Module = AssertionManager::LoadLib("Module", (std::string(MelonLoader::GamePath) + "\\MelonLoader\\Mono\\MonoUnityPlayer_x64.dll").c_str());
	else
		Module = AssertionManager::LoadLib("Module", (std::string(MelonLoader::GamePath) + "\\MelonLoader\\Mono\\MonoUnityPlayer_x86.dll").c_str());
	return !AssertionManager::Result;
}

bool MonoUnityPlayer::Setup()
{
	AssertionManager::Start("MonoUnityPlayer.cpp", "MonoUnityPlayer::Setup");

	UnityMain = (voidfunc_t)AssertionManager::GetExport(Module, "UnityMain");
	//SingleAppInstance_FindOtherInstance = (SingleAppInstance_FindOtherInstance_t)AssertionManager::FindPattern(Module, "SingleAppInstance_FindOtherInstance", "40 55 57 41 54 41 56 41 57 48 83 EC 60 48 8D 6C 24 ? 83 79 58 00 48 89 5D 68 48 8B D9 48 89 75 70 75 07 32 C0 E9 ? ? ? ? 48 8B 43 18");

	PlayerLoadFirstScene = (MonoPlayerLoadFirstScene_t)PointerUtils::FindPattern(Module, "40 53 48 83 EC 20 0F B6 D9 E8 ? ? ? ? 48 8B C8 E8 ? ? ? ? E8 ? ? ? ? 48 8D 88 ? ? ? ? E8 ? ? ? ? E8 ? ? ? ? 48 85 C0 74 2E E8");
	if (PlayerLoadFirstScene == NULL)
		PlayerLoadFirstScene = (MonoPlayerLoadFirstScene_t)PointerUtils::FindPattern(Module, "48 89 5C 24 ? 48 89 74 24 ? 57 48 83 EC 20 0F B6 F1 E8 ? ? ? ? 48 8B C8 E8 ? ? ? ? E8 ? ? ? ? 33 DB 39 98 ? ? ? ? 48 8D B8 ? ? ? ? 76 30 66 66 66 0F 1F 84 00 ? ? ? ? 48 8D");
	if (PlayerLoadFirstScene == NULL)
		PlayerLoadFirstScene = (MonoPlayerLoadFirstScene_t)PointerUtils::FindPattern(Module, "48 89 5C 24 ? 57 48 83 EC 20 0F B6 F9 33 DB 8B D3 48 8D 0D ? ? ? ? E8 ? ? ? ? E8 ? ? ? ? 48 8B C8 E8 ? ? ? ? 48 8B 05 ? ? ? ? 48 85 C0 75 13 48 8D 0D ? ? ? ? E8 ? ? ? ? 48 89 05 ? ? ? ? 48 8B C8 E8");
	AssertionManager::Decide(PlayerLoadFirstScene, "PlayerLoadFirstScene");

	return !AssertionManager::Result;
}