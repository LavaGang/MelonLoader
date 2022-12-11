use std::{
    error::{Error, self},
    ffi::{c_void, c_char, CStr}, mem::transmute
};
use unity_rs::{mono::{Mono, types::{MonoReflectionAssembly, AssemblyName, MonoAssembly, MonoString, MonoMethod, MonoObject}, AssemblyHookType}, runtime::{Runtime, UnityRuntime}};

use crate::{debug, internal_failure, err, utils::files};

static mut MONO: Option<Mono> = None;

pub fn init(mono: Mono) -> Result<(), Box<dyn Error>> {
    debug!("Initializing internal calls")?;

    mono.add_internal_call("MelonLoader.MelonUtils::IsGame32Bit", IsGame32Bit as usize)?;
    mono.add_internal_call("MelonLoader.BootstrapInterop::NativeHookAttach", NativeHookAttach as usize)?;
    mono.add_internal_call("MelonLoader.BootstrapInterop::NativeHookDetach", NativeHookDetach as usize)?;
    mono.add_internal_call("MelonLoader.MonoInternals.MonoLibrary::GetLibPtr", GetLibPtr as usize)?;
    mono.add_internal_call("MelonLoader.MonoInternals.MonoLibrary::CastManagedAssemblyPtr", CastManagedAssemblyPtr as usize)?;
    mono.add_internal_call("MelonLoader.MonoInternals.ResolveInternals.AssemblyManager::InstallHooks", InstallHooks as usize)?;
    mono.add_internal_call("MelonLoader.MonoInternals.MonoLibrary::GetRootDomainPtr", GetRootDomainPtr as usize)?;
    mono.add_internal_call("MelonLoader.Support.Preload::GetManagedDirectory", GetManagedDirectory as usize)?;

    unsafe {
        MONO = Some(mono);
    }
    Ok(())
}

fn IsGame32Bit() -> bool {
    cfg!(target_pointer_width = "32")
}

pub fn NativeHookAttach(mut target: *mut *mut c_void, detour: *mut c_void) {
    unsafe {
        match dobby_rs::hook(*target, detour) {
            Ok(res) => target = res as *mut *mut c_void,
            Err(e) => {
                err!("Failed to hook function: {e}");
            }
        };
    }
}

pub fn NativeHookDetach(target: *mut *mut c_void, _detour: *mut c_void)  {
    unsafe {
        dobby_rs::unhook(*target).unwrap_or_else(|e| {
            err!("Failed to unhook function: {e}");
        });
    }
}

fn GetLibPtr() -> *mut c_void {
    get_lib_ptr().unwrap_or_else(|e| {
        internal_failure!("Failed to get lib ptr: {e}");
    })
}

fn get_lib_ptr() -> Result<*mut c_void, Box<dyn Error>> {
    let runtime = Runtime::new()?;

    if let UnityRuntime::MonoRuntime(mono) = runtime.runtime {
        Ok(mono.mono_lib.handle)
    } else {
        Err("Game is not mono".into())
    }
}

fn CastManagedAssemblyPtr(assembly: *mut c_void) -> *mut MonoReflectionAssembly {
    cast_assembly_ptr(assembly).unwrap_or_else(|e| {
        internal_failure!("Failed to cast assembly ptr: {e}");
    })
}

fn cast_assembly_ptr(assembly: *mut c_void) -> Result<*mut MonoReflectionAssembly, Box<dyn Error>> {
    Ok(assembly.cast())
}

fn InstallHooks() {
    install_hooks().unwrap_or_else(|e| {
        internal_failure!("Failed to install hooks: {e}");
    })
}

fn install_hooks() -> Result<(), Box<dyn Error>> {
    let runtime = Runtime::new()?;

    if let UnityRuntime::MonoRuntime(mono) = runtime.runtime {
        mono.install_assembly_hook(AssemblyHookType::Preload, preload_hook as usize)?;
        mono.install_assembly_hook(AssemblyHookType::Search, search_hook as usize)?;
        mono.install_assembly_hook(AssemblyHookType::Load, load_hook as usize)?;
    } else {
        return Err("Game is not mono".into());
    }

    Ok(())
}

fn preload_hook(aname: *mut AssemblyName, _assemblies_path: *mut *mut c_char, user_data: *mut c_void) -> *mut MonoAssembly {
    assembly_resolve(aname, user_data, true).unwrap_or_else(|e| {
        internal_failure!("Failed to preload assembly: {e}");
    })
}

fn search_hook(aname: *mut AssemblyName, user_data: *mut c_void) -> *mut MonoAssembly {
    assembly_resolve(aname, user_data, false).unwrap_or_else(|e| {
        internal_failure!("Failed to search assembly: {e}");
    })
}

fn assembly_resolve(aname: *mut AssemblyName, _user_data: *mut c_void, mut is_preload: bool) -> Result<*mut MonoAssembly, Box<dyn error::Error>> {
    let resolve = unsafe {
        match super::mono::ASSEMBLYMANAGER_RESOLVE {
            Some(resolve) => resolve,
            None => return Err("AssemblyManager.Resolve is null".into())
        }
    };

    let safe_aname = unsafe {
        aname.as_ref().ok_or("AssemblyName is null")?
    };

    let a = unsafe { CStr::from_ptr(safe_aname.name) };
    let name = a.to_str()?;

    let name = MonoString::from_raw(safe_aname.name)?;

    let mut major = safe_aname.major;
    let mut minor = safe_aname.minor;
    let mut build = safe_aname.build;
    let mut revision = safe_aname.revision;

    let mut args = vec![
        name.cast::<c_void>(), 
        std::ptr::addr_of_mut!(major).cast::<c_void>(),
        std::ptr::addr_of_mut!(minor).cast::<c_void>(),
        std::ptr::addr_of_mut!(build).cast::<c_void>(),
        std::ptr::addr_of_mut!(revision).cast::<c_void>(),
        std::ptr::addr_of_mut!(is_preload).cast::<c_void>()
    ];

    let res = MonoMethod::invoke(resolve, None, Some(&mut args));

    if res.is_err() {
        return Ok(std::ptr::null_mut());
    }

    let res = res?;

    match res {
        Some(res) => {
            if res.is_null() {
                return Ok(std::ptr::null_mut());
            }

            let res = res.cast::<MonoReflectionAssembly>();
            let res = unsafe {
                res.as_ref().ok_or("AssemblyManager.Resolve returned null")?
            };
            Ok(res.assembly)
        },
        None => Ok(std::ptr::null_mut())
    }
}

fn load_hook(_assembly: *mut MonoAssembly, user_data: *mut c_void) {
    load_hook_inner(_assembly, user_data).unwrap_or_else(|e| {
        internal_failure!("Failed to load assembly: {e}");
    })
}

fn load_hook_inner(_assembly: *mut MonoAssembly, user_data: *mut c_void) -> Result<(), Box<dyn error::Error>> {
    if _assembly.is_null() {
        return Ok(());
    }
    let load_info = unsafe {
        match super::mono::ASSEMBLYMANAGER_LOADINFO {
            Some(load_info) => load_info,
            None => return Err("AssemblyManager.LoadInfo is null".into())
        }
    };

    let reference = MonoAssembly::as_object(_assembly)?;

    let mut args = vec![reference as *mut c_void];

    MonoMethod::invoke(load_info, None, Some(&mut args))?;

    Ok(())
}

fn GetRootDomainPtr() -> *mut c_void {
    unsafe {
        match MONO {
            Some(ref mono) => mono.get_domain().unwrap_or(std::ptr::null_mut()).cast(),
            None => std::ptr::null_mut()
        }
    }
}

fn GetManagedDirectory() -> *mut MonoString {
    get_managed_directory().unwrap_or_else(|e| {
        internal_failure!("Failed to get managed directory: {e}");
    })
}

fn get_managed_directory() -> Result<*mut MonoString, Box<dyn Error>> {
    let path = files::managed_dir()?;
    let path = path.to_str().ok_or_else(|| "Failed to convert path to string")?;
    Ok(MonoString::new(path)?)
}