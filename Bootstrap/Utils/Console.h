#pragma once
#include <Windows.h>

class Console
{
public:
	static HANDLE OutputHandle;
	static bool Enabled;
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
	static void SetColor(Color color);
	static void Write(const char* txt);

private:
	static HWND Window;
	static bool IsInitialized() { return ((Window != NULL) && (OutputHandle != NULL)); }
	static void AlwaysOnTopCheck();
	static int rainbow;
	static Color GetRainbowColor();
};