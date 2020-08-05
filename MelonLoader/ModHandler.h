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
	
	static void Initialize();
	static void OnApplicationStart();
	static void OnApplicationQuit();
};