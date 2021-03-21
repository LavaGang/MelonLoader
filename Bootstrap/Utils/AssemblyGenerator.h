#pragma once
#include <Windows.h>

class AssemblyGenerator
{
public:
    static char* PathMono;
    static int ProcessId;
    static void Cleanup();
};