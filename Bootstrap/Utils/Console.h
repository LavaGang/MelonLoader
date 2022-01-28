#pragma once
#include <Windows.h>
#include <string>

class Console
{
public:
	static HANDLE OutputHandle;
	static HANDLE InputHandle;
	static bool ShouldHide;
	static bool ShouldSetTitle;
	static bool AlwaysOnTop;
	static bool HideWarnings;
	static bool UseLegacyColoring;
	static bool CleanUnityLogs;
	enum DisplayMode
	{
		NORMAL,
		MAGENTA,
		RAINBOW,
		RANDOMRAINBOW,
		LEMON
	};
	static DisplayMode Mode;

	static bool Initialize();
	static void Flush();
	static void Close();
	static void SetTitle(const char* title) { if (ShouldSetTitle) SetConsoleTitleA(title); }
	static void SetDefaultTitle();
	static void SetDefaultTitleWithGameName(const char* GameName, const char* GameVersion = NULL);
	enum Color
	{
		Black = 0,
		DarkBlue = 1,
		DarkGreen = 2,
		DarkCyan = 3,
		DarkRed = 4,
		DarkMagenta = 5,
		DarkYellow = 6,
		Gray = 7,
		DarkGray = 8,
		Blue = 9,
		Green = 10,
		Cyan = 11,
		Red = 12,
		Magenta = 13,
		Yellow = 14,
		White = 15
	};
	static std::string ColorToAnsi(Color color, bool modecheck = true);
	static void EnableCloseButton(HWND mainWindow);
	static void DisableCloseButton(HWND mainWindow);
	static BOOL WINAPI EventHandler(DWORD evt);
	static void SetHandles();
	static void NullHandles();
	static bool IsInitialized() { return ((Window != NULL) && (Menu != NULL) && (OutputHandle != NULL)); }

private:
	static HWND Window;
	static HMENU Menu;
	static int rainbow;
	static Color GetRainbowColor();
};