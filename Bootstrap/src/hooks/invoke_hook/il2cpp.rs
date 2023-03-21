use std::{ffi::c_void, sync::RwLock, ptr::null_mut};

use lazy_static::lazy_static;
use unity_rs::{il2cpp::types::{Il2CppMethod, Il2CppObject}, common::method::UnityMethod};

use crate::{constants::InvokeFnIl2Cpp, errors::DynErr, hooks::NativeHook, internal_failure, runtime, debug, base_assembly};

lazy_static! {
    pub static ref INVOKE_HOOK: RwLock<NativeHook<InvokeFnIl2Cpp>> =
        RwLock::new(NativeHook::new(null_mut(), null_mut()));
}

pub fn detour(
    method: *mut Il2CppMethod,
    obj: *mut Il2CppObject,
    params: *mut *mut c_void,
    exc: *mut *mut Il2CppObject,
) -> *mut Il2CppObject {
    detour_inner(method, obj, params, exc).unwrap_or_else(|e| {
        internal_failure!("il2cpp_runtime_invoke detour failed: {e}");
    })
}

fn detour_inner(
    method: *mut Il2CppMethod,
    obj: *mut Il2CppObject,
    params: *mut *mut c_void,
    exc: *mut *mut Il2CppObject,
) -> Result<*mut Il2CppObject, DynErr> {
    let trampoline = INVOKE_HOOK.try_read()?;
    let result = trampoline(method, obj, params, exc);

    let runtime = runtime!()?;

    let safe_method = UnityMethod::new(method.cast())?;
    let name = safe_method.get_name(runtime)?;

    if name.contains("Internal_ActiveSceneChanged") {
        debug!("Detaching hook from il2cpp_runtime_invoke")?;
        trampoline.unhook()?;

        base_assembly::pre_start()?;
        base_assembly::start()?;
    }

    Ok(result)
}
