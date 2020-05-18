#include <Windows.h>
#include <string>
#include "ModHandler.h"
#include "MelonLoader.h"
#include "AssertionManager.h"
#include "Logger.h"

MonoMethod* ModHandler::onApplicationStart = NULL;
MonoMethod* ModHandler::onApplicationQuit = NULL;
MonoMethod* ModHandler::melonCoroutines_ProcessWaitForEndOfFrame = NULL;

void ModHandler::Initialize()
{
	AssertionManager::Start("ModHandler.cpp", "ModHandler::Initialize");
	if (Mono::Domain != NULL)
	{
		std::string modhandlerpath = std::string(MelonLoader::GamePath) + "\\MelonLoader\\MelonLoader.ModHandler.dll";
		MonoAssembly* assembly = Mono::mono_domain_assembly_open(Mono::Domain, modhandlerpath.c_str());
		AssertionManager::Decide(assembly, "MelonLoader.ModHandler.dll");
		if (assembly != NULL)
		{
			MonoImage* image = Mono::mono_assembly_get_image(assembly);
			AssertionManager::Decide(assembly, "Image");
			if (image != NULL)
			{
				MonoClass* klass = Mono::mono_class_from_name(image, "MelonLoader", "Main");
				AssertionManager::Decide(assembly, "MelonLoader.Main");
				if (klass != NULL)
				{
					MonoMethod* initialize = Mono::mono_class_get_method_from_name(klass, "Initialize", NULL);
					AssertionManager::Decide(initialize, "Initialize");
					if (initialize != NULL)
					{
						MonoObject* exceptionObject = NULL;
						Mono::mono_runtime_invoke(initialize, NULL, NULL, &exceptionObject);
						if (exceptionObject && MelonLoader::DebugMode)
							Mono::LogExceptionMessage(exceptionObject);
						else
						{
							onApplicationStart = Mono::mono_class_get_method_from_name(klass, "OnApplicationStart", NULL);
							AssertionManager::Decide(onApplicationStart, "OnApplicationStart");

							onApplicationQuit = Mono::mono_class_get_method_from_name(klass, "OnApplicationQuit", NULL);
							AssertionManager::Decide(onApplicationQuit, "OnApplicationQuit");

							if (MelonLoader::IsGameIl2Cpp)
							{
								klass = Mono::mono_class_from_name(image, "MelonLoader", "MelonCoroutines");
								AssertionManager::Decide(assembly, "MelonLoader.MelonCoroutines");
								if (klass != NULL)
								{
									melonCoroutines_ProcessWaitForEndOfFrame = Mono::mono_class_get_method_from_name(klass, "ProcessWaitForEndOfFrame", NULL);
									AssertionManager::Decide(melonCoroutines_ProcessWaitForEndOfFrame, "ProcessWaitForEndOfFrame");
								}
							}
						}
					}
				}
			}
		}
	}
}

void ModHandler::OnApplicationStart()
{
	if (onApplicationStart != NULL)
	{
		MonoObject* exceptionObject = NULL;
		Mono::mono_runtime_invoke(onApplicationStart, NULL, NULL, &exceptionObject);
		if (exceptionObject && MelonLoader::DebugMode)
			Mono::LogExceptionMessage(exceptionObject);
	}
}

void ModHandler::OnApplicationQuit()
{
	if (onApplicationQuit != NULL)
	{
		MonoObject* exceptionObject = NULL;
		Mono::mono_runtime_invoke(onApplicationQuit, NULL, NULL, &exceptionObject);
		if (exceptionObject && MelonLoader::DebugMode)
			Mono::LogExceptionMessage(exceptionObject);
	}
}

void ModHandler::MelonCoroutines_ProcessWaitForEndOfFrame()
{
	if (melonCoroutines_ProcessWaitForEndOfFrame != NULL)
	{
		MonoObject* exceptionObject = NULL;
		Mono::mono_runtime_invoke(melonCoroutines_ProcessWaitForEndOfFrame, NULL, NULL, &exceptionObject);
		if (exceptionObject && MelonLoader::DebugMode)
			Mono::LogExceptionMessage(exceptionObject);
	}
}