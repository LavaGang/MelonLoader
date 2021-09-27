#pragma once
#include <Windows.h>
#include <string>

class Exports
{
public:
	static bool IsFileNameCompatible(std::string proxy_filename, int& index);
	static void Load(int index, HMODULE originaldll) { LoadFuncs[index](originaldll); };

private:
	static const char* CompatibleFileNames[4];
	typedef void (*LoadExportsFunc) (HMODULE originaldll);
	static LoadExportsFunc LoadFuncs[4];

	static void Load_psapi(HMODULE originaldll);
	static void Load_version(HMODULE originaldll);
	static void Load_winhttp(HMODULE originaldll);
	static void Load_winmm(HMODULE originaldll);
};