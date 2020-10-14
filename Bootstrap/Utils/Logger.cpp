#include "Logger.h"
#include "../Managers/Game.h"
#include "../Base/Core.h"
#include "Assertion.h"
#include "Debug.h"
#include <direct.h>
#include <list>
#include <sstream>

const char* Logger::FilePrefix = "MelonLoader_";
const char* Logger::FileExtension = ".log";
int Logger::MaxLogs = 10;
int Logger::MaxWarnings = 100;
int Logger::MaxErrors = 100;
int Logger::WarningCount = 0;
int Logger::ErrorCount = 0;
Logger::FileStream Logger::LogFile;

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
	filepath << logFolderPath << "\\" << FilePrefix << std::put_time(&bt, "%y-%m-%d_%OH-%OM-%OS") << "." << std::setfill('0') << std::setw(3) << ms.count() << FileExtension;
	LogFile.coss = std::ofstream(filepath.str());
	std::string latest_path = (std::string(Game::BasePath) + "\\MelonLoader\\latest.log");
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

void Logger::WriteTimestamp(Console::Color color)
{
	std::chrono::system_clock::time_point now;
	std::chrono::milliseconds ms;
	std::tm bt;
	Core::GetLocalTime(&now, &ms, &bt);
	std::stringstream output;
	output << std::put_time(&bt, "%H:%M:%S") << "." << std::setfill('0') << std::setw(3) << ms.count();
	LogFile << "[" << output.str() << "] ";
	if (color == Console::Color::Black)
	{
		Console::SetColor(Console::Color::Gray);
		Console::Write("[");
		Console::SetColor(Console::Color::Green);
		Console::Write(output.str().c_str());
		Console::SetColor(Console::Color::Gray);
		Console::Write("] ");
		return;
	}
	Console::SetColor(color);
	Console::Write("[");
	Console::Write(output.str().c_str());
	Console::Write("] ");
}

void Logger::WriteSpacer()
{
	LogFile << std::endl;
	Console::Write("\n");
}

void Logger::Msg(const char* txt)
{
	WriteTimestamp(Console::Color::Black);
	LogFile << txt << std::endl;
	Console::SetColor(Console::Color::Gray);
	Console::Write(txt);
	Console::Write("\n");
}

void Logger::Warning(const char* txt)
{
	if (MaxWarnings > 0)
	{
		if (WarningCount >= MaxWarnings)
			return;
		WarningCount++;
	}
	WriteTimestamp(Console::Color::Yellow);
	LogFile << "[WARNING] " << txt << std::endl;
	Console::Write("[WARNING] ");
	Console::Write(txt);
	Console::Write("\n");
}

void Logger::Error(const char* txt)
{
	if (MaxErrors > 0)
	{
		if (ErrorCount >= MaxErrors)
			return;
		ErrorCount++;
	}
	WriteTimestamp(Console::Color::Red);
	Console::SetColor(Console::Color::Red);
	LogFile << "[ERROR] " << txt << std::endl;
	Console::Write("[ERROR] ");
	Console::Write(txt);
	Console::Write("\n");
}

void Logger::Internal_Msg(const char* namesection, const char* txt)
{
	if (namesection == NULL)
	{
		Msg(txt);
		return;
	}
	WriteTimestamp(Console::Color::Black);
	LogFile << "[" << namesection << "] " << txt << std::endl;
	Console::SetColor(Console::Color::Gray);
	Console::Write("[");
	Console::SetColor(Console::Color::Magenta);
	Console::Write(namesection);
	Console::SetColor(Console::Color::Gray);
	Console::Write("] ");
	Console::SetColor(Console::Color::Gray);
	Console::Write(txt);
	Console::Write("\n");
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
	WriteTimestamp(Console::Color::Yellow);
	LogFile << "[" << namesection << "] [WARNING] " << txt << std::endl;
	Console::Write("[");
	Console::Write(namesection);
	Console::Write("] [WARNING] ");
	Console::Write(txt);
	Console::Write("\n");
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
	WriteTimestamp(Console::Color::Red);
	Console::Write("[");
	Console::Write(namesection);
	Console::Write("] [ERROR] ");
	Console::Write(txt);
	Console::Write("\n");
}