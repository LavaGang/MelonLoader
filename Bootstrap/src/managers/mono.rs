use std::error;

use unity_rs::mono::{Mono, types::{MonoAssembly, MonoImage, MonoClass, MonoMethod}};

use crate::{utils::files, debug};

/// mono prestart method
pub static mut MONO_PRESTART: Option<*mut MonoMethod> = None;
/// mono start method
pub static mut MONO_START: Option<*mut MonoMethod> = None;

/// assembly manager resolve
pub static mut ASSEMBLYMANAGER_RESOLVE: Option<*mut MonoMethod> = None;
/// assembly manager load
pub static mut ASSEMBLYMANAGER_LOADINFO: Option<*mut MonoMethod> = None;

pub fn init(mono: &Mono) -> Result<(), Box<dyn error::Error>> {
    preload(mono)?;

    debug!("Initializing base assembly")?;

    let melonloader_dll = files::melonloader_dir()?
    .join("net35")
    .join("MelonLoader.dll");

    if !melonloader_dll.exists() {
        return Err("MelonLoader.dll not found".into());
    }

    let melonloader_dll = melonloader_dll.to_str().ok_or_else(|| "MelonLoader.dll path is not valid UTF-8")?;

    let image = MonoImage::open(melonloader_dll)?;
    let class = MonoImage::get_class(image, "MelonLoader", "Core")?;
    let method = MonoClass::get_method(class, "Initialize", 0)?;

    unsafe {
        MONO_PRESTART = Some(
            MonoClass::get_method(class, "PreStart", 0)?
        );

        MONO_START = Some(
            MonoClass::get_method(class, "Start", 0)?
        );
    }

    let assemblymanager_class = MonoImage::get_class(image, "MelonLoader.MonoInternals.ResolveInternals", "AssemblyManager")?;

    unsafe {
        ASSEMBLYMANAGER_RESOLVE = Some(
            MonoClass::get_method(assemblymanager_class, "Resolve", 6)?
        );

        ASSEMBLYMANAGER_LOADINFO = Some(
            MonoClass::get_method(assemblymanager_class, "LoadInfo", 1)?
        );
    }

    MonoMethod::invoke(method, None, None)?;

    Ok(())
}

pub fn pre_start(mono: &Mono) -> Result<(), Box<dyn error::Error>> {
    match unsafe { MONO_PRESTART } {
        Some(method) => {
            let res = MonoMethod::invoke(method, None, None);
            match res {
                Ok(_) => Ok(()),
                Err(e) => Err(e)
            }
        },
        None => {
            Err("Mono prestart method not found".into())
        }
    }
}

pub fn start(mono: &Mono) -> Result<(), Box<dyn error::Error>> {
    match unsafe { MONO_START } {
        Some(method) => {
            let res = MonoMethod::invoke(method, None, None);
            match res {
                Ok(_) => Ok(()),
                Err(e) => Err(e)
            }
        },
        None => {
            Err("Mono start method not found".into())
        }
    }
}

fn preload(mono: &Mono) -> Result<(), Box<dyn error::Error>> {
    if !mono.is_old {
        return Ok(());
    }

    let mut preload_path = files::melonloader_dir()?;
    preload_path.extend(["Dependencies", "SupportModule", "Preload.dll"].iter());

    

    Ok(())
}