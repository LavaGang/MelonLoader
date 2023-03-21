use std::{ffi::c_void, sync::RwLock, ptr::null_mut};

use lazy_static::lazy_static;
use unity_rs::{
    common::method::UnityMethod,
    mono::types::{MonoMethod, MonoObject},
    runtime::RuntimeType,
};

use crate::{
    base_assembly, constants::InvokeFnMono, debug, errors::DynErr, hooks::NativeHook,
    internal_failure, runtime,
};

lazy_static! {
    pub static ref INVOKE_HOOK: RwLock<NativeHook<InvokeFnMono>> =
        RwLock::new(NativeHook::new(null_mut(), null_mut()));
}

pub fn detour(
    method: *mut MonoMethod,
    obj: *mut MonoObject,
    params: *mut *mut c_void,
    exc: *mut *mut MonoObject,
) -> *mut MonoObject {
    detour_inner(method, obj, params, exc).unwrap_or_else(|e| {
        internal_failure!("mono_runtime_invoke detour failed: {e}");
    })
}

fn detour_inner(
    method: *mut MonoMethod,
    obj: *mut MonoObject,
    params: *mut *mut c_void,
    exc: *mut *mut MonoObject,
) -> Result<*mut MonoObject, DynErr> {
    let trampoline = INVOKE_HOOK.try_read()?;
    let result = trampoline(method, obj, params, exc);

    let runtime = runtime!()?;

    let safe_method = UnityMethod::new(method.cast())?;
    let name = safe_method.get_name(runtime)?;

    let mono = match runtime.get_type() {
        RuntimeType::Mono(mono) => mono,
        _ => return Ok(result),
    };

    if (name.contains("Internal_ActiveSceneChanged")
        || name.contains("UnityEngine.ISerializationCallbackReceiver.OnAfterSerialize"))
        || (mono.is_old && (name.contains("Awake") || name.contains("DoSendMouseEvents")))
    {
        debug!("Detaching hook from mono_runtime_invoke")?;
        trampoline.unhook()?;

        base_assembly::pre_start()?;
        base_assembly::start()?;
    }

    Ok(result)
}
