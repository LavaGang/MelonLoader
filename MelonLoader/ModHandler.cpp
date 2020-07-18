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
MonoMethod* ModHandler::runWarningCallbacks1 = NULL;
MonoMethod* ModHandler::runWarningCallbacks2 = NULL;
MonoMethod* ModHandler::runErrorCallbacks1 = NULL;
MonoMethod* ModHandler::runErrorCallbacks2 = NULL;

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
								runWarningCallbacks1 = Mono::mono_class_get_method_from_name(klass2, "RunWarningCallbacks", 1);
								AssertionManager::Decide(runWarningCallbacks1, "RunWarningCallbacks (1)");
								runWarningCallbacks2 = Mono::mono_class_get_method_from_name(klass2, "RunWarningCallbacks", 2);
								AssertionManager::Decide(runWarningCallbacks2, "RunWarningCallbacks (2)");
								runErrorCallbacks1 = Mono::mono_class_get_method_from_name(klass2, "RunErrorCallbacks", 1);
								AssertionManager::Decide(runErrorCallbacks1, "RunErrorCallbacks (1)");
								runErrorCallbacks2 = Mono::mono_class_get_method_from_name(klass2, "RunErrorCallbacks", 2);
								AssertionManager::Decide(runErrorCallbacks2, "RunErrorCallbacks (2)");
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

void ModHandler::RunWarningCallbacks(const char* msg)
{
	if (runWarningCallbacks1 != NULL)
	{
		MonoString* msgstr = Mono::mono_string_new(Mono::Domain, msg);
		void* args[1] = { msgstr };
		MonoObject* exceptionObject = NULL;
		Mono::mono_runtime_invoke(runWarningCallbacks1, NULL, args, &exceptionObject);
		if ((exceptionObject != NULL) && MelonLoader::DebugMode)
			Mono::LogExceptionMessage(exceptionObject);
	}
}

void ModHandler::RunWarningCallbacks(const char* namesection, const char* msg)
{
	if (runWarningCallbacks2 != NULL)
	{
		MonoString* msgstr = Mono::mono_string_new(Mono::Domain, msg);
		MonoString* namesectionstr = Mono::mono_string_new(Mono::Domain, namesection);
		void* args[2] = { namesectionstr, msgstr };
		MonoObject* exceptionObject = NULL;
		Mono::mono_runtime_invoke(runWarningCallbacks2, NULL, args, &exceptionObject);
		if ((exceptionObject != NULL) && MelonLoader::DebugMode)
			Mono::LogExceptionMessage(exceptionObject);
	}
}

void ModHandler::RunErrorCallbacks(const char* msg)
{
	if (runErrorCallbacks1 != NULL)
	{
		MonoString* msgstr = Mono::mono_string_new(Mono::Domain, msg);
		void* args[1] = { msgstr };
		MonoObject* exceptionObject = NULL;
		Mono::mono_runtime_invoke(runErrorCallbacks1, NULL, args, &exceptionObject);
		if ((exceptionObject != NULL) && MelonLoader::DebugMode)
			Mono::LogExceptionMessage(exceptionObject);
	}
}

void ModHandler::RunErrorCallbacks(const char* namesection, const char* msg)
{
	if (runErrorCallbacks2 != NULL)
	{
		MonoString* msgstr = Mono::mono_string_new(Mono::Domain, msg);
		MonoString* namesectionstr = Mono::mono_string_new(Mono::Domain, namesection);
		void* args[2] = { namesectionstr, msgstr };
		MonoObject* exceptionObject = NULL;
		Mono::mono_runtime_invoke(runErrorCallbacks2, NULL, args, &exceptionObject);
		if ((exceptionObject != NULL) && MelonLoader::DebugMode)
			Mono::LogExceptionMessage(exceptionObject);
	}
}