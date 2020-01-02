#include <Windows.h>
#include "MelonLoader.h"

HINSTANCE MelonLoader::melonloaderdll = NULL;

void MelonLoader::Initialize()
{
	if (strstr(GetCommandLine(), "--no-mods") == NULL)
	{
		melonloaderdll = LoadLibrary("MelonLoader\\MelonLoader.dll");
		if (!melonloaderdll)
			MessageBox(NULL, "Failed to Load MelonLoader.dll!", "MelonLoader", MB_ICONERROR | MB_OK);
	}
}