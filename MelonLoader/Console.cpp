#include <fstream>
#include "Console.h"

HWND Console::hwndConsole = NULL;
int Console::rainbow = 1;
bool Console::ConsoleEnabled = true;
bool Console::HideWarnings = false;
bool Console::RainbowMode = false;
bool Console::RandomRainbowMode = false;

void Console::Create()
{
	if (!IsInitialized())
	{
		if (AllocConsole())
		{
			hwndConsole = GetConsoleWindow();
			SetTitle("MelonLoader Debug Console");
			SetForegroundWindow(hwndConsole);
			freopen_s(reinterpret_cast<FILE**>(stdout), "CONOUT$", "w", stdout);
		}
		else
			MessageBox(NULL, "Failed to Create Debug Console!", NULL, MB_OK | MB_ICONEXCLAMATION);
	}
}

void Console::RainbowCheck()
{
	if (IsInitialized() && (RainbowMode || RandomRainbowMode))
	{
		if (RandomRainbowMode)
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
		if (RainbowMode || RandomRainbowMode)
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