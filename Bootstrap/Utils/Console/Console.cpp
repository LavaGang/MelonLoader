#include <string>

#include "Console.h"
#include "Debug.h"

#ifdef _WIN32
#include <iostream>
#include <locale.h>
#include <VersionHelpers.h>
#include <sstream>
#endif

#ifdef _WIN32
HWND Console::Window = NULL;
HMENU Console::Menu = NULL;
HANDLE Console::OutputHandle = NULL;
#endif

#ifdef __ANDROID__
Console::DisplayMode Console::Mode = Console::DisplayMode::LEMON;
#else
Console::DisplayMode Console::Mode = Console::DisplayMode::NORMAL;
#endif

bool Console::ShouldHide = false;
bool Console::ShouldSetTitle = true;
bool Console::AlwaysOnTop = false;
bool Console::HideWarnings = false;
int Console::rainbow = 1;
bool Console::UseManualColoring = false;

bool Console::Initialize()
{
	if (!Debug::Enabled && ShouldHide)
		return true;

#ifdef _WIN32
    if (!AllocConsole())
	{
		Assertion::ThrowInternalFailure("Failed to Allocate Console!");
		return false;
	}

    Window = GetConsoleWindow();
	Menu = GetSystemMenu(Window, FALSE);

    SetConsoleCtrlHandler(EventHandler, TRUE);
	SetDefaultTitle();
	SetForegroundWindow(Window);
	if (AlwaysOnTop)
		SetWindowPos(Window, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);

    freopen_s(reinterpret_cast<FILE**>(stdout), "CONOUT$", "w", stdout);
	freopen_s(reinterpret_cast<FILE**>(stderr), "CONOUT$", "w", stderr);
	OutputHandle = GetStdHandle(STD_OUTPUT_HANDLE);
	SetHandles();
	DWORD mode = 0;
	if (!GetConsoleMode(OutputHandle, &mode))
	{
		mode = 0x3;
		if (!SetConsoleMode(OutputHandle, mode))
		{
			UseManualColoring = true;
			return true;
		}
	}
	if (!SetConsoleMode(OutputHandle, (mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING)))
		UseManualColoring = true;
#endif

	return true;
}

void Console::Flush()
{
	if (!IsInitialized())
		return;
#ifndef __ANDROID__
	std::cout.flush();
#endif
}

void Console::Close()
{
	if (!IsInitialized())
		return;

#ifdef _WIN32
    ShowWindow(Window, 0);
	Window = NULL;
	Menu = NULL;
	OutputHandle = NULL;
#endif
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

std::string Console::ColorToAnsi(Color color, bool modecheck)
{
	if (modecheck)
		color = ((Mode == DisplayMode::MAGENTA)
			? Color::Magenta
			: (((Mode == DisplayMode::RAINBOW) || (Mode == DisplayMode::RANDOMRAINBOW))
				? GetRainbowColor()
				: ((Mode == DisplayMode::LEMON)
					? Color::Yellow
					: color)));
	if (UseManualColoring)
	{
#ifdef _WIN32
		SetConsoleTextAttribute(OutputHandle, color);
#endif
		return std::string();
	}
	switch (color)
	{
    case Color::Black:
		return "\x1b[30m";
	case Color::DarkBlue:
		return "\x1b[34m";
	case Color::DarkGreen:
		return "\x1b[32m";
	case Color::DarkCyan:
		return "\x1b[36m";
	case Color::DarkRed:
		return "\x1b[31m";
	case Color::DarkMagenta:
		return "\x1b[35m";
	case Color::DarkYellow:
		return "\x1b[33m";
	case Color::Gray:
		return "\x1b[37m";
	case Color::DarkGray:
		return "\x1b[90m";
	case Color::Blue:
		return "\x1b[94m";
	case Color::Green:
		return "\x1b[92m";
	case Color::Cyan:
		return "\x1b[96m";
	case Color::Red:
		return "\x1b[91m";
	case Color::Magenta:
		return "\x1b[95m";
	case Color::Yellow:
		return "\x1b[93m";
	case Color::White:
        return "\x1b[97m";
    case Color::Reset:
    default:
        return "\x1b[0m";
	}
}

#ifdef _WIN32
void Console::SetDefaultTitle()
{
	std::string versionstr = Core::GetVersionStr();
	SetTitle(((Debug::Enabled ? "[D] " : "") + versionstr).c_str());
}

void Console::SetDefaultTitleWithGameName(const char* GameVersion)
{
	std::string versionstr = Core::GetVersionStrWithGameName(GameVersion);
	SetTitle(((Debug::Enabled ? "[D] " : "") + versionstr).c_str());
}

void Console::SetHandles()
{
	SetStdHandle(STD_OUTPUT_HANDLE, Console::OutputHandle);
	SetStdHandle(STD_ERROR_HANDLE, Console::OutputHandle);
}

void Console::NullHandles()
{
	SetStdHandle(STD_OUTPUT_HANDLE, NULL);
	SetStdHandle(STD_ERROR_HANDLE, NULL);
}

void Console::EnableCloseButton() { if (!IsInitialized()) return; EnableMenuItem(Menu, SC_CLOSE, MF_BYCOMMAND | MF_ENABLED); }
void Console::DisableCloseButton() { if (!IsInitialized()) return; EnableMenuItem(Menu, SC_CLOSE, (MF_BYCOMMAND | MF_DISABLED | MF_GRAYED)); }
BOOL WINAPI Console::EventHandler(DWORD evt)
{
	switch (evt)
	{
	case CTRL_C_EVENT:
	case CTRL_CLOSE_EVENT:
	case CTRL_LOGOFF_EVENT:
	case CTRL_SHUTDOWN_EVENT:
		if (Game::IsIl2Cpp)
			Il2CppAssemblyGenerator::Cleanup();
		Logger::Flush();
		Flush();
		Close();
		Core::KillCurrentProcess();
	default:
		return FALSE;
	}
}
#endif