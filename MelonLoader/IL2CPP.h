#pragma once
#include <Windows.h>

struct Il2CppDomain;
struct Il2CppAssembly;
struct Il2CppType;
struct Il2CppClass;
struct Il2CppField;
struct Il2CppProperty;
struct Il2CppMethod
{
	void* targetMethod;
};

typedef Il2CppDomain* (*il2cpp_init_t) (const char* name);

class Il2Cpp
{
public:
	static Il2CppDomain* Domain;
	static il2cpp_init_t il2cpp_init;

	static bool Setup(HMODULE mod);
};