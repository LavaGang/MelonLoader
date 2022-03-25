#pragma once
#include <coreclr_delegates.h>
#include <nethost.h>
#include <hostfxr.h>
#include <Windows.h>

using string_t = std::basic_string<char_t>;

typedef void (*host_delegate)();

class DotnetRuntime
{
public:
	static bool LoadDotNet();
	static void CallInitialize();
	static void CallPreStart();
	static void CallStart();
private:
	struct host_exports
	{
		void (*detour_attach)(void** target, void* detour);
		void (*detour_detach)(void** target, void* detour);
	};

	struct host_imports
	{
		void(*load_assembly_and_get_ptr)(const char_t*, const char_t*, const char_t*, void(__stdcall**)(host_imports*, host_exports*));

		host_delegate initialize;
		host_delegate pre_start;
		host_delegate start;
	};

	static hostfxr_initialize_for_runtime_config_fn init_fptr;
	static hostfxr_get_runtime_delegate_fn get_delegate_fptr;
	static hostfxr_close_fn close_fptr;
	static load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer;

	static string_t ml_net6_directory;

	static host_imports imports;
	static host_exports exports;

	static bool LoadHostFxr();
	static void GetDotNetLoadAssembly(const char_t* config_path);
};