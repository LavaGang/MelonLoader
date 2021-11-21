#include "BaseAssembly.h"
#include "../Utils/Debug.h"
#include "../Core.h"
#include "Game.h"
#include <string>
#include "../Utils/Assertion.h"
#include "../Utils/Logging/Logger.h"
#include "../Utils/Il2CppAssemblyGenerator.h"

char* BaseAssembly::PathMono = NULL;
char* BaseAssembly::PreloadPath = NULL;
Mono::Method* BaseAssembly::Mono_Start = NULL;
Mono::Method* BaseAssembly::Mono_PreStart = NULL;
Mono::Method* BaseAssembly::AssemblyManager_Resolve = NULL;
Mono::Method* BaseAssembly::AssemblyManager_LoadInfo = NULL;

bool BaseAssembly::Initialize()
{
	Preload();
	Debug::Msg("Initializing Base Assembly...");
	Mono::Assembly* assembly = Mono::Exports::mono_domain_assembly_open(Mono::domain, PathMono);
	if (assembly == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Open Mono Assembly!");
		return false;
	}
	Mono::Image* image = Mono::Exports::mono_assembly_get_image(assembly);
	if (image == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get Image from Mono Assembly!");
		return false;
	}
	Mono::Class* klass = Mono::Exports::mono_class_from_name(image, "MelonLoader", "Core");
	if (image == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get MelonLoader.Core from Mono Image!");
		return false;
	}
	Mono::Method* Mono_Initialize = Mono::Exports::mono_class_get_method_from_name(klass, "Initialize", NULL);
	if (Mono_Initialize == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get MelonLoader.Core.Initialize!");
		return false;
	}
	Mono_PreStart = Mono::Exports::mono_class_get_method_from_name(klass, "PreStart", NULL);
	if (Mono_PreStart == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get MelonLoader.Core.PreStart!");
		return false;
	}
	Mono_Start = Mono::Exports::mono_class_get_method_from_name(klass, "Start", NULL);
	if (Mono_Start == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get MelonLoader.Core.Start!");
		return false;
	}

	Mono::Class* klass_AssemblyManager = Mono::Exports::mono_class_from_name(image, "MelonLoader.MonoInternals.ResolveInternals", "AssemblyManager");
	if (klass_AssemblyManager == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get MelonLoader.MonoInternals.ResolveInternals.AssemblyManager from Mono Image!");
		return false;
	}
	AssemblyManager_Resolve = Mono::Exports::mono_class_get_method_from_name(klass_AssemblyManager, "Resolve", 6);
	if (AssemblyManager_Resolve == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get MelonLoader.MonoInternals.ResolveInternals.AssemblyManager.Resolve!");
		return false;
	}
	AssemblyManager_LoadInfo = Mono::Exports::mono_class_get_method_from_name(klass_AssemblyManager, "LoadInfo", 1);
	if (AssemblyManager_LoadInfo == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get MelonLoader.MonoInternals.ResolveInternals.AssemblyManager.LoadInfo!");
		return false;
	}

	Logger::WriteSpacer();

	Mono::Object* exObj = NULL;
	Mono::Object* result = Mono::Exports::mono_runtime_invoke(Mono_Initialize, NULL, NULL, &exObj);
	if (exObj != NULL)
	{
		Mono::LogException(exObj);
		Assertion::ThrowInternalFailure("Failed to Invoke Initialize Method!");
		return false;
	}
	int returnval = *(int*)((char*)result + 0x8);
	Debug::Msg(("Return Value = " + std::to_string(returnval)).c_str());
	if (Debug::Enabled)
		Logger::WriteSpacer();
	return (returnval == 0);
}

void BaseAssembly::Preload()
{
	if (Game::IsIl2Cpp || !Mono::IsOldMono)
		return;

	std::string PreloadAssemblyPath = std::string(Core::BasePath) + "\\MelonLoader\\Dependencies\\SupportModules\\Preload.dll";
	if (!Game::IsIl2Cpp && !Core::FileExists(PreloadAssemblyPath.c_str()))
	{
		Assertion::ThrowInternalFailure("Preload.dll Does Not Exist!");
		return;
	}

	Debug::Msg("Initializing Preload Assembly...");
	Mono::Assembly* assembly = Mono::Exports::mono_domain_assembly_open(Mono::domain, PreloadAssemblyPath.c_str());
	if (assembly == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Open Preload Assembly!");
		return;
	}
	Mono::Image* image = Mono::Exports::mono_assembly_get_image(assembly);
	if (image == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get Image from Preload Assembly!");
		return;
	}
	Mono::Class* klass = Mono::Exports::mono_class_from_name(image, "MelonLoader.Support", "Preload");
	if (image == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get Class from Preload Image!");
		return;
	}
	Mono::Method* initialize = Mono::Exports::mono_class_get_method_from_name(klass, "Initialize", NULL);
	if (initialize == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get Initialize Method from Preload Class!");
		return;
	}
	Mono::Object* exObj = NULL;
	Mono::Exports::mono_runtime_invoke(initialize, NULL, NULL, &exObj);
}

bool BaseAssembly::PreStart()
{
	if (Mono_PreStart == NULL)
		return false;
	Debug::Msg("Pre-Starting Base Assembly...");
	Logger::WriteSpacer();
	Mono::Object* exObj = NULL;
	Mono::Object* result = Mono::Exports::mono_runtime_invoke(Mono_PreStart, NULL, NULL, &exObj);
	if (exObj != NULL)
	{
		Mono::LogException(exObj);
		Assertion::ThrowInternalFailure("Failed to Invoke PreStart Method!");
	}
	int returnval = *(int*)((char*)result + 0x8);
	if (Game::IsIl2Cpp)
		Il2CppAssemblyGenerator::Cleanup();
	Debug::Msg(("Return Value = " + std::to_string(returnval)).c_str());
	if (Debug::Enabled)
		Logger::WriteSpacer();
	return (returnval == 0);
}

void BaseAssembly::Start()
{
	if (Mono_Start == NULL)
		return;
	Debug::Msg("Starting Base Assembly...");
	Logger::WriteSpacer();
	Mono::Object* exObj = NULL;
	Mono::Object* result = Mono::Exports::mono_runtime_invoke(Mono_Start, NULL, NULL, &exObj);
	if (exObj != NULL)
	{
		Mono::LogException(exObj);
		Assertion::ThrowInternalFailure("Failed to Invoke Start Method!");
	}
	int returnval = *(int*)((char*)result + 0x8);
	Debug::Msg(("Return Value = " + std::to_string(returnval)).c_str());
	if (Debug::Enabled)
		Logger::WriteSpacer();
}