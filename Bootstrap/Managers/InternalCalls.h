#pragma once
#include "Mono.h"
#include "../Utils/Console.h"

class InternalCalls
{
public:
	static void Initialize();

	class MelonUtils
	{
	public:
		static void AddInternalCalls();
		static bool IsGame32Bit();
		static Mono::String* GetManagedDirectory();
		static void* GetLibPtr();
		static void* GetRootDomainPtr();
		static Mono::ReflectionAssembly* CastManagedAssemblyPtr(void* ptr);
	};
};