#pragma once
#include <jni.h>
#include <vector>
#include <unordered_map>

class bHapticsPlayer
{
public:
	class HapticPlayer
	{
	public:
		struct t_options
		{
			float intensity;
			float duration;
			float offsetAngleX;
			float offsetY;
		};

		static void TurnOff(const char*);
		static void TurnOffAll();
		static void RegisterProject(const char*, const char*);
		static void RegisterProjectReflected(const char*, const char*);
		static void SubmitRegistered(const char*, const char*, t_options&);
		static void SubmitRegisteredWithTime(const char*, int);
		static void SubmitDot(const char*, const char*, int*, size_t, int*, size_t, int);
		static void SubmitPath(const char*, const char*, float*, size_t, float*, size_t, int*, size_t, int);
		static std::vector<char> GetPositionStatus(const char*);
		static bool IsRegistered(const char*);
		static bool IsPlaying(const char*);
		static bool IsAnythingPlaying();
	private:
		static jclass PlayerClass;
		static jobject PlayerClassInstance;
		static std::tuple<jclass, jobject> GetPlayer();
	};
	class BhapticsManager
	{
	public:
		static void Scan();
		static void StopScan();
		static bool IsScanning();
		static void RefreshPairingInfo();
		static void Pair(const char*);
		static void PairPositioned(const char*, const char*);
		static void Unpair(const char*);
		static void UnpairAll();
		static void ChangePosition(const char*, const char*);
		static void TogglePosition(const char*);
		static void SetMotor(const char*, char*, size_t);
		static void Ping(const char*);
		static void PingAll();
		static bool IsDeviceConnected(const char*);
		static std::vector<jobject> GetDeviceList();
		
		static std::tuple<jclass, jobject> GetManager();
	private:
		static jobject ManagerClassInstance;
		static jclass ManagerClass;
	};
	class BhapticsDevice
	{
	public:
		static bool IsPing(jobject);
		static bool IsPaired(jobject);
		static int GetConnectFailedCount(jobject);
		static int GetRSSI(jobject);
		static const char* GetConnectionStatus(jobject);
		static const char* GetPosition(jobject);
		static const char* GetAddress(jobject);
		static const char* GetDeviceName(jobject);
		static int GetBattery(jobject);
		static const char* GetType(jobject);
		static std::vector<char> GetLastBytes(jobject);
		static uint64_t GetLastScannedTime(jobject);
		static const char* ToString(jobject);
		
		static jobject GetDevice(const char*);
	private:
		static std::unordered_map<std::hash<std::string>, jobject> DeviceMap;
		static std::hash<std::string> HashAddress(const char*);
	};
private:
	enum CachedMethodKeys
	{
		Player_TurnOff,
		Player_TurnOffAll,
		Player_RegisterProject,
		Player_RegisterProjectReflected,
		Player_SubmitRegistered,
		Player_SubmitRegisteredWithTime,
		Player_SubmitDot,
		Player_SubmitPath,
		Player_GetPositionStatus,
		Player_IsRegistered,
		Player_IsPlaying,
		Player_IsAnythingPlaying,
		PositionEnum_ValueOf,
		InlineDeviceTypeEnum_ValueOf,
		PositionEnum_Name,
		DeviceTypeEnum_Name,
		ConnectionStatusEnum_Name,
		Manager_Scan,
		Manager_StopScan,
		Manager_IsScanning,
		Manager_RefreshPairingInfo,
		Manager_Pair,
		Manager_PairPositioned,
		Manager_Unpair,
		Manager_UnpairAll,
		Manager_ChangePosition,
		Manager_TogglePosition,
		Manager_SetMotor,
		Manager_Ping,
		Manager_PingAll,
		Manager_IsDeviceConnected,
		Manager_GetDeviceList,
		List_ToArray,
		Device_IsPing,
		Device_IsPaired,
		Device_GetConnectFailCount,
		Device_GetRSSI,
		Device_GetConnectionStatus,
		Device_GetPosition,
		Device_GetAddress,
		Device_GetDeviceName,
		Device_GetBattery,
		Device_GetType,
		Device_GetLastBytes,
		Device_GetLastScannedTime,
		Device_ToString
	};
	static std::unordered_map<CachedMethodKeys, jmethodID> CachedMethods;
	static jmethodID GetMethod(jclass klass, const char* name, const char* sig, CachedMethodKeys key);

	enum CachedClassKeys
	{
		BhapticsManager,
		HapticPlayer,
		InlineDeviceType,
		DeviceType,
		PositionType,
		DeviceManager,
		JavaList,
		BhapticsDevice,
		ConnectionStatus
	};
	static std::unordered_map<CachedClassKeys, jclass> CachedClasses;
	static jclass GetKlass(const char* name, CachedClassKeys key);
};
