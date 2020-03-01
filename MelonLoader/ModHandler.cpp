#include "ModHandler.h"
#include "MelonLoader.h"
#include <Windows.h>
#include <string>
#include "Console.h"

MonoMethod* ModHandler::onApplicationQuit = NULL;

void ModHandler::Initialize()
{
	if (Mono::Domain != NULL)
	{
		MonoAssembly* assembly = Mono::mono_domain_assembly_open(Mono::Domain, (std::string(MelonLoader::GamePath) + "\\MelonLoader\\MelonLoader.ModHandler.dll").c_str());
		if (assembly != NULL)
		{
			MonoImage* image = Mono::mono_assembly_get_image(assembly);
			if (image != NULL)
			{
				MonoClass* klass = Mono::mono_class_from_name(image, "MelonLoader", "Main");
				if (klass != NULL)
				{
					onApplicationQuit = Mono::mono_class_get_method_from_name(klass, "OnApplicationQuit", NULL);

					MonoMethod* method = Mono::mono_class_get_method_from_name(klass, "Initialize", NULL);
					if (method != NULL)
						Mono::mono_runtime_invoke(method, NULL, NULL, NULL);
				}
			}
		}
	}
}

void ModHandler::OnApplicationQuit()
{
	if (onApplicationQuit != NULL)
		Mono::mono_runtime_invoke(onApplicationQuit, NULL, NULL, NULL);
}