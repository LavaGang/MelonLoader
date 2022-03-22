#include <Windows.h>
#include <string>

#include "DotnetRuntime.h"

#include "../Utils/Assertion.h"
#include "../Utils/Debug.h"
#include "../Core.h"

using string_t = std::basic_string<char_t>;

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

load_assembly_and_get_function_pointer_fn DotnetRuntime::GetDotNetLoadAssembly(const char_t* config_path)
{
	// Load .NET Core
	void* load_assembly_and_get_function_pointer = nullptr;
	hostfxr_handle cxt = nullptr;
	int rc = init_fptr(config_path, nullptr, &cxt);
	if (rc != 0 || cxt == nullptr)
	{
		Assertion::ThrowInternalFailure((std::string("Dotnet Init failed. Return code: ") + std::to_string(rc) + " )").c_str());
		close_fptr(cxt);
		return nullptr;
	}

	// Get the load assembly function pointer
	rc = get_delegate_fptr(
		cxt,
		hdt_load_assembly_and_get_function_pointer,
		&load_assembly_and_get_function_pointer);
	if (rc != 0 || load_assembly_and_get_function_pointer == nullptr) 
	{
		Assertion::ThrowInternalFailure((std::string("Dotnet: Get delegate failed. Return code: ") + std::to_string(rc) + " )").c_str());
	}

	close_fptr(cxt);
	return (load_assembly_and_get_function_pointer_fn)load_assembly_and_get_function_pointer;
}

bool DotnetRuntime::LoadHostFxr()
{
	// Pre-allocate a large buffer for the path to hostfxr
	char_t buffer[MAX_PATH];
	size_t buffer_size = sizeof(buffer) / sizeof(char_t);
	int rc = get_hostfxr_path(buffer, &buffer_size, nullptr);
	if (rc != 0)
		return false;

	// Load hostfxr and get desired exports
	void* lib = load_library(buffer);

	if(lib == nullptr)
		Assertion::ThrowInternalFailure("Failed to LoadLibrary hostfxr!");

	init_fptr = (hostfxr_initialize_for_runtime_config_fn)get_export(lib, "hostfxr_initialize_for_runtime_config");
	get_delegate_fptr = (hostfxr_get_runtime_delegate_fn)get_export(lib, "hostfxr_get_runtime_delegate");
	close_fptr = (hostfxr_close_fn)get_export(lib, "hostfxr_close");

	return (init_fptr && get_delegate_fptr && close_fptr);
}

void DotnetRuntime::Initialize()
{
	std::string baseDir = std::string(Core::BasePath) + std::string("\\MelonLoader\\net6\\");

	size_t length = strlen(baseDir.c_str()) + 1;
	
	wchar_t* wc = new wchar_t[length];
	mbstowcs(wc, baseDir.c_str(), length);

	string_t root_path = wc;
	//auto pos = root_path.find_last_of(DIR_SEPARATOR);
	//assert(pos != string_t::npos);
	//root_path = root_path.substr(0, pos + 1);

	if (!LoadHostFxr()) 
	{
		Assertion::ThrowInternalFailure("Failed to initialize hostfxr!");
		return;
	}

	
	Debug::Msg((std::string("HostFXR loaded. Using root_path = ") + baseDir).c_str());

	const string_t config_path = root_path + STR("MelonLoader.runtimeconfig.json");
	load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer = nullptr;
	load_assembly_and_get_function_pointer = GetDotNetLoadAssembly(config_path.c_str());

	if(load_assembly_and_get_function_pointer == nullptr)
		Assertion::ThrowInternalFailure("Failed to GetDotNetLoadAssembly!");

	Debug::Msg("Got DotNetLoadAssembly");

	const string_t ml_managed_path = root_path + STR("MelonLoader.NativeHost.dll");
	const char_t* dotnet_type = STR("MelonLoader.NativeHost.NativeEntryPoint, MelonLoader.NativeHost");
	const char_t* dotnet_type_method = STR("Initialize");

	void(__stdcall *init)() = nullptr;
	int rc = load_assembly_and_get_function_pointer(
		ml_managed_path.c_str(),
		dotnet_type,
		dotnet_type_method,
		UNMANAGEDCALLERSONLY_METHOD /*delegate_type_name*/,
		nullptr,
		(void**)&init);

	if(rc != 0 || init == nullptr)
	{
		Assertion::ThrowInternalFailure((std::string("Failed to get MelonLoader.NativeHost.NativeEntryPoint.Initialize! RC = " + std::to_string(rc)).c_str()));
		return;
	}

	init();
}

hostfxr_initialize_for_runtime_config_fn DotnetRuntime::init_fptr;
hostfxr_get_runtime_delegate_fn DotnetRuntime::get_delegate_fptr;
hostfxr_close_fn DotnetRuntime::close_fptr;