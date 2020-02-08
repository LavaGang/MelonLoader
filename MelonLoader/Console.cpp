#include "Console.h"
#include "MelonLoader.h"
#include <Windows.h>
#include <stdio.h>
#include <iostream>

debugstream Console::scout;
bool Console::IsInitialized() { return (GetConsoleWindow() != NULL); }

void Console::Create()
{
	if (!IsInitialized())
	{
		if (!AllocConsole())
		{
			MessageBox(NULL, "Failed to Create Debug Console!", NULL, MB_OK | MB_ICONEXCLAMATION);
			return;
		}
		SetConsoleTitle("MelonLoader Debug Console");
		freopen_s(reinterpret_cast<FILE**>(stdout), "CONOUT$", "w", stdout);
		scout.coss = std::ofstream((std::string(MelonLoader::GamePath) + "\\melonloader_debug.log"));
	}
}

void Console::Destroy()
{
	if (IsInitialized())
	{
		fclose(reinterpret_cast<FILE*>(stdout));
		FreeConsole();
	}
}

void Console::Write(const char* txt) { scout << txt; }
void Console::Write(std::string txt) { Write(txt.c_str()); }

void Console::WriteLine(const char* txt) { scout << txt << std::endl; }
void Console::WriteLine(std::string txt) { WriteLine(txt.c_str()); }