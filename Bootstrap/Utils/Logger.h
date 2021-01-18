#pragma once
#include <Windows.h>
#include <fstream>
#include <filesystem>
#include <string>
#include "Console.h"

class Logger
{
public:
	static int MaxLogs;
	static int MaxWarnings;
	static int MaxErrors;
	static bool Initialize();
	static std::string GetTimestamp();
	static void WriteSpacer();

	static void Msg(const char* txt);
	static void Warning(const char* txt);
	static void Error(const char* txt);

	static void Internal_PrintModName(Console::Color color, const char* name, const char* version);
	static void Internal_Msg(Console::Color color, const char* namesection, const char* txt);
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

private:
	static const char* FilePrefix;
	static const char* FileExtension;
	static const char* LatestLogFileName;
	static int WarningCount;
	static int ErrorCount;
	static void CleanOldLogs(const char* path);
	static bool CompareWritetime(const std::filesystem::directory_entry& first, const std::filesystem::directory_entry& second) { return first.last_write_time().time_since_epoch() >= second.last_write_time().time_since_epoch(); }
};