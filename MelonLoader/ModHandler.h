#pragma once
#include "Mono.h"

class ModHandler
{
public:
	static bool Is35;
	static MonoMethod* onUpdate;
	static MonoMethod* onFixedUpdate;
	static MonoMethod* onLateUpdate;
	static MonoMethod* onGUI;
	static MonoMethod* onApplicationQuit;
	
	static void Initialize();
	static void OnLevelWasLoaded(int level);
	static void OnUpdate();
	static void OnFixedUpdate();
	static void OnLateUpdate();
	static void OnGUI();
	static void OnApplicationQuit();
};