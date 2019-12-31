#include <Windows.h>
#include "MelonLoader.h"

void MelonLoader::Initialize()
{
	if (strstr(GetCommandLine(), "--no-mods") == NULL)
	{
		HINSTANCE melonloaderdll = LoadLibrary("MelonLoader\\MelonLoader.dll");
		if (!melonloaderdll)
			MessageBox(NULL, "Failed to Load MelonLoader.dll!", "MelonLoader", MB_ICONERROR | MB_OK);
	}
}