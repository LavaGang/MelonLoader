#include "CommandLine.h"
#include "../Core.h"
#include "Logger.h"
#include "Debug.h"
#include "AnalyticsBlocker.h"
#include "../Managers/InternalCalls.h"
#include "AssemblyGenerator.h"
#include "Encoding.h"
#include "../Managers/Game.h"

int CommandLine::argc = NULL;
//char** CommandLine::argvMono;
char* CommandLine::argv[64];
char* CommandLine::argvMono[64];
IniFile* CommandLine::iniFile = NULL;

void CommandLine::Read()
{
	ReadIniFile();

	//argc = __argc;
	//char** argv = __argv;

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

	//argvMono = (char**)malloc(sizeof(argv[0]) * argc);
	for (int i = 0; i < argc; i++)
	{
		const char* command = argv[i];
		//argvMono[i] = Encoding::OsToUtf8(command);
		if (command == NULL)
			continue;

		if (strstr(command, "--melonloader.magenta") != NULL)
			Console::Mode = Console::DisplayMode::MAGENTA;
		else if (strstr(command, "--melonloader.rainbow") != NULL)
			Console::Mode = Console::DisplayMode::RAINBOW;
		else if (strstr(command, "--melonloader.randomrainbow") != NULL)
			Console::Mode = Console::DisplayMode::RANDOMRAINBOW;
		else if (strstr(command, "--lemonloader") != NULL)
			Console::Mode = Console::DisplayMode::LEMON;

		else if (strstr(command, "--quitfix") != NULL)
			Core::QuitFix = true;
		else if (strstr(command, AddPrefixToLaunchOption("consoleontop")) != NULL)
			Console::AlwaysOnTop = true;
		else if (strstr(command, AddPrefixToLaunchOption("consoledst")) != NULL)
			Console::ShouldSetTitle = false;
		else if (strstr(command, AddPrefixToLaunchOption("dab")) != NULL)
			AnalyticsBlocker::ShouldDAB = true;
		
#ifndef DEBUG
		else if (strstr(command, AddPrefixToLaunchOption("debug")) != NULL)
			Debug::Enabled = true;
		else if (strstr(command, AddPrefixToLaunchOption("hideconsole")) != NULL)
			Console::ShouldHide = true;
		else if (strstr(command, AddPrefixToLaunchOption("hidewarnings")) != NULL)
			Console::HideWarnings = true;
		else if (strstr(command, AddPrefixToLaunchOption("maxlogs")) != NULL)
			Logger::MaxLogs = GetIntFromConstChar(argv[i + 1], 10);
		else if (strstr(command, AddPrefixToLaunchOption("maxwarnings")) != NULL)
			Logger::MaxWarnings = GetIntFromConstChar(argv[i + 1], 10);
		else if (strstr(command, AddPrefixToLaunchOption("maxerrors")) != NULL)
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

void CommandLine::ReadIniFile()
{
	if (iniFile == NULL)
		iniFile = new IniFile((std::string(Game::BasePath) + "\\MelonLoader\\LaunchOptions.ini"));

#ifndef DEBUG
	Debug::Enabled = (!iniFile->ReadValue("Core", "Debug").empty() && iniFile->ReadValue("Core", "Debug")._Equal("true"));
	iniFile->WriteValue("Core", "Debug", (Debug::Enabled ? "true" : "false"));
#endif
	Core::QuitFix = (!iniFile->ReadValue("Core", "QuitFix").empty() && iniFile->ReadValue("Core", "QuitFix")._Equal("true"));
	iniFile->WriteValue("Core", "QuitFix", (Core::QuitFix ? "true" : "false"));

#ifndef DEBUG
	Console::ShouldHide = (!iniFile->ReadValue("Console", "Enabled").empty() && iniFile->ReadValue("Console", "Enabled")._Equal("false"));
	iniFile->WriteValue("Console", "Enabled", (Console::ShouldHide ? "false" : "true"));
#endif
	Console::Mode = (iniFile->ReadValue("Console", "Mode").empty() ? Console::Mode : (
		iniFile->ReadValue("Console", "Mode")._Equal("1") ? Console::DisplayMode::MAGENTA : (
			iniFile->ReadValue("Console", "Mode")._Equal("2") ? Console::DisplayMode::RAINBOW :
			(iniFile->ReadValue("Console", "Mode")._Equal("3") ? Console::DisplayMode::RANDOMRAINBOW : 
				(iniFile->ReadValue("Console", "Mode")._Equal("4") ? Console::DisplayMode::LEMON : Console::Mode)))));
	iniFile->WriteValue("Console", "Mode", std::to_string(Console::Mode));
	Console::AlwaysOnTop = (!iniFile->ReadValue("Console", "AlwaysOnTop").empty() && iniFile->ReadValue("Console", "AlwaysOnTop")._Equal("true"));
	iniFile->WriteValue("Console", "AlwaysOnTop", (Console::AlwaysOnTop ? "true" : "false"));
	iniFile->WriteValue("Console", "DontSetTitle", (Console::ShouldSetTitle ? "false" : "true"));
#ifndef DEBUG
	Console::HideWarnings = (!iniFile->ReadValue("Console", "HideWarnings").empty() && iniFile->ReadValue("Console", "HideWarnings")._Equal("true"));
	iniFile->WriteValue("Console", "HideWarnings", (Console::HideWarnings ? "true" : "false"));

	Logger::MaxLogs = (!iniFile->ReadValue("Logger", "MaxLogs").empty() ? GetIntFromConstChar(iniFile->ReadValue("Console", "MaxLogs").c_str(), Logger::MaxLogs) : Logger::MaxLogs);
	iniFile->WriteValue("Logger", "MaxLogs", std::to_string(Logger::MaxLogs));
	Logger::MaxWarnings = (!iniFile->ReadValue("Logger", "MaxWarnings").empty() ? GetIntFromConstChar(iniFile->ReadValue("Console", "MaxWarnings").c_str(), Logger::MaxWarnings) : Logger::MaxWarnings);
	iniFile->WriteValue("Logger", "MaxWarnings", std::to_string(Logger::MaxWarnings));
	Logger::MaxErrors = (!iniFile->ReadValue("Logger", "MaxErrors").empty() ? GetIntFromConstChar(iniFile->ReadValue("Console", "MaxErrors").c_str(), Logger::MaxErrors) : Logger::MaxErrors);
	iniFile->WriteValue("Logger", "MaxErrors", std::to_string(Logger::MaxErrors));
#endif

	AnalyticsBlocker::ShouldDAB = (!iniFile->ReadValue("AnalyticsBlocker", "ShouldDAB").empty() && iniFile->ReadValue("AnalyticsBlocker", "ShouldDAB")._Equal("true"));
	iniFile->WriteValue("AnalyticsBlocker", "ShouldDAB", (AnalyticsBlocker::ShouldDAB ? "true" : "false"));
}