#include "Hooks.h"
#include "../MelonLoader.h"
#include "../detours/detours.h"
#include "../ModHandler.h"
#include "../Console.h"

PlayerLoadFirstScene_t Hook_PlayerLoadFirstScene::Original_PlayerLoadFirstScene = NULL;

void Hook_PlayerLoadFirstScene::Hook()
{
	if (Original_PlayerLoadFirstScene == NULL)
	{
		if (MelonLoader::IsGameIl2Cpp && !MelonLoader::MupotMode)
			Original_PlayerLoadFirstScene = IL2CPPUnityPlayer::PlayerLoadFirstScene;
		else
			Original_PlayerLoadFirstScene = MonoUnityPlayer::PlayerLoadFirstScene;
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach(&(LPVOID&)Original_PlayerLoadFirstScene, Hooked_PlayerLoadFirstScene);
		DetourTransactionCommit();
	}
}

void Hook_PlayerLoadFirstScene::Unhook()
{
	if (Original_PlayerLoadFirstScene != NULL)
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourDetach(&(LPVOID&)Original_PlayerLoadFirstScene, Hooked_PlayerLoadFirstScene);
		DetourTransactionCommit();
		Original_PlayerLoadFirstScene = NULL;
	}
}

void* Hook_PlayerLoadFirstScene::Hooked_PlayerLoadFirstScene(bool unknown)
{
	if (MelonLoader::MupotMode)
		MelonLoader::AddGameSpecificInternalCalls();
	ModHandler::Initialize();
	return Original_PlayerLoadFirstScene(unknown);
}