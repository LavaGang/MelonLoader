#pragma once
#include <Windows.h>

class Il2CppAssemblyGenerator
{
public:
    static char* PathMono;
    static int ProcessId;
    static void Cleanup();
};