#pragma once
#include <Windows.h>
#include <metahost.h>

class AssemblyGenerator
{
public:
    static char* Path;
	static bool Initialize();
    static void Cleanup();

private:
    static ICLRMetaHost* metahost;
    static ICLRRuntimeInfo* rinfo;
    static ICLRRuntimeHost* rhost;
};