#pragma once
#include "Mono.h"

class ModHandler
{
public:
	static bool HasInitialized;
	static MonoMethod* onApplicationStart;
	static MonoMethod* onApplicationQuit;
	
	static void Initialize();
	static void OnApplicationStart();
	static void OnApplicationQuit();
};

enum ModHandler_DLLStatus
{
	ModHandler_DLLStatus_UNIVERSAL,
	ModHandler_DLLStatus_COMPATIBLE,
	ModHandler_DLLStatus_NOATTRIBUTE,
	ModHandler_DLLStatus_INCOMPATIBLE
};