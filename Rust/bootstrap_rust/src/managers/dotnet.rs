//! dotnet runtime lol

use std::{error};

use netcorehost::{nethost::{get_hostfxr_path, load_hostfxr}, hostfxr::{Hostfxr, HostfxrContext, InitializedForRuntimeConfig}, pdcstring::{PdCString}, pdcstr};

use crate::{log, debug};

use super::{game::Game, il2cpp::Il2Cpp};

static mut HOST_FXR: Option<Hostfxr> = None;
static mut CONTEXT: Option<HostfxrContext<InitializedForRuntimeConfig>> = None;

#[allow(unused_variables)]
/// load the dotnet runtime
pub fn load(game: &Game, il2cpp: &Il2Cpp) -> Result<(), Box<dyn error::Error>> {

    let rc = get_hostfxr_path()?;
    let hostfxr_path = rc.to_str().ok_or_else(|| "Failed to get hostfxr path")?;
    log!("Using hostfx_path {}", hostfxr_path)?;

    unsafe { 
        HOST_FXR = Some(load_hostfxr()?);

        let hostfxr = HOST_FXR.as_ref().ok_or_else(|| "Failed to load hostfxr")?;

        let mut config_path = game.base_path.clone();
        config_path.push("MelonLoader");
        config_path.push("net6");
        config_path.push("MelonLoader.runtimeconfig.json");

        let config_path = config_path.as_os_str();
        let config_path = PdCString::from_os_str(config_path)?;

        CONTEXT = Some(hostfxr.initialize_for_runtime_config(config_path)?);
    }

    Ok(())
}

/// call the init function
pub fn call_init(game: &Game) -> Result<(), Box<dyn error::Error>> {
    let mut net6_dir = game.base_path.clone();
    net6_dir.push("MelonLoader");
    net6_dir.push("net6");

    let mut ml_managed_path = net6_dir.clone();
    ml_managed_path.push("MelonLoader.NativeHost.dll");

    let ml_managed_path = ml_managed_path.as_os_str();
    let ml_managed_path = PdCString::from_os_str(ml_managed_path)?;

    let dotnet_type = pdcstr!("MelonLoader.NativeHost.NativeEntryPoint, MelonLoader.NativeHost");
    let dotnet_method = pdcstr!("LoadStage1");

    unsafe {
        let context = CONTEXT.as_ref().ok_or_else(|| "Failed to get context")?;
        let loader = context.get_delegate_loader_for_assembly(ml_managed_path)?;

        let function = loader.get_function_with_unmanaged_callers_only::<fn()>(dotnet_type, dotnet_method)?;

        debug!("[Dotnet] Invoking Managed ML LoadStage1 Method...")?;
        function()?;
        
    }

    
    Ok(())
}