#include "Il2Cpp.h"
#include "AssertionManager.h"
#include "Mono.h"
#include "Logger.h"
#include "PointerUtils.h"

Il2CppDomain* Il2Cpp::Domain = NULL;
il2cpp_init_t Il2Cpp::il2cpp_init = NULL;

bool Il2Cpp::Setup(HMODULE mod)
{
	AssertionManager::Start("Il2Cpp.cpp", "Il2Cpp::Setup");

	il2cpp_init = (il2cpp_init_t)AssertionManager::GetExport(mod, "il2cpp_init");

	return !AssertionManager::Result;
}