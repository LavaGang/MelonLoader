#pragma once
#include "Mono.h"

class ModHandler
{
public:
	static MonoMethod* onApplicationStart;
	static MonoMethod* onApplicationQuit;
	static MonoMethod* melonCoroutines_ProcessWaitForEndOfFrame;
	
	static void Initialize();
	static void OnApplicationStart();
	static void OnApplicationQuit();
	static void MelonCoroutines_ProcessWaitForEndOfFrame();
};