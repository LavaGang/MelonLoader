#pragma once
#include <jni.h>
#include <vector>

class bHapticsPlayer
{
public:
	class Core
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
		static void SubmitRegisteredWithOption(const char*, const char*, t_options);
		static void SubmitRegisteredWithTime(const char*, int);
		static void SubmitDotArray(const char*, const char*, int*, size_t, int*, size_t, int);
		static void SubmitPathArray(const char*, const char*, int*, size_t, int*, size_t, int);
		static std::vector<int> GetPositionStatus(const char*);
		static bool IsRegistered(const char*);
		static bool IsPlaying(const char*);
		static bool IsAnythingPlaying(const char*);
	private:
		static jobject GetPlayer();
	};
};
