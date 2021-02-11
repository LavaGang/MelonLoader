#include "../Utils/Logger.h"
#include "Il2Cpp.h"
#include "Game.h"
#include "Mono.h"
#include "../Utils/AssemblyGenerator.h"
#include "Hook.h"

extern "C"
{
	__declspec(dllexport) void __stdcall Msg(const char* txt) { Logger::Msg(txt); }
	__declspec(dllexport) void __stdcall Warning(const char* txt) { Logger::Warning(txt); }
	__declspec(dllexport) void __stdcall Error(const char* txt) { Logger::Error(txt); }
	__declspec(dllexport) const char* __stdcall GetGameAssemblyPath() { return Il2Cpp::GameAssemblyPath; }
	__declspec(dllexport) const char* __stdcall GetUnityVersion() { return Game::UnityVersion; }
	_declspec(dllexport) const char* __stdcall GetGameName() { return Game::Name; }
	__declspec(dllexport) const char* __stdcall GetManagedDirectory() { return Mono::ManagedPath; }
	__declspec(dllexport) const char* __stdcall GetConfigDirectory() { return Mono::ConfigPath; }
	__declspec(dllexport) const char* __stdcall GetAssemblyGeneratorPath() { return AssemblyGenerator::Path; }
	__declspec(dllexport) bool __stdcall ForceRegeneration() { return AssemblyGenerator::ForceRegeneration; }
	__declspec(dllexport) const char* __stdcall ForceVersion_UnityDependencies() { return AssemblyGenerator::ForceVersion_UnityDependencies; }
	__declspec(dllexport) const char* __stdcall ForceVersion_Dumper() { return AssemblyGenerator::ForceVersion_Dumper; }
	__declspec(dllexport) const char* __stdcall ForceVersion_Il2CppAssemblyUnhollower() { return AssemblyGenerator::ForceVersion_Il2CppAssemblyUnhollower; }
	__declspec(dllexport) void __stdcall SetProcessId(int id) { AssemblyGenerator::ProcessId = id; }
}