#pragma once
#include "Mono.h"

class ModHandler
{
public:
	static bool HasInitialized;
	static MonoMethod* onApplicationStart;
	static MonoMethod* onApplicationQuit;
	static MonoMethod* runLogCallbacks;
	static MonoMethod* runWarningCallbacks;
	static MonoMethod* runErrorCallbacks;
	
	static void Initialize();
	static void OnApplicationStart();
	static void OnApplicationQuit();
	static void RunLogCallbacks(const char* msg);
};

enum ModHandler_DLLStatus
{
	ModHandler_DLLStatus_UNIVERSAL,
	ModHandler_DLLStatus_COMPATIBLE,
	ModHandler_DLLStatus_NOATTRIBUTE,
	ModHandler_DLLStatus_INCOMPATIBLE
};