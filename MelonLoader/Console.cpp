#include "Console.h"
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
		freopen_s(reinterpret_cast<FILE * *>(stdout), "CONOUT$", "w", stdout);
		//scout.coss = std::ofstream("debug.log");
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

void Console::Write(const char* txt)
{
	scout << txt;
}

void Console::WriteLine(const char* txt)
{
	scout << txt;
	scout << std::endl;
}