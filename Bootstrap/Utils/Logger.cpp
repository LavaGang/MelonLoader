#include "Logger.h"
#include "../Managers/Game.h"
#include "../Core.h"
#include "Assertion.h"
#include "Debug.h"
#include <direct.h>
#include <list>
#include <sstream>

const char* Logger::FilePrefix = "MelonLoader_";
const char* Logger::FileExtension = ".log";
const char* Logger::LatestLogFileName = "Latest";
int Logger::MaxLogs = 10;
int Logger::MaxWarnings = 100;
int Logger::MaxErrors = 100;
int Logger::WarningCount = 0;
int Logger::ErrorCount = 0;
Logger::FileStream Logger::LogFile;

std::string Log::BuildConsoleString() const
{
	// Always initialize string with timestamp
	std::string consoleStr = 
		Console::ColorToAnsi(logMeta->GetColorOverride(Console::Color::Gray)) +
		"[" +
		Console::ColorToAnsi(logMeta->GetColorOverride(Console::Color::Green)) +
		Logger::GetTimestamp() +
		Console::ColorToAnsi(logMeta->GetColorOverride(Console::Color::Gray)) +
		"] ";

	// If the logging melon has a name, print it
	if (namesection != nullptr) consoleStr = consoleStr + "[" +
		melonAnsiColor +
		namesection +
		Console::ColorToAnsi(logMeta->GetColorOverride(Console::Color::Gray)) +
		"] ";

	// Print the [LOGTYPE] prefix if needed
	if (logMeta->printLogTypeName) consoleStr = consoleStr + "[" + Console::ColorToAnsi(logMeta->logCategoryColor) + logMeta->logTypeString + Console::ColorToAnsi(logMeta->GetColorOverride(Console::Color::Gray)) + "] ";

	// If we're not coloring the whole line, use the specified input text color. If we are, the color would already be declared
	if (!logMeta->colorFullLine) consoleStr = consoleStr + textAnsiColor;
	
	return consoleStr +
		txt +
		Console::ColorToAnsi(logMeta->GetColorOverride(Console::Color::Gray), false);
}

std::string Log::BuildLogString() const
{
	std::string logStr = "[" + Logger::GetTimestamp() + "] ";
	if (namesection != nullptr)	logStr = logStr + "[" + namesection + "] ";
	if (logMeta->printLogTypeName) logStr = logStr + "[" + logMeta->logTypeString + "] ";
	return logStr + txt;
}

void Log::LogToConsoleAndFile() const
{
	std::cout << BuildConsoleString();
	Logger::LogFile << BuildLogString();
	Logger::WriteSpacer();
}

bool Logger::Initialize()
{
	if (Debug::Enabled)
	{
		MaxLogs = 0;
		MaxWarnings = 0;
		MaxErrors = 0;
	}
	std::string logFolderPath = std::string(Game::BasePath) + "\\MelonLoader\\Logs";
	if (Core::DirectoryExists(logFolderPath.c_str()))
		CleanOldLogs(logFolderPath.c_str());
	else if (_mkdir(logFolderPath.c_str()) != 0)
	{
		Assertion::ThrowInternalFailure("Failed to Create Logs Folder!");
		return false;
	}
	std::chrono::system_clock::time_point now;
	std::chrono::milliseconds ms;
	std::tm bt;	
	Core::GetLocalTime(&now, &ms, &bt);
	std::stringstream filepath;
	filepath << logFolderPath << "\\" << FilePrefix << std::put_time(&bt, "%y-%m-%d_%H-%M-%S") << "." << std::setfill('0') << std::setw(3) << ms.count() << FileExtension;
	LogFile.coss = std::ofstream(filepath.str());
	std::string latest_path = (std::string(Game::BasePath) + "\\MelonLoader\\" + LatestLogFileName + FileExtension);
	if (Core::FileExists(latest_path.c_str()))
		std::remove(latest_path.c_str());
	LogFile.latest = std::ofstream(latest_path.c_str());
	return true;
}

void Logger::CleanOldLogs(const char* path)
{
	if (MaxLogs <= 0)
		return;
	std::list<std::filesystem::directory_entry>entry_list;
	for (std::filesystem::directory_entry entry : std::filesystem::directory_iterator(path))
	{
		if (!entry.is_regular_file())
			continue;
		std::string entry_file = entry.path().filename().generic_string();
		if ((entry_file.rfind(FilePrefix, NULL) == NULL) && (entry_file.rfind(FileExtension) == (entry_file.size() - std::string(FileExtension).size())))
			entry_list.push_back(entry);
	}
	if (entry_list.size() < MaxLogs)
		return;
	entry_list.sort(Logger::CompareWritetime);
	for (std::list<std::filesystem::directory_entry>::iterator it = std::next(entry_list.begin(), (MaxLogs - 1)); it != entry_list.end(); ++it)
		remove((*it).path().u8string().c_str());
}

std::string Logger::GetTimestamp()
{
	std::chrono::system_clock::time_point now;
	std::chrono::milliseconds ms;
	std::tm bt;
	Core::GetLocalTime(&now, &ms, &bt);
	std::stringstream timestamp;
	timestamp << std::put_time(&bt, "%H:%M:%S") << "." << std::setfill('0') << std::setw(3) << ms.count();
	return timestamp.str();
}

void Logger::WriteSpacer()
{
	LogFile << std::endl;
	std::cout << std::endl;
}

void Logger::Internal_PrintModName(Console::Color meloncolor, const char* name, const char* version)
{
	// Not using log object for this as we're modifying conventional coloring
	std::string timestamp = GetTimestamp();
	LogFile << "[" << timestamp << "] " << name << " v" << version << std::endl;
	std::cout
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< "["
		<< Console::ColorToAnsi(Console::Color::Green)
		<< timestamp
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< "] "
		<< Console::ColorToAnsi(meloncolor)
		<< name
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< " v"
		<< version
		<< std::endl
		<< Console::ColorToAnsi(Console::Color::Gray, false);
}

void Logger::Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, const char* namesection, const char* txt)
{
	Log(Msg, meloncolor, txtcolor, namesection, txt).LogToConsoleAndFile();
}

void Logger::Internal_Warning(const char* namesection, const char* txt)
{
	if (MaxWarnings > 0)
	{
		if (WarningCount >= MaxWarnings)
			return;
		WarningCount++;
	}
	else if (MaxWarnings < 0)
		return;

	Log(Warning, namesection, txt).LogToConsoleAndFile();
}

void Logger::Internal_Error(const char* namesection, const char* txt)
{
	if (MaxErrors > 0)
	{
		if (ErrorCount >= MaxErrors)
			return;
		ErrorCount++;
	}
	else if (MaxErrors < 0)
		return;
	
	Log(Error, namesection, txt).LogToConsoleAndFile();
}