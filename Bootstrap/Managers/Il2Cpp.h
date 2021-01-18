#pragma once
#include <Windows.h>


class Il2Cpp
{
public:
	struct Domain;
	struct Method;
	struct Object;

	static char* GameAssemblyPath;
	static HMODULE Module;
	static Domain* domain;
	static void* UnityTLSInterfaceStruct;
	static bool Initialize();

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
	};

	class Hooks
	{
	public:
		static Domain* il2cpp_init(const char* name);
		static Object* il2cpp_runtime_invoke(Method* method, Object* obj, void** params, Object** exec);
		static void il2cpp_unity_install_unitytls_interface(void* unitytlsInterfaceStruct);
	};
};