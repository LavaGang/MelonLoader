#pragma once
#include <Windows.h>
#include <string>
extern "C" FARPROC OriginalFuncs_psapi[27];
extern "C" FARPROC OriginalFuncs_version[17];
extern "C" FARPROC OriginalFuncs_winhttp[65];
extern "C" FARPROC OriginalFuncs_winmm[181];

class Exports
{
public:
	static bool IsFileNameCompatible(std::string proxy_filename, int& index);
	static void Load(int index, HMODULE originaldll) { LoadFuncs[index](originaldll); };
	static void Load(HMODULE originaldll, const char** ExportNames, FARPROC* OriginalFuncs, int ArraySize);

private:
	static const char* CompatibleFileNames[4];
	typedef void (*LoadExportsFunc) (HMODULE originaldll);
	static LoadExportsFunc LoadFuncs[4];

	static const char* ExportNames_psapi[27];
	static void Load_psapi(HMODULE originaldll) { Load(originaldll, ExportNames_psapi, OriginalFuncs_psapi, 27); };

	static const char* ExportNames_version[17];
	static void Load_version(HMODULE originaldll) { Load(originaldll, ExportNames_version, OriginalFuncs_version, 17); };

	static const char* ExportNames_winhttp[65];
	static void Load_winhttp(HMODULE originaldll) { Load(originaldll, ExportNames_winhttp, OriginalFuncs_winhttp, 65); };

	static const char* ExportNames_winmm[181];
	static void Load_winmm(HMODULE originaldll) { Load(originaldll, ExportNames_winmm, OriginalFuncs_winmm, 181); };
};