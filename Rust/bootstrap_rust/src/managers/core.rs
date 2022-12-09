//! Here we take care of initializing all the components of the Bootstrap.

use std::{env, path::PathBuf, error};

use thiserror::Error;

use crate::{utils::{files, log}, debug};

use super::{game, mono, exports, hooks};

static VERSION: &str = "0.6.0";

/// core errors
#[derive(Debug, Error)]
pub enum CoreError {
    /// Failed to get the base path
    #[error("Failed to get the base path!")]
    FailedToGetBasePath,

    /// Failed to init mono
    #[error("Failed to init mono!")]
    FailedToInitMono,

    /// Failed to load mono
    #[error("Failed to load mono!")]
    FailedToLoadMono,

    /// Failed to init runtime
    #[error("Failed to init runtime!")]
    FailedToInitRuntime,
}

/// starts the core manager, and initializes MelonLoader.
pub fn init() -> Result<(), Box<dyn error::Error>> {
    msgbox::create("Proxy", "INIT", msgbox::IconType::Info)?;
    let base_path = base_path()?;
    let game_data = game::init()?;

    #[cfg(target_os = "windows")]
    {
        crate::utils::console::init()?;
    }

    files::check_ascii(&base_path)?;
    files::check_ascii(&game_data.data_path)?;

    log::init()?;
    
    if !game_data.il2cpp {
        debug!("Initializing Mono...")?;
    }

    let mono = mono::init( &game_data.base_path, &game_data.data_path, game_data.il2cpp)?;
    if !game_data.il2cpp && mono.is_none() {
        return Err(Box::new(CoreError::FailedToInitMono));
    }

    let mut runtime_inited = game_data.il2cpp;

    if mono.is_some() {
        let temp_mono = mono.unwrap();

        if !game_data.il2cpp {
            debug!("Mono::BasePath = {}", temp_mono.base_path.display())?;
            debug!("Mono::ManagedPath = {}",temp_mono.managed_path.display())?;
            debug!("Mono::ConfigPath = {}", temp_mono.config_path.display())?;
            debug!("Mono::IsOldMono = {}", temp_mono.old)?;
        }

        unsafe {
            exports::mono::MONO_LIB = Some(mono::load(&temp_mono)?);
        }

        runtime_inited = true;
    }

    if !runtime_inited {
        return Err(Box::new(CoreError::FailedToInitRuntime));
    }

    hooks::mono::hook_jit_init()?;

    #[cfg(target_os = "windows")]
    {
        crate::utils::console::null_handles();
    }

    Ok(())
}

/// gets the game's base directory.
pub fn base_path() -> Result<PathBuf, Box<CoreError>> {
    let app_path = env::current_exe()
        .map_err(|_| CoreError::FailedToGetBasePath)?;

    let base_path = app_path.parent()
        .ok_or(CoreError::FailedToGetBasePath)?;

    if !base_path.exists() {
        Err(Box::new(CoreError::FailedToGetBasePath))
    } else {
        Ok(base_path.to_path_buf())
    }
}

/// get version string
pub fn get_version_str() -> String {
    format!("MelonLoader v{} Open-Beta", VERSION)
}