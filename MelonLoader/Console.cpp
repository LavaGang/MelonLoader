#include "Console.h"
#include <Windows.h>
#include <stdio.h>
#include <iostream>

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
		freopen_s(reinterpret_cast<FILE * *>(stderr), "CONERR$", "w", stderr);
		SetConsoleTitle("MelonLoader Debug Console");
	}
}

void Console::Destroy()
{
	if (IsInitialized())
	{
		fclose(reinterpret_cast<FILE*>(stdout));
		fclose(reinterpret_cast<FILE*>(stderr));
		FreeConsole();
	}
}

void Console::Write(const char* txt)
{
	std::cout << txt;
}

void Console::WriteLine(const char* txt)
{
	std::cout << txt << std::endl;
}