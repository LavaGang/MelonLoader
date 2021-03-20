#include "BaseAssembly.h"
#include "../Utils/Console/Debug.h"
#include "../Base/Core.h"
#include "Game.h"
#include <string>
#include "../Utils/Assertion.h"
#include "../Utils/Console/Logger.h"

char* BaseAssembly::Path = NULL;
Mono::Method* BaseAssembly::Mono_Start = NULL;

bool BaseAssembly::Initialize()
{
	Debug::Msg("Initializing Base Assembly...");
	Mono::Assembly* assembly = Mono::Exports::mono_domain_assembly_open(Mono::domain, Path);
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
	Mono::Exports::mono_runtime_invoke(Mono_Initialize, NULL, NULL, &exObj);
	if (exObj != NULL)
	{
		Mono::LogException(exObj);
		Assertion::ThrowInternalFailure("Failed to Invoke Initialize Method!");
		return false;
	}
	if (Debug::Enabled)
		Logger::WriteSpacer();
	return true;
}

void BaseAssembly::Start()
{
	if (Mono_Start == NULL)
		return;
	Debug::Msg("Starting Base Assembly...");
	Logger::WriteSpacer();
	Mono::Object* exObj = NULL;
	Mono::Exports::mono_runtime_invoke(Mono_Start, NULL, NULL, &exObj);
	if (exObj != NULL)
	{
		Mono::LogException(exObj);
		Assertion::ThrowInternalFailure("Failed to Invoke Start Method!");
	}
	if (Debug::Enabled)
		Logger::WriteSpacer();
}
