use std::{ffi::{c_char, c_void, CStr}, mem::transmute, error};

#[derive(Debug, Error)]
pub enum HookError {
    #[error("Failed to get mono_jit_init_version")]
    MonoJitInitVersion,
    #[error("Failed to get mono_runtime_invoke")]
    MonoRuntimeInvoke,
    #[error("Failed to get il2cpp_method_get_name")]
    Il2CppMethodGetName,
    #[error("Failed to get il2cpp_runtime_invoke")]
    Il2CppRuntimeInvoke,
    #[error("Failed to hook function")]
    HookFunction,
}

use dobby_rs::Address;
use libc::{freopen, FILE};
use thiserror::Error;
use unity_rs::{mono::{types::{MonoDomain, MonoMethod, MonoObject}, Mono}, il2cpp::{types::{Il2CppDomain, Il2CppObject, Il2CppMethod}, Il2Cpp}, runtime::{UnityRuntime, Runtime}};

use crate::{internal_failure, debug, utils::debug, managers::{internal_calls, base_asm, hooks}, cstr};

static mut MONO_JIT_INIT_ORIGINAL: Option<Address> = None;
static mut MONO_RUNTIME_INVOKE_ORIGINAL: Option<Address> = None;

static mut IL2CPP_INIT_ORIGINAL: Option<Address> = None;
static mut IL2CPP_RUNTIME_INVOKE_ORIGINAL: Option<Address> = None;

pub fn init(runtime: UnityRuntime) -> Result<(), HookError> {
    match runtime {
        UnityRuntime::MonoRuntime(mono) => unsafe {
            debug!("Attaching hook to mono_jit_init_version");
            let func = mono.exports.mono_jit_init_version.ok_or(HookError::MonoJitInitVersion)?;

            let trampoline = dobby_rs::hook(*func as usize as Address, mono_jit_init_version_detour as usize as Address).map_err(|_| HookError::HookFunction)?;

            MONO_JIT_INIT_ORIGINAL = Some(trampoline);

            Ok(())
        },
        UnityRuntime::Il2Cpp(il2cpp) => unsafe {
            debug!("Attaching hook to il2cpp_init");
            let func = il2cpp.exports.il2cpp_init.ok_or(HookError::MonoJitInitVersion)?;

            let trampoline = dobby_rs::hook(*func as usize as Address, il2cpp_init_detour as usize as Address).map_err(|_| HookError::HookFunction)?;

            IL2CPP_INIT_ORIGINAL = Some(trampoline);

            Ok(())
        },
    }
}

fn invoke(runtime: UnityRuntime) -> Result<(), HookError> {
    match runtime {
        UnityRuntime::MonoRuntime(mono) => unsafe {
            let func = mono.exports.mono_runtime_invoke.ok_or(HookError::MonoRuntimeInvoke)?;

            let trampoline = dobby_rs::hook(*func as usize as Address, mono_runtime_invoke_detour as usize as Address).map_err(|_| HookError::HookFunction)?;
            
            MONO_RUNTIME_INVOKE_ORIGINAL = Some(trampoline);

            Ok(())
        },
        UnityRuntime::Il2Cpp(il2cpp) => unsafe {
            let func = il2cpp.exports.il2cpp_runtime_invoke.ok_or(HookError::MonoRuntimeInvoke)?;

            let trampoline = dobby_rs::hook(*func as usize as Address, il2cpp_runtime_invoke_detour as usize as Address).map_err(|_| HookError::HookFunction)?;

            IL2CPP_RUNTIME_INVOKE_ORIGINAL = Some(trampoline);
            Ok(())
        },
    }
}

fn mono_jit_init_version_detour(name: *const c_char, version: *const c_char) -> *mut MonoDomain {
    mono_jit_init_version_detour_inner(name, version).unwrap_or_else(|e| {
        internal_failure!("mono_jit_init_version detour failed: {e}");
    })
}

fn mono_runtime_invoke_detour(method: *mut MonoMethod, obj: *mut MonoObject, params: *mut *mut c_void, exc: *mut *mut MonoObject) -> *mut MonoObject {
    mono_runtime_invoke_detour_inner(method, obj, params, exc).unwrap_or_else(|e| {
        internal_failure!("mono_runtime_invoke detour failed: {e}");
    })
}


fn il2cpp_init_detour(name: *const c_char) -> *mut Il2CppDomain {
    il2cpp_init_detour_inner(name).unwrap_or_else(|e| {
        internal_failure!("il2cpp_init detour failed: {e}");
    })
}

fn il2cpp_runtime_invoke_detour(method: *mut Il2CppMethod, obj: *mut Il2CppObject, params: *mut *mut c_void, exc: *mut *mut Il2CppObject) -> *mut Il2CppObject {
    il2cpp_runtime_invoke_detour_inner(method, obj, params, exc).unwrap_or_else(|e| {
        internal_failure!("il2cpp_runtime_invoke detour failed: {e}");
    })
}


fn mono_jit_init_version_detour_inner(name: *const c_char, version: *const c_char) -> Result<*mut MonoDomain, Box<dyn std::error::Error>> {
    let trampoline: fn(*const c_char, *const c_char) -> *mut MonoDomain = unsafe {
        transmute(MONO_JIT_INIT_ORIGINAL.ok_or_else(|| "mono_jit_init_version trampoline not found")?)
    };

    let domain = trampoline(name, version);

    let runtime = Runtime::new()?;
    
    if let UnityRuntime::MonoRuntime(mono) = &runtime.runtime{
        debug!("Detaching hook from mono_jit_init_version");
        let func = mono.exports.clone();
        let func = func.mono_jit_init_version.ok_or(HookError::MonoJitInitVersion)?;

        unsafe {
            dobby_rs::unhook(*func as usize as Address)?;
        }

        if debug::enabled() {
            debug!("Creating Mono Debug Domain");
            if let Some(mono_debug_domain_create) = &mono.exports.mono_debug_domain_create {
                mono_debug_domain_create(domain);
            }
        }

        debug!("Setting Mono Main Thread");
        let thread = runtime.get_current_thread()?;
        mono.thread_set_main(thread)?;

        if !mono.is_old {
            if let Some(mono_domain_set_config) = &mono.exports.mono_domain_set_config {
                let base_dir = std::env::current_dir()?;
                let base_dir = base_dir.to_str().ok_or("Failed to convert base dir to string")?;
                let base_dir = std::ffi::CString::new(base_dir)?;

                debug!("Setting Mono Config");
                mono_domain_set_config(domain, base_dir.as_ptr(), name);
            }
        }

        internal_calls::init(mono.to_owned())?;
        base_asm::init()?;

        debug!("attaching hook to mono_runtime_invoke");
        invoke(runtime.runtime)?;
    }

    

    Ok(domain)
}

fn mono_runtime_invoke_detour_inner(method: *mut MonoMethod, obj: *mut MonoObject, params: *mut *mut c_void, exc: *mut *mut MonoObject) -> Result<*mut MonoObject, Box<dyn std::error::Error>> {
    let trampoline: fn(*mut MonoMethod, *mut MonoObject, *mut *mut c_void, *mut *mut MonoObject) -> *mut MonoObject = unsafe {
        transmute(MONO_RUNTIME_INVOKE_ORIGINAL.ok_or("mono_runtime_invoke trampoline not found")?)
    };

    let result = trampoline(method, obj, params, exc);

    let name = MonoMethod::get_name(method)?;

    let runtime = Runtime::new()?;
    let mono = match &runtime.runtime {
        UnityRuntime::MonoRuntime(mono) => mono,
        _ => return Ok(result),
    };

    if (name.contains("Internal_ActiveSceneChanged") || name.contains("UnityEngine.ISerializationCallbackReceiver.OnAfterSerialize")) ||
        (mono.is_old && (name.contains("Awake") || name.contains("DoSendMouseEvents"))) {
            debug!("Detaching hook from mono_runtime_invoke")?;

            let func = mono.exports.clone().mono_runtime_invoke.ok_or(HookError::MonoRuntimeInvoke)?;
            unsafe {
                dobby_rs::unhook(*func as usize as Address)?;
            }

            base_asm::pre_start()?;
            base_asm::start()?;
        }

    Ok(result)
}

fn il2cpp_init_detour_inner(name: *const c_char) -> Result<*mut Il2CppDomain, Box<dyn error::Error>> {
    let trampoline: fn(*const c_char) -> *mut Il2CppDomain = unsafe {
        transmute(IL2CPP_INIT_ORIGINAL.ok_or("il2cpp_init trampoline not found")?)
    };

    let domain = trampoline(name);

    let runtime = Runtime::new()?;

    if let UnityRuntime::Il2Cpp(il2cpp) = &runtime.runtime {
        debug!("Detaching hook from il2cpp_init");
        let func = il2cpp.exports.clone().il2cpp_init.ok_or(HookError::MonoJitInitVersion)?;

        unsafe {
            dobby_rs::unhook(*func as usize as Address)?;
        }

        base_asm::init()?;
        invoke(runtime.runtime)?;
    }

    Ok(domain)
}

fn il2cpp_runtime_invoke_detour_inner(method: *mut Il2CppMethod, obj: *mut Il2CppObject, params: *mut *mut c_void, exc: *mut *mut Il2CppObject) -> Result<*mut Il2CppObject, Box<dyn error::Error>> {
    let trampoline: fn(*mut Il2CppMethod, *mut Il2CppObject, *mut *mut c_void, *mut *mut Il2CppObject) -> *mut Il2CppObject = unsafe {
        transmute(IL2CPP_RUNTIME_INVOKE_ORIGINAL.ok_or("il2cpp_runtime_invoke trampoline not found")?)
    };

    let runtime = Runtime::new()?;

    if let UnityRuntime::Il2Cpp(il2cpp) = &runtime.runtime {
        let get_name = il2cpp.exports.clone().il2cpp_method_get_name.ok_or(HookError::Il2CppMethodGetName)?;

        let name = unsafe {
            let name = get_name(method);
            let name = CStr::from_ptr(name);
            name.to_str()?
        };

        if name.contains("Internal_ActiveSceneChanged") {
            debug!("Detaching hook from il2cpp_runtime_invoke")?;

            let func = il2cpp.exports.clone().il2cpp_runtime_invoke.ok_or(HookError::Il2CppRuntimeInvoke)?;
            unsafe {
                dobby_rs::unhook(*func as usize as Address)?;
            }

            base_asm::pre_start()?;
            base_asm::start()?;
        }
    }

    let result = trampoline(method, obj, params, exc);

    Ok(result)
}