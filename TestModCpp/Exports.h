#pragma once

#include <windows.h>

/* DO NOT MODIFY */
struct BuildInfo {
	const char* Version;
	const char* Author;
	const char* Name;
	const char* Company;
	const char* DownloadLink;

	void SetBuildInfo(const char* name, const char* author, const char* version, const char* company, const char* downloadLink);
};

struct GameInfo {
	const char* Developer;
	const char* GameName;

	void SetGameInfo(const char* developer, const char* gameName);
};

extern "C" {
	__declspec(dllexport) BuildInfo* __cdecl GetBuildInfo();
	__declspec(dllexport) GameInfo* __cdecl GetGameInfo();
}
/* DO NOT MODIFY */