#include "BaseAssembly.h"
#include "../Utils/Console/Debug.h"
#include "Game.h"
#include <string>
#include "../Utils/Assertion.h"
#include "../Utils/Console/Logger.h"
#include "../Utils/Il2CppAssemblyGenerator.h"

char* BaseAssembly::PathMono = NULL;
char* BaseAssembly::PreloadPath = NULL;
Mono::Method* BaseAssembly::Mono_PreStart = NULL;
Mono::Method* BaseAssembly::Mono_Start = NULL;
Mono::Assembly* BaseAssembly::Assembly = NULL;
Mono::Image* BaseAssembly::Image = NULL;

bool BaseAssembly::LoadAssembly()
{
	Assembly = Mono::Exports::mono_domain_assembly_open(Mono::domain, PathMono);
	if (Assembly == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Open Mono Assembly!");
		return false;
	}
	Image = Mono::Exports::mono_assembly_get_image(Assembly);
	if (Image == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get Image from Mono Assembly!");
		return false;
	}

	return true;
}

bool BaseAssembly::Initialize()
{
	Debug::Msg("Initializing Base Assembly...");
	Mono::Class* klass = Mono::Exports::mono_class_from_name(Image, "MelonLoader", "Core");
	if (klass == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get Class from Mono Image!");
		return false;
	}
	Mono::Method* Mono_Initialize = Mono::Exports::mono_class_get_method_from_name(klass, "Initialize", NULL);
	if (Mono_Initialize == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get Initialize Method from Mono Class!");
		return false;
	}

	Mono_PreStart = Mono::Exports::mono_class_get_method_from_name(klass, "PreStart", NULL);
	if (Mono_PreStart == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get PreStart Method from Mono Class!");
		return false;
	}
	
	Mono_Start = Mono::Exports::mono_class_get_method_from_name(klass, "Start", NULL);
	if (Mono_Start == NULL)
	{
		Assertion::ThrowInternalFailure("Failed to Get Start Method from Mono Class!");
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
#ifndef PORT_DISABLE
	if (Game::IsIl2Cpp)
		Il2CppAssemblyGenerator::Cleanup();
#endif
	Debug::Msg(("Return Value = " + std::to_string(returnval)).c_str());
	if (Debug::Enabled)
		Logger::WriteSpacer();

	return (returnval == 0);
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

bool BaseAssembly::SetupPaths()
{
	return false;
}
