#include <fstream>
#include "Console.h"
#include "HookManager.h"

HWND Console::hwndConsole = NULL;
int Console::rainbow = 1;

void Console::Create()
{
	if (!IsInitialized())
	{
		if (AllocConsole())
		{
			hwndConsole = GetConsoleWindow();
			SetConsoleTitle("MelonLoader Debug Console");
			SetForegroundWindow(hwndConsole);
			freopen_s(reinterpret_cast<FILE**>(stdout), "CONOUT$", "w", stdout);
		}
		else
			MessageBox(NULL, "Failed to Create Debug Console!", NULL, MB_OK | MB_ICONEXCLAMATION);
	}
}

void Console::RainbowCheck()
{
	if (IsInitialized() && (MelonLoader::RainbowMode || MelonLoader::RandomRainbowMode))
	{
		if (MelonLoader::RandomRainbowMode)
			SetColor((ConsoleColor)(1 + (rand() * (int)(15 - 1) / RAND_MAX)));
		else
		{
			SetColor((ConsoleColor)rainbow);
			rainbow++;
			if (rainbow > 15)
				rainbow = 1;
			else if (rainbow == 7)
				rainbow++;
		}
	}
}

void Console::Write(const char* txt)
{
	if (IsInitialized())
	{
		RainbowCheck();
		std::cout << txt;
		if (MelonLoader::RainbowMode || MelonLoader::RandomRainbowMode)
			ResetColor();
	}
};

void Console::Write(const char* txt, ConsoleColor color)
{
	if (IsInitialized())
	{
		SetColor(color);
		Write(txt);
		ResetColor();
	}
}