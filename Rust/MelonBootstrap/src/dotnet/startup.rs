use std::error::Error;

use netcorehost::{nethost::load_hostfxr, pdcstr};

use crate::{environment, utils};

pub fn start() -> Result<(), Box<dyn Error>> {
    let hostfxr = load_hostfxr()?;
    let runtime_dir = environment::paths::get_base_dir()?.join("MelonLoader").join("net6");
    
    let config_path = runtime_dir.join("MelonLoader.Bootstrap.runtimeconfig.json");
    if !config_path.exists() {
        return Err("MelonLoader.Bootstrap.runtimeconfig.json does not exist!".into());
    }

    let context = hostfxr.initialize_for_runtime_config(utils::strings::pdcstr(config_path)?)?;

    let bootstrap_path = runtime_dir.join("MelonLoader.Bootstrap.dll");

    if !bootstrap_path.exists() {
        return Err("MelonLoader.Bootstrap.dll does not exist!".into());
    }

    let loader = context.get_delegate_loader_for_assembly(utils::strings::pdcstr(
        bootstrap_path,
    )?)?;

    let init = loader.get_function_with_unmanaged_callers_only::<fn()>(
        pdcstr!("MelonLoader.Bootstrap.Entrypoint, MelonLoader.Bootstrap"),
        pdcstr!("Entry"),
    )?;

    init();

    Ok(())
}