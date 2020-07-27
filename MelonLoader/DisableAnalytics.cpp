#include "DisableAnalytics.h"
#include "HookManager.h"
#include "Logger.h"

gethostbyname_t DisableAnalytics::Original_gethostbyname = NULL;
getaddrinfo_t DisableAnalytics::Original_getaddrinfo = NULL;
std::list<std::string> DisableAnalytics::URL_Blacklist = {
	"api.amplitude.com",
	"api.uca.cloud.unity3d.com",
	"config.uca.cloud.unity3d.com",
	"perf-events.cloud.unity3d.com",
	"public.cloud.unity3d.com",
	"cdp.cloud.unity3d.com",
	"data-optout-service.uca.cloud.unity3d.com",
	"oculus.com",
	"oculuscdn.com",
	"facebook-hardware.com",
	"facebook.net",
	"facebook.com",
	"graph.facebook.com",
	"fbcdn.com",
	"fbsbx.com",
	"fbcdn.net",
	"fb.me",
	"fb.com",
	"crashlytics.com"
};

void DisableAnalytics::Setup()
{
	HMODULE wsock32 = GetModuleHandle("wsock32.dll");
	if (wsock32 != NULL)
	{
		Original_gethostbyname = (gethostbyname_t)GetProcAddress(wsock32, "gethostbyname");
		if (Original_gethostbyname != NULL)
			HookManager::Hook(&(LPVOID&)Original_gethostbyname, Hooked_gethostbyname);
		else
			LogWarning("Failed to GetProcAddress ( gethostbyname )");
	}
	else
		LogWarning("Failed to GetModuleHandle ( wsock32.dll )");

	HMODULE ws2_32 = GetModuleHandle("ws2_32");
	if (ws2_32 != NULL)
	{
		Original_getaddrinfo = (getaddrinfo_t)GetProcAddress(ws2_32, "getaddrinfo");
		if (Original_getaddrinfo != NULL)
			HookManager::Hook(&(LPVOID&)Original_getaddrinfo, Hooked_getaddrinfo);
		else
			LogWarning("Failed to GetProcAddress ( getaddrinfo )");
	}
	else
		LogWarning("Failed to GetModuleHandle ( ws2_32 )");
}

bool DisableAnalytics::CheckBlacklist(std::string url)
{
	bool url_found = (std::find(URL_Blacklist.begin(), URL_Blacklist.end(), url) != URL_Blacklist.end());
	if (url_found)
		Logger::DebugLog("Analytics URL Blocked: " + url);
	return url_found;
}

void DisableAnalytics::LogWarning(std::string msg)
{
	msg += (" for [ DisableAnalytics.cpp | DisableAnalytics::Setup ]");
	Logger::DebugLogWarning(msg);
}