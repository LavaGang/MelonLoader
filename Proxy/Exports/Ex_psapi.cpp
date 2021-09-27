#include "../Exports.h"

extern "C" FARPROC OriginalFuncs_psapi[27];
FARPROC OriginalFuncs_psapi[27];

void Exports::Load_psapi(HMODULE originaldll)
{
	const char* ExportNames[] = {
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

	for (int i = 0; i < (sizeof(ExportNames) / sizeof(ExportNames[0])); i++)
		OriginalFuncs_psapi[i] = GetProcAddress(originaldll, ExportNames[i]);
}