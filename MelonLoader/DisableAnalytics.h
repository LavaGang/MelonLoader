#pragma once
#include <Windows.h>
#include <list>
#include <string>

typedef void* (__stdcall* gethostbyname_t) (const char* name);
typedef int(__stdcall* getaddrinfo_t) (PCSTR pNodeName, PCSTR pServiceName, void* pHints, void* ppResult);

class DisableAnalytics
{
public:
	static gethostbyname_t Original_gethostbyname;
	static getaddrinfo_t Original_getaddrinfo;
	static std::list<std::string>URL_Blacklist;

	static void Setup();
	static bool CheckBlacklist(std::string url);
	static void LogWarning(std::string msg);
	static void* Hooked_gethostbyname(const char* name) { if (CheckBlacklist(name)) return NULL; return Original_gethostbyname(name); }
	static int Hooked_getaddrinfo(PCSTR pNodeName, PCSTR pServiceName, void* pHints, void* ppResult) { if (CheckBlacklist(pNodeName)) return WSAEHOSTDOWN; return Original_getaddrinfo(pNodeName, pServiceName, pHints, ppResult); }
};