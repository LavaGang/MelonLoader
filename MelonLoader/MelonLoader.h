#pragma once
#include <string>
#include <Windows.h>
#include "Mono.h"
#include "il2cpp-internals.h"

class MelonLoader
{
public:
	static HINSTANCE thisdll;
	static bool IsGameIl2Cpp;
	static bool DebugMode;
	static bool MupotMode;
	static char* GamePath;
	static char* DataPath;
	static HMODULE MonoUnityPlayerDLL;
	static HMODULE MonoDLL;
	static HMODULE IL2CPPUnityPlayerDLL;
	static HMODULE GameAssemblyDLL;
	static MonoAssembly* ModHandlerAssembly;

	static void Main();
	static void ModHandler();
	static bool LoadMono();
	static bool LoadMonoUnityPlayer();
	static void Detour(Il2CppMethod* target, void* detour);
	static void UnDetour(Il2CppMethod* target, void* detour);
};