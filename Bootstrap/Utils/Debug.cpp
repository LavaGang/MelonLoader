#include "Debug.h"
#include "Logging/Logger.h"
#include "Assertion.h"

bool Debug::Enabled = false;

void Debug::Msg(const char* txt)
{
	if (!Enabled || !Assertion::ShouldContinue)
		return;
	DirectWrite(txt);
}

void Debug::DirectWrite(const char* txt)
{
	Logger::LogToConsoleAndFile(Log(LogType::Debug, nullptr, txt));
}

void Debug::Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, const char* namesection, const char* txt)
{
	if (!Enabled || !Assertion::ShouldContinue)
		return;

	Logger::LogToConsoleAndFile(Log(LogType::Debug, meloncolor, txtcolor, namesection, txt));
}