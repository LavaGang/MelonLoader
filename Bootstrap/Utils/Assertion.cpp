#ifndef PORT_TODO_DISABLE
#include "Assertion.h"
#include "../Base/Core.h"
#include "Debug.h"
#include "Logger.h"
#include "Console.h"
#include <string>
#include <iostream>

bool Assertion::ShouldContinue = true;

void Assertion::ThrowInternalFailure(const char* msg)
{
	if (ShouldContinue)
	{
		ShouldContinue = false;
		std::string timestamp = Logger::GetTimestamp();
		Logger::LogFile << "[" << timestamp << "] [INTERNAL FAILURE] " << msg << std::endl;
		bool should_print_debug_info = (!Logger::LogFile.coss.is_open() || Debug::Enabled);
		if (should_print_debug_info)
		{
			std::cout
				<< Console::ColorToAnsi(Console::Color::Red)
				<< "["
				<< timestamp
				<< "] [INTERNAL FAILURE] "
				<< msg
				<< std::endl
				<< "\x1b[37m";
			MessageBoxA(NULL, msg, "MelonLoader - INTERNAL FAILURE", MB_OK | MB_ICONERROR);
		}
		else
		{
			Console::Close();
			MessageBoxA(NULL, "Please Post your latest.log File\nto #internal-failure in the MelonLoader Discord!", "MelonLoader - INTERNAL FAILURE!", MB_OK | MB_ICONERROR);
		}
	}
}

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