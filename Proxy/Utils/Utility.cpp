#include "Utility.h"

std::filesystem::path utility::get_module_file_path(HMODULE module_handle)
{
	wchar_t path[MAX_PATH];
	GetModuleFileNameW(module_handle, path, MAX_PATH);

	return path;
}
