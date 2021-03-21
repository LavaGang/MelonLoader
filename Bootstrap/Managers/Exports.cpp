#ifdef PORT_DISABLE
#include "../Utils/Console/Logger.h"
#include "Il2Cpp.h"
#include "Game.h"
#include "Mono.h"
#include "../Utils/AssemblyGenerator.h"
#include "Hook.h"

#ifdef __ANDROID__
#define __export JNIEXPORT
#else
#define __export __declspec(dllexport)
#endif

#ifndef __stdcall
#define __stdcall __attribute__((__stdcall__))
#endif

extern "C"
{
	__export void __stdcall Msg(const char* txt) { Logger::Msg(txt); }
	__export void __stdcall Warning(const char* txt) { Logger::Warning(txt); }
	__export void __stdcall Error(const char* txt) { Logger::Error(txt); }
	__export const char* __stdcall GetGameAssemblyPath() { return Il2Cpp::GameAssemblyPath; }
	__export const char* __stdcall GetUnityVersion() { return Game::UnityVersion; }
	__export const char* __stdcall GetManagedDirectory() { return Mono::ManagedPath; }
	__export const char* __stdcall GetConfigDirectory() { return Mono::ConfigPath; }
	__export const char* __stdcall GetAssemblyGeneratorPath() { return AssemblyGenerator::Path; }
	__export const char* __stdcall ForceVersion_UnityDependencies() { return AssemblyGenerator::ForceVersion_UnityDependencies; }
	__export const char* __stdcall ForceVersion_Il2CppDumper() { return AssemblyGenerator::ForceVersion_Il2CppDumper; }
	__export const char* __stdcall ForceVersion_Il2CppAssemblyUnhollower() { return AssemblyGenerator::ForceVersion_Il2CppAssemblyUnhollower; }
	__export void __stdcall SetProcessId(int id) { AssemblyGenerator::ProcessId = id; }
}
#endif