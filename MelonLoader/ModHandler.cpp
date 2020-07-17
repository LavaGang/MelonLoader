#include <Windows.h>
#include <string>
#include "ModHandler.h"
#include "MelonLoader.h"
#include "AssertionManager.h"
#include "Logger.h"
#include "HookManager.h"
#include "Il2Cpp.h"

bool ModHandler::HasInitialized = false;
MonoMethod* ModHandler::onApplicationStart = NULL;
MonoMethod* ModHandler::onApplicationQuit = NULL;
MonoMethod* ModHandler::runLogCallbacks = NULL;
MonoMethod* ModHandler::runLogOverrideCallbacks = NULL;
MonoMethod* ModHandler::runWarningCallbacks = NULL;
MonoMethod* ModHandler::runWarningOverrideCallbacks = NULL;
MonoMethod* ModHandler::runErrorCallbacks = NULL;
MonoMethod* ModHandler::runErrorOverrideCallbacks = NULL;

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
					MonoClass* klass2 = Mono::mono_class_from_name(image, "MelonLoader", "Console");
					AssertionManager::Decide(assembly, "MelonLoader.Console");
					if (klass2 != NULL)
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
								runLogCallbacks = Mono::mono_class_get_method_from_name(klass2, "RunLogCallbacks", 1);
								AssertionManager::Decide(runLogCallbacks, "RunLogCallbacks");
								runLogOverrideCallbacks = Mono::mono_class_get_method_from_name(klass2, "RunLogOverrideCallbacks", 1);
								AssertionManager::Decide(runLogOverrideCallbacks, "RunLogOverrideCallbacks");
								runWarningCallbacks = Mono::mono_class_get_method_from_name(klass2, "RunWarningCallbacks", 1);
								AssertionManager::Decide(runWarningCallbacks, "RunWarningCallbacks");
								runWarningOverrideCallbacks = Mono::mono_class_get_method_from_name(klass2, "RunWarningOverrideCallbacks", 1);
								AssertionManager::Decide(runWarningOverrideCallbacks, "RunWarningOverrideCallbacks");
								runErrorCallbacks = Mono::mono_class_get_method_from_name(klass2, "RunErrorCallbacks", 1);
								AssertionManager::Decide(runErrorCallbacks, "RunErrorCallbacks");
								runErrorOverrideCallbacks = Mono::mono_class_get_method_from_name(klass2, "RunErrorOverrideCallbacks", 1);
								AssertionManager::Decide(runErrorOverrideCallbacks, "RunErrorOverrideCallbacks");
								if (MelonLoader::IsGameIl2Cpp)
									HookManager::Hook(&(LPVOID&)Il2Cpp::il2cpp_runtime_invoke, HookManager::Hooked_runtime_invoke);
								else
									HookManager::Hook(&(LPVOID&)Mono::mono_runtime_invoke, HookManager::Hooked_runtime_invoke);
								HasInitialized = true;
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
		if ((exceptionObject != NULL) && MelonLoader::DebugMode)
			Mono::LogExceptionMessage(exceptionObject);
	}
}

void ModHandler::OnApplicationQuit()
{
	if (onApplicationQuit != NULL)
	{
		MonoObject* exceptionObject = NULL;
		Mono::mono_runtime_invoke(onApplicationQuit, NULL, NULL, &exceptionObject);
		if ((exceptionObject != NULL) && MelonLoader::DebugMode)
			Mono::LogExceptionMessage(exceptionObject);
	}
}

void ModHandler::RunLogCallbacks(const char* msg)
{
	if (runLogCallbacks != NULL)
	{
		MonoString* msgstr = Mono::mono_string_new(Mono::Domain, msg);
		void* args[1] = { msgstr };
		MonoObject* exceptionObject = NULL;
		Mono::mono_runtime_invoke(runLogCallbacks, NULL, args, &exceptionObject);
		if ((exceptionObject != NULL) && MelonLoader::DebugMode)
			Mono::LogExceptionMessage(exceptionObject);
	}
}

const char* ModHandler::RunLogOverrideCallbacks(const char* msg)
{
	if (runLogCallbacks != NULL)
	{
		MonoString* msgstr = Mono::mono_string_new(Mono::Domain, msg);
		void* args[1] = { msgstr };
		MonoObject* exceptionObject = NULL;
		MonoObject* returnobj = Mono::mono_runtime_invoke(runLogCallbacks, NULL, args, &exceptionObject);
		if (exceptionObject != NULL)
		{
			if (MelonLoader::DebugMode)
				Mono::LogExceptionMessage(exceptionObject);
		}
		else
		{
			if (returnobj != NULL)
				return Mono::mono_string_to_utf8((MonoString*)returnobj);
		}
		return NULL;
	}
}

void ModHandler::RunWarningCallbacks(const char* msg)
{
	if (runWarningCallbacks != NULL)
	{
		MonoString* msgstr = Mono::mono_string_new(Mono::Domain, msg);
		void* args[1] = { msgstr };
		MonoObject* exceptionObject = NULL;
		Mono::mono_runtime_invoke(runWarningCallbacks, NULL, args, &exceptionObject);
		if ((exceptionObject != NULL) && MelonLoader::DebugMode)
			Mono::LogExceptionMessage(exceptionObject);
	}
}

const char* ModHandler::RunWarningOverrideCallbacks(const char* msg)
{
	if (runWarningOverrideCallbacks != NULL)
	{
		MonoString* msgstr = Mono::mono_string_new(Mono::Domain, msg);
		void* args[1] = { msgstr };
		MonoObject* exceptionObject = NULL;
		MonoObject* returnobj = Mono::mono_runtime_invoke(runWarningOverrideCallbacks, NULL, args, &exceptionObject);
		if (exceptionObject != NULL)
		{
			if (MelonLoader::DebugMode)
				Mono::LogExceptionMessage(exceptionObject);
		}
		else
		{
			if (returnobj != NULL)
				return Mono::mono_string_to_utf8((MonoString*)returnobj);
		}
		return NULL;
	}
}

void ModHandler::RunErrorCallbacks(const char* msg)
{
	if (runErrorCallbacks != NULL)
	{
		MonoString* msgstr = Mono::mono_string_new(Mono::Domain, msg);
		void* args[1] = { msgstr };
		MonoObject* exceptionObject = NULL;
		Mono::mono_runtime_invoke(runErrorCallbacks, NULL, args, &exceptionObject);
		if ((exceptionObject != NULL) && MelonLoader::DebugMode)
			Mono::LogExceptionMessage(exceptionObject);
	}
}

const char* ModHandler::RunErrorOverrideCallbacks(const char* msg)
{
	if (runErrorOverrideCallbacks != NULL)
	{
		MonoString* msgstr = Mono::mono_string_new(Mono::Domain, msg);
		void* args[1] = { msgstr };
		MonoObject* exceptionObject = NULL;
		MonoObject* returnobj = Mono::mono_runtime_invoke(runErrorOverrideCallbacks, NULL, args, &exceptionObject);
		if (exceptionObject != NULL)
		{
			if (MelonLoader::DebugMode)
				Mono::LogExceptionMessage(exceptionObject);
		}
		else
		{
			if (returnobj != NULL)
				return Mono::mono_string_to_utf8((MonoString*)returnobj);
		}
		return NULL;
	}
}