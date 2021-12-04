#pragma once
#include <ostream>
#include "../Console.h"

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
	std::make_pair<LogType, LogMeta*>(Warning, new LogMeta("WARNING", true, Console::Color::Yellow)),
	std::make_pair<LogType, LogMeta*>(Error, new LogMeta("ERROR", true, Console::Color::Red)),
	std::make_pair<LogType, LogMeta*>(Debug, new LogMeta("DEBUG", false, Console::Color::Blue))
};

struct Log
{
	LogMeta* logMeta;
	
	Console::Color melonAnsiColor, textAnsiColor;
	
	const char* namesection;
	const char* txt;
	
	Log(const LogType type, const Console::Color meloncolor, const Console::Color txtcolor, const char* namesection, const char* txt) :
	logMeta(LogTypes[type].second),
	melonAnsiColor(logMeta->GetColorOverride(meloncolor)),	// If the log meta says we need to color the whole string,
	textAnsiColor(logMeta->GetColorOverride(txtcolor)),		// swap out the input colors with the overrides to avoid confusion
	namesection(namesection), txt(txt) {}

	Log(const LogType type, const Console::Color txtcolor, const char* namesection, const char* txt) :
	Log(type, Console::Color::Gray, txtcolor, namesection, txt) {}

	Log(const LogType type, const char* namesection, const char* txt) :
	Log(type, Console::Color::Gray, namesection, txt) {}

	void BuildConsoleString(std::ostream& stream) const;	// Constructs a string with color to print to the console (Doesn't include linebreak)
	std::string BuildLogString() const;	// Constructs a string without color to log to file (Doesn't include linebreak)
};
