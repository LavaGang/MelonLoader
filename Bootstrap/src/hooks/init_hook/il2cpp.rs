use std::{ffi::c_char, sync::RwLock};

use lazy_static::lazy_static;
use unity_rs::il2cpp::types::Il2CppDomain;

use crate::{
    base_assembly, console, constants::InitFnIl2Cpp, debug, errors::DynErr, hooks::{HookedFunction, invoke_hook},
    internal_failure, runtime,
};

lazy_static! {
    pub static ref INIT_HOOK: RwLock<HookedFunction<InitFnIl2Cpp>> =
        RwLock::new(HookedFunction::new());
}

pub fn detour(name: *const c_char) -> *mut Il2CppDomain {
    detour_inner(name).unwrap_or_else(|e| {
        internal_failure!("il2cpp_init detour failed: {e}");
    })
}

fn detour_inner(name: *const c_char) -> Result<*mut Il2CppDomain, DynErr> {
    console::set_handles()?;

    let trampoline = INIT_HOOK.try_read()?;
    let domain = trampoline(name);

    debug!("Detaching hook from il2cpp_init")?;
    trampoline.unhook()?;

    base_assembly::init(runtime!()?)?;
    invoke_hook::hook()?;

    Ok(domain)
}
