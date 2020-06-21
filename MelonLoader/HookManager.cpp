#include <string>
#include "HookManager.h"
#include "ModHandler.h"
#include "MelonLoader.h"
#include "Console.h"
#include "Detours/detours.h"
#include "AssertionManager.h"
#include "Logger.h"
#include "Exports.h"
#include <list>

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

#pragma region AllocConsole
AllocConsole_t HookManager::Original_AllocConsole = NULL;
void HookManager::AllocConsole_Hook()
{
	if (Original_AllocConsole == NULL)
	{
		Original_AllocConsole = AllocConsole;
		HookManager::INTERNAL_Hook(&(LPVOID&)Original_AllocConsole, HookManager::Hooked_AllocConsole);
	}
}
void HookManager::AllocConsole_Unhook()
{
	if (Original_AllocConsole != NULL)
	{
		HookManager::INTERNAL_Unhook(&(LPVOID&)Original_AllocConsole, HookManager::Hooked_AllocConsole);
		Original_AllocConsole = NULL;
	}
}
BOOL HookManager::Hooked_AllocConsole()
{
	if (MelonLoader::DebugMode || MelonLoader::ConsoleEnabled)
		return FALSE;
	return Original_AllocConsole();
}
#pragma endregion

#pragma region GetConsoleWindow
GetConsoleWindow_t HookManager::Original_GetConsoleWindow = NULL;
void HookManager::GetConsoleWindow_Hook()
{
	if (Original_GetConsoleWindow == NULL)
	{
		Original_GetConsoleWindow = GetConsoleWindow;
		HookManager::INTERNAL_Hook(&(LPVOID&)Original_GetConsoleWindow, HookManager::Hooked_GetConsoleWindow);
	}
}
void HookManager::GetConsoleWindow_Unhook()
{
	if (Original_GetConsoleWindow != NULL)
	{
		HookManager::INTERNAL_Unhook(&(LPVOID&)Original_GetConsoleWindow, HookManager::Hooked_GetConsoleWindow);
		Original_GetConsoleWindow = NULL;
	}
}
HWND HookManager::Hooked_GetConsoleWindow()
{
	return Console::hwndConsole;
}
#pragma endregion

#pragma region CloseWindow
CloseWindow_t HookManager::Original_CloseWindow = NULL;
void HookManager::CloseWindow_Hook()
{
	if (Original_CloseWindow == NULL)
	{
		Original_CloseWindow = CloseWindow;
		HookManager::INTERNAL_Hook(&(LPVOID&)Original_CloseWindow, HookManager::Hooked_CloseWindow);
	}
}
void HookManager::CloseWindow_Unhook()
{
	if (Original_CloseWindow != NULL)
	{
		HookManager::INTERNAL_Unhook(&(LPVOID&)Original_CloseWindow, HookManager::Hooked_CloseWindow);
		Original_CloseWindow = NULL;
	}
}
BOOL HookManager::Hooked_CloseWindow(HWND hwnd)
{
	if (hwnd == Console::hwndConsole)
		return FALSE;
	return Original_CloseWindow(hwnd);
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
			if (Il2Cpp::Setup(lib))
			{
				Mono::CreateDomain();
				HookManager::Hook(&(LPVOID&)Il2Cpp::il2cpp_init, Hooked_il2cpp_init);
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
			if (Mono::Setup())
				HookManager::Hook(&(LPVOID&)Mono::mono_jit_init_version, Hooked_mono_jit_init_version);
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
	Il2Cpp::Domain = Il2Cpp::il2cpp_init(name);
	HookManager::Unhook(&(LPVOID&)Il2Cpp::il2cpp_init, Hooked_il2cpp_init);
	return Il2Cpp::Domain;
}
#pragma endregion

#pragma region mono_jit_init_version
MonoDomain* HookManager::Hooked_mono_jit_init_version(const char* name, const char* version)
{
	HookManager::Unhook(&(LPVOID&)Mono::mono_jit_init_version, Hooked_mono_jit_init_version);
	Mono::Domain = Mono::mono_jit_init_version(name, version);
	Mono::FixDomainBaseDir();
	Exports::AddInternalCalls();
	ModHandler::Initialize();
	return Mono::Domain;
}
#pragma endregion

#pragma region runtime_invoke
void* HookManager::Hooked_runtime_invoke(const void* method, void* obj, void** params, void** exc)
{
	const char* method_name = NULL;
	if (MelonLoader::IsGameIl2Cpp)
		method_name = Il2Cpp::il2cpp_method_get_name((Il2CppMethod*)method);
	else
		method_name = Mono::mono_method_get_name((MonoMethod*)method);
	if ((strstr(method_name, "Internal_ActiveSceneChanged") != NULL) || (!Mono::IsOldMono && (strstr(method_name, "UnityEngine.ISerializationCallbackReceiver.OnAfterDeserialize") != NULL)))
	{
		if (MelonLoader::IsGameIl2Cpp)
			Unhook(&(LPVOID&)Il2Cpp::il2cpp_runtime_invoke, Hooked_runtime_invoke);
		else
			Unhook(&(LPVOID&)Mono::mono_runtime_invoke, Hooked_runtime_invoke);
		ModHandler::OnApplicationStart();
	}
	if (MelonLoader::IsGameIl2Cpp)
		return Il2Cpp::il2cpp_runtime_invoke((Il2CppMethod*)method, obj, params, (Il2CppObject**)exc);
	return Mono::mono_runtime_invoke((MonoMethod*)method, obj, params, (MonoObject**)exc);
}
#pragma endregion