#include "Debug.h"
#include "Logger.h"
#include "Assertion.h"
#include <iostream>
#include <string>
#include <sstream>

#ifdef DEBUG
bool Debug::Enabled = true;
#else
bool Debug::Enabled = false;
#endif

void Debug::Msg(const char* txt)
{
	if (!Enabled || !Assertion::ShouldContinue)
		return;
	ForceWrite(txt);
}

void Debug::ForceWrite(const char* txt)
{
	std::string timestamp = Logger::GetTimestamp();
	
	Logger::LogFile << "[" << timestamp << "] [DEBUG] " << txt << std::endl;

	const MessagePrefix prefixes[]{
		MessagePrefix{
			Console::Green,
			timestamp.c_str()
		},
		MessagePrefix{
			Console::Blue,
			"DEBUG"
		},
	};

	const char* response = BuildMsg(prefixes, sizeof prefixes, txt).c_str();
	DisplayMsg(response);
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

	const MessagePrefix prefixes [] {
		MessagePrefix{
			Console::Green,
			timestamp.c_str()
		},
		MessagePrefix{
			color,
			timestamp.c_str()
		},
		MessagePrefix{
			Console::Blue,
			"DEBUG"
		},
	};
	
	const char* response = BuildMsg(prefixes, sizeof prefixes, txt).c_str();
	DisplayMsg(response);
}

std::string Debug::BuildMsg(const MessagePrefix prefixes[], const int size, const char* txt)
{
	std::stringstream ss;

	for (int i = 0; i < size; i++)
		ss << Console::ColorToAnsi(Console::Color::Gray)
		<< "["
		<< Console::ColorToAnsi(prefixes[i].Color)
		<< prefixes[i].Message
		<< Console::ColorToAnsi(Console::Color::Gray)
		<< "]"
		<< Console::ColorToAnsi(Console::Color::Reset)
		<< " ";

#ifdef __ANDROID__
	ss << txt;
#else
	ss << txt
	<< std::endl
	<< Console::ColorToAnsi(Console::Color::Reset);
#endif
	return ss.str();
}

void Debug::DisplayMsg(const char* txt)
{
	
}
