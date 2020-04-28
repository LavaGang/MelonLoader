#pragma once
#include <Windows.h>
#include "il2cpp-internals.h"

typedef Il2CppDomain* (*il2cpp_init_t) (const char* name);
typedef void (*il2cpp_add_internal_call_t) (const char* name, void* method);

class IL2CPP
{
public:
	static HMODULE Module;
	static Il2CppDomain* Domain;
	static il2cpp_init_t il2cpp_init;
	static il2cpp_add_internal_call_t il2cpp_add_internal_call;

	static bool Setup();
	static void AddInternalCalls();
};