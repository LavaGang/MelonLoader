#pragma once
#include "Mono.h"

class BaseAssembly
{
public:
	static char* PathMono;
	static char* PreloadPath;
	static bool Initialize();
	static void Preload();
	static bool PreStart();
	static void Start();

private:
	static Mono::Method* Mono_PreStart;
	static Mono::Method* Mono_Start;
};