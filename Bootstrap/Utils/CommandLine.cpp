#include <Windows.h>
#include "CommandLine.h"
#include "Console.h"
#include "../Base/Core.h"
#include "Logger.h"
#include "Debug.h"
#include "AnalyticsBlocker.h"
#include "../Managers/InternalCalls.h"

int CommandLine::argc = NULL;
char* CommandLine::argv[64];

void CommandLine::Read()
{
	char* nextchar = NULL;
	char* curchar = strtok_s(GetCommandLineA(), " ", &nextchar);
	while (curchar && (argc < 63))
	{
		argv[argc++] = curchar;
		curchar = strtok_s(0, " ", &nextchar);
	}
	argv[argc] = 0;
	if (argc <= 0)
		return;
	for (int i = 0; i < argc; i++)
	{
		const char* command = argv[i];
		if (command == NULL)
			continue;
		else if (strstr(command, "--melonloader.consoleontop") != NULL)
			Console::AlwaysOnTop = true;
		else if (strstr(command, "--melonloader.magenta") != NULL)
			Console::Mode = Console::DisplayMode::MAGENTA;
		else if (strstr(command, "--melonloader.rainbow") != NULL)
			Console::Mode = Console::DisplayMode::RAINBOW;
		else if (strstr(command, "--melonloader.randomrainbow") != NULL)
			Console::Mode = Console::DisplayMode::RANDOMRAINBOW;
		else if (strstr(command, "--melonloader.dab") != NULL)
			AnalyticsBlocker::ShouldDAB = true;
		else if (strstr(command, "--melonloader.loadmodeplugins") != NULL)
		{
			int loadmode = atoi(argv[i + 1]);
			if (loadmode < 0)
				loadmode = 0;
			else if (loadmode > 2)
				loadmode = 0;
			InternalCalls::MelonHandler::LoadModeForPlugins = (InternalCalls::MelonHandler::LoadMode)loadmode;
		}
		else if (strstr(command, "--melonloader.loadmodemods") != NULL)
		{
			int loadmode = atoi(argv[i + 1]);
			if (loadmode < 0)
				loadmode = 0;
			else if (loadmode > 2)
				loadmode = 0;
			InternalCalls::MelonHandler::LoadModeForMods = (InternalCalls::MelonHandler::LoadMode)loadmode;
		}
#ifndef DEBUG
		else if (strstr(command, "--melonloader.debug") != NULL)
			Debug::Enabled = true;
		else if (strstr(command, "--melonloader.hideconsole") != NULL)
			Console::Enabled = false;
		else if (strstr(command, "--melonloader.hidewarnings") != NULL)
			Console::HideWarnings = false;
		else if (strstr(command, "--melonloader.maxlogs") != NULL)
			Logger::MaxLogs = GetIntFromConstChar(argv[i + 1], 10);
		else if (strstr(command, "--melonloader.maxwarnings") != NULL)
			Logger::MaxWarnings = GetIntFromConstChar(argv[i + 1], 10);
		else if (strstr(command, "--melonloader.maxerrors") != NULL)
			Logger::MaxErrors = GetIntFromConstChar(argv[i + 1], 10);
#endif
		/*
		if (strstr(command, "--quitfix") != NULL)
			QuitFix = true;
		else if (strstr(command, "--melonloader.agregenerate") != NULL)
			AG_Force_Regenerate = true;
		else if (strstr(command, "--melonloader.agfvunhollower"))
			ForceUnhollowerVersion = CommandLineV[i + 1];
		else if (strstr(command, "--melonloader.agfvunity"))
			ForceUnityVersion = CommandLineV[i + 1];
		*/
	}
}


int CommandLine::GetIntFromConstChar(const char* str, int defaultval)
{
	if ((str == NULL) || (*str == '\0'))
		return defaultval;
	bool negate = (str[0] == '-');
	if ((*str == '+') || (*str == '-'))
		++str;
	if (*str == '\0')
		return defaultval;
	int result = 0;
	while (*str)
	{
		if ((*str >= '0') && (*str <= '9'))
			result = ((result * 10) - (*str - '0'));
		else
			return defaultval;
		++str;
	}
	return (negate ? result : -result);
}