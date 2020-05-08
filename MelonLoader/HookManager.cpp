#include <string>
#include "HookManager.h"
#include "ModHandler.h"
#include "UnityPlayer.h"
#include "MelonLoader.h"
#include "Console.h"
#include "detours/detours.h"
#include "AssertionManager.h"
#include "Logger.h"
#include "Exports.h"

#pragma region Core
std::vector<HookManager_Hook*>HookManager::HookTbl;
HookManager_Hook* HookManager::FindHook(void** target, void* detour)
{
	HookManager_Hook* returnval = NULL;
	size_t HookTblSize = HookTbl.size();
	if (HookTblSize > 0)
	{
		for (size_t i = 0; i < HookTblSize; i++)
		{
			HookManager_Hook* hook = HookTbl[i];
			if ((hook != NULL) && (hook->Target == target) && (hook->Detour == detour))
			{
				returnval = hook;
				break;
			}
		}
	}
	return returnval;
}

void HookManager::Hook(void** target, void* detour)
{
	if ((target != NULL) && (detour != NULL))
	{
		HookManager_Hook* hook = FindHook(target, detour);
		if (hook == NULL)
		{
			hook = new HookManager_Hook(target, detour);
			HookTbl.push_back(hook);
			INTERNAL_Hook(target, detour);
		}
	}
}

void HookManager::Unhook(void** target, void* detour)
{
	if ((target != NULL) && (detour != NULL))
	{
		HookManager_Hook* hook = FindHook(target, detour);
		if (hook != NULL)
		{
			HookTbl.erase(std::find(HookManager::HookTbl.begin(), HookManager::HookTbl.end(), hook));
			delete hook;
			INTERNAL_Unhook(target, detour);
		}
	}
}

void HookManager::UnhookAll()
{
	size_t HookTblSize = HookTbl.size();
	if (HookTblSize < 0)
		return;
	for (size_t i = 0; i < HookTblSize; i++)
	{
		HookManager_Hook* hook = HookTbl[i];
		if (hook != NULL)
		{
			INTERNAL_Unhook(hook->Target, hook->Detour);
			delete hook;
		}
	}
	HookTbl.clear();
	LoadLibraryW_Unhook();
}

void HookManager::INTERNAL_Hook(void** target, void* detour)
{
	if ((target != NULL) && (detour != NULL))
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourAttach(target, detour);
		DetourTransactionCommit();
	}
}

void HookManager::INTERNAL_Unhook(void** target, void* detour)
{
	if ((target != NULL) && (detour != NULL))
	{
		DetourTransactionBegin();
		DetourUpdateThread(GetCurrentThread());
		DetourDetach(target, detour);
		DetourTransactionCommit();
	}
}
#pragma endregion


#pragma region LoadLibraryW
LoadLibraryW_t HookManager::Original_LoadLibraryW = NULL;
void HookManager::LoadLibraryW_Hook()
{
	if (Original_LoadLibraryW == NULL)
	{
		Original_LoadLibraryW = LoadLibraryW;
		HookManager::INTERNAL_Hook(&(LPVOID&)Original_LoadLibraryW, HookManager::Hooked_LoadLibraryW);
	}
}
void HookManager::LoadLibraryW_Unhook()
{
	if (Original_LoadLibraryW != NULL)
	{
		HookManager::INTERNAL_Unhook(&(LPVOID&)Original_LoadLibraryW, HookManager::Hooked_LoadLibraryW);
		Original_LoadLibraryW = NULL;
	}
}
HMODULE __stdcall HookManager::Hooked_LoadLibraryW(LPCWSTR lpLibFileName)
{
	HMODULE lib = Original_LoadLibraryW(lpLibFileName);
	if (MelonLoader::IsGameIl2Cpp)
	{
		if (wcsstr(lpLibFileName, L"GameAssembly.dll"))
		{
			IL2CPP::Module = lib;
			if (IL2CPP::Setup() && UnityPlayer::Load() && UnityPlayer::Setup())
			{
				Mono::CreateDomain();
				HookManager::Hook(&(LPVOID&)IL2CPP::il2cpp_init, Hooked_il2cpp_init);
				HookManager::Hook(&(LPVOID&)IL2CPP::il2cpp_add_internal_call, Hooked_add_internal_call);
				HookManager::Hook(&(LPVOID&)UnityPlayer::PlayerLoadFirstScene, Hooked_PlayerLoadFirstScene);
				HookManager::Hook(&(LPVOID&)UnityPlayer::PlayerCleanup, Hooked_PlayerCleanup);
				HookManager::Hook(&(LPVOID&)UnityPlayer::EndOfFrameCallbacks_DequeAll, Hooked_EndOfFrameCallbacks_DequeAll);
			}
			LoadLibraryW_Unhook();
		}
	}
	else
	{
		Mono::IsOldMono = wcsstr(lpLibFileName, L"mono.dll");
		if (Mono::IsOldMono || wcsstr(lpLibFileName, L"mono-2.0-bdwgc.dll") || wcsstr(lpLibFileName, L"mono-2.0-sgen.dll") || wcsstr(lpLibFileName, L"mono-2.0-boehm.dll"))
		{
			Mono::Module = lib;

			LPSTR filepath = new CHAR[MAX_PATH];
			GetModuleFileName(Mono::Module, filepath, MAX_PATH);
			std::string filepathstr = filepath;
			filepathstr = filepathstr.substr(0, filepathstr.find_last_of("\\/")).substr(0, filepathstr.find_last_of("\\/"));
			std::string configpath = filepathstr + "\\etc";
			Mono::ConfigPath = new char[configpath.size() + 1];
			std::copy(configpath.begin(), configpath.end(), Mono::ConfigPath);
			Mono::ConfigPath[configpath.size()] = '\0';

			if (Mono::Setup() && UnityPlayer::Load() && UnityPlayer::Setup())
			{
				HookManager::Hook(&(LPVOID&)Mono::mono_jit_init_version, Hooked_mono_jit_init_version);
				HookManager::Hook(&(LPVOID&)Mono::mono_add_internal_call, Hooked_add_internal_call);
				HookManager::Hook(&(LPVOID&)UnityPlayer::PlayerLoadFirstScene, Hooked_PlayerLoadFirstScene);
				HookManager::Hook(&(LPVOID&)UnityPlayer::PlayerCleanup, Hooked_PlayerCleanup);
			}
			LoadLibraryW_Unhook();
		}
	}
	return lib;
}
#pragma endregion


#pragma region il2cpp_init
Il2CppDomain* HookManager::Hooked_il2cpp_init(const char* name)
{
	Exports::AddInternalCalls();
	ModHandler::Initialize();
	IL2CPP::Domain = IL2CPP::il2cpp_init(name);
	HookManager::Unhook(&(LPVOID&)IL2CPP::il2cpp_init, Hooked_il2cpp_init);
	return IL2CPP::Domain;
}
#pragma endregion


#pragma region mono_jit_init_version
MonoDomain* HookManager::Hooked_mono_jit_init_version(const char* name, const char* version)
{
	HookManager::Unhook(&(LPVOID&)Mono::mono_jit_init_version, Hooked_mono_jit_init_version);
	Mono::Domain = Mono::mono_jit_init_version(name, version);
	Exports::AddInternalCalls();
	ModHandler::Initialize();
	return Mono::Domain;
}
#pragma endregion


#pragma region add_internal_call
void HookManager::Hooked_add_internal_call(const char* name, void* method)
{
	if (MelonLoader::IsGameIl2Cpp)
		IL2CPP::il2cpp_add_internal_call(name, method);
	Mono::mono_add_internal_call(name, method);
	if (strstr(name, "UnityEngine.Application::get_unityVersion"))
		Mono::mono_add_internal_call("MelonLoader.Imports::GetUnityVersion", method);
	else if (strstr(name, "UnityEngine.Application::get_version"))
		Mono::mono_add_internal_call("MelonLoader.Imports::GetGameVersion", method);
}
#pragma endregion


#pragma region PlayerLoadFirstScene
void* HookManager::Hooked_PlayerLoadFirstScene(bool unknown)
{
	ModHandler::OnApplicationStart();
	return UnityPlayer::PlayerLoadFirstScene(unknown);
}
#pragma endregion


#pragma region PlayerCleanup
bool HookManager::Hooked_PlayerCleanup(bool dopostquitmsg)
{
	MelonLoader::UNLOAD();
	if (MelonLoader::QuitFix)
		MelonLoader::KillProcess();
	return UnityPlayer::PlayerCleanup(dopostquitmsg);
}
#pragma endregion

#pragma region EndOfFrameCallbacks_DequeAll
void HookManager::Hooked_EndOfFrameCallbacks_DequeAll()
{
	ModHandler::MelonCoroutines_ProcessWaitForEndOfFrame();
	UnityPlayer::EndOfFrameCallbacks_DequeAll();
}
#pragma endregion