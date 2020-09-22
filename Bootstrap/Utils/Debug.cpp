#include "Debug.h"
#include "Logger.h"
#include "Assertion.h"

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
	Logger::WriteTimestamp(Console::Color::Black);
	Logger::LogFile << "[DEBUG] " << txt << std::endl;
	Console::SetColor(Console::Color::Gray);
	Console::Write("[");
	Console::SetColor(Console::Color::Blue);
	Console::Write("DEBUG");
	Console::SetColor(Console::Color::Gray);
	Console::Write("] ");
	Console::SetColor(Console::Color::Gray);
	Console::Write(txt);
	Console::Write("\n");
}

void Debug::Internal_Msg(const char* namesection, const char* txt)
{
	if (!Enabled || !Assertion::ShouldContinue)
		return;
	Logger::WriteTimestamp(Console::Color::Black);
	Logger::LogFile << "[" << namesection << "] [DEBUG] " << txt << std::endl;
	Console::SetColor(Console::Color::Gray);
	Console::Write("[");
	Console::SetColor(Console::Color::Magenta);
	Console::Write(namesection);
	Console::SetColor(Console::Color::Gray);
	Console::Write("] ");
	Console::SetColor(Console::Color::Gray);
	Console::Write("[");
	Console::SetColor(Console::Color::Blue);
	Console::Write("DEBUG");
	Console::SetColor(Console::Color::Gray);
	Console::Write("] ");
	Console::SetColor(Console::Color::Gray);
	Console::Write(txt);
	Console::Write("\n");
}