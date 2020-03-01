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

	static void Main();
	static void Detour(Il2CppMethod* target, void* detour);
	static void UnDetour(Il2CppMethod* target, void* detour);
	static void AddGameSpecificInternalCalls();
};