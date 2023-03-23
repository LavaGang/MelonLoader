use std::{ffi::c_char, sync::RwLock, ptr::null_mut};

use lazy_static::{lazy_static};
use unity_rs::{common::domain::UnityDomain, mono::types::MonoDomain, runtime::RuntimeType};

use crate::{
    base_assembly, console,
    constants::InitFnMono,
    debug, debug_enabled,
    errors::DynErr,
    hooks::{invoke_hook, NativeHook},
    icalls, internal_failure, melonenv, runtime, rust_str,
};

lazy_static! {
    pub static ref INIT_HOOK: RwLock<NativeHook<InitFnMono>> =
        RwLock::new(NativeHook::new(null_mut(), null_mut()));
}

pub fn detour(name: *const c_char, version: *const c_char) -> *mut MonoDomain {
    detour_inner(name, version).unwrap_or_else(|e| {
        internal_failure!("mono_jit_init_version detour failed: {e}");
    })
}

fn detour_inner(name: *const c_char, version: *const c_char) -> Result<*mut MonoDomain, DynErr> {
    console::set_handles()?;

    let rust_name = rust_str!(name)?;
    let base_dir: String = melonenv::paths::GAME_DIR.clone().try_into()?;

    let runtime = runtime!()?;

    let trampoline = INIT_HOOK.try_read()?;

    let domain_ptr = trampoline(name, version);
    let domain = UnityDomain::new(domain_ptr.cast());

    debug!("Detaching hook from mono_jit_init_version")?;
    trampoline.unhook()?;

    if debug_enabled!() {
        debug!("Creating Mono Debug Domain")?;

        //the result of this can be ignored. it is not guaranteed that this function exists
        let _ = runtime.create_debug_domain(&domain);
    }

    debug!("Setting Mono Main Thread")?;
    let thread = runtime.get_current_thread()?;
    runtime.set_main_thread(&thread)?;

    if let RuntimeType::Mono(mono) = runtime.get_type() {
        if !mono.is_old {
            debug!("Setting Mono Config")?;

            //the result of this can be ignored. it is not guaranteed that this function exists
            let _ = runtime.set_domain_config(&domain, base_dir.as_str(), rust_name);
        }
    }

    icalls::init(runtime)?;
    base_assembly::init(runtime)?;

    invoke_hook::hook()?;

    Ok(domain.inner.cast())
}
