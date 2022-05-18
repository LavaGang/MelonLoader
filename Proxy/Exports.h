#pragma once
#include <Windows.h>
#include <string>
#include "Exports/export_resources.h"

extern "C" FARPROC OriginalFuncs_psapi[27];
extern "C" FARPROC OriginalFuncs_version[17];
extern "C" FARPROC OriginalFuncs_winhttp[65];
extern "C" FARPROC OriginalFuncs_winmm[181];

namespace exports
{
	inline constexpr std::array<const wchar_t*, 4> compatible_file_names = {
		L"psapi.dll",
		L"version.dll",
		L"winhttp.dll",
		L"winmm.dll"
	};

	void load(HMODULE originaldll, const char* const* export_names, FARPROC* original_funcs, std::size_t array_size);

	inline void load_psapi(const HMODULE originaldll) { load(originaldll, export_resources::ExportNames_psapi.data(), OriginalFuncs_psapi, export_resources::ExportNames_psapi.size()); }
	inline void load_version(const HMODULE originaldll) { load(originaldll, export_resources::ExportNames_version.data(), OriginalFuncs_version, export_resources::ExportNames_version.size()); }
	inline void load_winhttp(const HMODULE originaldll) { load(originaldll, export_resources::ExportNames_winhttp.data(), OriginalFuncs_winhttp, export_resources::ExportNames_winhttp.size()); }
	inline void load_winmm(const HMODULE originaldll) { load(originaldll, export_resources::ExportNames_winmm.data(), OriginalFuncs_winmm, export_resources::ExportNames_winmm.size()); }

	using load_exports_func = decltype(&load_psapi);
	inline constexpr std::array<load_exports_func, 4> load_funcs = {
		load_psapi,
		load_version,
		load_winhttp,
		load_winmm
	};

	constexpr void load(const std::size_t index, const HMODULE originaldll) { load_funcs[index](originaldll); }
	bool is_file_name_compatible(const std::wstring& proxy_filename, std::size_t* index);
};