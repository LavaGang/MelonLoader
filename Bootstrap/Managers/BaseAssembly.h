#pragma once
#include "Mono.h"

class BaseAssembly
{
public:
	static char* PathMono;
	static bool Initialize();
	static bool PreStart();
	static void Start();
};