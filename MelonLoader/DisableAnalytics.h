#pragma once
#include <Windows.h>
#include <list>
#include <string>

typedef void* (__stdcall* gethostbyname_t) (const char* name);
typedef int(__stdcall* getaddrinfo_t) (PCSTR pNodeName, PCSTR pServiceName, void* pHints, void* ppResult);

class DisableAnalytics
{
public:
	static std::list<std::string>URL_Blacklist;
	static bool Setup();
	static bool CheckBlacklist(std::string url);

	static gethostbyname_t Original_gethostbyname;
	static getaddrinfo_t Original_getaddrinfo;
	static void* Hooked_gethostbyname(const char* name) { return Original_gethostbyname((CheckBlacklist(name) ? "0.0.0.0" : name)); }
	static int Hooked_getaddrinfo(PCSTR pNodeName, PCSTR pServiceName, void* pHints, void* ppResult) { return Original_getaddrinfo((CheckBlacklist(pNodeName) ? "0.0.0.0" : pNodeName), pServiceName, pHints, ppResult); }
};