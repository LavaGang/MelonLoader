#include "Core.h"
#include "../Utils/Console/CommandLine.h"
#include "../Utils/Console/Console.h"
#include "../Utils/Assertion.h"
#include "../Utils/Console/Logger.h"
#include "../Managers/Game.h"
#include "../Managers/Mono.h"
#include "../Managers/Il2Cpp.h"
#include "../Managers/Hook.h"
#include "../Utils/Console/Debug.h"
#include "../Utils/AnalyticsBlocker.h"
#include "../Utils/HashCode.h"
#include <dlfcn.h>
#include <sys/system_properties.h>
#include <android/log.h>
#include <stdio.h>
#include <string>
#include <sys/types.h>
#include <unistd.h>
#include <sys/mman.h>

#include "../Managers/BaseAssembly.h"
#include "../Managers/AssemblyVerifier.h"
#include "../Managers/InternalCalls.h"
#include "../Utils/UnitTesting/TestHelper.h"
#include "../Patcher/Tests/Suite.spec.h"
#include "../Utils/AssemblyUnhollower/XrefScannerBindings.h"

#ifdef __ANDROID__
#include <stdio.h>
#include <sys/stat.h>
#include "../Managers/AndroidData.h"
#endif

typedef void (*testFnDef)(void);

char* Core::Path = NULL;
const char* Core::Version = "v0.4.0";
const char* Core::ReleaseType = "Android Development Build";
bool Core::QuitFix = false;

bool Core::Initialize()
{
	UnitTesting::Test TestSequence[] = {
		{
			"Checking OS compatibility",
			OSVersionCheck
		},
#ifdef __ANDROID__
		{
			"Initializing Android data",
			AndroidData::Initialize
		},
#endif
		{
			"Loading basic game info",
			Game::Initialize
		},
#ifdef _WIN32
		{
			"Creates instance of patch map",
			[]() {
				CommandLine::Read();
				return true;
			}
		},
#endif
		{
			"Initializing Console Handle",
			Console::Initialize
		},
		{
			"Initializing Logging Service",
			Logger::Initialize
		},
		{
			"Creates instance of patch map",
			Game::ReadInfo
		},
#ifdef PORT_DISABLE
		{
			"Creates instance of patch map",
			HashCode::Initialize
		},
#endif
		{
			"Initializing Capstone",
			XrefScannerBindings::Init
		},
		{
			"Initializing Mono",
			Mono::Initialize
		},
		{
			"Show Welcome Message",
			[]() {
				WelcomeMessage();
				return true;
			}
		},
#ifdef PORT_DISABLE
		{
			"Creates instance of patch map",
			AnalyticsBlocker::Initialize
		},
#endif
		{
			"Initializing IL2CPP",
			Il2Cpp::Initialize
		},
		{
			"Load Mono",
			Mono::Load
		},
#ifdef PORT_DISABLE
		{
			"Creates instance of patch map",
			AnalyticsBlocker::Hook
		},
#endif
		{
			"Applying patches to IL2CPP",
			Il2Cpp::ApplyPatches
		},
#ifdef __ANDROID__
		{
			"Applying patches to Mono",
			Mono::ApplyPatches
		},
#endif
#ifdef _WIN32
		{
			"Creates instance of patch map",
			[]() {
				if (!Debug::Enabled)
					Console::NullHandles();
				return true;
			}
		},
#endif
	};

	return UnitTesting::RunTests(TestSequence, sizeof(TestSequence) / sizeof(TestSequence[0])) || Assertion::DontDie;
}

#ifdef _WIN32
#include <Windows.h>
#include <VersionHelpers.h>
#include <sys/types.h>
#include <sys/stat.h>
#include <sstream>

HINSTANCE Core::Bootstrap = NULL;

void Core::ApplyHooks()
{
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
#elif defined(__ANDROID__)
JavaVM* Core::Bootstrap = NULL;
JNIEnv* Core::Env = NULL;

bool Core::DirectoryExists(const char* path)
{
	struct stat sb;
	return stat(path, &sb) == 0 && S_ISDIR(sb.st_mode);
}

bool Core::FileExists(const char* path)
{
	struct stat sb;
	return stat(path, &sb) == 0;
}

void Core::WelcomeMessage()
{
	if (Debug::Enabled)
		Logger::WriteSpacer();
	
	Logger::Msg("------------------------------");
	Logger::Msg(("MelonLoader " + std::string(Version) + " " + std::string(ReleaseType)).c_str());
	// Logger::Msg(("Hash Code: " + std::to_string(HashCode::Hash)).c_str());
	Logger::Msg((std::string("OS: ") + GetOSVersion()).c_str());
	Logger::Msg("------------------------------");
	// Logger::Msg(("Name: " + std::string(Game::Name)).c_str());
	// Logger::Msg(("Developer: " + std::string(Game::Developer)).c_str());
	// Logger::Msg(("Unity Version: " + std::string(Game::UnityVersion)).c_str());
	// Logger::Msg(("Type: " + std::string((Game::IsIl2Cpp ? "Il2Cpp" : (Mono::IsOldMono ? "Mono" : "MonoBleedingEdge")))).c_str());
	Logger::Msg(
#if defined(_WIN64)
		"Arch: x64"
#elif defined(_WIN32)
		"Arch: x32"
#elif defined(_M_ARM64)
		"Arch: AArch64"
#else
		"Arch: Unknown"
#endif
	);
	Logger::Msg("------------------------------");
	
	if (Debug::Enabled)
		Logger::WriteSpacer();
	
	// Debug::Msg(("Game::BasePath = " + std::string(Game::BasePath)).c_str());
	// Debug::Msg(("Game::DataPath = " + std::string(Game::DataPath)).c_str());
	// Debug::Msg(("Game::ApplicationPath = " + std::string(Game::ApplicationPath)).c_str());
}

void Core::ApplyHooks()
{
	// Debug::Msg("Patching il2cpp");
	// Il2Cpp::ApplyPatches();
	//
	// Debug::Msg("Patching mono");
	// Mono::ApplyPatches();
	// Mono::CreateDomain("MelonLoader");
	// InternalCalls::Initialize();
	// AssemblyVerifier::InstallHooks();
	// BaseAssembly::Initialize();
}

const char* Core::GetFileInfoProductName(const char* path)
{
	return "Placeholder";
}

const char* Core::GetFileInfoProductVersion(const char* path)
{
	return Core::Version;
}

const char* Core::GetOSVersion()
{
	char osVersion[PROP_VALUE_MAX + 1];
	int osVersionLength = __system_property_get("ro.build.version.release", osVersion);

	return osVersion;
}

bool Core::OSVersionCheck()
{
	return true;
}

#endif