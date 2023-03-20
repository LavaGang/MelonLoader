use netcorehost::{nethost, pdcstr};
use std::{
    ffi::c_void,
    ptr::{addr_of, addr_of_mut, null_mut},
};

use crate::{
    debug,
    errors::DynErr,
    icalls, melonenv,
    utils::{self, strings::wide_str},
};

#[repr(C)]
#[derive(Debug)]
pub struct HostImports {
    pub load_assembly_get_ptr: fn(isize, isize, isize, *mut *mut c_void),

    pub initialize: fn(),
    pub pre_start: fn(),
    pub start: fn(),
}

#[repr(C)]
#[derive(Debug)]
pub struct HostExports {
    pub hook_attach: unsafe fn(*mut *mut c_void, *mut c_void),
    pub hook_detach: unsafe fn(*mut *mut c_void, *mut c_void),
}

static mut IMPORTS: Option<HostImports> = None;

pub fn init() -> Result<(), DynErr> {
    let runtime_dir = melonenv::paths::runtime_dir()?;

    let hostfxr = nethost::load_hostfxr()?;
    let config_path = runtime_dir.join("MelonLoader.runtimeconfig.json");
    if !config_path.exists() {
        Err("Failed to find the MelonLoader.runtimeconfig.json file!")?
    }

    let context = hostfxr.initialize_for_runtime_config(utils::strings::pdcstr(config_path)?)?;

    let native_host_path = utils::strings::pdcstr(runtime_dir.join("MelonLoader.NativeHost.dll"))?;

    let loader = context.get_delegate_loader_for_assembly(&native_host_path)?;
    let init = loader.get_function_with_unmanaged_callers_only::<fn(*mut HostImports)>(
        pdcstr!("MelonLoader.NativeHost.NativeEntryPoint, MelonLoader.NativeHost"),
        pdcstr!("LoadStage1"),
    )?;

    let mut imports = HostImports {
        load_assembly_get_ptr: |_, _, _, _| {}, //hackz
        initialize: || {},
        pre_start: || {},
        start: || {},
    };

    let mut exports = HostExports {
        hook_attach: icalls::bootstrap_interop::attach,
        hook_detach: icalls::bootstrap_interop::detach,
    };

    debug!("[Dotnet] Invoking LoadStage1")?;
    init(addr_of_mut!(imports));
    debug!(
        "[Dotnet] Reloading NativeHost into correct load context and getting LoadStage2 pointer"
    )?;

    let mut init_stage_two = null_mut::<c_void>();

    (imports.load_assembly_get_ptr)(
        wide_str(runtime_dir.join("MelonLoader.NativeHost.dll"))?.as_ptr() as isize,
        wide_str("MelonLoader.NativeHost.NativeEntryPoint, MelonLoader.NativeHost")?.as_ptr() as isize,
        wide_str("LoadStage2")?.as_ptr() as isize,
        addr_of_mut!(init_stage_two),
    );

    debug!("[Dotnet] Invoking LoadStage2")?;

    let init_stage_two: fn(*mut HostImports, *mut HostExports) =
        unsafe { std::mem::transmute(init_stage_two) };
    init_stage_two(addr_of_mut!(imports), addr_of_mut!(exports));

    if addr_of!(imports.initialize).is_null() {
        Err("Failed to get HostImports::Initialize!")?
    }

    (imports.initialize)();

    unsafe {
        IMPORTS = Some(imports);
    }

    Ok(())
}

pub fn pre_start() -> Result<(), DynErr> {
    let imports = unsafe { IMPORTS.as_ref() }.ok_or("HostImports are not initialized!")?;

    (imports.pre_start)();

    Ok(())
}

pub fn start() -> Result<(), DynErr> {
    let imports = unsafe { IMPORTS.as_ref() }.ok_or("HostImports are not initialized!")?;

    (imports.start)();

    Ok(())
}
