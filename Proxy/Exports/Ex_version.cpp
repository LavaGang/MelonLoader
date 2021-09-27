#include "../Core.h"
#include "../Exports.h"

extern "C" FARPROC OriginalFuncs_version[17];
FARPROC OriginalFuncs_version[17];

void Exports::Load_version(HMODULE originaldll)
{
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

	for (int i = 0; i < (sizeof(ExportNames_version) / sizeof(ExportNames_version[0])); i++)
		OriginalFuncs_version[i] = GetProcAddress(originaldll, ExportNames_version[i]);
}