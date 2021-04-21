#pragma once
#include <vector>
class bHapticsBridge
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

		static void start();
		static void stop();
		static void turnOff(const char*);
		static void turnOffAll();
		static void registerProject(const char*, const char*);
		static void registerProjectReflected(const char*, const char*);
		static void submitRegisteredWithOption(const char*, const char*, t_options);
		static void submitRegisteredWithTime(const char*, int);
		static void submitDotArray(const char*, const char*, int*, size_t, int*, size_t, int);
		static void submitPathArray(const char*, const char*, int*, size_t, int*, size_t, int);
		static std::vector<int> getPositionStatus(const char*);
		static bool isRegistered(const char*);
		static bool isPlaying(const char*);
		static bool isAnythingPlaying(const char*);
	};
};

