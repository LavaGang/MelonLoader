#pragma once
#include <Windows.h>
#include <filesystem>

namespace utility
{
	std::filesystem::path get_module_file_path(HMODULE module_handle);
};