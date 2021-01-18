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
char* AssemblyGenerator::ForceVersion_UnityDependencies = NULL;
char* AssemblyGenerator::ForceVersion_Il2CppDumper = NULL;
char* AssemblyGenerator::ForceVersion_Il2CppAssemblyUnhollower = NULL;
int AssemblyGenerator::ProcessId = 0;
ICLRMetaHost* AssemblyGenerator::metahost = NULL;
ICLRRuntimeInfo* AssemblyGenerator::rinfo = NULL;
ICLRRuntimeHost* AssemblyGenerator::rhost = NULL;

bool AssemblyGenerator::Initialize()
{
	Console::GeneratingAssembly = true;
	Logger::WriteSpacer();
	if (!Debug::Enabled && Console::ShouldHide && !Console::Initialize())
	{
		Assertion::ThrowInternalFailure("Failed to Initialize Console!");
		return false;
	}
	Console::DisableCloseButton();
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
	if (FAILED(rhost->ExecuteInDefaultAppDomain(std::wstring(assembly_path.begin(), assembly_path.end()).c_str(), L"MelonLoader.AssemblyGenerator.Core", L"Run", L"", &returnval)))
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
	if ((rhost == NULL) || (rinfo == NULL) || (metahost == NULL))
		return;
	Debug::Msg("Cleaning up Assembly Generator...");
	if (ProcessId != 0)
	{
		HANDLE hProcess = OpenProcess(PROCESS_TERMINATE, FALSE, ProcessId);
		if (hProcess != NULL)
		{
			BOOL result = TerminateProcess(hProcess, 0);
			CloseHandle(hProcess);
		}
		ProcessId = 0;
	}
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
	Game::Initialize();
	Game::ReadInfo();
	HashCode::SetupPaths();
	Mono::SetupPaths();
	Console::GeneratingAssembly = false;
	if (!Debug::Enabled && Console::ShouldHide)
		Console::Close();
	else
		Console::EnableCloseButton();
}