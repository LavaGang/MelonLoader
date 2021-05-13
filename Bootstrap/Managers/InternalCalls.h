#pragma once
#include "Mono.h"
#include "StaticSettings.h"
#include "../Utils/Console/Console.h"

class InternalCalls
{
public:
	static void Initialize();

	class MelonLogger
	{
	public:
		static void AddInternalCalls();
		static void Internal_PrintModName(Console::Color meloncolor, Mono::String* name, Mono::String* version);
		static void Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, Mono::String* namesection, Mono::String* txt);
		static void Internal_Warning(Mono::String* namesection, Mono::String* txt);
		static void Internal_Error(Mono::String* namesection, Mono::String* txt);
		static void ThrowInternalFailure(Mono::String* msg);
		static void WriteSpacer();
		static void Flush();
	};

	class MelonUtils
	{
	public:
		static void AddInternalCalls();
		static bool IsGame32Bit();
		static bool IsGameIl2Cpp();
		static bool IsOldMono();
		static Mono::String* GetApplicationPath();
		static Mono::String* GetGamePackage();
		static Mono::String* GetGameName();
		static Mono::String* GetGameDeveloper();
		static Mono::String* GetGameDirectory();
		static Mono::String* GetGameDataDirectory();
		static Mono::String* GetMainAssemblyLoc();
		static Mono::String* GetUnityVersion();
		static Mono::String* GetManagedDirectory();
		static Mono::String* GetHashCode();
		static void SCT(Mono::String* title);
		static Mono::String* GetFileProductName(Mono::String* filepath);
		static void GetStaticSettings(StaticSettings::Settings_t &settings);
	};

	class MelonDebug
	{
	public:
		static void AddInternalCalls();
		static void Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, Mono::String* namesection, Mono::String* txt);
	}; 

	class SupportModules
	{
	public:
		static void AddInternalCalls();
		static void SetDefaultConsoleTitleWithGameName(Mono::String* GameVersion);
	};

	class UnhollowerIl2Cpp
	{
	public:
		static void AddInternalCalls();
	private:
		static void* GetProcAddress(void* hModule, Mono::String* procName);
		static void* LoadLibrary(Mono::String* lpFileName);
		static void* GetAsmLoc();
		static void CleanupDisasm();
	};

	class BHaptics
	{
	public:
		static void AddInternalCalls();
	private:
		// native parser
		static int ReleaseAddress(intptr_t* address);
		
		// player
		static void TurnOff(Mono::String* key);
		static void TurnOffAll();
		static void RegisterProject(Mono::String* key, Mono::String* contents);
		static void RegisterProjectReflected(Mono::String* key, Mono::String* contents);
		static void SubmitRegistered(Mono::String* key, Mono::String* altKey, float intensity, float duration, float offsetAngleX, float offsetY);
		static void SubmitRegisteredWithTime(Mono::String* key, int startTime);
		static bool IsRegistered(Mono::String* key);
		static bool IsPlaying(Mono::String* key);
		static bool IsAnythingPlaying();
		static void Internal_SubmitDot(Mono::String* key, Mono::String* position, int* indexes, int* intensities, int* sizes, int duration);
		static void Internal_SubmitPath(Mono::String* key, Mono::String* position, float* x, float* y, int* intensities, int* sizes, int duration);
		static intptr_t Internal_GetPositionStatus(Mono::String* position);

		// connection manager
		static void Scan();
		static void StopScan();
		static void RefreshPairingInfo();
		static void Unpair(Mono::String* address);
		static void UnpairAll();
		static void TogglePosition(Mono::String* address);
		static void Ping(Mono::String* address);
		static void PingAll();
		static bool IsDeviceConnected(Mono::String* address);
		static void Internal_Pair(Mono::String* address);
		static void Internal_PairPositioned(Mono::String* address, Mono::String* position);
		static bool Internal_GetIsScanning();
		static void Internal_ChangePosition(Mono::String* address, Mono::String* position);
		static void Internal_SetMotor(Mono::String* address, char* bytes, size_t size);
		static intptr_t Internal_GetDeviceList();

		// device
		static bool Internal_IsPing(Mono::String* address);
		static bool Internal_IsPaired(Mono::String* address);
		static int Internal_GetConnectFailCount(Mono::String* address);
		static int Internal_GetRssi(Mono::String* address);
		static Mono::String* Internal_GetConnectionStatus(Mono::String* address);
		static Mono::String* Internal_GetPosition(Mono::String* address);
		static Mono::String* Internal_GetAddress(Mono::String* address);
		static Mono::String* Internal_GetDeviceName(Mono::String* address);
		static int Internal_GetBattery(Mono::String* address);
		static Mono::String* Internal_GetType(Mono::String* address);
		static intptr_t Internal_GetLastBytes(Mono::String* address);
		static int64_t Internal_GetLastScannedTime(Mono::String* address);
		static Mono::String* Internal_ToString(Mono::String* address);
	};
};
