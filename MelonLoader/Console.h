#pragma once
#include <string>

class Console
{
public:
	static bool IsInitialized();
	static void Create();
	static void Destroy();
	static void Write(const char* txt);
	static void WriteLine(const char* txt);
};