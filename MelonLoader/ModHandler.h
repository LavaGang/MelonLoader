#pragma once
#include "Mono.h"

class ModHandler
{
public:
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

enum ModHandler_DLLStatus
{
	ModHandler_DLLStatus_UNIVERSAL,
	ModHandler_DLLStatus_COMPATIBLE,
	ModHandler_DLLStatus_NOATTRIBUTE,
	ModHandler_DLLStatus_INCOMPATIBLE
};