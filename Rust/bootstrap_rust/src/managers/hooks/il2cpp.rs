//! il2cpp function hooks

use std::{ffi::c_char, mem::transmute};

use libc::c_void;
use thiserror::Error;

use crate::{managers::{il2cpp::{self}, exports::{il2cpp::{Il2CppDomain, Il2CppMethod, Il2CppObject, IL2CPP_INIT, IL2CPP_RUNTIME_INVOKE, il2cpp_method_get_name}}, detours, base_asm, game}, debug, internal_failure};

/// il2cpp hook errors
#[derive(Debug, Error)]
pub enum Il2CppHookError {
    /// Failed to get Mono Lib
    #[error("Failed to get Il2Cpp Lib!")]
    FailedToGetLib,
}

/// hook the init function
pub fn hook_init() -> Result<(), Box<dyn std::error::Error>> {
    debug!("Attaching Hook to il2cpp_init...")?;

    unsafe {
        let _ = detours::hook(
            IL2CPP_INIT.unwrap() as usize, 
            il2cpp_init_hook as usize,
        )?;
    }
    Ok(())
}

static mut RUNTIME_INVOKE_ORIG: Option<extern "C" fn(*mut Il2CppMethod, *mut Il2CppObject, *mut *mut c_void, *mut *mut Il2CppObject) -> *mut Il2CppObject> = None;

fn hook_runtime_invoke() -> Result<(), Box<dyn std::error::Error>> {
    debug!("Attaching Hook to il2cpp_runtime_invoke...")?;

    unsafe {
        let orig = detours::hook(
            IL2CPP_RUNTIME_INVOKE.unwrap() as usize, 
            il2cpp_runtime_invoke_hook as usize,
        )?;

        RUNTIME_INVOKE_ORIG = Some(transmute(orig));
    }
    Ok(())
}

#[no_mangle]
unsafe fn il2cpp_runtime_invoke_hook(method: *mut Il2CppMethod, obj: *mut Il2CppObject, params: *mut *mut c_void, exc: *mut *mut Il2CppObject) -> *mut Il2CppObject {
    let name = il2cpp_method_get_name(method).unwrap_or_else(|e| {
        internal_failure!("Failed to get method name: {}", e);
    });

    if name.contains("Internal_ActiveSceneChanged") {
        let _ = debug!("Detaching Hook from il2cpp_runtime_invoke...");

        let game_data = game::init()
        .unwrap_or_else(|e| internal_failure!("Failed to get game info: {}", e));

        let _ = detours::unhook(
            IL2CPP_RUNTIME_INVOKE.unwrap() as usize,
        ).unwrap_or_else(|e| {
            internal_failure!("Failed to unhook il2cpp_runtime_invoke: {}", e);
        });

        base_asm::pre_start(&game_data).unwrap_or_else(|e| {
            internal_failure!("Failed to prestart: {}", e);
        });

        base_asm::start(&game_data).unwrap_or_else(|e| {
            internal_failure!("Failed to start: {}", e);
        });
    }

    RUNTIME_INVOKE_ORIG.unwrap()(method, obj, params, exc)
}

#[no_mangle]
unsafe fn il2cpp_init_hook(name: *mut c_char) -> *mut Il2CppDomain {
    crate::utils::console::set_handles();

    let _ = debug!("Detaching Hook from il2cpp_init...");
    let _ = detours::unhook(IL2CPP_INIT.unwrap() as usize).unwrap_or_else(|e| {
        internal_failure!("Failed to unhook il2cpp_init: {}", e);
    });

    let game_data = game::init()
        .unwrap_or_else(|e| internal_failure!("Failed to get game info: {}", e));

    let il2cpp = il2cpp::init(&game_data).unwrap_or_else(|e| {
        internal_failure!("Failed to get il2cpp info: {}", e);
    });

    if il2cpp.is_none() {
        internal_failure!("Failed to get il2cpp info!");
    }

    base_asm::init(None, &game_data, Some(&il2cpp.unwrap())).unwrap_or_else(|e| {
        internal_failure!("Failed to init base assembly: {}", e);
    });

    hook_runtime_invoke().unwrap_or_else(|e| {
        internal_failure!("Failed to hook il2cpp_runtime_invoke: {}", e);
    });
    
    IL2CPP_INIT.unwrap()(name)
}