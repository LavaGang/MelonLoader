#pragma once
#include <string>

#ifdef _WIN32
#include <Windows.h>
#endif

class Console
{
public:
#ifdef _WIN32
    static HANDLE OutputHandle;
#endif
	static bool ShouldHide;
	static bool ShouldSetTitle;
	static bool AlwaysOnTop;
	static bool HideWarnings;
	static bool UseManualColoring;
	enum DisplayMode
	{
		NORMAL,
		MAGENTA,
		RAINBOW,
		RANDOMRAINBOW,
		LEMON
	};
	static DisplayMode Mode;

#ifdef _WIN32
    static void SetTitle(const char* title) { if (ShouldSetTitle) SetConsoleTitleA(title); }
    static BOOL WINAPI EventHandler(DWORD evt);

    static void SetHandles();
	static void NullHandles();

    static void SetDefaultTitle();
    static void SetDefaultTitleWithGameName(const char* GameVersion = NULL);
    static void EnableCloseButton();
    static void DisableCloseButton();
#endif

	static bool Initialize();
	static void Flush();
	static void Close();
	enum Color
	{
	    Reset = -1,
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

private:
#ifdef _WIN32
	static HWND Window;
	static HMENU Menu;
	static bool IsInitialized() { return ((Window != NULL) && (Menu != NULL) && (OutputHandle != NULL)); }
#elif defined(__ANDROID__)
    static bool IsInitialized() { return true; }
#endif
	static int rainbow;
	static Color GetRainbowColor();
};