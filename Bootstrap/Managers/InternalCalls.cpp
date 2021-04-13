#include "InternalCalls.h"
#include "../Utils/Console/Debug.h"
#include "../Utils/Console/Logger.h"
#include "Game.h"
#include "Hook.h"
#include "../Utils/Assertion.h"
#include "../Base/Core.h"

#include "Il2Cpp.h"
#include "../Utils/Helpers/ImportLibHelper.h"
#include "sys/mman.h"
#include "stdlib.h"
#include "../Utils/AssemblyUnhollower/XrefScannerBindings.h"
#include <android/log.h>


#include <dlfcn.h>

void InternalCalls::Initialize()
{
	Debug::Msg("Initializing Internal Calls...");
    MelonLogger::AddInternalCalls();
    MelonUtils::AddInternalCalls();
    MelonDebug::AddInternalCalls();
    SupportModules::AddInternalCalls();
    UnhollowerIl2Cpp::AddInternalCalls();
}

#pragma region MelonLogger
void InternalCalls::MelonLogger::Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, Mono::String* namesection, Mono::String* txt)
{
    auto nsStr = namesection != NULL ? Mono::Exports::mono_string_to_utf8(namesection) : NULL;
    auto txtStr = Mono::Exports::mono_string_to_utf8(txt);
    Logger::Internal_Msg(meloncolor, txtcolor, nsStr, txtStr);
    if (nsStr != NULL) Mono::Free(nsStr);
    Mono::Free(txtStr);
}

void InternalCalls::MelonLogger::Internal_PrintModName(Console::Color meloncolor, Mono::String* name, Mono::String* version)
{
    auto nameStr = Mono::Exports::mono_string_to_utf8(name);
    auto versionStr = Mono::Exports::mono_string_to_utf8(version);
    Logger::Internal_PrintModName(meloncolor, nameStr, versionStr);
    Mono::Free(nameStr);
    Mono::Free(versionStr);
}

void InternalCalls::MelonLogger::Internal_Warning(Mono::String* namesection, Mono::String* txt)
{
    auto nsStr = namesection != NULL ? Mono::Exports::mono_string_to_utf8(namesection) : NULL;
    auto txtStr = Mono::Exports::mono_string_to_utf8(txt);
    Logger::Internal_Warning(nsStr, txtStr);
    if (nsStr != NULL) Mono::Free(nsStr);
    Mono::Free(txtStr);
}

void InternalCalls::MelonLogger::Internal_Error(Mono::String* namesection, Mono::String* txt)
{
    auto nsStr = namesection != NULL ? Mono::Exports::mono_string_to_utf8(namesection) : NULL;
    auto txtStr = Mono::Exports::mono_string_to_utf8(txt);
    Logger::Internal_Error(nsStr, txtStr);
    if (nsStr != NULL) Mono::Free(nsStr);
    Mono::Free(txtStr);
}

void InternalCalls::MelonLogger::ThrowInternalFailure(Mono::String* msg)
{
    auto str = Mono::Exports::mono_string_to_utf8(msg);
    Assertion::ThrowInternalFailure(str);
    Mono::Free(str);
}

void InternalCalls::MelonLogger::WriteSpacer() { Logger::WriteSpacer(); }
void InternalCalls::MelonLogger::Flush() { 
#ifdef PORT_DISABLE
    Logger::Flush(); Console::Flush(); 
#endif
}
void InternalCalls::MelonLogger::AddInternalCalls()
{
    Mono::AddInternalCall("MelonLoader.MelonLogger::Internal_PrintModName", (void*)Internal_PrintModName);
    Mono::AddInternalCall("MelonLoader.MelonLogger::Internal_Msg", (void*)Internal_Msg);
    Mono::AddInternalCall("MelonLoader.MelonLogger::Internal_Warning", (void*)Internal_Warning);
    Mono::AddInternalCall("MelonLoader.MelonLogger::Internal_Error", (void*)Internal_Error);
    Mono::AddInternalCall("MelonLoader.MelonLogger::ThrowInternalFailure", (void*)ThrowInternalFailure);
    Mono::AddInternalCall("MelonLoader.MelonLogger::WriteSpacer", (void*)WriteSpacer);
    Mono::AddInternalCall("MelonLoader.MelonLogger::Flush", (void*)Flush);
}
#pragma endregion

#pragma region MelonUtils
bool InternalCalls::MelonUtils::IsGame32Bit()
{
#ifdef _WIN64
    return false;
#else
    return true;
#endif
}
bool InternalCalls::MelonUtils::IsGameIl2Cpp() { return Game::IsIl2Cpp; }
bool InternalCalls::MelonUtils::IsOldMono() { return Mono::IsOldMono; }
Mono::String* InternalCalls::MelonUtils::GetApplicationPath() { return Mono::Exports::mono_string_new(Mono::domain, Game::ApplicationPath); }
Mono::String* InternalCalls::MelonUtils::GetGameName() { return Mono::Exports::mono_string_new(Mono::domain, Game::Name); }
Mono::String* InternalCalls::MelonUtils::GetGameDeveloper() { return Mono::Exports::mono_string_new(Mono::domain, Game::Developer); }
Mono::String* InternalCalls::MelonUtils::GetGameDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Game::BasePath); }
Mono::String* InternalCalls::MelonUtils::GetGameDataDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Game::DataPath); }
Mono::String* InternalCalls::MelonUtils::GetUnityVersion() { return Mono::Exports::mono_string_new(Mono::domain, Game::UnityVersion); }
Mono::String* InternalCalls::MelonUtils::GetManagedDirectory() { return Mono::Exports::mono_string_new(Mono::domain, Mono::ManagedPath); }
#ifdef PORT_DISABLE
Mono::String* InternalCalls::MelonUtils::GetHashCode() { return Mono::Exports::mono_string_new(Mono::domain, HashCode::Hash.c_str()); }
#else 
Mono::String* InternalCalls::MelonUtils::GetHashCode() { return Mono::Exports::mono_string_new(Mono::domain, "Placeholder Hash"); }
#endif
void InternalCalls::MelonUtils::SCT(Mono::String* title)
{
#ifdef PORT_DISABLE
    if (title == NULL) return;
    auto str = Mono::Exports::mono_string_to_utf8(title);
    Console::SetTitle(str);
    Mono::Free(str);
#else 
    return;
#endif
}
Mono::String* InternalCalls::MelonUtils::GetFileProductName(Mono::String* filepath)
{
    char* filepathstr = Mono::Exports::mono_string_to_utf8(filepath);
    if (filepathstr == NULL)
        return NULL;
    const char* info = Core::GetFileInfoProductName(filepathstr);
    Mono::Free(filepathstr);
    if (info == NULL)
        return NULL;
    return Mono::Exports::mono_string_new(Mono::domain, info);
}

void InternalCalls::MelonUtils::AddInternalCalls()
{
    Mono::AddInternalCall("MelonLoader.MelonUtils::IsGame32Bit", (void*)IsGame32Bit);
    Mono::AddInternalCall("MelonLoader.MelonUtils::IsGameIl2Cpp", (void*)IsGameIl2Cpp);
    Mono::AddInternalCall("MelonLoader.MelonUtils::IsOldMono", (void*)IsOldMono);
    Mono::AddInternalCall("MelonLoader.MelonUtils::GetApplicationPath", (void*)GetApplicationPath);
    Mono::AddInternalCall("MelonLoader.MelonUtils::GetGameDataDirectory", (void*)GetGameDataDirectory);
    Mono::AddInternalCall("MelonLoader.MelonUtils::GetUnityVersion", (void*)GetUnityVersion);
    Mono::AddInternalCall("MelonLoader.MelonUtils::GetManagedDirectory", (void*)GetManagedDirectory);
    Mono::AddInternalCall("MelonLoader.MelonUtils::SetConsoleTitle", (void*)SCT);
    Mono::AddInternalCall("MelonLoader.MelonUtils::GetFileProductName", (void*)GetFileProductName);
    Mono::AddInternalCall("MelonLoader.MelonUtils::NativeHookAttach", (void*)Hook::Attach);
    Mono::AddInternalCall("MelonLoader.MelonUtils::NativeHookDetach", (void*)Hook::Detach);

    Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetGameName", (void*)GetGameName);
    Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetGameDeveloper", (void*)GetGameDeveloper);
    Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetGameDirectory", (void*)GetGameDirectory);
    Mono::AddInternalCall("MelonLoader.MelonUtils::Internal_GetHashCode", (void*)GetHashCode);
}
#pragma endregion

#pragma region MelonDebug
void InternalCalls::MelonDebug::Internal_Msg(Console::Color meloncolor, Console::Color txtcolor, Mono::String* namesection, Mono::String* txt)
{
    auto nsStr = namesection != NULL ? Mono::Exports::mono_string_to_utf8(namesection) : NULL;
    auto txtStr = Mono::Exports::mono_string_to_utf8(txt);
    Debug::Internal_Msg(meloncolor, txtcolor, nsStr, txtStr);
    if (nsStr != NULL) Mono::Free(nsStr);
    Mono::Free(txtStr);
}
void InternalCalls::MelonDebug::AddInternalCalls()
{
    Mono::AddInternalCall("MelonLoader.MelonDebug::Internal_Msg", (void*)Internal_Msg);
}
#pragma endregion

#pragma region SupportModules
void InternalCalls::SupportModules::SetDefaultConsoleTitleWithGameName(Mono::String* GameVersion) { 
#ifdef PORT_DISABLE
    Console::SetDefaultTitleWithGameName(GameVersion != NULL ? Mono::Exports::mono_string_to_utf8(GameVersion) : NULL);
#endif
}
void InternalCalls::SupportModules::AddInternalCalls()
{
    Mono::AddInternalCall("MelonLoader.Support.Preload::GetManagedDirectory", (void*)MelonUtils::GetManagedDirectory);
    Mono::AddInternalCall("MelonLoader.Support.Main::SetDefaultConsoleTitleWithGameName", (void*)SetDefaultConsoleTitleWithGameName);
}
#pragma endregion

#pragma region UnhollowerIl2Cpp
void InternalCalls::UnhollowerIl2Cpp::AddInternalCalls()
{
	const char* il2cppCalls[] = {
        "il2cpp_init",
        "il2cpp_init_utf16",
        "il2cpp_shutdown",
        "il2cpp_set_config_dir",
        "il2cpp_set_data_dir",
        "il2cpp_set_temp_dir",
        "il2cpp_set_commandline_arguments",
        "il2cpp_set_commandline_arguments_utf16",
        "il2cpp_set_config_utf16",
        "il2cpp_set_config",
        "il2cpp_set_memory_callbacks",
        "il2cpp_get_corlib",
        "il2cpp_add_internal_call",
        "il2cpp_resolve_icall",
        "il2cpp_alloc",
        "il2cpp_free",
        "il2cpp_array_class_get",
        "il2cpp_array_length",
        "il2cpp_array_get_byte_length",
        "il2cpp_array_new",
        "il2cpp_array_new_specific",
        "il2cpp_array_new_full",
        "il2cpp_bounded_array_class_get",
        "il2cpp_array_element_size",
        "il2cpp_assembly_get_image",
        "il2cpp_class_enum_basetype",
        "il2cpp_class_is_generic",
        "il2cpp_class_is_inflated",
        "il2cpp_class_is_assignable_from",
        "il2cpp_class_is_subclass_of",
        "il2cpp_class_has_parent",
        "il2cpp_class_from_il2cpp_type",
        "il2cpp_class_from_name",
        "il2cpp_class_from_system_type",
        "il2cpp_class_get_element_class",
        "il2cpp_class_get_events",
        "il2cpp_class_get_fields",
        "il2cpp_class_get_nested_types",
        "il2cpp_class_get_interfaces",
        "il2cpp_class_get_properties",
        "il2cpp_class_get_property_from_name",
        "il2cpp_class_get_field_from_name",
        "il2cpp_class_get_methods",
        "il2cpp_class_get_method_from_name",
        "il2cpp_class_get_name",
        "il2cpp_class_get_namespace",
        "il2cpp_class_get_parent",
        "il2cpp_class_get_declaring_type",
        "il2cpp_class_instance_size",
        "il2cpp_class_num_fields",
        "il2cpp_class_is_valuetype",
        "il2cpp_class_value_size",
        "il2cpp_class_is_blittable",
        "il2cpp_class_get_flags",
        "il2cpp_class_is_abstract",
        "il2cpp_class_is_interface",
        "il2cpp_class_array_element_size",
        "il2cpp_class_from_type",
        "il2cpp_class_get_type",
        "il2cpp_class_get_type_token",
        "il2cpp_class_has_attribute",
        "il2cpp_class_has_references",
        "il2cpp_class_is_enum",
        "il2cpp_class_get_image",
        "il2cpp_class_get_assemblyname",
        "il2cpp_class_get_rank",
        "il2cpp_class_get_bitmap_size",
        "il2cpp_class_get_bitmap",
        "il2cpp_stats_dump_to_file",
        "il2cpp_domain_get",
        "il2cpp_domain_assembly_open",
        "il2cpp_domain_get_assemblies",
        "il2cpp_exception_from_name_msg",
        "il2cpp_get_exception_argument_null",
        "il2cpp_format_exception",
        "il2cpp_format_stack_trace",
        "il2cpp_unhandled_exception",
        "il2cpp_field_get_flags",
        "il2cpp_field_get_name",
        "il2cpp_field_get_parent",
        "il2cpp_field_get_offset",
        "il2cpp_field_get_type",
        "il2cpp_field_get_value",
        "il2cpp_field_get_value_object",
        "il2cpp_field_has_attribute",
        "il2cpp_field_set_value",
        "il2cpp_field_static_get_value",
        "il2cpp_field_static_set_value",
        "il2cpp_field_set_value_object",
        "il2cpp_gc_collect",
        "il2cpp_gc_collect_a_little",
        "il2cpp_gc_disable",
        "il2cpp_gc_enable",
        "il2cpp_gc_is_disabled",
        "il2cpp_gc_get_used_size",
        "il2cpp_gc_get_heap_size",
        "il2cpp_gc_wbarrier_set_field",
        "il2cpp_gchandle_new",
        "il2cpp_gchandle_new_weakref",
        "il2cpp_gchandle_get_target",
        "il2cpp_gchandle_free",
        "il2cpp_unity_liveness_calculation_begin",
        "il2cpp_unity_liveness_calculation_end",
        "il2cpp_unity_liveness_calculation_from_root",
        "il2cpp_unity_liveness_calculation_from_statics",
        "il2cpp_method_get_return_type",
        "il2cpp_method_get_declaring_type",
        "il2cpp_method_get_name",
        "il2cpp_method_get_from_reflection",
        "il2cpp_method_get_object",
        "il2cpp_method_is_generic",
        "il2cpp_method_is_inflated",
        "il2cpp_method_is_instance",
        "il2cpp_method_get_param_count",
        "il2cpp_method_get_param",
        "il2cpp_method_get_class",
        "il2cpp_method_has_attribute",
        "il2cpp_method_get_flags",
        "il2cpp_method_get_token",
        "il2cpp_method_get_param_name",
        "il2cpp_profiler_install",
        "il2cpp_profiler_install_enter_leave",
        "il2cpp_profiler_install_allocation",
        "il2cpp_profiler_install_gc",
        "il2cpp_profiler_install_fileio",
        "il2cpp_profiler_install_thread",
        "il2cpp_property_get_flags",
        "il2cpp_property_get_get_method",
        "il2cpp_property_get_set_method",
        "il2cpp_property_get_name",
        "il2cpp_property_get_parent",
        "il2cpp_object_get_class",
        "il2cpp_object_get_size",
        "il2cpp_object_get_virtual_method",
        "il2cpp_object_new",
        "il2cpp_object_unbox",
        "il2cpp_value_box",
        "il2cpp_monitor_enter",
        "il2cpp_monitor_try_enter",
        "il2cpp_monitor_exit",
        "il2cpp_monitor_pulse",
        "il2cpp_monitor_pulse_all",
        "il2cpp_monitor_wait",
        "il2cpp_monitor_try_wait",
        "il2cpp_runtime_invoke",
        "il2cpp_runtime_invoke_convert_args",
        "il2cpp_runtime_class_init",
        "il2cpp_runtime_object_init",
        "il2cpp_runtime_object_init_exception",
        "il2cpp_string_length",
        "il2cpp_string_chars",
        "il2cpp_string_new",
        "il2cpp_string_new_len",
        "il2cpp_string_new_utf16",
        "il2cpp_string_new_wrapper",
        "il2cpp_string_intern",
        "il2cpp_string_is_interned",
        "il2cpp_thread_current",
        "il2cpp_thread_attach",
        "il2cpp_thread_detach",
        "il2cpp_thread_get_all_attached_threads",
        "il2cpp_is_vm_thread",
        "il2cpp_current_thread_walk_frame_stack",
        "il2cpp_thread_walk_frame_stack",
        "il2cpp_current_thread_get_top_frame",
        "il2cpp_thread_get_top_frame",
        "il2cpp_current_thread_get_frame_at",
        "il2cpp_thread_get_frame_at",
        "il2cpp_current_thread_get_stack_depth",
        "il2cpp_thread_get_stack_depth",
        "il2cpp_type_get_object",
        "il2cpp_type_get_type",
        "il2cpp_type_get_class_or_element_class",
        "il2cpp_type_get_name",
        "il2cpp_type_is_byref",
        "il2cpp_type_get_attrs",
        "il2cpp_type_equals",
        "il2cpp_type_get_assembly_qualified_name",
        "il2cpp_image_get_assembly",
        "il2cpp_image_get_name",
        "il2cpp_image_get_filename",
        "il2cpp_image_get_entry_point",
        "il2cpp_image_get_class_count",
        "il2cpp_image_get_class",
        "il2cpp_capture_memory_snapshot",
        "il2cpp_free_captured_memory_snapshot",
        "il2cpp_set_find_plugin_callback",
        "il2cpp_register_log_callback",
        "il2cpp_debugger_set_agent_options",
        "il2cpp_is_debugger_attached",
        "il2cpp_unity_install_unitytls_interface",
        "il2cpp_custom_attrs_from_class",
        "il2cpp_custom_attrs_from_method",
        "il2cpp_custom_attrs_get_attr",
        "il2cpp_custom_attrs_has_attr",
        "il2cpp_custom_attrs_construct",
        "il2cpp_custom_attrs_free"
	};

    for (int i = 0; i < sizeof(il2cppCalls) / sizeof(il2cppCalls[0]); i++)
    {
        void* resolvedFunc = ImportLibHelper::GetExport(Il2Cpp::Handle, il2cppCalls[i]);

        if (!Assertion::ShouldContinue) {
            Logger::Error((std::string("Failed importing ") + il2cppCalls[i]).c_str());
            break;
        }
        Mono::AddInternalCall((std::string("UnhollowerBaseLib.IL2CPP::") + il2cppCalls[i]).c_str(), (void*)resolvedFunc);
    }

    Mono::AddInternalCall("UnhollowerRuntimeLib.ClassInjector::GetProcAddress", (void*)GetProcAddress);
    Mono::AddInternalCall("UnhollowerRuntimeLib.ClassInjector::LoadLibrary", (void*)LoadLibrary);

    Mono::AddInternalCall("UnhollowerRuntimeLib.XrefScans.XrefScanner::XrefScanImplNative", (void*)XrefScannerBindings::XrefScanner::XrefScanImplNative);

    Mono::AddInternalCall("UnhollowerRuntimeLib.XrefScans.XrefScannerLowLevel::JumpTargetsImpl", (void*)XrefScannerBindings::XrefScannerLowLevel::JumpTargetsImpl);

    Mono::AddInternalCall("UnhollowerRuntimeLib.XrefScans.XrefScanUtilFinder::FindLastRcxReadAddressBeforeCallTo", (void*)XrefScannerBindings::XrefScanUtilFinder::FindLastRcxReadAddressBeforeCallTo);
    Mono::AddInternalCall("UnhollowerRuntimeLib.XrefScans.XrefScanUtilFinder::FindByteWriteTargetRightAfterCallTo", (void*)XrefScannerBindings::XrefScanUtilFinder::FindByteWriteTargetRightAfterCallTo);
}

void* InternalCalls::UnhollowerIl2Cpp::GetProcAddress(void* hModule, Mono::String* procName)
{
    char* parsedSym = Mono::Exports::mono_string_to_utf8(procName);
    void* res = dlsym(hModule, parsedSym);
    Mono::Free(parsedSym);
    return res;
}

void* InternalCalls::UnhollowerIl2Cpp::LoadLibrary(Mono::String* lpFileName)
{
    char* parsedLib = Mono::Exports::mono_string_to_utf8(lpFileName);
    Debug::Msg(parsedLib);
    if (strcmp(parsedLib, "GameAssembly.dll") == 0)
    {
        __android_log_print(ANDROID_LOG_INFO, "MelonLoader", "c++ %p", Il2Cpp::MemLoc);

        //Dl_info dlInfo;
        //dladdr(Il2Cpp::Handle, &dlInfo);
        //Mono::Free(parsedLib);
        return Il2Cpp::MemLoc;
    }

    void* res = dlopen(parsedLib, RTLD_NOW | RTLD_GLOBAL);
    Dl_info dlInfo;
    dladdr(res, &dlInfo);
    Mono::Free(parsedLib);
    return dlInfo.dli_fbase;
}
#pragma endregion
