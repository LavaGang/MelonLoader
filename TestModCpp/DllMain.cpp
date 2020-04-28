#include <windows.h>
#include "Exports.h"
#include "TestModCpp.h"

/* DO NOT MODIFY */
BuildInfo* ModBuildInfo = NULL;
GameInfo* ModGameInfo = NULL;

BuildInfo* GetBuildInfo() { return ModBuildInfo; }
GameInfo* GetGameInfo() { return ModGameInfo; }

void BuildInfo::SetBuildInfo(const char* name, const char* author,
    const char* version, const char* company,
    const char* downloadLink) {
    Name = name;
    Author = author;
    Version = version;
    Company = company;
    DownloadLink = downloadLink;
}

void GameInfo::SetGameInfo(const char* developer, const char* gameName) {
    Developer = developer;
    GameName = gameName;
}

void Setup() {
    ModBuildInfo = new BuildInfo();
    ModGameInfo = new GameInfo();
}

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved) {
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
        DisableThreadLibraryCalls(hModule);
        Setup();
        TestModCpp::Init(ModBuildInfo, ModGameInfo);
        break;
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}
/* DO NOT MODIFY */

