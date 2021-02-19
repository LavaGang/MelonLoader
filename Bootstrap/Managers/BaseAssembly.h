#ifndef PORT_TODO_DISABLE
#pragma once
#include "Mono.h"

class BaseAssembly
{
public:
	static char* Path;
	static bool Initialize();
	static void Start();

private:
	static Mono::Method* Mono_Start;
};
#endif