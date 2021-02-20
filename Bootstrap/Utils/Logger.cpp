#include "Logger.h"
#include "../Managers/Game.h"
#include "../Base/Core.h"
#include "Assertion.h"
#include "Debug.h"

#ifdef _WIN32
#include <direct.h>
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

#ifdef PORT_DISABLE
#endif

void Logger::Msg(const char* txt)
{
	const Logger::MessagePrefix prefixes[]{
		Logger::MessagePrefix{
			Console::Green,
			GetTimestamp().c_str()
		}
	};

	Internal_DirectWrite(LogLevel::Warning, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), txt);
}

void Logger::Warning(const char* txt)
{
	if (MaxWarnings > 0)
	{
		if (WarningCount >= MaxWarnings)
			return;
		WarningCount++;
	}
	
	const Logger::MessagePrefix prefixes[]{
		Logger::MessagePrefix{
			Console::Yellow,
			GetTimestamp().c_str()
		},
		Logger::MessagePrefix{
			Console::Yellow,
			"WARNING"
		}
	};

	Internal_DirectWrite(LogLevel::Warning, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), txt);
}

void Logger::Error(const char* txt)
{
	if (MaxErrors > 0)
	{
		if (ErrorCount >= MaxErrors)
			return;
		ErrorCount++;
	}

	const Logger::MessagePrefix prefixes[]{
		Logger::MessagePrefix{
			Console::Red,
			GetTimestamp().c_str()
		},
		Logger::MessagePrefix{
			Console::Red,
			"ERROR"
		}
	};

	Internal_DirectWrite(LogLevel::Warning, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), txt);
}

void Logger::Internal_PrintModName(Console::Color color, const char* name, const char* version)
{
	const Logger::MessagePrefix prefixes[]{
		Logger::MessagePrefix{
			Console::Green,
			GetTimestamp().c_str()
		}
	};

	std::stringstream versionStr;
	versionStr << Console::ColorToAnsi(color)
		<< name
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< " v"
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< version;

	Internal_DirectWrite(LogLevel::Warning, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), versionStr.str().c_str());
}

void Logger::Internal_Msg(Console::Color color, const char* namesection, const char* txt)
{
	if (namesection == NULL)
	{
		Msg(txt);
		return;
	}

	const Logger::MessagePrefix prefixes[]{
		Logger::MessagePrefix{
			Console::Green,
			GetTimestamp().c_str()
		},
		Logger::MessagePrefix{
			color,
			namesection
		}
	};

	Internal_DirectWrite(LogLevel::Warning, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), txt);
}

void Logger::Internal_Warning(const char* namesection, const char* txt)
{
	if (namesection == NULL)
	{
		Warning(txt);
		return;
	}
	if (MaxWarnings > 0)
	{
		if (WarningCount >= MaxWarnings)
			return;
		WarningCount++;
	}
	
	const Logger::MessagePrefix prefixes[]{
		Logger::MessagePrefix{
			Console::Yellow,
			GetTimestamp().c_str()
		},
		Logger::MessagePrefix{
			Console::Yellow,
			namesection
		},
		Logger::MessagePrefix{
			Console::Yellow,
			"WARNING"
		},
	};

	Internal_DirectWrite(LogLevel::Warning, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), txt);
}

void Logger::Internal_Error(const char* namesection, const char* txt)
{
	if (namesection == NULL)
	{
		Error(txt);
		return;
	}

	if (MaxErrors > 0)
	{
		if (ErrorCount >= MaxErrors)
			return;
		ErrorCount++;
	}

	const Logger::MessagePrefix prefixes[]{
		Logger::MessagePrefix{
			Console::Red,
			GetTimestamp().c_str()
		},
		Logger::MessagePrefix{
			Console::Red,
			namesection
		},
		Logger::MessagePrefix{
			Console::Red,
			"ERROR"
		},
	};
	
	Internal_DirectWrite(LogLevel::Error, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), txt);
}

void Logger::Internal_DirectWrite(LogLevel level, const MessagePrefix prefixes[], const int size, const char* txt)
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

#ifdef __ANDROID__
	msgColor << txt;
	msgPlain << txt;
#else
	msgColor << txt
		<< std::endl
		<< Console::ColorToAnsi(Console::Color::Reset);
	msgPlain << txt
		<< std::endl
		<< Console::ColorToAnsi(Console::Color::Reset);
#endif

#ifdef __ANDROID__
// todo: write to logfile
	const char* messageC = msgColor.str().c_str();
	__android_log_print(ANDROID_LOG_INFO, "MelonLoader", messageC);
#elif defined(_WIN32)
	LogFile << msgPlain.str();
	std::cout << msgColor.str();
#endif
}