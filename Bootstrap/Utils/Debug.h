#pragma once
#include "../Utils/Console.h"

#ifdef _WIN64
#define __USE_COUT
#endif

struct MessagePrefix
{
	Console::Color Color;
	const char* Message;
};

class Debug
{
public:
	static bool Enabled;
	static void Msg(const char* txt);
	static void ForceWrite(const char* txt);
	static void Internal_Msg(Console::Color color, const char* namesection, const char* txt);
private:
	static std::string BuildMsg(const MessagePrefix prefixes[], const int size, const char* txt);
	static void DisplayMsg(const char* txt);
};