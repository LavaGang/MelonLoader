#pragma once
#include "Mono.h"

class BaseAssembly
{
public:
	static char* PathMono;
	static char* PreloadPath;
	static bool Initialize();
	static void Preload();
	static void Start();
	static bool SetupPaths();

private:
	static Mono::Method* Mono_Start;
};