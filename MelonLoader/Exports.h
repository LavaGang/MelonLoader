#pragma once
#include "Console.h"

extern "C"
{
	__declspec(dllexport) void __stdcall Logger_Log(const char* txt);
	__declspec(dllexport) void __stdcall Logger_LogColor(const char* txt, ConsoleColor color);
	__declspec(dllexport) void __stdcall Logger_LogError(const char* namesection, const char* txt);
	__declspec(dllexport) void __stdcall Logger_LogModError(const char* namesection, const char* msg);
}


class Exports
{
public:
	static void AddInternalCalls();
};