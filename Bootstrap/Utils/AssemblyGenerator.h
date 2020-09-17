#pragma once
#include <Windows.h>
#include <metahost.h>

class AssemblyGenerator
{
public:
    static char* Path;
    static bool ForceRegeneration;
    static char* ForceVersion_UnityDependencies;
    static char* ForceVersion_Il2CppDumper;
    static char* ForceVersion_Il2CppAssemblyUnhollower;
    static int ProcessId;
	static bool Initialize();
    static void Cleanup();

private:
    static ICLRMetaHost* metahost;
    static ICLRRuntimeInfo* rinfo;
    static ICLRRuntimeHost* rhost;
};