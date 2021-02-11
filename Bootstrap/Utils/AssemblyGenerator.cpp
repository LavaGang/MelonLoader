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

bool AssemblyGenerator::ForceRegeneration = false;
char* AssemblyGenerator::Path = NULL;
char* AssemblyGenerator::ForceVersion_UnityDependencies = NULL;
char* AssemblyGenerator::ForceVersion_Dumper = NULL;
char* AssemblyGenerator::ForceVersion_Il2CppAssemblyUnhollower = NULL;
int AssemblyGenerator::ProcessId = 0;

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

	Mono::Assembly* assembly = Mono::Exports::mono_domain_assembly_open(Mono::domain, Path);
	if (assembly == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Open Assembly Generator!");
		return false;
	}
	Mono::Image* image = Mono::Exports::mono_assembly_get_image(assembly);
	if (image == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get Image from Assembly Generator!");
		return false;
	}
	Mono::Class* klass = Mono::Exports::mono_class_from_name(image, "MelonLoader.AssemblyGenerator", "Core");
	if (image == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get Class from Assembly Generator Image!");
		return false;
	}
	Mono::Method* initialize = Mono::Exports::mono_class_get_method_from_name(klass, "Run", NULL);
	if (initialize == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get Run Method from MelonLoader.AssemblyGenerator Class!");
		return false;
	}
	Mono::Object* exObj = NULL;
	Mono::Object* result = Mono::Exports::mono_runtime_invoke(initialize, NULL, NULL, &exObj);
	if (exObj)
	{
		Mono::LogException(exObj);
		Assertion::ThrowInternalFailure("Assembly Generator failed with exception!");
		return false;
	}
	int returnval = *(int*)((char*)result + 0x8);
	Cleanup();
	Debug::Msg(("Return Value = " + std::to_string(returnval)).c_str());
	if (Debug::Enabled)
		Logger::WriteSpacer();
	return (returnval == 0);
}

void AssemblyGenerator::Cleanup()
{
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