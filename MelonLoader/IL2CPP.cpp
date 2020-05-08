#include "IL2CPP.h"
#include "AssertionManager.h"
#include "Mono.h"
#include "Logger.h"
#include "PointerUtils.h"

HMODULE IL2CPP::Module = NULL;
Il2CppDomain* IL2CPP::Domain = NULL;
il2cpp_init_t IL2CPP::il2cpp_init = NULL;
il2cpp_add_internal_call_t IL2CPP::il2cpp_add_internal_call = NULL;

bool IL2CPP::Setup()
{
	AssertionManager::Start("IL2CPP.cpp", "IL2CPP::Setup");

	il2cpp_init = (il2cpp_init_t)AssertionManager::GetExport(Module, "il2cpp_init");
	il2cpp_add_internal_call = (il2cpp_add_internal_call_t)AssertionManager::GetExport(Module, "il2cpp_add_internal_call");

	return !AssertionManager::Result;
}