#include <Windows.h>
#include <string>
#include <iostream>
#include <fstream>
#include <ostream>
#include "MelonLoader.h"

HINSTANCE MelonLoader::thisdll = NULL;
HINSTANCE MelonLoader::melonloaderdll = NULL;

void MelonLoader::Initialize()
{
	LPSTR filepath = new CHAR[MAX_PATH];
	GetModuleFileName(GetModuleHandle(NULL), filepath, MAX_PATH);
	if ((strstr(filepath, "UnityCrashHandler") == NULL) && (strstr(GetCommandLine(), "--no-mods") == NULL))
	{
		melonloaderdll = LoadLibrary("MelonLoader\\MelonLoader.dll");
		if (!melonloaderdll)
			MessageBox(NULL, "Failed to Load MelonLoader.dll!", "MelonLoader", MB_ICONERROR | MB_OK);
	}
}