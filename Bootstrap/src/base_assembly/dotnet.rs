use lazy_static::lazy_static;
use netcorehost::{nethost, pdcstr};
use std::{
    ffi::c_void,
    ptr::{addr_of, addr_of_mut, null_mut},
    sync::RwLock,
};

use crate::{
    debug,
    errors::{dotneterr::DotnetErr, DynErr},
    icalls, melonenv,
    utils::{self, strings::wide_str},
};

/// These are functions that MelonLoader.NativeHost.dll will fill in, once we call LoadStage1.
/// Interacting with the .net runtime is a pain, so it's a lot easier to just have it give us pointers like this directly.
#[repr(C)]
#[derive(Debug)]
pub struct HostImports {
    pub load_assembly_get_ptr: fn(isize, isize, isize, *mut *mut c_void),

    pub initialize: fn(),
    pub pre_start: fn(),
    pub start: fn(),
}

/// These are functions that we will pass to MelonLoader.NativeHost.dll.
/// CoreCLR does not have internal calls like mono does, so we have to pass these ourselves.
/// They are stored in Managed, and are accessed by MelonLoader for hooking.
#[repr(C)]
#[derive(Debug)]
pub struct HostExports {
    pub hook_attach: unsafe fn(*mut *mut c_void, *mut c_void),
    pub hook_detach: unsafe fn(*mut *mut c_void, *mut c_void),
}

// Initializing the host imports as a static variable. Later on this is replaced with a filled in version of the struct.
lazy_static! {
    pub static ref IMPORTS: RwLock<HostImports> = RwLock::new(HostImports {
        load_assembly_get_ptr: |_, _, _, _| {},
        initialize: || {},
        pre_start: || {},
        start: || {},
    });
}

pub fn init() -> Result<(), DynErr> {
    let runtime_dir = melonenv::paths::runtime_dir()?;

    let hostfxr = nethost::load_hostfxr().map_err(|_| DotnetErr::FailedHostFXRLoad)?;

    let config_path = runtime_dir.join("MelonLoader.runtimeconfig.json");
    if !config_path.exists() {
        return Err(DotnetErr::RuntimeConfig.into());
    }

    let context = hostfxr.initialize_for_runtime_config(utils::strings::pdcstr(config_path)?)?;

    let loader = context.get_delegate_loader_for_assembly(utils::strings::pdcstr(
        runtime_dir.join("MelonLoader.NativeHost.dll"),
    )?)?;

    let init = loader.get_function_with_unmanaged_callers_only::<fn(*mut HostImports)>(
        pdcstr!("MelonLoader.NativeHost.NativeEntryPoint, MelonLoader.NativeHost"),
        pdcstr!("LoadStage1"),
    )?;

    let mut imports = HostImports {
        load_assembly_get_ptr: |_, _, _, _| {},
        initialize: || {},
        pre_start: || {},
        start: || {},
    };

    let mut exports = HostExports {
        hook_attach: icalls::bootstrap_interop::attach,
        hook_detach: icalls::bootstrap_interop::detach,
    };

    debug!("[Dotnet] Invoking LoadStage1")?;
    //MelonLoader.NativeHost will fill in the HostImports struct with pointers to functions
    init(addr_of_mut!(imports));

    debug!("[Dotnet] Reloading NativeHost into correct load context and getting LoadStage2 pointer")?;

    //a function pointer to be filled
    let mut init_stage_two = null_mut::<c_void>();

    //have to make all strings utf16 for C# to understand, of course they can only be passed as IntPtrs
    (imports.load_assembly_get_ptr)(
        wide_str(runtime_dir.join("MelonLoader.NativeHost.dll"))?.as_ptr() as isize,
        wide_str("MelonLoader.NativeHost.NativeEntryPoint, MelonLoader.NativeHost")?.as_ptr()
            as isize,
        wide_str("LoadStage2")?.as_ptr() as isize,
        addr_of_mut!(init_stage_two),
    );

    debug!("[Dotnet] Invoking LoadStage2")?;

    //turn the function pointer into a function we can invoke
    let init_stage_two: fn(*mut HostImports, *mut HostExports) =
        unsafe { std::mem::transmute(init_stage_two) };
    init_stage_two(addr_of_mut!(imports), addr_of_mut!(exports));

    if addr_of!(imports.initialize).is_null() {
        Err("Failed to get HostImports::Initialize!")?
    }

    (imports.initialize)();

    *IMPORTS.try_write()? = imports;

    Ok(())
}

pub fn pre_start() -> Result<(), DynErr> {
    let imports = IMPORTS.try_read()?;

    (imports.pre_start)();

    Ok(())
}

pub fn start() -> Result<(), DynErr> {
    let imports = IMPORTS.try_read()?;

    (imports.start)();

    Ok(())
}
