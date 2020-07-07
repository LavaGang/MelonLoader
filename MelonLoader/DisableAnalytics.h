#pragma once
#include <Windows.h>
#include <list>
#include <string>

class DisableAnalytics
{
public:
	static std::list<std::string>URL_Blacklist;
	static void Initialize();
	static bool CheckBlacklist(std::string url);
};