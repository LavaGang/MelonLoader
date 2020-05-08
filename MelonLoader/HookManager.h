#pragma once
#include <Windows.h>
#include "Mono.h"
#include "IL2CPP.h"
#include <vector>

typedef HMODULE(__stdcall* LoadLibraryW_t) (LPCWSTR lpLibFileName);
typedef MonoString* (*infovoidcall_t) ();

class HookManager_Hook
{
public:
	HookManager_Hook(void** target, void* detour) { Target = target; Detour = detour; }
	void** Target;
	void* Detour;
};

class HookManager
{
public:
	static std::vector<HookManager_Hook*>HookTbl;
	static HookManager_Hook* FindHook(void** target, void* detour);

	static LoadLibraryW_t Original_LoadLibraryW;
	static void LoadLibraryW_Hook();
	static void LoadLibraryW_Unhook();

	static void Hook(Il2CppMethod* target, void* detour) { INTERNAL_Hook(&(LPVOID&)target->targetMethod, detour); };
	static void Hook(void** target, void* detour);
	static void INTERNAL_Hook(void** target, void* detour);

	static void Unhook(Il2CppMethod* target, void* detour) { INTERNAL_Unhook(&(LPVOID&)target->targetMethod, detour); };
	static void Unhook(void** target, void* detour);
	static void UnhookAll();
	static void INTERNAL_Unhook(void** target, void* detour);

	static HMODULE __stdcall Hooked_LoadLibraryW(LPCWSTR lpLibFileName);
	static Il2CppDomain* Hooked_il2cpp_init(const char* name);
	static MonoDomain* Hooked_mono_jit_init_version(const char* name, const char* version);
	static void Hooked_add_internal_call(const char* name, void* method);
	static void* Hooked_PlayerLoadFirstScene(bool unknown);
	static bool Hooked_PlayerCleanup(bool dopostquitmsg);
	static void Hooked_EndOfFrameCallbacks_DequeAll();
};