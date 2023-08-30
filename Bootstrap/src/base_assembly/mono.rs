use std::{ptr::null_mut, sync::{RwLock, Mutex}};

use lazy_static::lazy_static;
use unity_rs::{
    common::{assembly::UnityAssembly, method::UnityMethod},
    runtime::FerrexRuntime,
};

use crate::{debug, errors::DynErr, melonenv::{self, paths}, runtime};

lazy_static! {
    pub static ref MONO_PRESTART: Mutex<UnityMethod> =
        Mutex::new(UnityMethod { inner: null_mut() });
    pub static ref MONO_START: Mutex<UnityMethod> = Mutex::new(UnityMethod { inner: null_mut() });
    pub static ref ASSEMBLYMANAGER_RESOLVE: RwLock<UnityMethod> =
        RwLock::new(UnityMethod { inner: null_mut() });
    pub static ref ASSEMBLYMANAGER_LOADINFO: RwLock<UnityMethod> =
        RwLock::new(UnityMethod { inner: null_mut() });
}

pub fn init(runtime: &FerrexRuntime) -> Result<(), DynErr> {
    preload(runtime)?;

    debug!("Initializing BaseAssembly")?;

    let _runtime_dir = paths::runtime_dir()?;

    //get MelonLoader.dll's path and confirm it exists
    let mut melonloader_dll = melonenv::paths::MELONLOADER_FOLDER.clone();
    melonloader_dll.extend(&["net35", "MelonLoader.dll"]);

    if !melonloader_dll.exists() {
        return Err("MelonLoader.dll not found".into());
    }

    //load the MelonLoader Assembly into the current domain, and grab some methods from the Core class
    let melonloader_assembly = UnityAssembly::open(melonloader_dll.as_path(), runtime)?;
    let core_class = melonloader_assembly.get_class("MelonLoader", "Core", runtime)?;
    let initialize_method = core_class.get_method("Initialize", 0, runtime)?;

    let prestart_method = core_class.get_method("PreStart", 0, runtime)?;
    let start_method = core_class.get_method("Start", 0, runtime)?;

    //get the AssemblyManager class and grab some methods from it
    let assemblymanager_class = melonloader_assembly.get_class(
        "MelonLoader.MonoInternals.ResolveInternals",
        "AssemblyManager",
        runtime,
    )?;

    let resolve_method = assemblymanager_class.get_method("Resolve", 6, runtime)?;
    let loadinfo_method = assemblymanager_class.get_method("LoadInfo", 1, runtime)?;

    //store the methods for later, in a thread safe global static.
    *MONO_PRESTART.try_lock()? = prestart_method;
    *MONO_START.try_lock()? = start_method;
    *ASSEMBLYMANAGER_RESOLVE.try_write()? = resolve_method;
    *ASSEMBLYMANAGER_LOADINFO.try_write()? = loadinfo_method;

    //invoke the MelonLoader initialize method.
    let _ = initialize_method.invoke(None, None, runtime)?;

    Ok(())
}

pub fn pre_start() -> Result<(), DynErr> {
    let prestart_method = MONO_PRESTART.try_lock()?;
    if prestart_method.inner.is_null() {
        return Err("PreStart method not found".into());
    }

    let _ = prestart_method.invoke(None, None, runtime!()?)?;
    Ok(())
}

pub fn start() -> Result<(), DynErr> {
    let start_method = MONO_START.try_lock()?;
    if start_method.inner.is_null() {
        return Err("Start method not found".into());
    }

    let _ = start_method.invoke(None, None, runtime!()?)?;
    Ok(())
}

fn preload(runtime: &FerrexRuntime) -> Result<(), DynErr> {
    if !melonenv::paths::PRELOAD_DLL.exists() {
        return Err("Preload.dll not found".into());
    }

    let _ = UnityAssembly::open(melonenv::paths::PRELOAD_DLL.as_path(), runtime)?
        .get_class("MelonLoader.Support", "Preload", runtime)?
        .get_method("Initialize", 0, runtime)?
        .invoke(None, None, runtime)?;

    Ok(())
}
