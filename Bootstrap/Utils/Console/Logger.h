#pragma once
#ifdef _WIN32
#include <Windows.h>
#include <filesystem>
#include <fstream>
#endif
#include <string>
#include <list>
#include <mutex>
#include <shared_mutex>
#include <thread>
#include <vector>

#include "Console.h"

enum LogLevel
{
	Verbose = 0,
	Info,
	Warning,
	Error
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

	static void Msg(const char* txt) { Msg(Console::Color::Gray, txt); }
	static void Msg(Console::Color txtcolor, const char* txt);
	static void Warning(const char* txt);
	static void Error(const char* txt);

	static void Msgf(Console::Color txtcolor, const char* fmt, ...);
	static void Warningf(const char* fmt, ...);
	static void Errorf(const char* fmt, ...);

	static void vMsgf(Console::Color txtcolor, const char* fmt, va_list args);
	static void vWarningf(const char* fmt, va_list args);
	static void vErrorf(const char* fmt, va_list args);

	static void Internal_PrintModName(Console::Color meloncolor, const char* name, const char* version);
	static void Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, const char* namesection, const char* txt);
	static void Internal_Warning(const char* namesection, const char* txt);
	static void Internal_Error(const char* namesection, const char* txt);

	static void Internal_Msgf(Console::Color meloncolor, Console::Color txtcolor, const char* namesection, const char* fmt, ...);
	static void Internal_Warningf(const char* namesection, const char* fmt, ...);
	static void Internal_Errorf(const char* namesection, const char* fmt, ...);

	static void Internal_vMsgf(Console::Color meloncolor, Console::Color txtcolor, const char* namesection, const char* fmt, va_list args);
	static void Internal_vWarningf(const char* namesection, const char* fmt, va_list args);
	static void Internal_vErrorf(const char* namesection, const char* fmt, va_list args);
	
	struct MessagePrefix
	{
		Console::Color Color;
		const std::string Message;
	};
	
	static void Internal_DirectWrite(Console::Color txtcolor, LogLevel level, const MessagePrefix prefixes[], const int size, const char* txt);
	static void Internal_DirectWritef(Console::Color txtcolor, LogLevel level, const MessagePrefix prefixes[], const int size, const char* fmt, ...);
	static void Internal_vDirectWritef(Console::Color txtcolor, LogLevel level, const MessagePrefix prefixes[], const int size, const char* fmt, va_list args);

	
#ifdef PORT_DISABLE
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
#endif

private:
	struct LogArgs
	{
		Console::Color txtcolor;
		LogLevel level;
		size_t prefixes_len;
		std::string buffer;
		std::vector<MessagePrefix> prefixes;

		// Thanks sc2ad for this pretty good code
		// Logic is now here so it's lifetime is guaranteed to be as long as we need it to be
		LogArgs(Console::Color color_, LogLevel level_, const MessagePrefix prefixes_[], const int size, const char* fmt, va_list args) : txtcolor(color_), level(level_),prefixes_len(size), prefixes(make_prefixes(prefixes_, size))
		{
			auto charBuffer = make_buffer(fmt, args);
			buffer.assign(charBuffer);
			delete charBuffer;
		}
	private:
		 static std::vector<MessagePrefix> make_prefixes(const MessagePrefix prefixes_[], const int size) {
			auto prefixes = std::vector<MessagePrefix>(size);
			memcpy(prefixes.data(), prefixes_, sizeof(MessagePrefix) * size);
			return prefixes;
		}
		static char* make_buffer(const char* fmt, va_list args) {
			char* buffer;
			auto sz = vsnprintf(nullptr, 0, fmt, args);
			if (sz <= 0) {
				buffer = "";
			} else {
				buffer = new char[sz];
				vsprintf(buffer, fmt, args);
			}
			return buffer;
		}
	};

	
	static void LogThreadHandle();
	static void LogWrite(LogArgs& pair);
	inline static std::mutex mutex_;
	inline static std::thread logThread;
	// Plain : Colored str
	inline static std::list<Logger::LogArgs> logQueue;

	static const char* FilePrefix;
	static const char* FileExtension;
	static const char* LatestLogFileName;
	static int WarningCount;
	static int ErrorCount;
	static void CleanOldLogs(const char* path);
#ifdef PORT_DISABLE
	static bool CompareWritetime(const std::filesystem::directory_entry& first, const std::filesystem::directory_entry& second) { return first.last_write_time().time_since_epoch() >= second.last_write_time().time_since_epoch(); }
#endif
};