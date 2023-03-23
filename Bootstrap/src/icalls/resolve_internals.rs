use std::ffi::{c_char, c_void};

use unity_rs::{
    common::{assembly::UnityAssembly, method::MethodPointer, string::UnityString},
    mono::{
        types::{AssemblyName, MonoAssembly, MonoReflectionAssembly},
        AssemblyHookType,
    },
};

use crate::{base_assembly, errors::DynErr, internal_failure, runtime};

pub fn install_hooks() {
    install_hooks_inner().unwrap_or_else(|e| {
        internal_failure!("Failed to install assembly hooks: {}", e.to_string());
    })
}

fn install_hooks_inner() -> Result<(), DynErr> {
    let runtime = runtime!()?;

    runtime.install_assembly_hook(AssemblyHookType::Preload, preload_hook as MethodPointer)?;
    runtime.install_assembly_hook(AssemblyHookType::Search, search_hook as MethodPointer)?;
    runtime.install_assembly_hook(AssemblyHookType::Load, load_hook as MethodPointer)?;

    Ok(())
}

fn assembly_resolve(
    aname: *mut AssemblyName,
    _user_data: *mut c_void,
    mut is_preload: bool,
) -> Result<*mut MonoAssembly, DynErr> {
    let runtime = runtime!()?;

    let resolve_method = base_assembly::mono::ASSEMBLYMANAGER_RESOLVE.try_lock()?;

    if resolve_method.inner.is_null() {
        return Err("AssemblyManager.Resolve is null".into());
    }

    let safe_aname = unsafe { aname.as_ref().ok_or("AssemblyName is null")? };

    let (mut major, mut minor, mut build, mut revision) = (
        safe_aname.major,
        safe_aname.minor,
        safe_aname.build,
        safe_aname.revision,
    );

    let name = UnityString::from_raw(safe_aname.name.cast(), runtime)?;

    let mut args = vec![
        name.inner.cast::<c_void>(),
        std::ptr::addr_of_mut!(major).cast::<c_void>(),
        std::ptr::addr_of_mut!(minor).cast::<c_void>(),
        std::ptr::addr_of_mut!(build).cast::<c_void>(),
        std::ptr::addr_of_mut!(revision).cast::<c_void>(),
        std::ptr::addr_of_mut!(is_preload).cast::<c_void>(),
    ];

    let res = resolve_method.invoke(None, Some(&mut args), runtime);

    if res.is_err() {
        return Ok(std::ptr::null_mut());
    }

    let res = res?;

    match res {
        Some(res) => {
            if res.inner.is_null() {
                return Ok(std::ptr::null_mut());
            }

            let res = res.inner.cast::<MonoReflectionAssembly>();
            let res = unsafe {
                res.as_ref()
                    .ok_or("AssemblyManager.Resolve returned null")?
            };

            Ok(res.assembly)
        }
        None => Ok(std::ptr::null_mut()),
    }
}

fn load_hook_inner(assembly: *mut MonoAssembly) -> Result<(), DynErr> {
    if assembly.is_null() {
        return Ok(());
    }

    let load_method = base_assembly::mono::ASSEMBLYMANAGER_LOADINFO.try_lock()?;
    if load_method.inner.is_null() {
        return Ok(());
    }

    let runtime = runtime!()?;
    let safe_assembly = UnityAssembly::new(assembly.cast())?;

    let assembly_object = runtime.get_assembly_object(&safe_assembly)?;

    let mut args = vec![assembly_object.inner];

    let _ = load_method.invoke(None, Some(&mut args), runtime!()?)?;

    Ok(())
}

fn preload_hook(
    aname: *mut AssemblyName,
    _assemblies_path: *mut *mut c_char,
    user_data: *mut c_void,
) -> *mut MonoAssembly {
    assembly_resolve(aname, user_data, true).unwrap_or_else(|e| {
        internal_failure!("Failed to preload assembly: {e}");
    })
}

fn search_hook(aname: *mut AssemblyName, user_data: *mut c_void) -> *mut MonoAssembly {
    assembly_resolve(aname, user_data, false).unwrap_or_else(|e| {
        internal_failure!("Failed to search assembly: {e}");
    })
}

fn load_hook(_assembly: *mut MonoAssembly, _user_data: *mut c_void) {
    load_hook_inner(_assembly).unwrap_or_else(|e| {
        internal_failure!("Failed to load assembly: {e}");
    })
}
