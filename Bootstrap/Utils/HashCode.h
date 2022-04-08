#pragma once
#include <Windows.h>
#include <string>

class HashCode
{
public:
	static std::string Hash;
	static bool Initialize();
	static bool SetupPaths();
};