#pragma once
#include <Windows.h>
#include <list>
#include <string>

typedef int(__stdcall* getaddrinfo_t) (PCSTR pNodeName, PCSTR pServiceName, void* pHints, void* ppResult);

class DisableAnalytics
{
public:
	static getaddrinfo_t original_getaddrinfo;
	static int getaddrinfo_hook(PCSTR pNodeName, PCSTR pServiceName, void* pHints, void* ppResult) { return original_getaddrinfo((CheckBlacklist(pNodeName) ? "0.0.0.0" : pNodeName), pServiceName, pHints, ppResult); }

	static std::list<std::string>URL_Blacklist;
	static bool Setup();
	static bool CheckBlacklist(std::string url);
};