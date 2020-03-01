#include "Hooks.h"
#include "../MelonLoader.h"
#include "../detours/detours.h"
#include "../Console.h"

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
			lib = Mono::Module;
			Unhook();
		}
		else
		{
			lib = Original_LoadLibraryW(lpLibFileName);
			if (wcsstr(lpLibFileName, L"GameAssembly.dll"))
			{
				IL2CPPUnityPlayer::Module = PointerUtils::GetModuleHandlePtr("UnityPlayer");
				IL2CPP::Module = lib;
				if (IL2CPP::Setup() && IL2CPPUnityPlayer::Setup())
				{
					if (!MelonLoader::MupotMode)
					{
						Mono::CreateDomain();
						Hook_PlayerLoadFirstScene::Hook();
						Hook_PlayerCleanup::Hook();
					}
					Hook_il2cpp_init::Hook();
					Hook_il2cpp_add_internal_call::Hook();
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
			MonoUnityPlayer::Module = PointerUtils::GetModuleHandlePtr("UnityPlayer");
			Mono::Module = lib;
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