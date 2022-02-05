#pragma once
#include <string>
#include <algorithm>
#include <filesystem>
#include <chrono>

#ifdef _WIN32
#include <Windows.h>
#elif defined(__ANDROID__)
#include <jni.h>
#endif

class Core
{
public:
#ifdef _WIN32
    static HINSTANCE Bootstrap;
#elif defined(__ANDROID__)
    static JavaVM* Bootstrap;
    static JNIEnv* Env;
#endif

	static char* Path;
	static std::string Version;
	static std::string ReleaseType;

    // Android: Starts when application first loads. This means that some functionality wont be available until a context is created.
    // Windows: Runs first during initialization
    static bool Inject();
    static bool Initialize();

	static bool DirectoryExists(const char* path);
	static bool FileExists(const char* path);
	static void GetLocalTime(std::chrono::system_clock::time_point* now, std::chrono::milliseconds* ms, std::tm* bt);
	static void WelcomeMessage();
	static void KillCurrentProcess();
	static const char* GetFileInfoProductName(const char* path);
	static const char* GetFileInfoProductVersion(const char* path);
	static std::string GetVersionStr();
	static std::string GetVersionStrWithGameName(const char* GameVersion = NULL);

private:
#ifdef _WIN32
    typedef const char* (*wine_get_version_t) ();
	static wine_get_version_t wine_get_version;

	static bool IsRunningInWine() { return ((wine_get_version == NULL) ? false : true); }
	static void SetupWineCheck();
#endif

	static const char* GetOSVersion();
	static bool OSVersionCheck();
};