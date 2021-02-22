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
#include "./Liberation/Liberation.h"
#include <dlfcn.h>
#include <sys/system_properties.h>
#include <android/log.h>
#include <stdio.h>
#include <string>
#include <sys/types.h>
#include <unistd.h>
#include <sys/mman.h>

#include "../Utils/PatchHelper.h"
#include "./Keystone/include/keystone/keystone.h"

typedef void (*testFnDef)(void);

char* Core::Path = NULL;
const char* Core::Version = "v0.3.0";
bool Core::QuitFix = false;

bool Core::Initialize()
{	
	if (
		!OSVersionCheck()
#ifdef PORT_DISABLE
		|| 
		!Game::Initialize()
#endif
		)
		return false;

	Logger::Msg(((std::string)"PID: " + std::to_string(getpid())).c_str());
#ifdef PORT_DISABLE
	CommandLine::Read();
	
	if (!Console::Initialize()
		|| !Logger::Initialize()
		|| !Game::ReadInfo()
		|| !HashCode::Initialize()
		|| !Mono::Initialize())
		return false;
#endif
	
	WelcomeMessage();

	if (
#ifdef PORT_DISABLE
		!AnalyticsBlocker::Initialize() ||
#endif
		!Il2Cpp::Initialize()
#ifdef PORT_DISABLE
		|| !Mono::Load()
#endif
		)
		return false;

	Logger::Msg("Initialized Il2Cpp");
	PatchHelper::Init();
	TestDirectMemAccess();

	// Il2Cpp::Exports::test_fn();
	
#ifdef PORT_DISABLE
	AnalyticsBlocker::Hook();

	ApplyHooks();
#endif
	
#ifdef _WIN32
	if (!Debug::Enabled)
		Console::NullHandles();
#endif
	
	return true;
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

void Core::WelcomeMessage()
{
	if (Debug::Enabled)
		Logger::WriteSpacer();
	
	Logger::Msg("------------------------------");
	Logger::Msg(("MelonLoader " + std::string(Version) + " Open-Beta").c_str());
	// Logger::Msg(("Hash Code: " + std::to_string(HashCode::Hash)).c_str());
	Logger::Msg((std::string("OS: ") + GetOSVersion()).c_str());
	Logger::Msg("------------------------------");
	// Logger::Msg(("Name: " + std::string(Game::Name)).c_str());
	// Logger::Msg(("Developer: " + std::string(Game::Developer)).c_str());
	// Logger::Msg(("Unity Version: " + std::string(Game::UnityVersion)).c_str());
	// Logger::Msg(("Type: " + std::string((Game::IsIl2Cpp ? "Il2Cpp" : (Mono::IsOldMono ? "Mono" : "MonoBleedingEdge")))).c_str());
	Logger::Msg(
#if defined(_WIN64) | defined(_M_ARM64)
		"Arch: x64"
#elif defined(_WIN32)
		"Arch: x32"
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

void Core::TestDirectMemAccess()
{
	__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "Lib Result: %p", Il2Cpp::Exports::test_fn_untyped);
	unsigned char* encoded;
	size_t size;
	
	if (PatchHelper::GenerateAsm((void*)Core::TestRedirectFunction, &encoded, &size))
	{
		__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "size: %lu", size);
		// char* buffer[size];
		// memcpy(buffer, encoded, size);

		// size_t i;
		// for (i = 0; i < size; i++) {
		// 	__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "%02x ", encoded[i]);
		// }
		
		int value;
		memcpy(&value, Il2Cpp::Exports::test_fn_untyped, 2);
		__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "PRE: %p", value);

		// __android_log_print(ANDROID_LOG_INFO, "MelonLoader", "Compiled: %lu bytes, statements: %lu\n", *size, count);
		
		Patch* testFn = Patch::Setup(Il2Cpp::Exports::test_fn_untyped, (char *)encoded, size);
		Logger::Msg("Patch Created");
		
		testFn->Apply();
		Logger::Msg("Patch Applied");
		
		memcpy(&value, Il2Cpp::Exports::test_fn_untyped, 2);
		__android_log_print(ANDROID_LOG_INFO, "MelonLoader", "ACTIVE: %p", value);
	} else
	{
		dlclose(Il2Cpp::Handle);
	}
	// volatile unsigned int& UART0CTL = *((volatile unsigned int*)Il2Cpp::Exports::test_fn_untyped);
	// Logger::Msg("defined value");
	// __android_log_print(ANDROID_LOG_INFO, "MelonLoader", "Lib Result 2: %p", &UART0);
	// UART0CTL &= ~1;
	// Logger::Msg("wrote value");

	
	 // unsigned long long msg = 0xDEADBEEFDEADBEEFEE;
	 // memmove(Il2Cpp::Exports::test_fn_untyped, &msg, 16);
	// memset(Il2Cpp::Exports::test_fn_untyped, 0, 16);
	
	 // Logger::Msg("Memory Test written");

	// int value;
	// memcpy(&value, Il2Cpp::Exports::test_fn_untyped, 2);
	// __android_log_print(ANDROID_LOG_INFO, "MelonLoader", "PRE: %p", value);
	//
	//
	// Patch* testFn = Patch::Setup(Il2Cpp::Exports::test_fn_untyped, "38467047");
	// Logger::Msg("Patch Created");
	//
	// testFn->Apply();
	// Logger::Msg("Patch Applied");
	//
	// memcpy(&value, Il2Cpp::Exports::test_fn_untyped, 2);
	// __android_log_print(ANDROID_LOG_INFO, "MelonLoader", "ACTIVE: %p", value);
	
	// testFn->Reset();
	// Logger::Msg("Patch Cleared");
	//
	// memcpy(&value, Il2Cpp::Exports::test_fn_untyped, 2);
	// __android_log_print(ANDROID_LOG_INFO, "MelonLoader", "POST: %p", value);
	
	// dlclose(Il2Cpp::Handle);

	// PATCH ASSEMBLY
	// MOV x11, 0x155c
	// SUB x11, x12, 0x4
	// MOV x11, 0x5af2
	// SUB x11, x12, 0x4
	// MOV x11, 0x0072
	// SUB x11, x12, 0x4
	// MOV x11, 0x0000
	// blr x11
}

void Core::TestRedirectFunction()
{
	Logger::Msg("THIS METHOD HAS BEEN PATCHED :)");
}
#endif