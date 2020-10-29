#pragma once
#include <Windows.h>
#include <string>

class Console
{
public:
	static HANDLE OutputHandle;
	static bool ShouldHide;
	static bool GeneratingAssembly;
	static bool AlwaysOnTop;
	static bool HideWarnings;
	enum DisplayMode
	{
		NORMAL,
		MAGENTA,
		RAINBOW,
		RANDOMRAINBOW
	};
	static DisplayMode Mode;

	static bool Initialize();
	static void Flush();
	static void Close();
	static void SetTitle(const char* title) { SetConsoleTitleA(title); }
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
	static std::string GetColor(Color color);
	static void EnableCloseButton();
	static void DisableCloseButton();
	static BOOL WINAPI EventHandler(DWORD evt);

private:
	static HWND Window;
	static HMENU Menu;
	static CONSOLE_SCREEN_BUFFER_INFO ConsoleInfo;
	static bool IsInitialized() { return ((Window != NULL) && (Menu != NULL) && (OutputHandle != NULL)); }
	static int rainbow;
	static Color GetRainbowColor();
};