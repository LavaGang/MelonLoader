#ifndef PORT_DISABLE
#include <Windows.h>
#include "CommandLine.h"
#include "Console.h"
#include "../Base/Core.h"
#include "Logger.h"
#include "Debug.h"
#include "AnalyticsBlocker.h"
#include "../Managers/InternalCalls.h"
#include "AssemblyGenerator.h"
#include "../Managers/Game.h"

int CommandLine::argc = NULL;
char* CommandLine::argv[64];
IniFile* CommandLine::iniFile = NULL;

void CommandLine::Read()
{
	ReadIniFile();
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
		else if (strstr(command, "--quitfix") != NULL)
			Core::QuitFix = true;
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
		else if (strstr(command, "--melonloader.agfvunity"))
		{
			std::string version = argv[i + 1];
			AssemblyGenerator::ForceVersion_UnityDependencies = new char[version.size() + 1];
			std::copy(version.begin(), version.end(), AssemblyGenerator::ForceVersion_UnityDependencies);
			AssemblyGenerator::ForceVersion_UnityDependencies[version.size()] = '\0';
		}
		else if (strstr(command, "--melonloader.agfvdumper"))
		{
			std::string version = argv[i + 1];
			AssemblyGenerator::ForceVersion_Il2CppDumper = new char[version.size() + 1];
			std::copy(version.begin(), version.end(), AssemblyGenerator::ForceVersion_Il2CppDumper);
			AssemblyGenerator::ForceVersion_Il2CppDumper[version.size()] = '\0';
		}
		else if (strstr(command, "--melonloader.agfvunhollower"))
		{
			std::string version = argv[i + 1];
			AssemblyGenerator::ForceVersion_Il2CppAssemblyUnhollower = new char[version.size() + 1];
			std::copy(version.begin(), version.end(), AssemblyGenerator::ForceVersion_Il2CppAssemblyUnhollower);
			AssemblyGenerator::ForceVersion_Il2CppAssemblyUnhollower[version.size()] = '\0';
		}
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
			(iniFile->ReadValue("Console", "Mode")._Equal("3") ? Console::DisplayMode::RANDOMRAINBOW : Console::Mode))));
	iniFile->WriteValue("Console", "Mode", std::to_string(Console::Mode));
	Console::AlwaysOnTop = (!iniFile->ReadValue("Console", "AlwaysOnTop").empty() && iniFile->ReadValue("Console", "AlwaysOnTop")._Equal("true"));
	iniFile->WriteValue("Console", "AlwaysOnTop", (Console::AlwaysOnTop ? "true" : "false"));
#ifndef DEBUG
	Console::HideWarnings = (!iniFile->ReadValue("Console", "HideWarnings").empty() && iniFile->ReadValue("Console", "HideWarnings")._Equal("true"));
	iniFile->WriteValue("Console", "HideWarnings", (Console::AlwaysOnTop ? "true" : "false"));

	Logger::MaxLogs = (!iniFile->ReadValue("Logger", "MaxLogs").empty() ? GetIntFromConstChar(iniFile->ReadValue("Console", "MaxLogs").c_str(), Logger::MaxLogs) : Logger::MaxLogs);
	iniFile->WriteValue("Logger", "MaxLogs", std::to_string(Logger::MaxLogs));
	Logger::MaxWarnings = (!iniFile->ReadValue("Logger", "MaxWarnings").empty() ? GetIntFromConstChar(iniFile->ReadValue("Console", "MaxWarnings").c_str(), Logger::MaxWarnings) : Logger::MaxWarnings);
	iniFile->WriteValue("Logger", "MaxWarnings", std::to_string(Logger::MaxWarnings));
	Logger::MaxErrors = (!iniFile->ReadValue("Logger", "MaxErrors").empty() ? GetIntFromConstChar(iniFile->ReadValue("Console", "MaxErrors").c_str(), Logger::MaxErrors) : Logger::MaxErrors);
	iniFile->WriteValue("Logger", "MaxErrors", std::to_string(Logger::MaxErrors));
#endif

	AnalyticsBlocker::ShouldDAB = (!iniFile->ReadValue("AnalyticsBlocker", "ShouldDAB").empty() && iniFile->ReadValue("AnalyticsBlocker", "ShouldDAB")._Equal("true"));
	iniFile->WriteValue("AnalyticsBlocker", "ShouldDAB", (AnalyticsBlocker::ShouldDAB ? "true" : "false"));

	InternalCalls::MelonHandler::LoadModeForPlugins = (iniFile->ReadValue("LoadMode", "Plugins").empty() ? InternalCalls::MelonHandler::LoadMode::NORMAL : (
		iniFile->ReadValue("LoadMode", "Plugins")._Equal("1") ? InternalCalls::MelonHandler::LoadMode::DEV : (
			iniFile->ReadValue("LoadMode", "Plugins")._Equal("2") ? InternalCalls::MelonHandler::LoadMode::BOTH : InternalCalls::MelonHandler::LoadMode::NORMAL)));
	iniFile->WriteValue("LoadMode", "Plugins", std::to_string(InternalCalls::MelonHandler::LoadModeForPlugins));
	InternalCalls::MelonHandler::LoadModeForMods = (iniFile->ReadValue("LoadMode", "Mods").empty() ? InternalCalls::MelonHandler::LoadMode::NORMAL : (
		iniFile->ReadValue("LoadMode", "Mods")._Equal("1") ? InternalCalls::MelonHandler::LoadMode::DEV : (
			iniFile->ReadValue("LoadMode", "Mods")._Equal("2") ? InternalCalls::MelonHandler::LoadMode::BOTH : InternalCalls::MelonHandler::LoadMode::NORMAL)));
	iniFile->WriteValue("LoadMode", "Mods", std::to_string(InternalCalls::MelonHandler::LoadModeForMods));

	if (iniFile->ReadValue("AssemblyGenerator", "ForceUnityDependencies").empty() || !iniFile->ReadValue("AssemblyGenerator", "ForceUnityDependencies")._Equal("true"))
	{
		iniFile->WriteValue("AssemblyGenerator", "ForceUnityDependencies", "false");
		iniFile->WriteValue("AssemblyGenerator", "ForceUnityDependencies_Version", "0.0.0.0");
	}
	else
	{
		std::string version = iniFile->ReadValue("AssemblyGenerator", "ForceUnityDependencies_Version");
		if (!version.empty())
		{
			AssemblyGenerator::ForceVersion_UnityDependencies = new char[version.size() + 1];
			std::copy(version.begin(), version.end(), AssemblyGenerator::ForceVersion_UnityDependencies);
			AssemblyGenerator::ForceVersion_UnityDependencies[version.size()] = '\0';
		}
		iniFile->WriteValue("AssemblyGenerator", "ForceUnityDependencies_Version", (std::string(AssemblyGenerator::ForceVersion_UnityDependencies).empty() ? "0.0.0.0" : AssemblyGenerator::ForceVersion_UnityDependencies));
	}
	if (iniFile->ReadValue("AssemblyGenerator", "ForceIl2CppDumper").empty() || !iniFile->ReadValue("AssemblyGenerator", "ForceIl2CppDumper")._Equal("true"))
	{
		iniFile->WriteValue("AssemblyGenerator", "ForceIl2CppDumper", "false");
		iniFile->WriteValue("AssemblyGenerator", "ForceIl2CppDumper_Version", "0.0.0.0");
	}
	else
	{
		std::string version = iniFile->ReadValue("AssemblyGenerator", "ForceIl2CppDumper_Version");
		if (!version.empty())
		{
			AssemblyGenerator::ForceVersion_Il2CppDumper = new char[version.size() + 1];
			std::copy(version.begin(), version.end(), AssemblyGenerator::ForceVersion_Il2CppDumper);
			AssemblyGenerator::ForceVersion_Il2CppDumper[version.size()] = '\0';
		}
		iniFile->WriteValue("AssemblyGenerator", "ForceIl2CppDumper_Version", (std::string(AssemblyGenerator::ForceVersion_Il2CppDumper).empty() ? "0.0.0.0" : AssemblyGenerator::ForceVersion_Il2CppDumper));
	}
	if (iniFile->ReadValue("AssemblyGenerator", "ForceIl2CppAssemblyUnhollower").empty() || !iniFile->ReadValue("AssemblyGenerator", "ForceIl2CppAssemblyUnhollower")._Equal("true"))
	{
		iniFile->WriteValue("AssemblyGenerator", "ForceIl2CppAssemblyUnhollower", "false");
		iniFile->WriteValue("AssemblyGenerator", "ForceIl2CppAssemblyUnhollower_Version", "0.0.0.0");
	}
	else
	{
		std::string version = iniFile->ReadValue("AssemblyGenerator", "ForceIl2CppAssemblyUnhollower_Version");
		if (!version.empty())
		{
			AssemblyGenerator::ForceVersion_Il2CppAssemblyUnhollower = new char[version.size() + 1];
			std::copy(version.begin(), version.end(), AssemblyGenerator::ForceVersion_Il2CppAssemblyUnhollower);
			AssemblyGenerator::ForceVersion_Il2CppAssemblyUnhollower[version.size()] = '\0';
		}
		iniFile->WriteValue("AssemblyGenerator", "ForceIl2CppAssemblyUnhollower_Version", (std::string(AssemblyGenerator::ForceVersion_Il2CppAssemblyUnhollower).empty() ? "0.0.0.0" : AssemblyGenerator::ForceVersion_Il2CppAssemblyUnhollower));
	}
}
#endif