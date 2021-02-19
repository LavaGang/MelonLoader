#pragma once

#ifdef _WIN32
#include <Windows.h>
#endif

#include <string>

class Console
{
public:
#ifdef _WIN32
	static HANDLE OutputHandle;
	static bool AlwaysOnTop;
	static bool ShouldHide;
#endif
	static bool GeneratingAssembly;
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
		White = 15,
		Reset = -1
	};
	static std::string ColorToAnsi(Color color);
#ifdef _WIN32
	static void EnableCloseButton();
	static void DisableCloseButton();
	static void SetHandles();
	static void NullHandles();
	static void Flush();
	static void Close();
	static void SetTitle(const char* title) { SetConsoleTitleA(title); }
	static BOOL WINAPI EventHandler(DWORD evt);
#endif

private:
#ifdef _WIN32
	static HWND Window;
	static HMENU Menu;
	static bool IsInitialized() { return ((Window != NULL) && (Menu != NULL) && (OutputHandle != NULL)); }
#endif
	static int rainbow;
	static Color GetRainbowColor();
};