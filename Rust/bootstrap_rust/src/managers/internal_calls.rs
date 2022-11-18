//! internal calls that C# can make to the bootstrap
 
use std::mem::transmute;

use libc::c_void;

use crate::{debug, internal_failure, utils::libs};

use super::{exports::mono::{mono_add_internal_call, MonoString, mono_string_new, mono_get_root_domain, MonoReflectionAssembly}, mono, game, hooks, detours};

/// initializes internal_calls
pub fn init() -> Result<(), Box<dyn std::error::Error>> {
    debug!("Initializing Internal Calls...")?;

    mono_add_internal_call("MelonLoader.MelonUtils::IsGame32Bit", is_32_bit as usize as *mut c_void)?;
    mono_add_internal_call("MelonLoader.BootstrapInterop::NativeHookAttach", hook as usize as *mut c_void)?;
    mono_add_internal_call("MelonLoader.BootstrapInterop::NativeHookDetach", unhook as usize as *mut c_void)?;
    mono_add_internal_call("MelonLoader.MonoInternals.MonoLibrary::GetLibPtr", get_lib_ptr as usize as *mut c_void)?;
    mono_add_internal_call("MelonLoader.MonoInternals.MonoLibrary::CastManagedAssemblyPtr", cast_assembly_ptr as usize as *mut c_void)?;
    mono_add_internal_call("MelonLoader.MonoInternals.ResolveInternals.AssemblyManager::InstallHooks", install_hooks as usize as *mut c_void)?;
    mono_add_internal_call("MelonLoader.MonoInternals.MonoLibrary::GetRootDomainPtr", get_domain as usize as *mut c_void)?;
    mono_add_internal_call("MelonLoader.Support.Preload::GetManagedDirectory", get_managed_dir as usize as *mut c_void)?;

    Ok(())
}

fn get_managed_dir() -> *mut MonoString {
    let game_data = game::init()
        .unwrap_or_else(|e| internal_failure!("Failed to get game info: {}", e));

    let mono = mono::init( &game_data.base_path, &game_data.data_path, game_data.il2cpp)
        .unwrap_or_else(|e| internal_failure!("Failed to init mono: {}", e))
        .unwrap_or_else(|| internal_failure!("Failed to init mono"));

    let managed_dir = mono.managed_path
        .to_str()
        .unwrap_or_else(|| internal_failure!("Failed to get managed path as str"));

    let managed_dir_mono = mono_string_new(managed_dir)
        .unwrap_or_else(|e| internal_failure!("Failed to get managed path as MonoString: {}", e));

    managed_dir_mono
}

fn is_32_bit() -> bool {
    cfg!(target_pointer_width = "32")
}

#[allow(unused_variables)]
unsafe fn hook(target: *mut *mut c_void, detour: *mut c_void) -> *mut c_void {
    let res = detours::hook(*target as usize, detour as usize)
        .unwrap_or_else(|e| internal_failure!("Failed to hook: {}", e));

    transmute::<&(), fn()>(res) as usize as *mut c_void
}

#[allow(unused_variables)]
unsafe fn unhook(target: *mut *mut c_void, _detour: *mut c_void) {
    let _ = detours::unhook(*target as usize)
        .unwrap_or_else(|e| internal_failure!("Failed to unhook: {}", e));


}

unsafe fn get_lib_ptr() -> *mut c_void {
    let game_data = game::init()
        .unwrap_or_else(|e| internal_failure!("Failed to get game info: {}", e));

    let mono = mono::init( &game_data.base_path, &game_data.data_path, game_data.il2cpp)
        .unwrap_or_else(|e| internal_failure!("Failed to init mono: {}", e))
        .unwrap_or_else(|| internal_failure!("Failed to init mono"));

    libs::load_lib(&mono.lib_path)
        .unwrap_or_else(|e| internal_failure!("Failed to load mono lib: {}", e))
        .handle
}

unsafe fn get_domain() -> *mut c_void {
    let d = mono_get_root_domain()
        .unwrap_or_else(|e| internal_failure!("Failed to get root domain: {}", e));

    d as *mut c_void
}

unsafe fn cast_assembly_ptr(ptr: *mut c_void) -> *mut MonoReflectionAssembly {
    ptr.cast()
}

unsafe fn install_hooks() {
    let _ = debug!("Installing Hooks...");
    hooks::mono::install_assembly_hooks().unwrap_or_else(|e| internal_failure!("Failed to install assembly hooks: {}", e));
}