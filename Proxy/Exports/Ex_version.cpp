#include "../Core.h"

extern "C" FARPROC OriginalFuncs_version[17];
FARPROC OriginalFuncs_version[17];
const char* ExportNames_version[] = {
	"GetFileVersionInfoA",
	"GetFileVersionInfoByHandle",
	"GetFileVersionInfoExA",
	"GetFileVersionInfoExW",
	"GetFileVersionInfoSizeA",
	"GetFileVersionInfoSizeExA",
	"GetFileVersionInfoSizeExW",
	"GetFileVersionInfoSizeW",
	"GetFileVersionInfoW",
	"VerFindFileA",
	"VerFindFileW",
	"VerInstallFileA",
	"VerInstallFileW",
	"VerLanguageNameA",
	"VerLanguageNameW",
	"VerQueryValueA",
	"VerQueryValueW"
};

void Core::LoadExports_version(HMODULE originaldll)
{
	for (int i = 0; i < (sizeof(ExportNames_version) / sizeof(ExportNames_version[0])); i++)
		OriginalFuncs_version[i] = GetProcAddress(originaldll, ExportNames_version[i]);
}