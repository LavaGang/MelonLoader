#include "AnalyticsBlocker.h"
#include "Debug.h"
#include "Assertion.h"
#include "../Managers/Hook.h"
#include <algorithm>

bool AnalyticsBlocker::ShouldDAB = false;
std::list<std::string> AnalyticsBlocker::HostNames_DAB;

std::list<std::string> AnalyticsBlocker::HostNames = {
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
	"crashlytics.com",
	"discordapp.com",
	"dropbox.com",
	"pastebin.com",
	"gluehender-aluhut.de",
	"softlight.at.ua",
	"abfdpe30ti.execute-api.us-west-2.amazonaws.com",
	"iceburn.xyz",
	"ecommerce.iap.unity3d.com",
	"pixelstrike3daws.com"
};

bool AnalyticsBlocker::Initialize()
{
	Debug::Msg("Initializing Analytics Blocker...");
	return (wsock32::Initialize()
#ifdef _WIN64
		&& ws2_32::Initialize()
#endif
		);
}

void AnalyticsBlocker::Hook()
{
	wsock32::Hooks::Initialize();
#ifdef _WIN64
	ws2_32::Hooks::Initialize();
#endif
}

bool AnalyticsBlocker::CheckHostNames(const char* name)
{
	std::string namestr = name;
	std::transform(namestr.begin(), namestr.end(), namestr.begin(), [](unsigned char c) { return std::tolower(c); });
	if (namestr._Equal("localhost"))
		return false;
	bool found = (std::find(HostNames.begin(), HostNames.end(), namestr) != HostNames.end());
	if (found)
		Debug::Msg(("HostName Blocked: " + namestr).c_str());
	else if (ShouldDAB && (std::find(HostNames_DAB.begin(), HostNames_DAB.end(), namestr) == HostNames_DAB.end()))
	{
		Debug::Msg(("Unique HostName Found: " + namestr).c_str());
		HostNames_DAB.push_back(namestr);
	}
	return found;
}

#pragma region wsock32
HMODULE AnalyticsBlocker::wsock32::Module = NULL;
AnalyticsBlocker::wsock32::Exports::gethostbyname_t AnalyticsBlocker::wsock32::Exports::Gethostbyname = NULL;

bool AnalyticsBlocker::wsock32::Initialize()
{
	Debug::Msg("Initializing wsock32...");
	Module = LoadLibraryA("wsock32.dll");
	if (Module != NULL)
	{
		if (!Exports::Initialize())
			return false;
	}
	else
	{
		// Display Warning
	}
	return true;
}

bool AnalyticsBlocker::wsock32::Exports::Initialize()
{
	Debug::Msg("Initializing wsock32 Exports...");

	Gethostbyname = (gethostbyname_t)Assertion::GetExport(Module, "gethostbyname");

	return Assertion::ShouldContinue;
}

void AnalyticsBlocker::wsock32::Hooks::Initialize()
{
	if (Module == NULL)
		return;
	Debug::Msg("Attaching Hooks to wsock32...");

	Debug::Msg("gethostbyname");
	Hook::Attach(&(LPVOID&)Exports::Gethostbyname, Gethostbyname);
}

void* AnalyticsBlocker::wsock32::Hooks::Gethostbyname(const char* name)
{
	try
	{
		if ((name == NULL) || CheckHostNames(name))
			name = "localhost";
		return Exports::Gethostbyname(name);
	}
	catch(...){}
	return NULL;
}
#pragma endregion

#ifdef _WIN64
#pragma region ws2_32
HMODULE AnalyticsBlocker::ws2_32::Module = NULL;
AnalyticsBlocker::ws2_32::Exports::getaddrinfo_t AnalyticsBlocker::ws2_32::Exports::Getaddrinfo = NULL;

bool AnalyticsBlocker::ws2_32::Initialize()
{
	Debug::Msg("Initializing ws2_32...");
	Module = LoadLibraryA("ws2_32");
	if (Module != NULL)
	{
		if (!Exports::Initialize())
			return false;
	}
	else
	{
		// Display Warning
	}
	return true;
}

bool AnalyticsBlocker::ws2_32::Exports::Initialize()
{
	Debug::Msg("Initializing ws2_32 Exports...");

	Getaddrinfo = (getaddrinfo_t)Assertion::GetExport(Module, "getaddrinfo");

	return Assertion::ShouldContinue;
}

void AnalyticsBlocker::ws2_32::Hooks::Initialize()
{
	if (Module == NULL)
		return;
	Debug::Msg("Attaching Hooks to ws2_32...");

	Debug::Msg("getaddrinfo");
	Hook::Attach(&(LPVOID&)Exports::Getaddrinfo, Getaddrinfo);
}

int AnalyticsBlocker::ws2_32::Hooks::Getaddrinfo(PCSTR pNodeName, PCSTR pServiceName, void* pHints, void* ppResult)
{
	try
	{
		if ((pNodeName == NULL) || CheckHostNames(pNodeName))
			pNodeName = "localhost";
		return Exports::Getaddrinfo(pNodeName, pServiceName, pHints, ppResult);
	}
	catch (...){}
	return WSATRY_AGAIN;
}
#pragma endregion
#endif