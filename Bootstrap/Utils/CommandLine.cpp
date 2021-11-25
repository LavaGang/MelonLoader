#include "CommandLine.h"
#include "Logging/Logger.h"
#include "Debug.h"
#include "AnalyticsBlocker.h"
#include "Encoding.h"
#include "../Core.h"
#include "../Managers/Game.h"
#include "../Managers/InternalCalls.h"

int CommandLine::argc = NULL;
char* CommandLine::argv[64];
char* CommandLine::argvMono[64];

void CommandLine::Read()
{
#ifdef DEBUG
	Debug::Enabled = true;
#endif

	char* nextchar = NULL;
	char* curchar = strtok_s(GetCommandLineA(), " ", &nextchar);
	while (curchar && (argc <= 63))
	{
		argv[argc] = curchar;
		argvMono[argc] = Encoding::OsToUtf8(curchar);
		curchar = strtok_s(0, " ", &nextchar);
		argc++;
	}

	if (argc <= 0)
		return;

	for (int i = 0; i < argc; i++)
	{
		const char* command = argv[i];
		if (command == NULL)
			continue;
		else if (strstr(command, "--melonloader.consolemode") != NULL)
		{
			int mode = GetIntFromConstChar(argv[i + 1], 0);
			int min = (int)Console::DisplayMode::NORMAL;
			int max = (int)Console::DisplayMode::LEMON;
			if (mode < min)
				mode = min;
			if (mode > max)
				mode = max;
			Console::Mode = (Console::DisplayMode)mode;
		}
		else if (strstr(command, "--melonloader.consoleontop") != NULL)
			Console::AlwaysOnTop = true;
		else if (strstr(command, "--melonloader.consoledst") != NULL)
			Console::ShouldSetTitle = false;
		else if (strstr(command, "--melonloader.dab") != NULL)
			AnalyticsBlocker::ShouldDAB = true;
		else if (strstr(command, "--melonloader.disableunityclc") != NULL)
			Console::CleanUnityLogs = false;

#ifndef DEBUG
		else if (strstr(command, "--melonloader.debug") != NULL)
			Debug::Enabled = true;
		else if (strstr(command, "--melonloader.hideconsole") != NULL)
			Console::ShouldHide = true;
		else if (strstr(command, "--melonloader.hidewarnings") != NULL)
			Console::HideWarnings = true;
		else if (strstr(command, "--melonloader.maxlogs") != NULL)
			Logger::MaxLogs = GetIntFromConstChar(argv[i + 1], 10);
		else if (strstr(command, "--melonloader.maxwarnings") != NULL)
			Logger::MaxWarnings = GetIntFromConstChar(argv[i + 1], 10);
		else if (strstr(command, "--melonloader.maxerrors") != NULL)
			Logger::MaxErrors = GetIntFromConstChar(argv[i + 1], 10);
#endif
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