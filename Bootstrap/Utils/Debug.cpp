#include "Debug.h"
#include "Logger.h"
#include "Assertion.h"
#include <iostream>

#ifdef DEBUG
bool Debug::Enabled = true;
#else
bool Debug::Enabled = false;
#endif

void Debug::Msg(const char* txt)
{
	if (!Enabled || !Assertion::ShouldContinue)
		return;
	DirectWrite(txt);
}

void Debug::DirectWrite(const char* txt)
{
	std::string timestamp = Logger::GetTimestamp();
	Logger::LogFile << "[" << timestamp << "] [DEBUG] " << txt << std::endl;
	std::cout
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< "["
		<< Console::ColorToAnsi(Console::Color::Green)
		<< timestamp
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< "] "
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< "["
		<< Console::ColorToAnsi(Console::Color::Blue)
		<< "DEBUG"
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< "] "
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< txt
		<< std::endl
		<< "\x1b[37m";
}

void Debug::Internal_Msg(Console::Color color, const char* namesection, const char* txt)
{
	if (namesection == NULL)
	{
		Msg(txt);
		return;
	}
	if (!Enabled || !Assertion::ShouldContinue)
		return;
	std::string timestamp = Logger::GetTimestamp();
	Logger::LogFile << "[" << timestamp << "] [" << namesection << "] [DEBUG] " << txt << std::endl;
	std::cout
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< "["
		<< Console::ColorToAnsi(Console::Color::Green)
		<< timestamp
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< "] "
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< "["
		<< Console::ColorToAnsi(color)
		<< namesection
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< "] "
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< "["
		<< Console::ColorToAnsi(Console::Color::Blue)
		<< "DEBUG"
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< "] "
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< txt
		<< std::endl
		<< "\x1b[37m";
}