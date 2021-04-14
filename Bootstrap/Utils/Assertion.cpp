#include "Assertion.h"
#include "../Base/Core.h"
#include "Console/Debug.h"
#include "Console/Logger.h"
#include "Console/Console.h"
#include <string>
#include <iostream>

bool Assertion::ShouldContinue = true;

void Assertion::ThrowInternalFailure(const char* msg)
{
// TODO: implement JNI bindings to show message box and error
	if (!ShouldContinue)
		return;
	
	ShouldContinue = false;

	std::string timestamp = Logger::GetTimestamp();
	
	const Logger::MessagePrefix prefixes[]{
#ifndef __ANDROID__
		Logger::MessagePrefix{
			Console::Green,
			timestamp.c_str()
		},
#endif
		Logger::MessagePrefix{
			Console::Red,
			"INTERNAL FAILURE"
		},
	};
	
#ifdef _WIN32
	bool should_print_debug_info = (!Logger::LogFile.coss.is_open() || Debug::Enabled);
	
	if (should_print_debug_info)
	{
		Logger::Internal_DirectWrite(Console::Color::Red, LogLevel::Error, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), msg);

		MessageBoxA(NULL, msg, "MelonLoader - INTERNAL FAILURE", MB_OK | MB_ICONERROR);
	}
	else
	{
		Console::Close();
		MessageBoxA(NULL, "Please Post your latest.log File\nto #internal-failure in the MelonLoader Discord!", "MelonLoader - INTERNAL FAILURE!", MB_OK | MB_ICONERROR);
	}
#elif defined(__ANDROID__)
	Logger::Internal_DirectWrite(Console::Color::Red, LogLevel::Error, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), msg);
#endif
}
