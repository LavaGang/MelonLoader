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
	Error
};

static const char* LogTypeToString(LogType logType)
{
	switch (logType)
	{
		case Warning: return "WARNING";
		case Error: return "ERROR";

		default:
		case Msg: return "MSG";
	}
}

class Log
{
	LogType logType;
	
	std::string melonAnsiColor, textAnsiColor;
	
	const char* namesection;
	const char* txt;

	static Console::Color GetConsoleColorForLogType(const LogType type)
	{
		switch (type)
		{
			case Warning: return Console::Color::Yellow;
			case Error: return Console::Color::Red;
			default: return Console::Color::Gray;
		}
	}

	Console::Color CheckForColorOverride(const Console::Color colorSuggestion) const
	{
		if (logType == Msg) return colorSuggestion;
		return GetConsoleColorForLogType(logType);
	}
	
public:
	Log(const LogType type, const Console::Color meloncolor, const Console::Color txtcolor, const char* namesection, const char* txt) :
	logType(type),
	melonAnsiColor(Console::ColorToAnsi(CheckForColorOverride(meloncolor))),
	textAnsiColor(Console::ColorToAnsi(CheckForColorOverride(txtcolor))),
	namesection(namesection), txt(txt) {}

	Log(const LogType type, const Console::Color txtcolor, const char* namesection, const char* txt) :
	logType(type),
	melonAnsiColor(Console::ColorToAnsi(CheckForColorOverride(Console::Color::Gray))),
	textAnsiColor(Console::ColorToAnsi(CheckForColorOverride(txtcolor))),
	namesection(namesection), txt(txt) {}

	Log(const LogType type, const char* namesection, const char* txt) :
	logType(type),
	melonAnsiColor(Console::ColorToAnsi(GetConsoleColorForLogType(type))),
	textAnsiColor(Console::ColorToAnsi(GetConsoleColorForLogType(type))),
	namesection(namesection), txt(txt) {}

	std::string BuildConsoleString() const;
	std::string BuildLogString() const;
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
	static void QuickLog(const char* txt, LogType logType = Msg)
	{
		Log newLog = Log(logType, nullptr, txt);
		LogFile << newLog.BuildLogString();
		std::cout << newLog.BuildConsoleString();
	}
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