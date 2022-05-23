#pragma once
#include <Windows.h>
#include <string>
#include <filesystem>

class core
{
public:
	static void initialize(HINSTANCE hinst_dll);

private:
	static HMODULE load_original_proxy(const std::filesystem::path& proxy_filepath, const std::wstring& proxy_filepath_no_ext);
	static std::filesystem::path get_bootstrap_path(const std::filesystem::path& base_path);
	static bool is_unity(const std::filesystem::path& exe_filepath);

	static void error(const std::string& reason, bool should_kill = false);
	static void error(const std::wstring& reason, bool should_kill = false);

	static void terminate_process();
};