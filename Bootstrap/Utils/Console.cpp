#include "Console.h"
#include "../Base/Core.h"
#include <string>
#include "Assertion.h"
#include "Debug.h"
#include <iostream>
#include <locale.h>

bool Console::Enabled = true;
bool Console::AlwaysOnTop = false;
bool Console::HideWarnings = false;
Console::DisplayMode Console::Mode = Console::DisplayMode::NORMAL;
HWND Console::Window = NULL;
HANDLE Console::OutputHandle = NULL;
int Console::rainbow = 1;

bool Console::Initialize()
{
	if (!Enabled)
		return true;
	if (!AllocConsole())
	{
		Assertion::ThrowInternalFailure("Failed to Allocate Console!");
		return false;
	}
	Window = GetConsoleWindow();
	std::string window_name = std::string("MelonLoader ") + Core::Version + " Open-Beta";
	if (Debug::Enabled)
		SetTitle((window_name + " - Debug Mode").c_str());
	else
		SetTitle(window_name.c_str());
	SetForegroundWindow(Window);
	AlwaysOnTopCheck();
	freopen_s(reinterpret_cast<FILE**>(stdout), "CONOUT$", "w", stdout);
	OutputHandle = GetStdHandle(STD_OUTPUT_HANDLE);
	return true;
}

void Console::AlwaysOnTopCheck()
{
	if (!AlwaysOnTop || !IsInitialized())
		return;
	SetWindowPos(Window, HWND_TOPMOST, 0, 0, 0, 0, (SWP_DRAWFRAME | SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW));
	ShowWindow(Window, SW_NORMAL);
}

Console::Color Console::GetRainbowColor()
{
	if (Mode == DisplayMode::RANDOMRAINBOW)
		return (Console::Color)(1 + (rand() * (int)(15 - 1) / RAND_MAX));
	Console::Color returnval = (Console::Color)rainbow;
	rainbow++;
	if (rainbow > 15)
		rainbow = 1;
	else if (rainbow == 7)
		rainbow++;
	return returnval;
}

void Console::SetColor(Color color)
{
	color = ((Mode == DisplayMode::MAGENTA)
		? Color::Magenta
		: (((Mode == DisplayMode::RAINBOW) || (Mode == DisplayMode::RANDOMRAINBOW))
			? GetRainbowColor()
			: color));
	SetConsoleTextAttribute(OutputHandle, color);
}

void Console::Write(const char* txt)
{
	if (IsInitialized())
		std::cout << txt;
};