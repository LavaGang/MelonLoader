#pragma once
#include "Mono.h"

class ModHandler
{
public:
	static bool HasInitialized;
	static MonoMethod* onApplicationStart;
	static MonoMethod* onApplicationQuit;
	static MonoMethod* runLogCallbacks;
	static MonoMethod* runLogOverrideCallbacks;
	static MonoMethod* runWarningCallbacks;
	static MonoMethod* runWarningOverrideCallbacks;
	static MonoMethod* runErrorCallbacks;
	static MonoMethod* runErrorOverrideCallbacks;
	
	static void Initialize();
	static void OnApplicationStart();
	static void OnApplicationQuit();
	static void RunLogCallbacks(const char* msg);
	static const char* RunLogOverrideCallbacks(const char* msg);
	static void RunWarningCallbacks(const char* msg);
	static const char* RunWarningOverrideCallbacks(const char* msg);
	static void RunErrorCallbacks(const char* msg);
	static const char* RunErrorOverrideCallbacks(const char* msg);
};

enum ModHandler_DLLStatus
{
	ModHandler_DLLStatus_UNIVERSAL,
	ModHandler_DLLStatus_COMPATIBLE,
	ModHandler_DLLStatus_NOATTRIBUTE,
	ModHandler_DLLStatus_INCOMPATIBLE
};