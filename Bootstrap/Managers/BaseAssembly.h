#pragma once
#include "Mono.h"

class BaseAssembly
{
public:
	static char* PathMono;
	static char* PreloadPath;
	static bool LoadAssembly();
	static bool Initialize();
	static bool PreStart();
	static void Start();
	static bool SetupPaths();

	static Mono::Assembly* Assembly;
	static Mono::Image* Image;

private:
	static Mono::Method* Mono_PreStart;
	static Mono::Method* Mono_Start;
};