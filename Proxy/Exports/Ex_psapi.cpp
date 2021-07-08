#include "../Core.h"

extern "C" FARPROC OriginalFuncs_psapi[27];
FARPROC OriginalFuncs_psapi[27];
const char* ExportNames_psapi[] = {
	"EmptyWorkingSet",
	"EnumDeviceDrivers",
	"EnumPageFilesA",
	"EnumPageFilesW",
	"EnumProcessModules",
	"EnumProcessModulesEx",
	"EnumProcesses",
	"GetDeviceDriverBaseNameA",
	"GetDeviceDriverBaseNameW",
	"GetDeviceDriverFileNameA",
	"GetDeviceDriverFileNameW",
	"GetMappedFileNameA",
	"GetMappedFileNameW",
	"GetModuleBaseNameA",
	"GetModuleBaseNameW",
	"GetModuleFileNameExA",
	"GetModuleFileNameExW",
	"GetModuleInformation",
	"GetPerformanceInfo",
	"GetProcessImageFileNameA",
	"GetProcessImageFileNameW",
	"GetProcessMemoryInfo",
	"GetWsChanges",
	"GetWsChangesEx",
	"InitializeProcessForWsWatch",
	"QueryWorkingSet",
	"QueryWorkingSetEx"
};

void Core::LoadExports_psapi(HMODULE originaldll)
{
	for (int i = 0; i < (sizeof(ExportNames_psapi) / sizeof(ExportNames_psapi[0])); i++)
		OriginalFuncs_psapi[i] = GetProcAddress(originaldll, ExportNames_psapi[i]);
}