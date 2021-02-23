#pragma once
#ifdef _WIN32
#include <Windows.h>
#endif
#include "../Patcher.h"

class Il2Cpp
{
public:
	struct Domain;
	struct Method;
	struct Object;

	static char* GameAssemblyPath;
	static Domain* domain;
	static void* UnityTLSInterfaceStruct;

#ifdef _WIN32
	static HMODULE Module;
#elif defined(__ANDROID__)
	static void* Handle;
#endif
	
	static bool Initialize();
	static bool ApplyPatches();

	class Exports
	{
	public:
		static bool Initialize();

		typedef Domain* (*il2cpp_init_t) (const char* name);
		static il2cpp_init_t il2cpp_init;
		typedef Object* (*il2cpp_runtime_invoke_t) (Method* method, Object* obj, void** params, Object** exec);
		static il2cpp_runtime_invoke_t il2cpp_runtime_invoke;
		typedef const char* (*il2cpp_method_get_name_t) (Method* method);
		static il2cpp_method_get_name_t il2cpp_method_get_name;
		typedef void (*il2cpp_unity_install_unitytls_interface_t) (void* unitytlsInterfaceStruct);
		static il2cpp_unity_install_unitytls_interface_t il2cpp_unity_install_unitytls_interface;

		typedef void (*testFnDef)(int);
		static testFnDef test_fn;
	};

	class Hooks
	{
	public:
		static Domain* il2cpp_init(const char* name);
		static Object* il2cpp_runtime_invoke(Method* method, Object* obj, void** params, Object** exec);
		static void il2cpp_unity_install_unitytls_interface(void* unitytlsInterfaceStruct);
		static void test_fn(int value);
	};
	
#ifdef __ANDROID__
	class Patches
	{
	public:
		static Patcher* test_fn;
	};
private:
	static bool ImportError;
	static void* GetExport(const char* name);
#endif
};
