#include "AssemblyGenerator.h"
#include "Assertion.h"
#include "Console.h"
#include "Debug.h"
#include "../Managers/Game.h"
#include <string>
#include "../Base/Core.h"
#include "../Utils/Logger.h"
#include "../Managers/Mono.h"
#include "HashCode.h"
#pragma comment(lib, "mscoree.lib")

char* AssemblyGenerator::Path = NULL;
bool AssemblyGenerator::ForceRegeneration = false;
char* AssemblyGenerator::ForceVersion_UnityDependencies = NULL;
char* AssemblyGenerator::ForceVersion_Il2CppDumper = NULL;
char* AssemblyGenerator::ForceVersion_Il2CppAssemblyUnhollower = NULL;
ICLRMetaHost* AssemblyGenerator::metahost = NULL;
ICLRRuntimeInfo* AssemblyGenerator::rinfo = NULL;
ICLRRuntimeHost* AssemblyGenerator::rhost = NULL;

bool AssemblyGenerator::Initialize()
{
	Logger::WriteSpacer();
	Debug::Msg("Initializing Assembly Generator...");
	if (FAILED(CLRCreateInstance(CLSID_CLRMetaHost, IID_PPV_ARGS(&metahost))))
	{
		Assertion::ThrowInternalFailure("Failed to Create CLR Metahost Instance!");
		Cleanup();
		return false;
	}
	if (FAILED(metahost->GetRuntime(L"v4.0.30319", IID_PPV_ARGS(&rinfo))))
	{
		Assertion::ThrowInternalFailure("Failed to Get CLR Runtime v4.0.30319!");
		Cleanup();
		return false;
	}
	if (FAILED(rinfo->GetInterface(CLSID_CLRRuntimeHost, IID_PPV_ARGS(&rhost))))
	{
		Assertion::ThrowInternalFailure("Failed to Get CLR Runtime Host Interface!");
		Cleanup();
		return false;
	}
	if (FAILED(rhost->Start()))
	{
		Assertion::ThrowInternalFailure("Failed to Start CLR Runtime Host!");
		Cleanup();
		return false;
	}
	std::string assembly_path = Path;
    DWORD returnval;
	if (FAILED(rhost->ExecuteInDefaultAppDomain(std::wstring(assembly_path.begin(), assembly_path.end()).c_str(), L"MelonLoader.AssemblyGenerator.Main", L"Run", L"", &returnval)))
	{
		Debug::Msg(("Return Value = " + std::to_string(returnval)).c_str());
		Assertion::ThrowInternalFailure("Failed to Execute Assembly Generator!");
		Cleanup();
		return false;
	}
	Cleanup();
	Debug::Msg(("Return Value = " + std::to_string(returnval)).c_str());
	if (Debug::Enabled)
		Logger::WriteSpacer();
	return (returnval == 0);
}

void AssemblyGenerator::Cleanup()
{
	Debug::Msg("Cleaning up Assembly Generator...");
	if (rhost != NULL)
	{
		rhost->Stop();
		rhost->Release();
		rhost = NULL;
	}
	if (rinfo != NULL)
	{
		rinfo->Release();
		rinfo = NULL;
	}
	if (metahost != NULL)
	{
		metahost->Release();
		metahost = NULL;
	}
	Game::SetupPaths();
	HashCode::SetupPaths();
	Mono::SetupPaths();
}