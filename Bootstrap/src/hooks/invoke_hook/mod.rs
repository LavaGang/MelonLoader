mod il2cpp;
mod mono;

use crate::{
    debug,
    errors::DynErr,
    runtime,
};
use std::ffi::c_void;
use unity_rs::runtime::RuntimeType;

use super::{NativeHook};

pub fn hook() -> Result<(), DynErr> {
    let runtime = runtime!()?;

    match runtime.get_type() {
        RuntimeType::Mono(_) => {
            debug!("Attaching hook to mono_runtime_invoke")?;

            let init_function = runtime.get_export_ptr("mono_runtime_invoke")?;
            let detour = mono::detour as *mut c_void;

            let mut init_hook = mono::INVOKE_HOOK.try_write()?;
            *init_hook = NativeHook::new(init_function, detour);

            init_hook.hook()?;
        }

        RuntimeType::Il2Cpp(_) => {
            debug!("Attaching hook to il2cpp_runtime_invoke")?;

            let init_function = runtime.get_export_ptr("il2cpp_runtime_invoke")?;
            let detour = il2cpp::detour as usize;

            let mut init_hook = il2cpp::INVOKE_HOOK.try_write()?;
            *init_hook = NativeHook::new(init_function, detour as *mut c_void);

            init_hook.hook()?;
        }
    };

    Ok(())
}
