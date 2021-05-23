#include "Debug.h"
#include "Logger.h"
#include "Assertion.h"
#include <iostream>

bool Debug::Enabled = false;

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
		<< Console::ColorToAnsi(Console::Color::Gray, false);
}

void Debug::Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, const char* namesection, const char* txt)
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
		<< Console::ColorToAnsi(meloncolor)
		<< namesection
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< "] "
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< "["
		<< Console::ColorToAnsi(Console::Color::Blue)
		<< "DEBUG"
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< "] "
		<< Console::ColorToAnsi(txtcolor)
		<< txt
		<< std::endl
		<< Console::ColorToAnsi(Console::Color::Gray, false);
}