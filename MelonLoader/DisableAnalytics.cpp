#include "DisableAnalytics.h"
#include "HookManager.h"
#include "Logger.h"
#include "AssertionManager.h"

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

bool DisableAnalytics::Setup()
{
	AssertionManager::Start("DisableAnalytics.cpp", "DisableAnalytics::Setup");

	HMODULE wsock32 = AssertionManager::GetModuleHandlePtr("wsock32.dll");
	if (wsock32 != NULL)
	{
		Original_gethostbyname = (gethostbyname_t)AssertionManager::GetExport(wsock32, "gethostbyname");
		if (Original_gethostbyname != NULL)
			HookManager::Hook(&(LPVOID&)Original_gethostbyname, Hooked_gethostbyname);
	}

	HMODULE ws2_32 = AssertionManager::GetModuleHandlePtr("ws2_32");
	if (ws2_32 != NULL)
	{
		Original_getaddrinfo = (getaddrinfo_t)AssertionManager::GetExport(ws2_32, "getaddrinfo");
		if (Original_getaddrinfo != NULL)
			HookManager::Hook(&(LPVOID&)Original_getaddrinfo, Hooked_getaddrinfo);
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