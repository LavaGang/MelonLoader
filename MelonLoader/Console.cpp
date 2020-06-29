#include <fstream>
#include "Console.h"

HWND Console::hwndConsole = NULL;
int Console::rainbow = 1;
bool Console::Enabled = true;
bool Console::HideWarnings = false;
bool Console::RainbowMode = false;
bool Console::RandomRainbowMode = false;
bool Console::ChromiumMode = false;

void Console::Create()
{
	if (!IsInitialized())
	{
		if (AllocConsole())
		{
			hwndConsole = GetConsoleWindow();
			SetTitle(("MelonLoader " + (MelonLoader::DebugMode ? std::string("Debug") : std::string("Normal")) + " Console").c_str());
			SetForegroundWindow(hwndConsole);
			freopen_s(reinterpret_cast<FILE**>(stdout), "CONOUT$", "w", stdout);
		}
		else
			MessageBox(NULL, ("Failed to Create the " + (MelonLoader::DebugMode ? std::string("Debug") : std::string("Normal")) + " Console!").c_str(), NULL, MB_OK | MB_ICONEXCLAMATION);
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

void Console::ChromiumCheck()
{
	if (IsInitialized() && ChromiumMode)
		SetColor(ConsoleColor_Magenta);
}

void Console::Write(const char* txt)
{
	if (IsInitialized())
	{
		ChromiumCheck();
		RainbowCheck();
		std::cout << txt;
		ResetColor();
	}
};

void Console::Write(const char* txt, ConsoleColor color)
{
	if (IsInitialized())
	{
		SetColor(color);
		ChromiumCheck();
		RainbowCheck();
		Write(txt);
		ResetColor();
	}
}