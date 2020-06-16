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