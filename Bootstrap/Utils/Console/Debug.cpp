#include "Debug.h"
#include "Logger.h"
#include "../Assertion.h"
#include "./Logger.h"
#include <iostream>
#include <string>

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
	
	const Logger::MessagePrefix prefixes[]{
		Logger::MessagePrefix{
			Console::Green,
			timestamp.c_str()
		},
		Logger::MessagePrefix{
			Console::Blue,
			"DEBUG"
		},
	};
	
	Logger::Internal_DirectWrite(LogLevel::Verbose, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), txt);
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

	const Logger::MessagePrefix prefixes [] {
		Logger::MessagePrefix{
			Console::Green,
			timestamp.c_str()
		},
		Logger::MessagePrefix{
			color,
			namesection
		},
		Logger::MessagePrefix{
			Console::Blue,
			"DEBUG"
		},
	};
	
	Logger::Internal_DirectWrite(LogLevel::Verbose, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), txt);
}
