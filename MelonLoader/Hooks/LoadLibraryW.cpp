#include "Hooks.h"
#include "../MelonLoader.h"
#include "../detours/detours.h"
#include "../MonoUnityPlayer.h"
#include "../PointerUtils.h"
#include "../IL2CPPUnityPlayer.h"

LoadLibraryW_t Hook_LoadLibraryW::Original_LoadLibraryW = NULL;

void Hook_LoadLibraryW::Hook()
{
	if (Original_LoadLibraryW == NULL)
	{
		Original_LoadLibraryW = LoadLibraryW;
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach(&(LPVOID&)Original_LoadLibraryW, Hooked_LoadLibraryW);
		DetourTransactionCommit();
	}
}

void Hook_LoadLibraryW::Unhook()
{
	if (Original_LoadLibraryW != NULL)
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourDetach(&(LPVOID&)Original_LoadLibraryW, Hooked_LoadLibraryW);
		DetourTransactionCommit();
		Original_LoadLibraryW = NULL;
	}
}

HMODULE __stdcall Hook_LoadLibraryW::Hooked_LoadLibraryW(LPCWSTR lpLibFileName)
{
	HMODULE lib = NULL;
	if (MelonLoader::IsGameIl2Cpp)
	{
		if (wcsstr(lpLibFileName, L"mono.dll") || wcsstr(lpLibFileName, L"mono-2.0-bdwgc.dll") || wcsstr(lpLibFileName, L"mono-2.0-sgen.dll"))
		{
			lib = MelonLoader::MonoDLL;
			Unhook();
		}
		else
		{
			lib = Original_LoadLibraryW(lpLibFileName);
			if (wcsstr(lpLibFileName, L"GameAssembly.dll"))
			{
				MelonLoader::IL2CPPUnityPlayerDLL = PointerUtils::GetModuleHandlePtr("UnityPlayer");
				MelonLoader::GameAssemblyDLL = lib;
				if (IL2CPP::Setup() && IL2CPPUnityPlayer::Setup())
				{
					if (!MelonLoader::MupotMode)
						Mono::CreateDomain();

					Hook_il2cpp_init::Hook();
					Hook_il2cpp_add_internal_call::Hook();
					Hook_PlayerLoadFirstScene::Hook();
				}
				else
					Unhook();
			}
		}
	}
	else
	{
		lib = Original_LoadLibraryW(lpLibFileName);
		if (wcsstr(lpLibFileName, L"mono.dll") || wcsstr(lpLibFileName, L"mono-2.0-bdwgc.dll") || wcsstr(lpLibFileName, L"mono-2.0-sgen.dll"))
		{
			MelonLoader::MonoUnityPlayerDLL = PointerUtils::GetModuleHandlePtr("UnityPlayer");
			MelonLoader::MonoDLL = lib;
			if (Mono::Setup() && MonoUnityPlayer::Setup())
			{
				Hook_mono_jit_init_version::Hook();
				Hook_PlayerLoadFirstScene::Hook();
			}
			Unhook();
		}
	}
	return lib;
}