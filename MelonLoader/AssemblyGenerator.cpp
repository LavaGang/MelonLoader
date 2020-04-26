#include "AssemblyGenerator.h"
#include "AssertionManager.h"
#include "MelonLoader.h"
#include "Mono.h"
#include "Logger.h"
#include <thread>
#include <stdio.h>
#include <atlstr.h>

bool AssemblyGenerator::Initialize()
{
	AssertionManager::Start("AssemblyGenerator.cpp", "AssemblyGenerator::Initialize");
	if (Mono::Domain != NULL)
	{
		std::string modhandlerpath = std::string(MelonLoader::GamePath) + "\\MelonLoader\\MelonLoader.AssemblyGenerator.dll";
		MonoAssembly* assembly = Mono::mono_domain_assembly_open(Mono::Domain, modhandlerpath.c_str());
		AssertionManager::Decide(assembly, "Assembly");
		if (assembly != NULL)
		{
			MonoImage* image = Mono::mono_assembly_get_image(assembly);
			AssertionManager::Decide(image, "Image");
			if (image != NULL)
			{
				MonoClass* klass = Mono::mono_class_from_name(image, "MelonLoader.AssemblyGenerator", "Main");
				AssertionManager::Decide(klass, "Class");
				if (klass != NULL)
				{
					MonoMethod* method = Mono::mono_class_get_method_from_name(klass, "Initialize", NULL);
					AssertionManager::Decide(method, "Method");
					if (method != NULL)
					{
						// Do Please Wait Message Here
						bool result = *(bool*)Mono::mono_object_unbox(Mono::mono_runtime_invoke(method, NULL, NULL, NULL));
						if (!result)
							MelonLoader::UNLOAD();
					}
				}
			}
		}
	}
	return !AssertionManager::Result;
}