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
		PositionEnum_ValueOf
	};
	static std::unordered_map<CachedMethodKeys, jmethodID> CachedMethods;
	static jmethodID GetMethod(jclass klass, const char* name, const char* sig, CachedMethodKeys key);
};
