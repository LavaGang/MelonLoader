#include "Core.h"
#include "Exports.h"
#include "Utils/Utility.h"

#pragma warning (disable:4244)

void core::initialize(HINSTANCE hinst_dll)
{
	// @note: get execution path
	std::vector<char> path_buf;
	DWORD copied = 0;
	do
	{
		path_buf.resize(path_buf.size() + MAX_PATH);
		copied = GetModuleFileNameA(nullptr, path_buf.data(), static_cast<DWORD>(path_buf.size()));
	}
	while (copied >= path_buf.size());

	path_buf.resize(copied);
	const std::filesystem::path filepath(path_buf.begin(), path_buf.end());

	// @note: get file path of proxy, tolowercase the file name
	const auto proxy_filepath = utility::get_module_file_path(hinst_dll);
	auto proxy_filename = proxy_filepath.filename().wstring();
	std::transform(proxy_filename.begin(), proxy_filename.end(), proxy_filename.begin(), towlower);

	// @note: is compatible proxy
	std::size_t index = -1;
	if (!exports::is_file_name_compatible(proxy_filename, &index))
	{
		error("Proxy has an Incompatible File Name!", true);
		return;
	}

	// @note: load original libs
	const HMODULE originaldll = load_original_proxy(proxy_filepath, proxy_filepath.filename().stem().wstring());
	if (!originaldll)
	{
		error(L"Failed to Load Original " + proxy_filepath.wstring() + L"!", true);
		return;
	}

	// @note: load original lib exports
	exports::load(index, originaldll);

	// @note: confirm we're loading into a unity game
	if (!is_unity(filepath))
		return;

	if (strstr(GetCommandLineA(), "--no-mods") != nullptr)
		return;

	// @note: confirm bootstrap exists
	const auto bootstrap_path = get_bootstrap_path(filepath.parent_path());
	if (bootstrap_path.empty())
		return;

	if (!exists(bootstrap_path))
	{
		error("Bootstrap.dll does not Exist in Base Directory!");
		return;
	}

	// @note: load bootstrap lib
	const HMODULE bootstrap_module = LoadLibraryW(bootstrap_path.c_str());
	if (!bootstrap_module)
		error("Unable to Load Bootstrap.dll from Base Directory!");
}

HMODULE core::load_original_proxy(const std::filesystem::path& proxy_filepath, const std::wstring& proxy_filepath_no_ext)
{
	HMODULE originaldll = LoadLibraryW((proxy_filepath_no_ext + L"_original.dll").c_str());

	if (!originaldll)
	{
		wchar_t system32_path[MAX_PATH];

		if (GetSystemDirectoryW(system32_path, MAX_PATH) == NULL)
		{
			error("Failed to Get System32 Directory!");
			terminate_process();
			return nullptr;
		}

		const auto path = std::filesystem::path(system32_path);
		originaldll = LoadLibraryW((path / proxy_filepath.filename()).c_str());
	}

	return originaldll;
}

std::filesystem::path core::get_bootstrap_path(const std::filesystem::path& base_path)
{
	constexpr auto defaultpath = L"MelonLoader\\Dependencies\\Bootstrap.dll";
	std::filesystem::path returnval = base_path / defaultpath;

	int argc = __argc;
	wchar_t** argv = CommandLineToArgvW(GetCommandLineW(), &argc);

	for (int i = 0; i < argc; i++)
	{
		wchar_t* arg = argv[i];
		if (!arg)
			continue;

		if (wcscmp(arg, L"--melonloader.basedir") == 0)
		{
			wchar_t* arg_dir = argv[i + 1];
			if (!arg_dir)
				break;

			returnval = arg_dir;
			returnval /= defaultpath;
			break;
		}
	}
	LocalFree(argv);

	return returnval;
}

bool core::is_unity(const std::filesystem::path& exe_filepath)
{
	const auto filename = exe_filepath.filename().stem().wstring();
	const auto datapath = exe_filepath.parent_path() / (filename + L"_Data");

	if (!exists(datapath))
		return false;

	if (exists(datapath / L"globalgamemanagers") || exists(datapath / L"data.unity3d") || exists(datapath / L"mainData"))
		return true;

	return false;
}

void core::error(const std::string& reason, const bool should_kill)
{
	MessageBoxA(nullptr, (reason + " " + (should_kill ? "Preventing Startup" : "Continuing without MelonLoader") + "...").c_str(), "MelonLoader", MB_ICONERROR | MB_OK);
	if (should_kill)
		terminate_process();
}

void core::error(const std::wstring& reason, const bool should_kill)
{
	MessageBoxW(nullptr, (reason + L" " + (should_kill ? L"Preventing Startup" : L"Continuing without MelonLoader") + L"...").c_str(), L"MelonLoader", MB_ICONERROR | MB_OK);
	if (should_kill)
		terminate_process();
}

void core::terminate_process()
{
	const HANDLE current_process = GetCurrentProcess();
	TerminateProcess(current_process, NULL);
	CloseHandle(current_process);
}
