mod il2cpp;
mod mono;

use crate::{
    constants::{InvokeFnIl2Cpp, InvokeFnMono},
    debug,
    errors::DynErr,
    runtime,
};
use unity_rs::runtime::RuntimeType;

use super::functions;

pub fn hook() -> Result<(), DynErr> {
    let runtime = runtime!()?;

    match runtime.get_type() {
        RuntimeType::Mono(_) => {
            debug!("Attaching hook to mono_runtime_invoke")?;

            let init_function = runtime.get_export_ptr("mono_runtime_invoke")?;
            let detour = mono::detour as usize;

            let hooked = functions::hook::<InvokeFnMono>(init_function as usize, detour)?;

            let mut init_hook = mono::INVOKE_HOOK.try_write()?;
            *init_hook = hooked;
        }

        RuntimeType::Il2Cpp(_) => {
            debug!("Attaching hook to il2cpp_runtime_invoke")?;

            let init_function = runtime.get_export_ptr("il2cpp_runtime_invoke")?;
            let detour = il2cpp::detour as usize;

            let hooked = functions::hook::<InvokeFnIl2Cpp>(init_function as usize, detour)?;

            let mut init_hook = il2cpp::INVOKE_HOOK.try_write()?;
            *init_hook = hooked;
        }
    };

    Ok(())
}
