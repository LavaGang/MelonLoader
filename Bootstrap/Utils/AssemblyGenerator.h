#pragma once

#ifdef _WIN32
#include <Windows.h>
#include <metahost.h>
#endif

class AssemblyGenerator
{
public:
    static char* Path;
    static char* ForceVersion_UnityDependencies;
    static char* ForceVersion_Il2CppDumper;
    static char* ForceVersion_Il2CppAssemblyUnhollower;
    static int ProcessId;
	static bool Initialize();
    static void Cleanup();

#ifndef PORT_TODO_DISABLE
private:
	static ICLRMetaHost* metahost;
	static ICLRRuntimeInfo* rinfo;
	static ICLRRuntimeHost* rhost;
#endif
};