use std::{
    error::Error,
    ffi::c_void,
    ptr::{addr_of, addr_of_mut, null_mut},
    sync::{LazyLock, Mutex},
};

use netcorehost::{
    nethost::{load_hostfxr, load_hostfxr_with_dotnet_root},
    pdcstr,
};

use crate::{
    debug, environment, icalls,
    utils::{self, strings::wide_str},
};

#[repr(C)]
#[derive(Debug)]
pub struct HostImports {
    pub load_assembly_get_ptr: fn(isize, isize, isize, *mut *mut c_void),

    pub initialize: fn(),
}

#[repr(C)]
#[derive(Debug)]
pub struct HostExports {
    pub hook_attach: unsafe fn(*mut c_void, *mut c_void) -> *mut c_void,
    pub hook_detach: unsafe fn(*mut c_void),
}

pub static IMPORTS: LazyLock<Mutex<HostImports>> = LazyLock::new(|| {
    Mutex::new(HostImports {
        load_assembly_get_ptr: |_, _, _, _| {},
        initialize: || {},
    })
});

pub fn start() -> Result<(), Box<dyn Error>> {
    let hostfxr = load_hostfxr_with_dotnet_root(utils::strings::pdcstr(
        environment::paths::get_base_dir()?
            .join("MelonLoader")
            .join("Dependencies")
            .join("dotnet"),
    )?)?;
    
    let runtime_dir = environment::paths::get_base_dir()?
        .join("MelonLoader")
        .join("net6");

    let config_path = runtime_dir.join("MelonLoader.Bootstrap.runtimeconfig.json");
    if !config_path.exists() {
        return Err("MelonLoader.Bootstrap.runtimeconfig.json does not exist!".into());
    }

    let context = hostfxr.initialize_for_runtime_config(utils::strings::pdcstr(config_path)?)?;

    let bootstrap_path = runtime_dir.join("MelonLoader.NativeHost.dll");

    if !bootstrap_path.exists() {
        return Err("MelonLoader.Bootstrap.dll does not exist!".into());
    }

    let loader =
        context.get_delegate_loader_for_assembly(utils::strings::pdcstr(bootstrap_path)?)?;

    let init = loader.get_function_with_unmanaged_callers_only::<fn(*mut HostImports)>(
        pdcstr!("MelonLoader.NativeHost.NativeEntryPoint, MelonLoader.NativeHost"),
        pdcstr!("LoadStage1"),
    )?;

    let mut imports = HostImports {
        load_assembly_get_ptr: |_, _, _, _| {},
        initialize: || {},
    };

    let mut exports = HostExports {
        hook_attach: icalls::bootstrap_interop::attach,
        hook_detach: icalls::bootstrap_interop::detach,
    };

    debug!("[Dotnet] Invoking LoadStage1")?;
    //MelonLoader.NativeHost will fill in the HostImports struct with pointers to functions
    init(addr_of_mut!(imports));

    debug!(
        "[Dotnet] Reloading NativeHost into correct load context and getting LoadStage2 pointer"
    )?;

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

    *IMPORTS.try_lock()? = imports;

    Ok(())
}
