#pragma once
#include <Windows.h>
#include <string>

class File
{
public:
	static bool Exists(const char* path)
	{
		WIN32_FIND_DATAA data;
		return (FindFirstFileA(path, &data) != INVALID_HANDLE_VALUE);
	}

	static std::string GetModuleFilePath(HMODULE module_handle)
	{
		LPSTR a1 = new CHAR[MAX_PATH];
		GetModuleFileNameA(module_handle, a1, MAX_PATH);
		std::filesystem::path a2(a1);
		return a2.filename().string();
	}
};