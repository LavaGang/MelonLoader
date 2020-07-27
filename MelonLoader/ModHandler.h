#pragma once
#include "Mono.h"

class ModHandler
{
public:
	enum MelonCompatibility
	{
		UNIVERSAL,
		COMPATIBLE,
		NOATTRIBUTE,
		INCOMPATIBLE
	};

	static bool HasInitialized;
	static MonoMethod* onApplicationStart;
	static MonoMethod* onApplicationQuit;
	static MonoMethod* runLogCallbacks;
	static MonoMethod* runWarningCallbacks1;
	static MonoMethod* runWarningCallbacks2;
	static MonoMethod* runErrorCallbacks1;
	static MonoMethod* runErrorCallbacks2;
	
	static void Initialize();
	static void OnApplicationStart();
	static void OnApplicationQuit();
	static void RunLogCallbacks(const char* msg);
	static void RunWarningCallbacks(const char* msg);
	static void RunWarningCallbacks(const char* namesection, const char* msg);
	static void RunErrorCallbacks(const char* msg);
	static void RunErrorCallbacks(const char* namesection, const char* msg);
};