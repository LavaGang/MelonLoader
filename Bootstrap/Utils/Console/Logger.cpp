#include "Logger.h"
#include "../../Managers/Game.h"
#include "../../Base/Core.h"
#include "../Assertion.h"
#include "Debug.h"
#include <stdio.h>

#ifdef _WIN32
#include <direct.h>
#elif defined(__ANDROID__)
#include <android/log.h>
#endif

#include <list>
#include <sstream>
#include <iostream>

const char* Logger::FilePrefix = "MelonLoader_";
const char* Logger::FileExtension = ".log";
const char* Logger::LatestLogFileName = "Latest";
int Logger::MaxLogs = 10;
int Logger::MaxWarnings = 100;
int Logger::MaxErrors = 100;
int Logger::WarningCount = 0;
int Logger::ErrorCount = 0;

#ifdef PORT_DISABLE
Logger::FileStream Logger::LogFile;
#endif

bool Logger::Initialize()
{
	if (Debug::Enabled)
	{
		MaxLogs = 0;
		MaxWarnings = 0;
		MaxErrors = 0;
	}

#ifdef PORT_DISABLE
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
	filepath << logFolderPath << "\\" << FilePrefix << std::put_time(&bt, "%y-%m-%d_%OH-%OM-%OS") << "." << std::setfill('0') << std::setw(3) << ms.count() << FileExtension;
	LogFile.coss = std::ofstream(filepath.str());
	std::string latest_path = (std::string(Game::BasePath) + "\\MelonLoader\\" + LatestLogFileName + FileExtension);
	if (Core::FileExists(latest_path.c_str()))
		std::remove(latest_path.c_str());
	LogFile.latest = std::ofstream(latest_path.c_str());
#endif
	return true;
}

#ifdef PORT_DISABLE
#endif

std::string Logger::GetTimestamp()
{
#ifdef PORT_DISABLE
	std::chrono::system_clock::time_point now;
	std::chrono::milliseconds ms;
	std::tm bt;
	Core::GetLocalTime(&now, &ms, &bt);
	std::stringstream timestamp;

	timestamp << "placeholder time";
	
	return timestamp.str();
#else
	return "PLACEHOLDER TIME";
#endif
}

void Logger::WriteSpacer()
{
#ifdef __ANDROID__
	// todo: write to logfile
	__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "");
#elif _WIN32
	LogFile << std::endl;
	std::cout << std::endl;
#endif
}

void Logger::Msg(Console::Color txtcolor, const char* txt)
{
	Msgf(txtcolor, "%s", txt);
}

void Logger::Warning(const char* txt)
{
	Warningf("%s", txt);
}

void Logger::Error(const char* txt)
{
	Errorf("%s", txt);
}

void Logger::Msgf(Console::Color txtcolor, const char* fmt, ...)
{
	va_list args;
	va_start(args, fmt);
	Logger::vMsgf(txtcolor, fmt, args);
	va_end(args);
}

void Logger::Warningf(const char* fmt, ...)
{
	va_list args;
	va_start(args, fmt);
	Logger::vWarningf(fmt, args);
	va_end(args);
}

void Logger::Errorf(const char* fmt, ...)
{
	va_list args;
	va_start(args, fmt);
	Logger::vErrorf(fmt, args);
	va_end(args);
}

void Logger::vMsgf(Console::Color txtcolor, const char* fmt, va_list args)
{
#ifdef __ANDROID__
	Internal_vDirectWritef(txtcolor, LogLevel::Warning, NULL, 0, fmt, args);
#else
	const Logger::MessagePrefix prefixes[]{
		Logger::MessagePrefix{
			Console::Green,
			GetTimestamp().c_str()
		}
	};

	Internal_vDirectWritef(txtcolor, LogLevel::Warning, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), fmt, args);
#endif
}

void Logger::vWarningf(const char* fmt, va_list args)
{
	if (MaxWarnings > 0)
	{
		if (WarningCount >= MaxWarnings)
			return;
		WarningCount++;
	}

	const Logger::MessagePrefix prefixes[]{
#ifndef __ANDROID__
		Logger::MessagePrefix{
			Console::Yellow,
			GetTimestamp().c_str()
		},
#endif
		Logger::MessagePrefix{
			Console::Yellow,
			"WARNING"
		}
	};

	Internal_vDirectWritef(Console::Color::Yellow, LogLevel::Warning, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), fmt, args);
}

void Logger::vErrorf(const char* fmt, va_list args)
{
	if (MaxErrors > 0)
	{
		if (ErrorCount >= MaxErrors)
			return;
		ErrorCount++;
	}

	const Logger::MessagePrefix prefixes[]{
#ifndef __ANDROID__
		Logger::MessagePrefix{
			Console::Red,
			GetTimestamp().c_str()
		},
#endif
		Logger::MessagePrefix{
			Console::Red,
			"ERROR"
		}
	};

	Internal_vDirectWritef(Console::Color::Red, LogLevel::Warning, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), fmt, args);
}

void Logger::Internal_PrintModName(Console::Color color, const char* name, const char* version)
{
#ifndef __ANDROID__
	const Logger::MessagePrefix prefixes[]{
		Logger::MessagePrefix{
			Console::Green,
			GetTimestamp().c_str()
		}
	};
#endif

	std::stringstream versionStr;
	versionStr << Console::ColorToAnsi(color)
		<< name
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< " v"
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< version;
#ifdef __ANDROID__
	Internal_DirectWrite(color, LogLevel::Warning, NULL, 0, versionStr.str().c_str());
#else
	Internal_DirectWrite(color, LogLevel::Warning, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), versionStr.str().c_str());
#endif
}

void Logger::Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, const char* namesection, const char* txt)
{
	Internal_Msgf(meloncolor, txtcolor, namesection, "%s", txt);
}

void Logger::Internal_Warning(const char* namesection, const char* txt)
{
	Internal_Warningf(namesection, "%s", txt);
}

void Logger::Internal_Error(const char* namesection, const char* txt)
{
	Internal_Errorf(namesection, "%s", txt);
}

void Logger::Internal_Msgf(Console::Color meloncolor, Console::Color txtcolor, const char* namesection, const char* fmt, ...)
{
	va_list args;
	va_start(args, fmt);
	Logger::Internal_vMsgf(meloncolor, txtcolor, namesection, fmt, args);
	va_end(args);
}

void Logger::Internal_Warningf(const char* namesection, const char* fmt, ...)
{
	va_list args;
	va_start(args, fmt);
	Logger::Internal_vWarningf(namesection, fmt, args);
	va_end(args);
}

void Logger::Internal_Errorf(const char* namesection, const char* fmt, ...)
{
	va_list args;
	va_start(args, fmt);
	Logger::Internal_vErrorf(namesection, fmt, args);
	va_end(args);
}

void Logger::Internal_vMsgf(Console::Color meloncolor, Console::Color txtcolor, const char* namesection, const char* fmt, va_list args)
{
	if (namesection == NULL)
	{
		vMsgf(txtcolor, fmt, args);
		return;
	}

	const Logger::MessagePrefix prefixes[]{
#ifndef __ANDROID__
		Logger::MessagePrefix{
			Console::Green,
			GetTimestamp().c_str()
		},
#endif
		Logger::MessagePrefix{
			meloncolor,
			namesection
		}
	};

	Internal_vDirectWritef(txtcolor, LogLevel::Warning, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), fmt, args);
}

void Logger::Internal_vWarningf(const char* namesection, const char* fmt, va_list args)
{
	if (namesection == NULL)
	{
		vWarningf(fmt, args);
		return;
	}

	if (MaxWarnings > 0)
	{
		if (WarningCount >= MaxWarnings)
			return;
		WarningCount++;
	}

	const Logger::MessagePrefix prefixes[]{
#ifndef __ANDROID__
		Logger::MessagePrefix{
			Console::Yellow,
			GetTimestamp().c_str()
		},
#endif
		Logger::MessagePrefix{
			Console::Yellow,
			namesection
		},
		Logger::MessagePrefix{
			Console::Yellow,
			"WARNING"
		},
	};

	Internal_vDirectWritef(Console::Color::Yellow, LogLevel::Warning, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), fmt, args);
}

void Logger::Internal_vErrorf(const char* namesection, const char* fmt, va_list args)
{
	if (namesection == NULL)
	{
		vErrorf(fmt, args);
		return;
	}

	if (MaxErrors > 0)
	{
		if (ErrorCount >= MaxErrors)
			return;
		ErrorCount++;
	}

	const Logger::MessagePrefix prefixes[]{
#ifndef __ANDROID__
		Logger::MessagePrefix{
			Console::Red,
			GetTimestamp().c_str()
		},
#endif
		Logger::MessagePrefix{
			Console::Red,
			namesection
		},
		Logger::MessagePrefix{
			Console::Red,
			"ERROR"
		},
	};

	Internal_vDirectWritef(Console::Color::Red, LogLevel::Error, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), fmt, args);
}

void Logger::Internal_DirectWrite(Console::Color txtcolor, LogLevel level, const MessagePrefix prefixes[], const int size, const char* txt)
{
	Logger::Internal_DirectWritef(txtcolor, level, prefixes, size, "%s", txt);
}

void Logger::Internal_DirectWritef(Console::Color txtcolor, LogLevel level, const MessagePrefix prefixes[], const int size, const char* fmt, ...)
{
	va_list args;
	va_start(args, fmt);
	Logger::Internal_vDirectWritef(txtcolor, level, prefixes, size, fmt, args);
	va_end(args);
}

void Logger::Internal_vDirectWritef(Console::Color txtcolor, LogLevel level, const MessagePrefix prefixes[], const int size, const char* fmt, va_list args)
{
	std::stringstream msgColor;
	std::stringstream msgPlain;

	for (int i = 0; i < size; i++)
	{
		msgColor << Console::ColorToAnsi(Console::Color::Gray)
			<< "["
			<< Console::ColorToAnsi(prefixes[i].Color)
			<< prefixes[i].Message
			<< Console::ColorToAnsi(Console::Color::Gray)
			<< "]"
			<< Console::ColorToAnsi(Console::Color::Reset)
			<< " ";

		msgPlain << "["
			<< prefixes[i].Message
			<< "] ";
	}

	// prob super inefficient to malloc then free
	// TODO: find a better (threadsafe) way
	char* buffer = (char*)malloc(vsnprintf(NULL, 0, fmt, args));
	vsprintf(buffer, fmt, args);

#ifdef __ANDROID__
	msgColor
		<< Console::ColorToAnsi(txtcolor)
		<< buffer
		<< Console::ColorToAnsi(Console::Color::Reset);

	msgPlain
		<< buffer
		<< Console::ColorToAnsi(Console::Color::Reset);
#else
	msgColor
		<< Console::ColorToAnsi(txtcolor)
		<< buffer
		<< std::endl
		<< Console::ColorToAnsi(Console::Color::Reset);
	msgPlain
		<< buffer
		<< std::endl
		<< Console::ColorToAnsi(Console::Color::Reset);
#endif

#ifdef __ANDROID__
	// todo: write to logfile
	const char* messageC = msgColor.str().c_str();
	__android_log_print(ANDROID_LOG_INFO, "MelonLoader", buffer);
#elif defined(_WIN32)
	//TODO: implement printf
	LogFile << msgPlain.str();
	std::cout << msgColor.str();
#endif

	// oof
	free(buffer);
}
