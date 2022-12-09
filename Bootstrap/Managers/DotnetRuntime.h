#pragma once
#include <coreclr_delegates.h>
#include <hostfxr.h>
#include <string>

using string_t = std::basic_string<char_t>;

typedef void (*host_delegate)();

#define HOST_LOADLIB_FAILED 0x80008082
#define HOST_MISSING_FILE 0x80008083
#define HOST_MISSING_ENTRYPOINT 0x80008084
#define HOSTFXR_NO_FRAMEWORK 0x80008096

class DotnetRuntime
{
public:
    struct host_exports
    {
        void (*detour_attach)(void** target, void* detour);
        void (*detour_detach)(void** target, void* detour);
    };

    struct host_imports
    {
        void(*load_assembly_and_get_ptr)(const char_t*, const char_t*, const char_t*, void(__stdcall**)(host_imports*, host_exports*));

        int(*load_assembly_from_bytes)(char* data, int length);
        int(*get_type_from_assembly)(int assembly_id, const char_t* type_name);
        int(*construct_type)(int type_id, int num_params, const char_t** param_types, void** param_values);
        int(*invoke_method)(int type_id, const char_t* method_name, int instance_id, int num_params, const char_t** param_types, void** param_values);
        void*(*get_uco_method_pointer)(int type_id, const char_t* method_name, int num_params, const char_t** param_types);

        host_delegate initialize;
        host_delegate pre_start;
        host_delegate start;
    };

    static bool LoadDotNet();
    static void CallInitialize();
    static void CallPreStart();
    static void CallStart();

    static host_imports imports;
private:

    static hostfxr_initialize_for_runtime_config_fn init_fptr;
    static hostfxr_get_runtime_delegate_fn get_delegate_fptr;
    static hostfxr_close_fn close_fptr;
    static load_assembly_and_get_function_pointer_fn load_assembly_and_get_function_pointer;

    static string_t ml_net6_directory;

    static host_exports exports;

    static bool LoadHostFxr();
    static void GetDotNetLoadAssembly(const char_t* config_path);
};