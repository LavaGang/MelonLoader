//! mono hooks
//! 
//! # Safety
//! this is incredibly unsafe

use std::{error, ffi::{c_char, c_void}, ptr, mem::transmute};


use thiserror::Error;

use crate::{managers::{exports::{mono::{MonoDomain, mono_jit_init_raw, MONO_LIB, MonoExportError, mono_debug_domain_create, mono_thread_set_main, mono_thread_current, mono_domain_set_config, MonoMethod, MonoObject, mono_method_get_name, mono_install_assembly_hook, AssemblyHookType, AssemblyName, MonoAssembly, mono_string_new_raw, mono_runtime_invoke, MonoReflectionAssembly, mono_assembly_get_object}}, mono::{self, MonoError, Mono}, core, game, internal_calls, base_asm, detours}, debug, internal_failure, utils::{debug}};

/// mono hook errors
#[derive(Debug, Error)]
pub enum HookError {
    /// Failed to get Mono Lib
    #[error("Failed to get Mono Lib!")]
    FailedToGetMonoLib,
}

/// hook mono_jit_init_version
pub fn hook_jit_init() -> Result<(), Box<dyn error::Error>> {
    debug!("Attaching hook to mono_jit_init_version")?;
    unsafe {

        let target = MONO_LIB
        .as_ref()
        .ok_or(HookError::FailedToGetMonoLib)?
        .get_fn_ptr("mono_jit_init_version")?;

        let _ = detours::hook(
            target as usize, 
            jit_init_hook as usize,
        )?;
    }

    Ok(())
}

static mut INVOKE_ORIG: Option<fn(*mut MonoMethod, *mut MonoObject, *mut *mut c_void, *mut *mut MonoObject) -> *mut MonoObject> = None;
static mut INVOKE_ORIG_PTR: usize = 0;

fn hook_runtime_invoke() -> Result<(), Box<dyn error::Error>> {
    unsafe {
        let target = MONO_LIB
        .as_ref()
        .ok_or(HookError::FailedToGetMonoLib)?
        .get_fn_ptr("mono_runtime_invoke")?;

        INVOKE_ORIG_PTR = target as usize;

        let orig = detours::hook(
            target as usize, 
            runtime_invoke_hook as usize,
        )?;

        INVOKE_ORIG = Some(transmute(orig));
    }

    Ok(())
}

#[no_mangle]
unsafe extern "C" fn runtime_invoke_hook(method: *mut MonoMethod, obj: *mut MonoObject, params: *mut *mut c_void, exception: *mut *mut MonoObject) -> *mut MonoObject {
    let function = INVOKE_ORIG.unwrap_or_else(|| {
        internal_failure!("Failed to pre start!");
    });

    let game_data = game::init()
        .unwrap_or_else(|e| internal_failure!("Failed to get game info: {}", e));

    let mono = get_mono()
        .unwrap_or_else(|e| internal_failure!("Failed to get mono: {}", e));

    let method_name = mono_method_get_name(method).unwrap_or_else(|e| {
        internal_failure!("Failed to get method name: {}", e);
    });

    if (method_name.contains("Internal_ActiveSceneChanged") || method_name.contains("UnityEngine.ISerializationCallbackReceiver.OnAfterSerialize")) ||
        mono.old && (method_name.contains("Awake") || method_name.contains("DoSendMouseEvents")) {
        let _ = debug!("Detaching hook from mono_runtime_invoke...");
        
        use crate::utils::console;

        console::set_handles();

        detours::unhook(INVOKE_ORIG_PTR).unwrap_or_else(|e| {
            internal_failure!("Failed to unhook mono_runtime_invoke: {}", e);
        });
            
        base_asm::pre_start(&game_data).unwrap_or_else(|e| {
            internal_failure!("Failed to pre start: {}", e);
        });
    
        base_asm::start(&game_data).unwrap_or_else(|e| {
            internal_failure!("Failed to start: {}", e);
        });
        

        return function(method, obj, params, exception);
    }

    let ret = function(method, obj, params, exception);

    ret
}

#[no_mangle]
unsafe extern "C" fn jit_init_hook(name: *const c_char, version: *const c_char) -> *mut MonoDomain {
    //check if windows
    use crate::utils::console;

    console::set_handles();

    let _ = debug!("Detaching Hook from mono_jit_init_version");

    let base_path = core::base_path()
        .unwrap_or_else(|e| internal_failure!("Failed to get base path: {}", e));

    let game_data = game::init()
        .unwrap_or_else(|e| internal_failure!("Failed to get game info: {}", e));

    let mono = get_mono()
        .unwrap_or_else(|e| internal_failure!("Failed to get mono: {}", e));

    let func = get_fn("mono_jit_init_version")
        .unwrap_or_else(|e| internal_failure!("Failed to get mono_jit_init_version: {}", e));

    if func.is_null() {
        internal_failure!("mono_jit_init_version is null!");
    }

    detours::unhook(func as usize).unwrap_or_else(|e| {
        internal_failure!("Failed to unhook mono_jit_init_version: {}", e);
    });

    let _ = debug!("Creating Mono Domain...");

    let domain = mono_jit_init_raw(name, version);
    if domain.is_null() {
        internal_failure!("Failed to create Mono Domain!");
    }

    if debug::enabled() {
        let _ = debug!("Creating Mono Debug Domain...");

        mono_debug_domain_create(domain).unwrap_or_else(|e| {
            internal_failure!("Failed to create Mono Debug Domain: {}", e);
        });
    }

    let _ = debug!("Setting Mono Main Thread...");
    mono_thread_set_main(mono_thread_current().unwrap_or_else(|e| {
        internal_failure!("Failed to get Mono Thread: {}", e);
    })).unwrap_or_else(|e| {
        internal_failure!("Failed to set Mono Main Thread: {}", e);
    });

    if !mono.old {
        let base_str = base_path.to_str().unwrap_or_else(|| {
            internal_failure!("Failed to convert base path to &str!");
        });

        let _ = debug!("Setting Mono Domain Config...");
        mono_domain_set_config(domain, base_str, name).unwrap_or_else(|e| {
            internal_failure!("Failed to set Mono Domain Config: {}", e);
        });
    }

    internal_calls::init().unwrap_or_else(|e| {
        internal_failure!("Failed to init internal calls: {}", e);
    });

    base_asm::init(&mono, &game_data).unwrap_or_else(|e| {
        internal_failure!("Failed to initialize Base Assembly: {}", e);
    });

    let _ = debug!("Attaching Hook to mono_runtime_invoke...");

    hook_runtime_invoke().unwrap_or_else(|e| {
        internal_failure!("Failed to hook mono_runtime_invoke: {}", e);
    });

    domain
}

/// install assembly hooks
pub fn install_assembly_hooks() -> Result<(), Box<dyn error::Error>> {
    debug!("Installing Assembly Hooks...")?;

    mono_install_assembly_hook(AssemblyHookType::Preload, preload_hook as usize as *mut c_void)?;
    mono_install_assembly_hook(AssemblyHookType::Search, search_hook as usize as *mut c_void)?;
    mono_install_assembly_hook(AssemblyHookType::Load, load_hook as usize as *mut c_void)?;

    Ok(())
}

unsafe fn preload_hook(aname: *mut AssemblyName, _assemblies_path: *mut *mut c_char, user_data: *mut c_void) -> *mut MonoAssembly {
    assembly_resolve(aname, user_data, true)
}

unsafe fn search_hook(aname: *mut AssemblyName, user_data: *mut c_void) -> *mut MonoAssembly {
    assembly_resolve(aname, user_data, false)
}

unsafe fn assembly_resolve(aname: *mut AssemblyName, _user_data: *mut c_void, mut is_preload: bool) -> *mut MonoAssembly {
    if mono::ASSEMBLYMANAGER_RESOLVE.is_none() || aname.is_null() {
        return std::ptr::null_mut();
    }

    let resolve = mono::ASSEMBLYMANAGER_RESOLVE.unwrap_or_else(|| {
        internal_failure!("Failed to get AssemblyManager.Resolve!");
    });

    let safe_aname = aname.as_ref().unwrap_or_else(|| {
        internal_failure!("Failed to get AssemblyName!");
    });

    let name = mono_string_new_raw(safe_aname.name).unwrap_or_else(|e| {
        internal_failure!("Failed to create Mono String: {}", e);
    });

    let mut major = safe_aname.major;
    let mut minor = safe_aname.minor;
    let mut build = safe_aname.build;
    let mut revision = safe_aname.revision;

    let mut args = vec![
        name.cast::<c_void>(), 
        std::ptr::addr_of_mut!(major).cast::<c_void>(),
        std::ptr::addr_of_mut!(minor).cast::<c_void>(),
        std::ptr::addr_of_mut!(build).cast::<c_void>(),
        std::ptr::addr_of_mut!(revision).cast::<c_void>(),
        std::ptr::addr_of_mut!(is_preload).cast::<c_void>()
    ];

    let res = mono_runtime_invoke(
        resolve, 
        None, 
        Some(&mut args), 
    );

    let res = res.unwrap_or_else(|e| {
        internal_failure!("Failed to get AssemblyManager.Resolve result: {}", e);
    });

    if res.is_none() {
        return ptr::null_mut();
    }

    let res = res.unwrap_or_else(|| {
        internal_failure!("Failed to get AssemblyManager.Resolve result!");
    });

    transmute::<*mut MonoObject, *mut MonoReflectionAssembly>(res).as_ref().unwrap_or_else(|| {
        internal_failure!("Failed to get MonoReflectionAssembly!");
    }).assembly
}

unsafe fn load_hook(assembly: *mut MonoAssembly, _user_data: *mut c_void) {
    if mono::ASSEMBLYMANAGER_LOADINFO.is_none() || assembly.is_null() {
        return;
    }

    let load_info = mono::ASSEMBLYMANAGER_LOADINFO.unwrap_or_else(|| {
        internal_failure!("Failed to get AssemblyManager.LoadInfo!");
    });

    let ref_asm = mono_assembly_get_object(assembly);

    if ref_asm.is_err() {
        return;
    }

    let ref_asm = ref_asm.unwrap_or_else(|e| {
        internal_failure!("Failed to get MonoReflectionAssembly: {}", e);
    });

    let mut args = vec![ref_asm as *mut c_void];

    let res = mono_runtime_invoke(
        load_info, 
        None, 
        Some(&mut args), 
    );

    let _ = res.unwrap_or_else(|e| {
        internal_failure!("Failed to get AssemblyManager.Resolve result: {}", e);
    });
}

fn get_fn(name: &str) -> Result<*mut c_void, Box<dyn error::Error>> {
    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(HookError::FailedToGetMonoLib)?
        .get_fn_ptr(name)?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    Ok(func as usize as *mut c_void)
}

/// gets mono
pub fn get_mono() -> Result<Mono, Box<dyn error::Error>> {
    let game_data = game::init()
        .unwrap_or_else(|e| internal_failure!("Failed to get game info: {}", e));

    let mono = mono::init( &game_data.base_path, &game_data.data_path, game_data.il2cpp)?
    .ok_or_else(|| MonoError::FailedToInit)?;

    Ok(mono)
}