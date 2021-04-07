#include "InternalCalls.h"
#include "../Utils/Console/Debug.h"
#include "../Utils/Console/Logger.h"
#include "Game.h"
#include "Hook.h"
#include "../Utils/Assertion.h"
#include "../Base/Core.h"

#include "Il2Cpp.h"
#include "../Utils/Helpers/ImportLibHelper.h"

#include <dlfcn.h>

void InternalCalls::Initialize()
{
	Debug::Msg("Initializing Internal Calls...");
	MelonCore::AddInternalCalls();
	MelonLogger::AddInternalCalls();
	MelonUtils::AddInternalCalls();
	MelonHandler::AddInternalCalls();
	MelonDebug::AddInternalCalls();
    UnhollowerIl2Cpp::AddInternalCalls();
}

#pragma region MelonCore
bool InternalCalls::MelonCore::QuitFix() { return Core::QuitFix; }
void InternalCalls::MelonCore::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.External.Core::QuitFix", (void*)QuitFix);
}
#pragma endregion

#pragma region MelonLogger
void InternalCalls::MelonLogger::Internal_Msg(Console::Color color, Mono::String* namesection, Mono::String* txt)
{
	auto nsStr = namesection != NULL ? Mono::Exports::mono_string_to_utf8(namesection) : NULL;
	auto txtStr = Mono::Exports::mono_string_to_utf8(txt);
	Logger::Internal_Msg(color, nsStr, txtStr);
	if (nsStr != NULL) Mono::Free(nsStr);
	Mono::Free(txtStr);
}

void InternalCalls::MelonLogger::Internal_PrintModName(Console::Color color, Mono::String* name, Mono::String* version)
{
	auto nameStr = Mono::Exports::mono_string_to_utf8(name);
	auto versionStr = Mono::Exports::mono_string_to_utf8(version);
	Logger::Internal_PrintModName(color, nameStr, versionStr);
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
void InternalCalls::MelonLogger::Flush()
{
#ifdef PORT_DISABLE
	Logger::Flush();
	Console::Flush();
#endif
}
void InternalCalls::MelonLogger::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.External.Logger::Internal_PrintModName", (void*)Internal_PrintModName);
	Mono::AddInternalCall("MelonLoader.External.Logger::Internal_Msg", (void*)Internal_Msg);
	Mono::AddInternalCall("MelonLoader.External.Logger::Internal_Warning", (void*)Internal_Warning);
	Mono::AddInternalCall("MelonLoader.External.Logger::Internal_Error", (void*)Internal_Error);
	Mono::AddInternalCall("MelonLoader.External.Logger::ThrowInternalFailure", (void*)ThrowInternalFailure);
	Mono::AddInternalCall("MelonLoader.External.Logger::WriteSpacer", (void*)WriteSpacer);
	Mono::AddInternalCall("MelonLoader.External.Logger::Flush", (void*)Flush);
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

void InternalCalls::MelonUtils::SCT(Mono::String* title)
{
#ifdef _WIN32
	if (title == NULL) return;
	auto str = Mono::Exports::mono_string_to_utf8(title);
	Console::SetTitle(str);
	Mono::Free(str);
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
	Mono::AddInternalCall("MelonLoader.External.Utils::IsGame32Bit", (void*)IsGame32Bit);
	Mono::AddInternalCall("MelonLoader.External.Utils::IsGameIl2Cpp", (void*)IsGameIl2Cpp);
	Mono::AddInternalCall("MelonLoader.External.Utils::IsOldMono", (void*)IsOldMono);
	Mono::AddInternalCall("MelonLoader.External.Utils::GetApplicationPath", (void*)GetApplicationPath);
	Mono::AddInternalCall("MelonLoader.External.Utils::GetGameDataDirectory", (void*)GetGameDataDirectory);
	Mono::AddInternalCall("MelonLoader.External.Utils::GetUnityVersion", (void*)GetUnityVersion);
	Mono::AddInternalCall("MelonLoader.External.Utils::GetManagedDirectory", (void*)GetManagedDirectory);
	Mono::AddInternalCall("MelonLoader.External.Utils::SetConsoleTitle", (void*)SCT);
	Mono::AddInternalCall("MelonLoader.External.Utils::GetFileProductName", (void*)GetFileProductName);
	Mono::AddInternalCall("MelonLoader.External.Utils::NativeHookAttach", (void*)Hook::Attach);
	Mono::AddInternalCall("MelonLoader.External.Utils::NativeHookDetach", (void*)Hook::Detach);
	
	Mono::AddInternalCall("MelonLoader.External.Utils::Internal_GetGameName", (void*)GetGameName);
	Mono::AddInternalCall("MelonLoader.External.Utils::Internal_GetGameDeveloper", (void*)GetGameDeveloper);
	Mono::AddInternalCall("MelonLoader.External.Utils::Internal_GetGameDirectory", (void*)GetGameDirectory);
}
#pragma endregion

#pragma region MelonHandler
InternalCalls::MelonHandler::LoadMode InternalCalls::MelonHandler::LoadModeForPlugins = InternalCalls::MelonHandler::LoadMode::NORMAL;
InternalCalls::MelonHandler::LoadMode InternalCalls::MelonHandler::LoadModeForMods = InternalCalls::MelonHandler::LoadMode::NORMAL;
InternalCalls::MelonHandler::LoadMode InternalCalls::MelonHandler::GetLoadModeForPlugins() { return LoadModeForPlugins; }
InternalCalls::MelonHandler::LoadMode InternalCalls::MelonHandler::GetLoadModeForMods() { return LoadModeForMods; }
void InternalCalls::MelonHandler::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.External.Handler::GetLoadModeForPlugins", (void*)GetLoadModeForPlugins);
	Mono::AddInternalCall("MelonLoader.External.Handler::GetLoadModeForMods", (void*)GetLoadModeForMods);
}
#pragma endregion

#pragma region MelonDebug
bool InternalCalls::MelonDebug::IsEnabled() { return Debug::Enabled; }
void InternalCalls::MelonDebug::Internal_Msg(Console::Color color, Mono::String* namesection, Mono::String* txt)
{
	auto nsStr = namesection != NULL ? Mono::Exports::mono_string_to_utf8(namesection) : NULL;
	auto txtStr = Mono::Exports::mono_string_to_utf8(txt);
	Debug::Internal_Msg(color, nsStr, txtStr);
	if (nsStr != NULL) Mono::Free(nsStr);
	Mono::Free(txtStr);
}
void InternalCalls::MelonDebug::AddInternalCalls()
{
	Mono::AddInternalCall("MelonLoader.External.Debug::IsEnabled", (void*)IsEnabled);
	Mono::AddInternalCall("MelonLoader.External.Debug::Internal_Msg", (void*)Internal_Msg);
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
        //"il2cpp_type",
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
        Mono::AddInternalCall((std::string("UnhollowerBaseLib.IL2CPP::") + il2cppCalls[i]).c_str(), resolvedFunc);

    }
}
#pragma endregion