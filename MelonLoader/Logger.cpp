#include <chrono>
#include <sstream>
#include <direct.h>
#include <ctime>
#include <list>
#include "Logger.h"
#include "MelonLoader.h"

LogStream Logger::logFile;

void Logger::Initialize(std::string filepathstr)
{
	auto now = std::chrono::system_clock::now();
	auto ms = std::chrono::duration_cast<std::chrono::milliseconds>(now.time_since_epoch()) % 1000;
	auto timer = std::chrono::system_clock::to_time_t(now);
	std::tm bt;
	localtime_s(&bt, &timer);
	std::string logFolderPath = filepathstr + "\\Logs";
	if (!MelonLoader::DirectoryExists(logFolderPath.c_str()))
		int returnval = _mkdir(logFolderPath.c_str());
	else
		CleanOldLogs(logFolderPath);
	std::stringstream filepath;
	filepath << logFolderPath << "\\MelonLoader_" << std::put_time(&bt, "%y-%m-%d_%OH-%OM-%OS") << "." << std::setfill('0') << std::setw(3) << ms.count() << ".log";
	logFile.coss = std::ofstream(filepath.str());
}

void Logger::CleanOldLogs(std::string logFolderPath)
{
	std::list<std::filesystem::directory_entry>entry_list;
	for (std::filesystem::directory_entry entry : std::filesystem::directory_iterator(logFolderPath))
		entry_list.push_back(entry);
	if (entry_list.size() >= 10)
	{
		entry_list.sort(Logger::CompareWritetime);
		for (std::list<std::filesystem::directory_entry>::iterator it = std::next(entry_list.begin(), 9); it != entry_list.end(); ++it)
			remove((*it).path().u8string().c_str());
	}
}

void Logger::LogTimestamp(ConsoleColor color)
{
	auto now = std::chrono::system_clock::now();
	auto ms = std::chrono::duration_cast<std::chrono::milliseconds>(now.time_since_epoch()) % 1000;
	auto timer = std::chrono::system_clock::to_time_t(now);
	std::tm bt;
	localtime_s(&bt, &timer);

	std::stringstream output;
	output << std::put_time(&bt, "%H:%M:%S") << "." << std::setfill('0') << std::setw(3) << ms.count();
	logFile << "[" << output.str() << "] ";

	if (MelonLoader::DebugMode)
	{
		Console::Write("[", ((color != ConsoleColor_Black) ? color : ConsoleColor_Gray));
		Console::Write(output.str(), ((color != ConsoleColor_Black) ? color : ConsoleColor_Green));
		Console::Write("] ", ((color != ConsoleColor_Black) ? color : ConsoleColor_Gray));
	}
}

void Logger::Log(const char* txt)
{
	LogTimestamp();
	logFile << txt << std::endl;
	if (MelonLoader::DebugMode)
	{
		Console::Write("[");
		Console::Write("MelonLoader", ConsoleColor_Magenta);
		Console::Write("] ");
		Console::WriteLine(txt);
	}
}

void Logger::Log(const char* txt, ConsoleColor color)
{
	LogTimestamp(color);
	logFile << txt << std::endl;
	if (MelonLoader::DebugMode)
	{
		Console::Write("[");
		Console::Write("MelonLoader", ConsoleColor_Magenta);
		Console::Write("] ");
		Console::WriteLine(txt, color);
	}
}

void Logger::LogError(const char* txt)
{
	LogTimestamp(ConsoleColor_Red);
	logFile << "[Error] " << txt << std::endl;
	if (MelonLoader::DebugMode)
	{
		Console::Write("[MelonLoader] ", ConsoleColor_Red);
		Console::WriteLine(("[Error] " + std::string(txt)), ConsoleColor_Red);
	}
}

void Logger::LogError(const char* namesection, const char* txt)
{
	LogTimestamp(ConsoleColor_Red);
	logFile << namesection << "[Error] " << txt << std::endl;
	if (MelonLoader::DebugMode)
	{
		Console::Write("[MelonLoader] ", ConsoleColor_Red);
		Console::WriteLine((std::string(namesection) + "[Error] " + std::string(txt)), ConsoleColor_Red);
	}
}

void Logger::LogModError(const char* namesection, const char* msg)
{
	LogTimestamp(ConsoleColor_Yellow);
	logFile << namesection << "[Error] " << msg << std::endl;
	if (MelonLoader::DebugMode)
	{
		Console::Write("[MelonLoader] ", ConsoleColor_Yellow);
		Console::WriteLine((std::string(namesection) + "[Error] " + std::string(msg)), ConsoleColor_Yellow);
	}
}

void Logger::LogModStatus(int type)
{
	LogTimestamp();
	logFile << "Status: " << ((type == 0) ? "Universal" : ((type == 1) ? "Compatible" : ((type == 2) ? "No MelonModGameAttribute!" : "INCOMPATIBLE!"))) << std::endl;
	if (MelonLoader::DebugMode)
	{
		Console::Write("[");
		Console::Write("MelonLoader", ConsoleColor_Magenta);
		Console::Write("] ");
		Console::Write("Status: ", ConsoleColor_Blue);
		if (type == 0)
			Console::WriteLine("Universal", ConsoleColor_Cyan);
		else if (type == 1)
			Console::WriteLine("Compatible", ConsoleColor_Green);
		else if (type == 2)
			Console::WriteLine("No MelonModGameAttribute!", ConsoleColor_Yellow);
		else
			Console::WriteLine("INCOMPATIBLE!", ConsoleColor_Red);
	}
}