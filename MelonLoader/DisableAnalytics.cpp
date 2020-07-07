#include "DisableAnalytics.h"
#include "HookManager.h"
#include "Logger.h"

std::list<std::string> DisableAnalytics::URL_Blacklist = {
	"api.amplitude.com",
	"api.uca.cloud.unity3d.com",
	"config.uca.cloud.unity3d.com",
	"perf-events.cloud.unity3d.com",
	"public.cloud.unity3d.com",
	"cdp.cloud.unity3d.com",
	"data-optout-service.uca.cloud.unity3d.com"
};

void DisableAnalytics::Initialize()
{

}

bool DisableAnalytics::CheckBlacklist(std::string url)
{
	bool url_found = (std::find(URL_Blacklist.begin(), URL_Blacklist.end(), url) != URL_Blacklist.end());
	if (url_found)
		Logger::Log("Analytics URL Detected: " + url);
	return url_found;
}