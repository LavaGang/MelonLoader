#include "Console.h"
#include "../Base/Core.h"
#include <string>
#include "Assertion.h"
#include "Debug.h"
#include <iostream>
#include <locale.h>
#include "../Managers/Game.h"
#include "AssemblyGenerator.h"

bool Console::ShouldHide = false;
bool Console::GeneratingAssembly = false;
bool Console::AlwaysOnTop = false;
bool Console::HideWarnings = false;
Console::DisplayMode Console::Mode = Console::DisplayMode::NORMAL;
HWND Console::Window = NULL;
HMENU Console::Menu = NULL;
HANDLE Console::OutputHandle = NULL;
int Console::rainbow = 1;

bool Console::Initialize()
{
	if (!Debug::Enabled && ShouldHide && !GeneratingAssembly)
		return true;
	if (!AllocConsole())
	{
		Assertion::ThrowInternalFailure("Failed to Allocate Console!");
		return false;
	}
	Window = GetConsoleWindow();
	Menu = GetSystemMenu(Window, FALSE);
	SetConsoleCtrlHandler(EventHandler, TRUE);
	std::string window_name = std::string("MelonLoader ") + Core::Version + " Open-Beta";
	if (Debug::Enabled)
		SetTitle((window_name + " - Debug Mode").c_str());
	else
		SetTitle(window_name.c_str());
	SetForegroundWindow(Window);
	if (AlwaysOnTop)
		SetWindowPos(Window, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
	freopen_s(reinterpret_cast<FILE**>(stdout), "CONOUT$", "w", stdout);
	OutputHandle = GetStdHandle(STD_OUTPUT_HANDLE);
	DWORD mode = 0;
	if (!GetConsoleMode(OutputHandle, &mode))
	{
		Assertion::ThrowInternalFailure("Failed to Get Console Mode!");
		return false;
	}
	if (!SetConsoleMode(OutputHandle, (mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING)))
	{
		Assertion::ThrowInternalFailure("Failed to Enable Virtual Terminal Processing!");
		return false;
	}
	return true;
}

void Console::Close()
{
	if (!IsInitialized())
		return;
	ShowWindow(Window, 0);
	Window = NULL;
	Menu = NULL;
	OutputHandle = NULL;
}

void Console::EnableCloseButton() { if (!IsInitialized()) return; EnableMenuItem(Menu, SC_CLOSE, MF_BYCOMMAND | MF_ENABLED); }
void Console::DisableCloseButton() { if (!IsInitialized()) return; EnableMenuItem(Menu, SC_CLOSE, (MF_BYCOMMAND | MF_DISABLED | MF_GRAYED)); }
BOOL WINAPI Console::EventHandler(DWORD evt)
{
	switch (evt)
	{
	case CTRL_CLOSE_EVENT:
		if (Game::IsIl2Cpp)
			AssemblyGenerator::Cleanup();
		Close();
		Core::KillCurrentProcess();
	default:
		return FALSE;
	}
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
	switch (color)
	{
	case Color::Black:
		Write("\x1b[30m");
		break;
	case Color::DarkBlue:
		Write("\x1b[34m");
		break;
	case Color::DarkGreen:
		Write("\x1b[32m");
		break;
	case Color::DarkCyan:
		Write("\x1b[36m");
		break;
	case Color::DarkRed:
		Write("\x1b[31m");
		break;
	case Color::DarkMagenta:
		Write("\x1b[35m");
		break;
	case Color::DarkYellow:
		Write("\x1b[33m");
		break;
	case Color::Gray:
		Write("\x1b[37m");
		break;
	case Color::DarkGray:
		Write("\x1b[90m");
		break;
	case Color::Blue:
		Write("\x1b[94m");
		break;
	case Color::Green:
		Write("\x1b[92m");
		break;
	case Color::Cyan:
		Write("\x1b[96m");
		break;
	case Color::Red:
		Write("\x1b[91m");
		break;
	case Color::Magenta:
		Write("\x1b[95m");
		break;
	case Color::Yellow:
		Write("\x1b[93m");
		break;
	case Color::White:
		Write("\x1b[97m");
		break;
	default:
		break;
	}
}

void Console::Write(const char* txt)
{
	if (!IsInitialized())
		return;
	std::cout << txt;
	std::cout.flush();
};