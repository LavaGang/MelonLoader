#include "../Exports.h"

FARPROC OriginalFuncs_psapi[27];
const char* Exports::ExportNames_psapi[27] = {
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