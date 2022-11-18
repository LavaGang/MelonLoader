//! Manages Mono (if the game is not il2cpp)

use std::{path::{PathBuf, Path}, error};

use thiserror::Error;

use crate::{join_dll_path, debug, utils::libs::{MelonLib, self}, internal_failure};

use super::{game::Game, core, exports::mono::{mono_domain_assembly_open, mono_assembly_get_image, mono_class_from_name, mono_class_get_method_from_name, mono_runtime_invoke, MonoMethod}};

/// possible errors under mono
#[derive(Debug, Error)]
pub enum MonoError {
    /// Failed to get the mono path
    #[error("Failed to find Mono Library!")]
    FailedToFindLib,

    /// Failed to load mono
    #[error("Failed to load Mono Library!")]
    FailedToLoadLib,

    /// Failed to get the config path
    #[error("Failed to find Mono Config!")]
    FailedToFindConfig,

    /// Failed to get base assembly
    #[error("Failed to find Base Assembly!")]
    FailedToFindBaseAssembly,

    /// Failed to find Base Path
    #[error("Failed to find Base Path!")]
    FailedToFindBasePath,

    /// Failed to init
    #[error("Failed to init Mono!")]
    FailedToInit,

    /// Failed to find Preload
    #[error("Failed to find Preload!")]
    FailedToFindPreload,

    /// Failed to load Preload
    #[error("Failed to load Preload!")]
    FailedToLoadPreload,

    /// Failed to Prestart
    #[error("Failed to Prestart!")]
    FailedToPrestart,

    /// Failed to start
    #[error("Failed to start!")]
    FailedToStart,
}

/// Contains various paths to mono
#[allow(dead_code)] //TODO: Remove this
#[derive(Debug)]
pub struct Mono {
    /// the path to the mono library
    pub lib_path: PathBuf,
    /// the path to the mono directory
    pub base_path: PathBuf,
    /// the path to the managed directory
    pub managed_path: PathBuf,
    /// the path to the mono config
    pub config_path: PathBuf,
    /// the path to the base assembly
    pub melonloader_dll_path: PathBuf,
    /// is this old mono or not
    pub old: bool,
    /// the mono library
    pub lib: Option<MelonLib>,
}

/// Initializes Mono, if the game is not il2cpp
pub fn init(game_base_path: &PathBuf, game_data_path: &PathBuf, il2cpp: bool) -> Result<Option<Mono>, Box<dyn error::Error>> {
    if il2cpp {
        return Ok(None);
    }

    let lib_path = find_lib_path(game_base_path, game_data_path)?;

    let base_path = lib_path.parent()
        .ok_or(MonoError::FailedToFindBasePath)?.to_path_buf();

    let managed_path = game_data_path.join("Managed");

    let config_path = find_config_path(base_path.clone())?;

    let base_asm_path = PathBuf::from(game_base_path.clone())
        .join("MelonLoader")
        .join("net35")
        .join("MelonLoader.dll");

    if !base_asm_path.exists() {
        return Err(Box::new(MonoError::FailedToFindBaseAssembly));
    }

    let is_old = join_dll_path!(base_path.clone(), "mono").exists() || join_dll_path!(base_path.clone(), "libmono").exists();

    Ok(Some(Mono {
        lib_path: lib_path,
        base_path: base_path,
        managed_path: managed_path,
        config_path: config_path,
        melonloader_dll_path: base_asm_path,
        old: is_old,
        lib: None,
    }))
}

/// permanently loads mono
pub fn load(m: &Mono) -> Result<MelonLib, Box<dyn error::Error>> {
    debug!("Loading Mono...")?;

    libs::load_lib(&m.lib_path)
}

/// mono prestart method
pub static mut MONO_PRESTART: Option<*mut MonoMethod> = None;
/// mono start method
pub static mut MONO_START: Option<*mut MonoMethod> = None;

/// assembly manager resolve
pub static mut ASSEMBLYMANAGER_RESOLVE: Option<*mut MonoMethod> = None;
/// assembly manager load
pub static mut ASSEMBLYMANAGER_LOADINFO: Option<*mut MonoMethod> = None;


/// invokes MelonLoader's init
pub fn invoke_init(mono: &Mono, game: &Game) -> Result<(), Box<dyn error::Error>> {
    preload(mono, game)?;
    
    debug!("Initializing Base Assembly...")?;
    
    let assembly = mono_domain_assembly_open(mono.melonloader_dll_path.to_str()
        .ok_or_else(|| MonoError::FailedToLoadLib)?)?;

    let image = mono_assembly_get_image(assembly)?;
    let mono_class = mono_class_from_name(image, "MelonLoader", "Core")?;
    let method = mono_class_get_method_from_name(mono_class, "Initialize", 0)?;

    unsafe {
        MONO_PRESTART = Some(
            mono_class_get_method_from_name(mono_class, "PreStart", 0)?
        );

        MONO_START = Some(
            mono_class_get_method_from_name(mono_class, "Start", 0)?
        );
    }

    let assembly_manager = mono_class_from_name(image, "MelonLoader.MonoInternals.ResolveInternals", "AssemblyManager")?;
    unsafe {
        ASSEMBLYMANAGER_RESOLVE = Some(
            mono_class_get_method_from_name(assembly_manager, "Resolve", 6)?
        );
        
        ASSEMBLYMANAGER_LOADINFO = Some(
            mono_class_get_method_from_name(assembly_manager, "LoadInfo", 1)?
        );
    }

    let _ = mono_runtime_invoke(method, None, None)?;

    Ok(())
}

/// prestarts MelonLoader
pub fn invoke_pre_start() -> Result<(), Box<dyn error::Error>> {
    let method = unsafe {
        MONO_PRESTART
            .ok_or_else(|| MonoError::FailedToPrestart)?
    };

    let _ = mono_runtime_invoke(method, None, None)?;

    Ok(())
}

/// starts MelonLoader
pub fn invoke_start() -> Result<(), Box<dyn error::Error>> {
    let method = unsafe {
        MONO_START
            .ok_or_else(|| MonoError::FailedToStart)?
    };

    let _ = mono_runtime_invoke(method, None, None)?;

    Ok(())
}

fn preload(mono: &Mono, game: &Game) -> Result<(), Box<dyn error::Error>> {
    if game.il2cpp || !mono.old {
        return Ok(());
    }

    let preload_path = Path::new(core::base_path()?.to_str().ok_or_else(|| MonoError::FailedToFindPreload)?)
        .join("MelonLoader")
        .join("Dependencies")
        .join("SupportModules")
        .join("Preload.dll");

    if !preload_path.exists() {
        return Err(Box::new(MonoError::FailedToLoadPreload));
    }

    debug!("Initializing Preload Assembly...")?;

    let assembly = mono_domain_assembly_open(preload_path.to_str().ok_or_else(|| MonoError::FailedToFindPreload)?)?;
    let image = mono_assembly_get_image(assembly)?;
    let mono_class = mono_class_from_name(image, "MelonLoader.Support", "Preload")?;
    let mono_init = mono_class_get_method_from_name(mono_class, "Initialize", 0)?;

    let res = mono_runtime_invoke(mono_init, None, None);
    if res.is_err() {
        internal_failure!("Failed to invoke MelonLoader.Support.Preload.Initialize: {}", res.err().unwrap());
    }

    Ok(())
}

fn find_config_path(base_path: PathBuf) -> Result<PathBuf, Box<MonoError>> {
    let mut new_path = base_path.clone();
    let path_str = base_path.to_str()
        .ok_or(MonoError::FailedToFindConfig)?;

        if path_str.contains("EmbedRuntime") {
            //pop path, and return error if false
            new_path.pop().then(|| ()).ok_or(MonoError::FailedToFindConfig)?;
        }

        if path_str.contains("x86_64") {
            new_path.pop().then(|| ()).ok_or(MonoError::FailedToFindConfig)?;
        }

        new_path.push("etc");

        if !base_path.exists() {
            return Err(Box::new(MonoError::FailedToFindConfig));
        }

        Ok(base_path.to_path_buf())
}

fn find_lib_path(game_base_path: &PathBuf, game_data_path: &PathBuf) -> Result<PathBuf, Box<dyn error::Error>> {
    let folder_names = ["MonoBleedingEdge", "Mono", "MonoBleedingEdge.x64", "MonoBleedingEdge.x86"];
    let lib_names = ["libmono.so", "mono.dll", "mono-2.0-bdwgc.dll", "mono-2.0-sgen.dll", "mono-2.0-boehm.dll", "libmonobdwgc-2.0.so"];

    for folder_name in folder_names.iter() {
        for lib_name in lib_names.iter() {
            let lib_path = join_dll_path!(game_base_path.join(folder_name), lib_name);
            if lib_path.exists() {
                return Ok(lib_path);
            }

            let lib_path = join_dll_path!(game_base_path.join(folder_name).join("EmbedRuntime"), lib_name);
            if lib_path.exists() {
                return Ok(lib_path);
            }

            let lib_path = join_dll_path!(game_data_path.join(folder_name), lib_name);
            if lib_path.exists() {
                return Ok(lib_path);
            }

            let lib_path = join_dll_path!(game_data_path.join(folder_name).join("x86_64"), lib_name);
            if lib_path.exists() {
                return Ok(lib_path);
            }

            let lib_path = join_dll_path!(game_data_path.join(folder_name).join("EmbedRuntime"), lib_name);
            if lib_path.exists() {
                return Ok(lib_path);
            }
        }
    }

    Err(Box::new(MonoError::FailedToFindLib))
}