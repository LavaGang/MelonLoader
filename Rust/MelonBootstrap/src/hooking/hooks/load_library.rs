use std::{
    error::Error,
    ffi::{c_char, c_void}, sync::Mutex,
};

use dobby_rs::Address;

use crate::{
    hooking::NativeHook,
    internal_failure,
    utils::{
        libs::{load_lib, NativeLibrary},
        type_aliases::LoadLibraryFn,
    }, dotnet, debug,
};

lazy_static::lazy_static! {
    static ref KERNEL32: Mutex<NativeLibrary> = Default::default();
    static ref LOAD_LIBRARY_HOOK: Mutex<NativeHook<LoadLibraryFn>> = Default::default();
}

pub fn init() -> Result<(), Box<dyn Error>> {
    let mut kernel_32 = KERNEL32.try_lock()?;
    let mut load_library_hook = LOAD_LIBRARY_HOOK.try_lock()?;
    debug!("Loading Kernel32.dll")?;
    *kernel_32 = load_lib("Kernel32.dll\0")?;

    load_library_hook.target = kernel_32.sym::<LoadLibraryFn>("LoadLibraryA\0")?.inner;
    load_library_hook.detour = detour as Address;

    debug!("Hooking LoadLibraryA")?;
    load_library_hook.hook()?;

    Ok(())
}

unsafe fn detour(lib: *mut c_char) -> *mut c_void {
    detour_inner(lib).unwrap_or_else(|e| {
        internal_failure!("LoadLibrary detour failed: {e}");
    })
}

unsafe fn detour_inner(lib: *mut c_char) -> Result<*mut c_void, Box<dyn Error>> {
    let load_library_hook = LOAD_LIBRARY_HOOK.try_lock()?;
    load_library_hook.unhook()?;

    dotnet::startup::start()?;

    Ok(load_library_hook(lib))
}
