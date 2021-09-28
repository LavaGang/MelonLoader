#include "Exports.h"

const char* Exports::CompatibleFileNames[] = {
	"psapi",
	"version",
	"winhttp",
	"winmm"
};

Exports::LoadExportsFunc Exports::LoadFuncs[4] = {
	Load_psapi,
	Load_version,
	Load_winhttp,
	Load_winmm
};

bool Exports::IsFileNameCompatible(std::string proxy_filename, int& index)
{
	index = -1;
	bool found = false;
	for (int i = 0; i < (sizeof(CompatibleFileNames) / sizeof(CompatibleFileNames[0])); i++)
	{
		if (strstr(proxy_filename.c_str(), CompatibleFileNames[i]) != NULL)
		{
			index = i;
			found = true;
			break;
		}
	}
	return found;
}

void Exports::Load(HMODULE originaldll, const char** ExportNames, FARPROC* OriginalFuncs, int ArraySize)
{
	for (int i = 0; i < ArraySize; i++)
		OriginalFuncs[i] = GetProcAddress(originaldll, ExportNames[i]);
}