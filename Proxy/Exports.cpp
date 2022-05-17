#include "Exports.h"

FARPROC OriginalFuncs_psapi[27];
FARPROC OriginalFuncs_version[17];
FARPROC OriginalFuncs_winhttp[65];
FARPROC OriginalFuncs_winmm[181];

bool exports::is_file_name_compatible(const std::wstring& proxy_filename, std::size_t* index)
{
	for (std::size_t i = 0; i < compatible_file_names.size(); ++i)
	{
		if (proxy_filename == compatible_file_names[i])
		{
			*index = i;
			return true;
		}
	}

	return false;
}

void exports::load(const HMODULE originaldll, const char* const* export_names, FARPROC* original_funcs, const std::size_t array_size)
{
	for (int i = 0; i < array_size; i++)
		original_funcs[i] = GetProcAddress(originaldll, export_names[i]);
}