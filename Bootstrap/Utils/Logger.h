#pragma once
#include <fstream>
#include <filesystem>
#include <iostream>
#include <string>
#include "Console.h"


enum LogType
{
	Msg,
	Warning,
	Error,
	Debug
};

// Contains metadata about all log types and how they should be constructed in console and log file
struct LogMeta
{
	Console::Color logCategoryColor = Console::Color::Gray;	// Used to color the log type and as an override if we need to color the whole log
	const char* logTypeString;	// A string literal representation of the log name. E.G. 'WARNING', 'ERROR'
	bool colorFullLine;	// Override ALL line colors with the log category color. Used primarily for Warning and Error logs to stand out
	bool printLogTypeName;	// The [LOGTYPE] text

	explicit LogMeta(const char* stringLiteral = nullptr, const bool _colorFullLine = false, const Console::Color logMainColor = static_cast<Console::Color>(-1)) :
	logCategoryColor(logMainColor), logTypeString(stringLiteral), colorFullLine(_colorFullLine && logMainColor >= 0), printLogTypeName(stringLiteral != nullptr)
	{}

	// If we're printing a log category that requires the whole line to be one color, use that color instead
	Console::Color GetColorOverride(Console::Color colorToReturnIfNone)
	{return colorFullLine ? logCategoryColor : colorToReturnIfNone;}
};

// Declares metadata for logs
static std::pair<LogType, LogMeta*> LogTypes[] = {
	std::make_pair<LogType, LogMeta*>(Msg, new LogMeta()),
	std::make_pair<LogType, LogMeta*>(Msg, new LogMeta("WARNING", true, Console::Color::Yellow)),
	std::make_pair<LogType, LogMeta*>(Msg, new LogMeta("ERROR", true, Console::Color::Red)),
	std::make_pair<LogType, LogMeta*>(Msg, new LogMeta("DEBUG", false, Console::Color::Blue))
};

class Log
{
	LogMeta* logMeta;
	
	std::string melonAnsiColor, textAnsiColor;
	
	const char* namesection;
	const char* txt;
	
public:
	Log(const LogType type, const Console::Color meloncolor, const Console::Color txtcolor, const char* namesection, const char* txt) :
	logMeta(LogTypes[type].second),
	melonAnsiColor(Console::ColorToAnsi(logMeta->GetColorOverride(meloncolor))),	// If the log meta says we need to color the whole string,
	textAnsiColor(Console::ColorToAnsi(logMeta->GetColorOverride(txtcolor))),		// swap out the input colors with the overrides to avoid confusion
	namesection(namesection), txt(txt) {}

	Log(const LogType type, const Console::Color txtcolor, const char* namesection, const char* txt) :
	Log(type, Console::Color::Gray, txtcolor, namesection, txt) {}

	Log(const LogType type, const char* namesection, const char* txt) :
	Log(type, Console::Color::Gray, namesection, txt) {}

	std::string BuildConsoleString() const;	// Constructs a string with color to print to the console (Doesn't include linebreak)
	std::string BuildLogString() const;	// Constructs a string without color to log to file (Doesn't include linebreak)

	void LogToConsoleAndFile() const;	// Shorthand way to log to console and write to file with linebreak afterwards
};

class Logger
{
public:
	static int MaxLogs;
	static int MaxWarnings;
	static int MaxErrors;
	static bool Initialize();
	static std::string GetTimestamp();
	static void WriteSpacer();

	static void Internal_PrintModName(Console::Color meloncolor, const char* name, const char* version);
	
	static void QuickLog(const char* txt, LogType logType = Msg)	// Like std::cout but prepends timestamp and appends to logfile
	{Log(logType, nullptr, txt).LogToConsoleAndFile();}
	
	static void Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, const char* namesection, const char* txt);
	static void Internal_Warning(const char* namesection, const char* txt);
	static void Internal_Error(const char* namesection, const char* txt);

	class FileStream
	{
	public:
		std::ofstream coss;
		std::ofstream latest;
		template <class T>
		FileStream& operator<< (T val) { if (coss.is_open()) coss << val; if (latest.is_open()) latest << val; return *this; }
		FileStream& operator<< (std::ostream& (*pfun)(std::ostream&)) { if (coss.is_open()) pfun(coss); if (latest.is_open()) pfun(latest); return *this; }
		void Flush() { if (coss.is_open()) coss.flush(); if (latest.is_open()) latest.flush(); }
	};
	static FileStream LogFile;
	static void Flush() { LogFile.Flush(); }

	static const char* LatestLogFileName;
	static const char* FileExtension;
private:
	static const char* FilePrefix;
	static int WarningCount;
	static int ErrorCount;
	static void CleanOldLogs(const char* path);
	static bool CompareWritetime(const std::filesystem::directory_entry& first, const std::filesystem::directory_entry& second) { return first.last_write_time().time_since_epoch() >= second.last_write_time().time_since_epoch(); }
};