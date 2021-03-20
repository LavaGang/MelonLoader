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
		Logger::MessagePrefix{
			Console::Green,
			timestamp.c_str()
		},
		Logger::MessagePrefix{
			Console::Red,
			"INTERNAL FAILURE"
		},
	};
	
#ifdef _WIN32
	bool should_print_debug_info = (!Logger::LogFile.coss.is_open() || Debug::Enabled);
	
	if (should_print_debug_info)
	{
		Logger::Internal_DirectWrite(LogLevel::Error, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), msg);

		MessageBoxA(NULL, msg, "MelonLoader - INTERNAL FAILURE", MB_OK | MB_ICONERROR);
	}
	else
	{
		Console::Close();
		MessageBoxA(NULL, "Please Post your latest.log File\nto #internal-failure in the MelonLoader Discord!", "MelonLoader - INTERNAL FAILURE!", MB_OK | MB_ICONERROR);
	}
#elif defined(__ANDROID__)
	Logger::Internal_DirectWrite(LogLevel::Error, prefixes, sizeof(prefixes) / sizeof(prefixes[0]), msg);
#endif
}

#ifdef _WIN32
FARPROC Assertion::GetExport(HMODULE mod, const char* export_name)
{
	if (!ShouldContinue)
		return NULL;
	Debug::Msg(export_name);
	FARPROC returnval = GetProcAddress(mod, export_name);
	if (returnval == NULL)
		ThrowInternalFailure((std::string("Failed to GetExport ( ") + export_name + " )").c_str());
	return returnval;
}
#endif