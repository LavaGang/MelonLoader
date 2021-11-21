#include "Log.h"

#include "Logger.h"

void Log::BuildConsoleString(std::ostream& stream) const
{
    // Always initialize stream with timestamp
    stream << 
        Console::ColorToAnsi(logMeta->GetColorOverride(Console::Color::Gray)) <<
        "[" <<
        Console::ColorToAnsi(logMeta->GetColorOverride(Console::Color::Green)) <<
        Logger::GetTimestamp() <<
        Console::ColorToAnsi(logMeta->GetColorOverride(Console::Color::Gray)) <<
        "] ";

    // If the logging melon has a name, print it
    if (namesection != nullptr) stream << "[" <<
        Console::ColorToAnsi(melonAnsiColor) <<
        namesection <<
        Console::ColorToAnsi(logMeta->GetColorOverride(Console::Color::Gray)) <<
        "] ";

    // Print the [LOGTYPE] prefix if needed
    if (logMeta->printLogTypeName) stream << "[" << Console::ColorToAnsi(logMeta->logCategoryColor) << logMeta->logTypeString << Console::ColorToAnsi(logMeta->GetColorOverride(Console::Color::Gray)) << "] ";

    // If we're not coloring the whole line, use the specified input text color. If we are, the color would already be declared
    if (!logMeta->colorFullLine) stream << Console::ColorToAnsi(textAnsiColor);
	
    stream << txt <<
        Console::ColorToAnsi(logMeta->GetColorOverride(Console::Color::Gray), false);
}

std::string Log::BuildLogString() const
{
    std::string logStr = "[" + Logger::GetTimestamp() + "] ";
    if (namesection != nullptr)	logStr = logStr + "[" + namesection + "] ";
    if (logMeta->printLogTypeName) logStr = logStr + "[" + logMeta->logTypeString + "] ";
    return logStr + txt;
}