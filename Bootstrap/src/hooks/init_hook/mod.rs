mod il2cpp;
mod mono;

use crate::{
    constants::{InitFnIl2Cpp, InitFnMono},
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
            debug!("Attaching hook to mono_jit_init_version")?;

            let init_function = runtime.get_export_ptr("mono_jit_init_version")?;
            let detour = mono::detour as usize;

            let hooked = functions::hook::<InitFnMono>(init_function as usize, detour)?;

            let mut init_hook = mono::INIT_HOOK.try_write()?;
            *init_hook = hooked;
        }

        RuntimeType::Il2Cpp(_) => {
            debug!("Attaching hook to il2cpp_init")?;

            let init_function = runtime.get_export_ptr("il2cpp_init")?;
            let detour = il2cpp::detour as usize;

            let hooked = functions::hook::<InitFnIl2Cpp>(init_function as usize, detour)?;

            let mut init_hook = il2cpp::INIT_HOOK.try_write()?;
            *init_hook = hooked;
        }
    };

    Ok(())
}
