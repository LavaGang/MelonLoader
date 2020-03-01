#pragma once
#include "Mono.h"

class ModHandler
{
public:
	static MonoMethod* onApplicationQuit;
	
	static void Initialize();
	static void OnApplicationQuit();
};