#include <Windows.h>
#include <VersionHelpers.h>
#include <sys/types.h>
#include <sys/stat.h>
#include "Core.h"
#include "Managers/Game.h"
#include "Managers/Mono.h"
#include "Managers/Il2Cpp.h"
#include "Managers/Hook.h"
#include "Utils/CommandLine.h"
#include "Utils/Console.h"
#include "Utils/Assertion.h"
#include "Utils/Logging/Logger.h"
#include "Utils/Debug.h"
#include "Utils/HashCode.h"
#include "Utils/Encoding.h"

HINSTANCE Core::Bootstrap = NULL;
char* Core::BasePath = NULL;
char* Core::BasePathMono = NULL;
char* Core::Path = NULL;
std::string Core::Version = "0.6.0";
bool Core::Is_ALPHA_PreRelease = false;
Core::wine_get_version_t Core::wine_get_version = NULL;

std::string Core::GetVersionStr()
{
	std::string versionstr = std::string();
	if (Console::Mode == Console::DisplayMode::LEMON)
		versionstr += "Lemon";
	else
		versionstr += "Melon";
	versionstr += "Loader v" + Version + " ";
	if (Is_ALPHA_PreRelease)
		versionstr += "ALPHA Pre-Release";
	else
		versionstr += "Open-Beta";
	return versionstr;
}

std::string Core::GetVersionStrWithGameName(const char* GameName, const char* GameVersion)
{
	return (GetVersionStr()
		+ " - "
		+ GameName
		+ " "
		+ ((GameVersion == NULL)
			? ""
			: GameVersion));
}

void Core::Initialize(HINSTANCE hinstDLL)
{
	Bootstrap = hinstDLL;
	SetBasePath();
	SetupWineCheck();

	if (!OSVersionCheck() 
		|| !Game::Initialize())
		return;

	CommandLine::Read();
	if (!Console::Initialize()
		|| !Logger::Initialize()
		|| !CheckPathASCII()
		|| !HashCode::Initialize())
		return;

	if (!Game::IsIl2Cpp && !Mono::Initialize())
		//If we're a mono game and we failed to init mono, die
		return;

	if (!Il2Cpp::Initialize())
		return;

	bool runtime_initialized = Game::IsIl2Cpp ? /*DotnetRuntime::LoadDotNet()*/ true : Mono::Load();

	if (!runtime_initialized)
		return;

	if (Game::IsIl2Cpp)
	{
		Debug::Msg("Attaching Hook to il2cpp_init...");
		Hook::Attach(&(LPVOID&)Il2Cpp::Exports::il2cpp_init, Il2Cpp::Hooks::il2cpp_init);
	}
	else
	{
		Debug::Msg("Attaching Hook to mono_jit_init_version...");
		Hook::Attach(&(LPVOID&)Mono::Exports::mono_jit_init_version, Mono::Hooks::mono_jit_init_version);
	}

	if (Console::CleanUnityLogs)
		Console::NullHandles();
}

bool Core::CheckPathASCII() 
{
	if (std::string(BasePath).find('?') != std::string::npos)
	{
		Assertion::ThrowInternalFailure("The base directory path contains non-ASCII characters,\nwhich are not supported by MelonLoader.\nPlease remove them and try again.");
		return false;
	}
	if (std::string(Game::BasePath).find('?') != std::string::npos)
	{
		Assertion::ThrowInternalFailure("The game directory path contains non-ASCII characters,\nwhich are not supported by MelonLoader.\nPlease remove them and try again.");
		return false;
	}
	return true;
}

bool Core::OSVersionCheck()
{
	if (IsRunningInWine() || IsWindows7OrGreater())
		return true;
	int result = MessageBoxA(NULL, "You are running on an Older Operating System.\nWe can not offer support if there are any issues.\nContinue?", "MelonLoader", MB_ICONWARNING | MB_YESNO);
	if (result == IDYES)
		return true;
	return false;
}

void Core::KillCurrentProcess()
{
	HANDLE current_process = GetCurrentProcess();
	TerminateProcess(current_process, NULL);
	CloseHandle(current_process);
}

VERSIONHELPERAPI IsWindows11OrGreater()
{
	OSVERSIONINFOEXW osinfo = { sizeof(osinfo), HIBYTE(_WIN32_WINNT_WIN10), LOBYTE(_WIN32_WINNT_WIN10), 22000, 0, { 0 }, 0, 0 };

	DWORDLONG const mask = VerSetConditionMask(
		VerSetConditionMask(
			VerSetConditionMask(
				0, VER_MAJORVERSION, VER_GREATER_EQUAL),
			VER_MINORVERSION, VER_GREATER_EQUAL),
		VER_BUILDNUMBER, VER_GREATER_EQUAL);

	return VerifyVersionInfoW(&osinfo, VER_MAJORVERSION | VER_MINORVERSION | VER_BUILDNUMBER, mask) != FALSE;
}

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

void Core::SetBasePath()
{
	LPSTR filepathstr = new CHAR[MAX_PATH];
	GetModuleFileNameA(Bootstrap, filepathstr, MAX_PATH);
	std::string filepathstr2 = filepathstr;
	delete[] filepathstr;
	filepathstr2 = filepathstr2.substr(0, filepathstr2.find_last_of("\\/"));
	filepathstr2 = filepathstr2.substr(0, filepathstr2.find_last_of("\\/"));
	filepathstr2 = filepathstr2.substr(0, filepathstr2.find_last_of("\\/"));
	BasePath = new char[filepathstr2.size() + 1];
	std::copy(filepathstr2.begin(), filepathstr2.end(), BasePath);
	BasePath[filepathstr2.size()] = '\0';
	BasePathMono = Encoding::OsToUtf8(BasePath);
}

bool Core::DirectoryExists(const char* path) { struct stat Stat; return ((stat(path, &Stat) == 0) && (Stat.st_mode & S_IFDIR)); }
bool Core::FileExists(const char* path) { WIN32_FIND_DATAA data; return (FindFirstFileA(path, &data) != INVALID_HANDLE_VALUE); }
void Core::GetLocalTime(std::chrono::system_clock::time_point* now, std::chrono::milliseconds* ms, std::tm* bt) { *now = std::chrono::system_clock::now(); *ms = std::chrono::duration_cast<std::chrono::milliseconds>((*now).time_since_epoch()) % 1000; time_t timer = std::chrono::system_clock::to_time_t(*now); localtime_s(bt, &timer); }