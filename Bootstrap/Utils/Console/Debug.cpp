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
	Msgf("%s", txt);
}

void Debug::Msgf(const char* fmt, ...)
{
	va_list args;
	va_start(args, fmt);
	vMsgf(fmt, args);
	va_end(args);
}

void Debug::vMsgf(const char* fmt, va_list args)
{
	if (!Enabled || !Assertion::ShouldContinue)
		return;
	vForceWritef(fmt, args);
}

void Debug::ForceWrite(const char* txt)
{
	ForceWritef("%s", txt);
}

void Debug::ForceWritef(const char* fmt, ...)
{
	va_list args;
	va_start(args, fmt);
	vForceWritef(fmt, args);
	va_end(args);
}

void Debug::vForceWritef(const char* fmt, va_list args)
{
	const Logger::MessagePrefix prefixes[]{
#ifndef __ANDROID__
		Logger::MessagePrefix{
			Console::Green,
			Logger::GetTimestamp().c_str()
		},
#endif
		Logger::MessagePrefix{
			Console::Blue,
			"DEBUG"
		},
	};

	Logger::Internal_vDirectWritef(Console::Color::Reset, LogLevel::Verbose, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), fmt, args);
}

void Debug::Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, const char* namesection, const char* txt)
{
	Internal_Msgf(meloncolor, txtcolor, namesection, "%s", txt);
}

void Debug::Internal_Msgf(Console::Color meloncolor, Console::Color txtcolor, const char* namesection, const char* fmt, ...)
{
	va_list args;
	va_start(args, fmt);
	Internal_vMsgf(meloncolor, txtcolor, namesection, fmt, args);
	va_end(args);
}

void Debug::Internal_vMsgf(Console::Color meloncolor, Console::Color txtcolor, const char* namesection, const char* fmt, va_list args)
{
	if (namesection == NULL)
	{
		vMsgf(fmt, args);
		return;
	}

	if (!Enabled || !Assertion::ShouldContinue)
		return;

	const Logger::MessagePrefix prefixes[]{
#ifndef __ANDROID__
		Logger::MessagePrefix{
			Console::Green,
			Logger::GetTimestamp().c_str()
		},
#endif
		Logger::MessagePrefix{
			meloncolor,
			namesection
		},
		Logger::MessagePrefix{
			Console::Blue,
			"DEBUG"
		},
	};

	Logger::Internal_vDirectWritef(txtcolor, LogLevel::Verbose, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), fmt, args);
}
