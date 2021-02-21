#pragma once
#include <Windows.h>

class AssemblyGenerator
{
public:
    static char* Path;
    static bool ForceRegeneration;
    static char* ForceVersion_UnityDependencies;
    static char* ForceVersion_Dumper;
    static char* ForceVersion_Il2CppAssemblyUnhollower;
    static int ProcessId;
	static bool Initialize();
    static void Cleanup();
};