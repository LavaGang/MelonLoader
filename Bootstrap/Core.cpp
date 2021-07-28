#include <sys/types.h>
#include <sys/stat.h>
#include <sstream>
#include <time.h>

#include "Core.h"
#include "Managers/Game.h"
#include "Managers/Mono.h"
#include "Managers/Il2Cpp.h"
#include "Managers/Hook.h"
#include "Utils/Console/CommandLine.h"
#include "Utils/Console/Console.h"
#include "Utils/Assertion.h"
#include "Utils/Console/Logger.h"
#include "Utils/Console/Debug.h"
#include "Utils/AnalyticsBlocker.h"
#include "Utils/HashCode.h"
#include "Utils/Sequence.h"

#ifdef _WIN32
#include <Windows.h>
#include <VersionHelpers.h>
#elif defined(__ANDROID__)
#include <unistd.h>

#include "./Managers/AndroidData.h"
#include "./Managers/AssetManagerHelper.h"
#include "./Managers/StaticSettings.h"
#include "./Utils/AssemblyUnhollower/XrefScannerBindings.h"
#endif

#ifdef _WIN32
HINSTANCE Core::Bootstrap = NULL;
Core::wine_get_version_t Core::wine_get_version = NULL;
#elif defined(__ANDROID__)
JavaVM* Core::Bootstrap = NULL;
JNIEnv* Core::Env = NULL;
#endif

char* Core::Path = NULL;

// FIXME: this should be moved to preprocessor define
std::string Core::Version = "0.4.1";
std::string Core::ReleaseType = "Android Development Build";
//std::string Core::ReleaseType = "Open-Beta";

std::string Core::GetVersionStr()
{
	std::string versionstr = std::string();
	if (Console::Mode == Console::DisplayMode::LEMON)
		versionstr += "Lemon";
	else
		versionstr += "Melon";
	versionstr += "Loader v" + Version;
	if (ReleaseType.size() == 0)
		versionstr += " " + ReleaseType;
	return versionstr;
}

std::string Core::GetVersionStrWithGameName(const char* GameVersion)
{
    std::string acc = GetVersionStr() + " - " + Game::Name;

    if (GameVersion != NULL)
        acc += GameVersion;

    return acc;
}

bool Core::Inject()
{
    std::vector<Sequence::Element> Sequence = {
            {
                    "Checking OS compatibility",
                    OSVersionCheck
            },
            {
                    "Initializing Console Handle",
                    Console::Initialize
            },
            {
                    "Initializing Logging Service",
                    Logger::Initialize
            },
    };

    return Sequence::Run(Sequence) || Assertion::DontDie;
}

bool Core::Initialize()
{
    std::vector<Sequence::Element> Sequence = {
#ifdef __ANDROID__
            {
                    "Initializing Android data",
                    AndroidData::Initialize
            },
            {
                    "Loading Asset Manager",
                    AssetManagerHelper::Initialize
            },
            {
                    "Loading Static Settings",
                    StaticSettings::Initialize
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
                    "Reading Game Info",
                    Game::ReadInfo
            },
#ifndef PORT_DISABLE
            {
			"Creates instance of patch map",
			HashCode::Initialize
		},
#endif
#ifdef __ANDROID__
            {
                    "Initializing Capstone",
                    XrefScannerBindings::Init
            },
#endif
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
#ifndef PORT_DISABLE
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
#ifndef PORT_DISABLE
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
            // {
            // 	"Initializing Bhaptics",
            // 	bHapticsPlayer::Initialize
            // },
#endif
#ifdef _WIN32
            {
			"Applying Null Handles",
			[]() {
				if (!Debug::Enabled)
					Console::NullHandles();
				return true;
			}
		},
#endif
    };

    return Sequence::Run(Sequence) || Assertion::DontDie;
}

void Core::WelcomeMessage()
{
	if (Debug::Enabled)
		Logger::WriteSpacer();
	Logger::Msg("------------------------------");
	Logger::Msg(GetVersionStr().c_str());
	Logger::Msg((std::string("OS: ") + GetOSVersion()).c_str());
#ifndef PORT_DISABLE
	Logger::Msg(("Hash Code: " + HashCode::Hash).c_str());
#endif
	Logger::Msg("------------------------------");
	Logger::Msg(("Name: " + std::string(Game::Name)).c_str());
	Logger::Msg(("Developer: " + std::string(Game::Developer)).c_str());
	Logger::Msg(("Unity Version: " + std::string(Game::UnityVersion)).c_str());
	Logger::Msg(("Game Type: " + std::string((Game::IsIl2Cpp ? "Il2Cpp" : (Mono::IsOldMono ? "Mono" : "MonoBleedingEdge")))).c_str());
	Logger::Msg(
#ifdef _WIN64
		"Game Arch: x64"
#elif defined(_WIN32)
		"Game Arch: x86"
#elif defined(__ANDROID__)
            "Game Arch: Arm64"
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
#ifdef _WIN32
	if (IsRunningInWine() || IsWindows7OrGreater())
		return true;
    // FIXME: this should be abstracted for better cross platform
	int result = MessageBoxA(NULL, "You are running on an Older Operating System.\nWe can not offer support if there are any issues.\nContinue?", "MelonLoader", MB_ICONWARNING | MB_YESNO);
	if (result == IDYES)
		return true;
	return false;
#elif defined(__ANDROID__)
    return true;
#endif
}

void Core::KillCurrentProcess()
{
#ifdef _WIN32
	HANDLE current_process = GetCurrentProcess();
	TerminateProcess(current_process, NULL);
	CloseHandle(current_process);
#elif defined(__ANDROID__)
    Logger::Error("Thread Core::KillCurrentProcess() invoked, killing process.");
    pthread_kill(getpid(), SIGQUIT);
#endif
}

const char* Core::GetFileInfoProductName(const char* path)
{
#ifdef _WIN32
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
#elif defined(__ANDROID__)
    return nullptr;
#endif
}

const char* Core::GetFileInfoProductVersion(const char* path)
{
#ifdef _WIN32
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
	productverpath << "\\StringFileInfo\\" << std::setw(4) << std::setfill('0') << std::hex << std::uppercase << buffer2[0] << std::setw(4) << std::setfill('0') << std::hex << std::uppercase << buffer2[1] << "\\FileVersion";
	if (!VerQueryValueA(buffer, productverpath.str().c_str(), (LPVOID*)&buffer2, &size2) || (size2 <= 0))
		return NULL;
	return (LPCSTR)buffer2;
#elif defined(__ANDROID__)
	return nullptr;
#endif
}

bool Core::DirectoryExists(const char* path) { struct stat Stat; return ((stat(path, &Stat) == 0) && (Stat.st_mode & S_IFDIR)); }
void Core::GetLocalTime(std::chrono::system_clock::time_point* now, std::chrono::milliseconds* ms, std::tm* bt) {
    *now = std::chrono::system_clock::now();
    *ms = std::chrono::duration_cast<std::chrono::milliseconds>((*now).time_since_epoch()) % 1000;
    time_t timer = std::chrono::system_clock::to_time_t(*now);
#if _WIN32
    localtime_s(bt, &timer);
#elif defined(__ANDROID__)
    localtime_r(&timer, bt);
#endif
}

bool Core::FileExists(const char* path) {
#ifdef _WIN32
    WIN32_FIND_DATAA data;
    return (FindFirstFileA(path, &data) != INVALID_HANDLE_VALUE);
#elif defined(__ANDROID__)
    return access( path, F_OK ) == 0;
#endif
}

const char* Core::GetOSVersion()
{
#ifdef _WIN32
	if (IsRunningInWine())
		return (std::string("Wine ") + wine_get_version()).c_str();
	else if (IsWindows10OrGreater())
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
#elif defined(__ANDROID__)
	// TODO: pull more info
	return "Android";
#endif
}

#ifdef _WIN32
void Core::SetupWineCheck()
{
	HMODULE ntdll = LoadLibraryA("ntdll.dll");
	if (ntdll == NULL)
		return;
	FARPROC wine_get_version_proc = GetProcAddress(ntdll, "wine_get_version");
	if (wine_get_version_proc == NULL)
		return;
	wine_get_version = (wine_get_version_t)wine_get_version_proc;
}
#endif