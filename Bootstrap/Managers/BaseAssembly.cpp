#include "BaseAssembly.h"
#include "../Utils/Console/Debug.h"
#include "../Base/Core.h"
#include "Game.h"
#include <string>
#include "../Utils/Assertion.h"
#include "../Utils/Console/Logger.h"
#include "../Utils/UnitTesting/TestHelper.h"

char* BaseAssembly::PathMono = NULL;
char* BaseAssembly::PreloadPath = NULL;
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
	Preload();
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
#ifdef PORT_DISABLE
	if (Game::IsIl2Cpp)
		Il2CppAssemblyGenerator::Cleanup();
#endif
	Debug::Msg(("Return Value = " + std::to_string(returnval)).c_str());
	if (Debug::Enabled)
		Logger::WriteSpacer();

	return (returnval == 0);
}

void BaseAssembly::Preload()
{
	SetupPaths();

	if (Game::IsIl2Cpp || !Mono::IsOldMono)
		return;

	if (!Game::IsIl2Cpp && !Core::FileExists(PreloadPath))
	{
		Assertion::ThrowInternalFailure("Preload.dll Does Not Exist!");
		return;
	}

	Debug::Msg("Initializing Preload Assembly...");
	Mono::Assembly* assembly = Mono::Exports::mono_domain_assembly_open(Mono::domain, PreloadPath);
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
