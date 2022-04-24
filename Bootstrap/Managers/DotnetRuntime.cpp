#include <Windows.h>
#include <string>

#include "DotnetRuntime.h"

#include "../Utils/Assertion.h"
#include "../Utils/Debug.h"
#include "../Core.h"
#include "Hook.h"
#include <iostream>

//Windows specific defines
#define STR(s) L ## s 
#define DIR_SEPARATOR L'\\'

void* load_library(const char_t* path)
{
	HMODULE h = ::LoadLibraryW(path);
	return (void*)h;
}
void* get_export(void* h, const char* name)
{
	void* f = ::GetProcAddress((HMODULE)h, name);
	if(f == nullptr)
		Assertion::ThrowInternalFailure((std::string("Failed to GetProcAddress ( ") + name + " )").c_str());
	return f;
}
//End Windows specific defines

void DotnetRuntime::GetDotNetLoadAssembly(const char_t* config_path)
{
	// Init .NET Runtime
	void* result_ptr = nullptr;
	hostfxr_handle cxt = nullptr;

	//Call the initialization function
	Debug::Msg("Calling init_fptr...");
	int rc = init_fptr(config_path, nullptr, &cxt);

	//Check for errors
	if (rc != 0 || cxt == nullptr)
	{
		close_fptr(cxt);

		if (rc == HOSTFXR_NO_FRAMEWORK)
			Assertion::ThrowInternalFailure("HostFXR returned no viable frameworks. Make sure you have .NET Runtime 6.0.3 installed for the correct architecture!");
		else 
			Assertion::ThrowInternalFailure((std::string("Dotnet Init failed. Return code: ") + std::to_string(rc) + " )").c_str());

		return;
	}

	Debug::Msg("Calling get_delegate_fptr to get load_assembly_and_get_function_pointer...");
	// Get the load assembly function pointer
	rc = get_delegate_fptr(
		cxt,
		hdt_load_assembly_and_get_function_pointer,
		&result_ptr);

	//Check for errors
	if (rc != 0 || result_ptr == nullptr) 
	{
		Assertion::ThrowInternalFailure((std::string("Dotnet: Get delegate failed. Return code: ") + std::to_string(rc) + " )").c_str());
	}

	//Save the pointer
	DotnetRuntime::load_assembly_and_get_function_pointer = (load_assembly_and_get_function_pointer_fn) result_ptr;

	Debug::Msg("Calling close_fptr...");
	close_fptr(cxt);
}

bool DotnetRuntime::LoadHostFxr()
{
	// Pre-allocate a large buffer for the path to hostfxr
	char_t buffer[MAX_PATH];
	size_t buffer_size = sizeof(buffer) / sizeof(char_t);

	int rc = get_hostfxr_path(buffer, &buffer_size, nullptr);
	if (rc != 0) 
	{
		if (rc == HOST_LOADLIB_FAILED)
			Assertion::ThrowInternalFailure("Failed to locate hostfxr - library load call failed. Wrong architecture dotnet installed?");
		else if(rc == HOST_MISSING_FILE)
			Assertion::ThrowInternalFailure("No .NET runtime installations were found. Please visit https://dot.net and download the latest .NET Runtime version 6");
		else if(rc == HOST_MISSING_ENTRYPOINT)
			Assertion::ThrowInternalFailure("Failed to locate hostfxr - could not find a required entry point. Outdated dotnet installation?");
		else
			Assertion::ThrowInternalFailure((std::string("Failed to locate hostfxr - unknown error, got HRESULT ") + std::to_string(rc)).c_str());

		return false;
	}

	std::wcout << L"Using hostfxr_path = " << buffer << L"." << std::endl;

	// Load hostfxr and get desired exports
	void* lib = load_library(buffer);

	int hr = GetLastError();
	if(lib == nullptr)
		Assertion::ThrowInternalFailure((std::string("Failed to LoadLibrary hostfxr! GetLastError() = ") + std::to_string(hr)).c_str());

	init_fptr = (hostfxr_initialize_for_runtime_config_fn)get_export(lib, "hostfxr_initialize_for_runtime_config");
	get_delegate_fptr = (hostfxr_get_runtime_delegate_fn)get_export(lib, "hostfxr_get_runtime_delegate");
	close_fptr = (hostfxr_close_fn)get_export(lib, "hostfxr_close");

	return (init_fptr && get_delegate_fptr && close_fptr);
}

bool DotnetRuntime::LoadDotNet() 
{
	//Because ML uses char* everywhere, and dotnet expects wchar*, we have to convert.
	std::string baseDir = std::string(Core::BasePath) + std::string("\\MelonLoader\\net6\\");

	size_t length = strlen(baseDir.c_str()) + 1;
	wchar_t* wc = new wchar_t[length];
	mbstowcs(wc, baseDir.c_str(), length);
	ml_net6_directory = wc;

	//Conversion done, load the runtime host itself. This will fail if the user doesn't have .net 6 installed.
	if (!LoadHostFxr())
	{
		Assertion::ThrowInternalFailure("Failed to initialize hostfxr!");
		return false;
	}

	Debug::Msg((std::string("HostFXR loaded. Using root_path = ") + baseDir).c_str());

	//Dotnet needs to load the runtime config json file
	const string_t config_path = ml_net6_directory + STR("MelonLoader.runtimeconfig.json");
	GetDotNetLoadAssembly(config_path.c_str());

	if (DotnetRuntime::load_assembly_and_get_function_pointer == nullptr) 
	{
		Assertion::ThrowInternalFailure("Failed to GetDotNetLoadAssembly!");
		return false;
	}

	Debug::Msg("Got DotNetLoadAssembly");

	return true;
}

void DotnetRuntime::CallInitialize()
{
	//Pointer for managed initialize function
	void(__stdcall * init)(host_imports*) = nullptr;

	const string_t ml_managed_path = ml_net6_directory + STR("MelonLoader.NativeHost.dll"); //Path to assembly
	const char_t* dotnet_type = STR("MelonLoader.NativeHost.NativeEntryPoint, MelonLoader.NativeHost"); //Assembly-Qualified name of the type to load
	const char_t* dotnet_type_method = STR("LoadStage1"); //Name of the function to load

	int rc = DotnetRuntime::load_assembly_and_get_function_pointer(
		ml_managed_path.c_str(), //Path
		dotnet_type, //AQN of type
		dotnet_type_method, //Method
		UNMANAGEDCALLERSONLY_METHOD, //Function type - in this case, an [UnmanagedCallersOnly] one
		nullptr, //Reserved
		(void**)&init //Result ptr
	); 

	if(rc != 0 || init == nullptr)
	{
		Assertion::ThrowInternalFailure((std::string("Failed to get MelonLoader.NativeHost.NativeEntryPoint.LoadStage1! RC = " + std::to_string(rc)).c_str()));
		return;
	}

	Debug::Msg("[Dotnet] Invoking Managed ML LoadStage1 Method...");

	//Now we have the pointer, call managed init method
	//This gives us the ptr to our own LoadAssemblyAndGetFuncPtr method, which actually uses the default assembly load context. Microsoft, WHY?
	init(&DotnetRuntime::imports);
	
	const char_t* dotnet_type_method_secondstage = STR("LoadStage2"); //Name of the function to load
	void(__stdcall * init2)(host_imports*, host_exports*) = nullptr;

	Debug::Msg("[Dotnet] Reloading NativeHost into correct load context and getting LoadStage2 pointer...");
	DotnetRuntime::imports.load_assembly_and_get_ptr(
		ml_managed_path.c_str(), //Path
		dotnet_type, //AQN of type
		dotnet_type_method_secondstage, //Method
		&init2
	);

	if (init2 == nullptr) 
	{
		Assertion::ThrowInternalFailure("Failed to get MelonLoader.NativeHost.NativeEntryPoint.LoadStage2!");
		return;
	}

	DotnetRuntime::exports.detour_attach = &Hook::Attach;
	DotnetRuntime::exports.detour_detach = &Hook::Detach;

	Debug::Msg("[Dotnet] Invoking LoadStage2 to get pointers for initialization functions in correct context...");
	init2(&DotnetRuntime::imports, &DotnetRuntime::exports);

	Debug::Msg("[Dotnet] Invoking Initialize...");
	DotnetRuntime::imports.initialize();

	//Save the returned functions
	DotnetRuntime::imports = imports;
}

void DotnetRuntime::CallPreStart()
{
	if (DotnetRuntime::imports.pre_start == nullptr)
	{
		Assertion::ThrowInternalFailure("Trying to call PreStart when we haven't called Initialize yet.");
		return;
	}

	DotnetRuntime::imports.pre_start();
}

void DotnetRuntime::CallStart()
{
	if (DotnetRuntime::imports.start == nullptr)
	{
		Assertion::ThrowInternalFailure("Trying to call Start when we haven't called Initialize yet.");
		return;
	}

	DotnetRuntime::imports.start();
}

extern "C" __declspec(dllexport) int dotnet_runtime_load_assembly_from_bytes(char* data, int length)
{
	return DotnetRuntime::imports.load_assembly_from_bytes(data, length);
}

extern "C" __declspec(dllexport) int dotnet_runtime_get_type(int assembly_id, const char_t* type_name)
{
	return DotnetRuntime::imports.get_type_from_assembly(assembly_id, type_name);
}

extern "C" __declspec(dllexport) int dotnet_runtime_construct_type(int type_id, int num_params, const char_t** param_types, void** param_values)
{
	return DotnetRuntime::imports.construct_type(type_id, num_params, param_types, param_values);
}

extern "C" __declspec(dllexport) int dotnet_runtime_invoke_method(int type_id, const char_t * method_name, int instance_id, int num_params, const char_t** param_types, void** param_values)
{
	return DotnetRuntime::imports.invoke_method(type_id, method_name, instance_id, num_params, param_types, param_values);
}

extern "C" __declspec(dllexport) void* dotnet_runtime_get_uco_method_pointer(int type_id, const char_t * method_name, int num_params, const char_t** param_types)
{
	return DotnetRuntime::imports.get_uco_method_pointer(type_id, method_name, num_params, param_types);
}

hostfxr_initialize_for_runtime_config_fn DotnetRuntime::init_fptr;
hostfxr_get_runtime_delegate_fn DotnetRuntime::get_delegate_fptr;
hostfxr_close_fn DotnetRuntime::close_fptr;
load_assembly_and_get_function_pointer_fn DotnetRuntime::load_assembly_and_get_function_pointer;
string_t DotnetRuntime::ml_net6_directory;
DotnetRuntime::host_imports DotnetRuntime::imports;
DotnetRuntime::host_exports DotnetRuntime::exports;