#pragma once
#include <Windows.h>
#include "../Mono.h"
#include "../Il2Cpp.h"
#include "../MonoUnityPlayer.h"
#include <vector>
#include <string>

typedef HMODULE (__stdcall* LoadLibraryW_t) (LPCWSTR lpLibFileName);
class Hook_LoadLibraryW
{
public:
	static LoadLibraryW_t Original_LoadLibraryW;

	static void Hook();
	static void Unhook();
	static HMODULE __stdcall Hooked_LoadLibraryW(LPCWSTR lpLibFileName);
};

class Hook_il2cpp_init
{
public:
	static il2cpp_init_t Original_il2cpp_init;

	static void Hook();
	static void Unhook();
	static Il2CppDomain* Hooked_il2cpp_init(const char* name);
};

class Hook_mono_jit_init_version
{
public:
	static mono_jit_init_version_t Original_mono_jit_init_version;

	static void Hook();
	static void Unhook();
	static MonoDomain* Hooked_mono_jit_init_version(const char* name, const char* version);
};

class Hook_il2cpp_add_internal_call
{
public:
	static il2cpp_add_internal_call_t Original_il2cpp_add_internal_call;

	static void Hook();
	static void Unhook();
	static void Hooked_il2cpp_add_internal_call(const char* name, void* method);
};

class Hook_SingleAppInstance_FindOtherInstance
{
public:
	static SingleAppInstance_FindOtherInstance_t Original_SingleAppInstance_FindOtherInstance;

	static void Hook();
	static void Unhook();
	static bool Hooked_SingleAppInstance_FindOtherInstance(LPARAM lParam);
};

typedef void* (*PlayerLoadFirstScene_t) (bool unknown);
class Hook_PlayerLoadFirstScene
{
public:
	static PlayerLoadFirstScene_t Original_PlayerLoadFirstScene;

	static void Hook();
	static void Unhook();
	static void* Hooked_PlayerLoadFirstScene(bool unknown);
};