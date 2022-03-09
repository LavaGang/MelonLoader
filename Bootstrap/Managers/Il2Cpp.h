#pragma once
#ifdef _WIN32
#include <Windows.h>
#endif

#include "./il2cpp/Il2CppApi.h"

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
	static void* MemLoc;
	static const char* LibPath;
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

        static il2cpp_thread_get_all_attached_threads_t il2cpp_thread_get_all_attached_threads;
        static il2cpp_thread_attach_t il2cpp_thread_attach;
        static il2cpp_thread_detach_t il2cpp_thread_detach;
        static il2cpp_gc_set_mode_t il2cpp_gc_set_mode;
    };

private:
    static int SceneChanges;

    static void OnIl2cppReady();
    static void MonoThreadHandle();

	class Hooks
	{
	public:
		static Domain* il2cpp_init(const char* name);
		static Object* il2cpp_runtime_invoke(Method* method, Object* obj, void** params, Object** exec);
		static void il2cpp_unity_install_unitytls_interface(void* unitytlsInterfaceStruct);
        static Il2CppThread* on_il2cpp_thread_attach(Il2CppDomain * domain);
        static void on_il2cpp_thread_detach(Il2CppThread * thread);
        static void on_il2cpp_gc_set_mode(Il2CppGCMode mode);
	};
};
