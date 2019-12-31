#pragma once
#include <string>
#include <Windows.h>
#include "Mono.h"

class MelonLoader
{
public:
	static bool IsGameIL2CPP;
	static bool DebugMode;
	static const char* GamePath;
	static HMODULE MonoDLL;
	static HMODULE GameAssemblyDLL;
	static HINSTANCE thisdll;
	static MonoImage* ModHandlerImage;

	static void Main();
	static void ApplicationQuit();
	static void ModHandler();
	static bool LoadMono();
};