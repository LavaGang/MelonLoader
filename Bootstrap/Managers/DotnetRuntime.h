#pragma once
#include <coreclr_delegates.h>
#include <nethost.h>
#include <hostfxr.h>

class DotnetRuntime
{
public:
	static void Initialize();
private:
	static hostfxr_initialize_for_runtime_config_fn init_fptr;
	static hostfxr_get_runtime_delegate_fn get_delegate_fptr;
	static hostfxr_close_fn close_fptr;

	static bool LoadHostFxr();
	static load_assembly_and_get_function_pointer_fn GetDotNetLoadAssembly(const char_t* config_path);
};