#include "DisableAnalytics.h"
#include "HookManager.h"
#include "Logger.h"
#include "AssertionManager.h"

getaddrinfo_t DisableAnalytics::original_getaddrinfo = NULL;
std::list<std::string> DisableAnalytics::URL_Blacklist = {
	"api.amplitude.com",
	"api.uca.cloud.unity3d.com",
	"config.uca.cloud.unity3d.com",
	"perf-events.cloud.unity3d.com",
	"public.cloud.unity3d.com",
	"cdp.cloud.unity3d.com",
	"data-optout-service.uca.cloud.unity3d.com"
};

bool DisableAnalytics::Setup()
{
	AssertionManager::Start("Mono.cpp", "DisableAnalytics::Setup");

	HMODULE ws2_32 = AssertionManager::GetModuleHandlePtr("ws2_32");
	if (ws2_32 != NULL)
	{
		original_getaddrinfo = (getaddrinfo_t)AssertionManager::GetExport(AssertionManager::GetModuleHandlePtr("ws2_32"), "getaddrinfo");
		if (original_getaddrinfo != NULL)
			HookManager::Hook(&(LPVOID&)original_getaddrinfo, getaddrinfo_hook);
	}

	return !AssertionManager::Result;
}

bool DisableAnalytics::CheckBlacklist(std::string url)
{
	bool url_found = (std::find(URL_Blacklist.begin(), URL_Blacklist.end(), url) != URL_Blacklist.end());
	if (url_found)
		Logger::DebugLog("Analytics URL Detected and Blocked: " + url);
	return url_found;
}