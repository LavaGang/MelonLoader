#include <Windows.h>
#include <VersionHelpers.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <sstream>
#include "Core.h"
#include "../Utils/CommandLine.h"
#include "../Utils/Console.h"
#include "../Utils/Assertion.h"
#include "../Utils/Logger.h"
#include "../Managers/Game.h"
#include "../Managers/Mono.h"
#include "../Managers/Il2Cpp.h"
#include "../Managers/Hook.h"
#include "../Utils/Debug.h"
#include "../Utils/AnalyticsBlocker.h"
#include "../Utils/HashCode.h"

HINSTANCE Core::Bootstrap = NULL;
char* Core::Path = NULL;
const char* Core::Version = "v0.3.0";
bool Core::QuitFix = false;

bool Core::Initialize()
{
	if (!OSVersionCheck())
		return false;
	CommandLine::Read();
	if (!Game::Initialize()
		|| !Console::Initialize()
		|| !Logger::Initialize()
		|| !Game::ReadInfo()
		|| !HashCode::Initialize()
		|| !Mono::Initialize())
		return false;
	WelcomeMessage();
	if (!AnalyticsBlocker::Initialize()
		|| !Il2Cpp::Initialize()
		|| !Mono::Load())
		return false;
	AnalyticsBlocker::Hook();
	if (Game::IsIl2Cpp)
	{
		if (!Mono::IsOldMono)
		{
			Debug::Msg("Attaching Hook to il2cpp_unity_install_unitytls_interface...");
			Hook::Attach(&(LPVOID&)Il2Cpp::Exports::il2cpp_unity_install_unitytls_interface, Il2Cpp::Hooks::il2cpp_unity_install_unitytls_interface);
			Debug::Msg("Attaching Hook to mono_unity_get_unitytls_interface...");
			Hook::Attach(&(LPVOID&)Mono::Exports::mono_unity_get_unitytls_interface, Mono::Hooks::mono_unity_get_unitytls_interface);
		}
		Debug::Msg("Attaching Hook to il2cpp_init...");
		Hook::Attach(&(LPVOID&)Il2Cpp::Exports::il2cpp_init, Il2Cpp::Hooks::il2cpp_init);
	}
	else
	{
		Debug::Msg("Attaching Hook to mono_jit_init_version...");
		Hook::Attach(&(LPVOID&)Mono::Exports::mono_jit_init_version, Mono::Hooks::mono_jit_init_version);
	}
	return true;
}

void Core::WelcomeMessage()
{
	if (Debug::Enabled)
		Logger::WriteSpacer();
	Logger::Msg("------------------------------");
	Logger::Msg((std::string("Hash Code:\n") + HashCode::Hash1 + "\n" + HashCode::Hash2 + (Game::IsIl2Cpp ? ("\n" + std::string(HashCode::Hash3)) : "")).c_str());
	Logger::Msg("------------------------------");
	Logger::Msg(("MelonLoader " + std::string(Version) + " Open-Beta").c_str());
	Logger::Msg((std::string("OS: ") + GetOSVersion()).c_str());
	Logger::Msg("------------------------------");
	Logger::Msg(("Name: " + std::string(Game::Name)).c_str());
	Logger::Msg(("Developer: " + std::string(Game::Developer)).c_str());
	Logger::Msg(("Unity Version: " + std::string(Game::UnityVersion)).c_str());
	Logger::Msg(("Type: " + std::string((Game::IsIl2Cpp ? "Il2Cpp" : (Mono::IsOldMono ? "Mono" : "MonoBleedingEdge")))).c_str());
	Logger::Msg(
#ifdef _WIN64
		"Arch: x64"
#else
		"Arch: x86"
#endif
	);
	Logger::Msg("------------------------------");
	if (Debug::Enabled)
		Logger::WriteSpacer();
	Debug::Msg(("Game::BasePath = " + std::string(Game::BasePath)).c_str());
	Debug::Msg(("Game::DataPath = " + std::string(Game::DataPath)).c_str());
	Debug::Msg(("Game::ApplicationPath = " + std::string(Game::ApplicationPath)).c_str());
}

bool Core::OSVersionCheck()
{
	if (IsWindows7OrGreater())
		return true;
	int result = MessageBoxA(NULL, "You are running on an Unsupported OS.\nWe can not offer support if there are any issues.\nContinue with MelonLoader?", "MelonLoader", MB_ICONWARNING | MB_YESNO);
	if (result == IDYES)
		return true;
	return false;
}

void Core::KillCurrentProcess()
{
	HANDLE current_process = GetCurrentProcess();
	if (current_process == NULL)
		return;
	TerminateProcess(current_process, NULL);
	CloseHandle(current_process);
}

const char* Core::GetFileInfoProductName(const char* path)
{
	DWORD handle;
	DWORD size = GetFileVersionInfoSizeA(path, &handle);
	if (size == NULL)
		return NULL;
	LPSTR buffer = new char[size];
	if (!GetFileVersionInfoA(path, handle, size, buffer))
		return NULL;
	UINT size2;
	WORD* buffer2;
	if (!VerQueryValueA(buffer, "\\VarFileInfo\\Translation", (LPVOID*)&buffer2, &size2) || (size2 <= 0))
		return NULL;
	std::stringstream productverpath;
	productverpath << "\\StringFileInfo\\" << std::setw(4) << std::setfill('0') << std::hex << std::uppercase << buffer2[0] << std::setw(4) << std::setfill('0') << std::hex << std::uppercase << buffer2[1] << "\\ProductName";
	if (!VerQueryValueA(buffer, productverpath.str().c_str(), (LPVOID*)&buffer2, &size2) || (size2 <= 0))
		return NULL;
	return (LPCSTR)buffer2;
}

const char* Core::GetFileInfoProductVersion(const char* path)
{
	DWORD handle;
	DWORD size = GetFileVersionInfoSizeA(path, &handle);
	if (size == NULL)
		return NULL;
	LPSTR buffer = new char[size];
	if (!GetFileVersionInfoA(path, handle, size, buffer))
		return NULL;
	UINT size2;
	WORD* buffer2;
	if (!VerQueryValueA(buffer, "\\VarFileInfo\\Translation", (LPVOID*)&buffer2, &size2) || (size2 <= 0))
		return NULL;
	std::stringstream productverpath;
	productverpath << "\\StringFileInfo\\" << std::setw(4) << std::setfill('0') << std::hex << std::uppercase << buffer2[0] << std::setw(4) << std::setfill('0') << std::hex << std::uppercase << buffer2[1] << "\\ProductVersion";
	if (!VerQueryValueA(buffer, productverpath.str().c_str(), (LPVOID*)&buffer2, &size2) || (size2 <= 0))
		return NULL;
	return (LPCSTR)buffer2;
}

const char* Core::GetOSVersion()
{
	if (IsWindows10OrGreater())
		return "Windows 10";
	else if (IsWindows8Point1OrGreater())
		return "Windows 8.1";
	else if (IsWindows8OrGreater())
		return "Windows 8";
	else if (IsWindows7SP1OrGreater())
		return "Windows 7 SP1";
	else if (IsWindows7OrGreater())
		return "Windows 7";
	else if (IsWindowsVistaSP2OrGreater())
		return "Windows Vista SP2";
	else if (IsWindowsVistaSP1OrGreater())
		return "Windows Vista SP1";
	else if (IsWindowsVistaOrGreater())
		return "Windows Vista";
	else if (IsWindowsXPSP3OrGreater())
		return "Windows XP SP3";
	else if (IsWindowsXPSP2OrGreater())
		return "Windows XP SP2";
	else if (IsWindowsXPSP1OrGreater())
		return "Windows XP SP1";
	else if (IsWindowsXPOrGreater())
		return "Windows XP";
	return "UNKNOWN";
}

bool Core::DirectoryExists(const char* path) { struct stat Stat; return ((stat(path, &Stat) == 0) && (Stat.st_mode & S_IFDIR)); }
bool Core::FileExists(const char* path) { WIN32_FIND_DATAA data; return (FindFirstFileA(path, &data) != INVALID_HANDLE_VALUE); }
void Core::GetLocalTime(std::chrono::system_clock::time_point* now, std::chrono::milliseconds* ms, std::tm* bt) { *now = std::chrono::system_clock::now(); *ms = std::chrono::duration_cast<std::chrono::milliseconds>((*now).time_since_epoch()) % 1000; time_t timer = std::chrono::system_clock::to_time_t(*now); localtime_s(bt, &timer); }