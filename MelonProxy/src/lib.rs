//! Most of this Project was generated using [ProxyGen](https://github.com/WarrenHood/proxygen)
//! 
//! Altered to work with MelonLoader, and be cross platform.

#![feature(naked_functions)]
#![allow(named_asm_labels)]
#![allow(non_snake_case)]
#![feature(asm_const)]
#![feature(lazy_cell)]

#[cfg(target_os = "windows")]
mod export_indices;
#[cfg(target_os = "windows")]
mod intercepted_exports;
#[cfg(target_os = "windows")]
mod orig_exports;
#[cfg(target_os = "windows")]
mod proxied_exports;

mod utils;
mod core;


#[allow(unused_imports)]
#[cfg(target_os = "windows")]
pub use intercepted_exports::*;
#[cfg(target_os = "windows")]
pub use proxied_exports::*;

#[cfg(target_os = "windows")]
use export_indices::TOTAL_EXPORTS;
#[cfg(target_os = "windows")]
use orig_exports::load_dll_funcs;
#[cfg(target_arch="x86_64")]
#[cfg(target_os = "windows")]
use std::arch::x86_64::_mm_pause;
#[cfg(target_arch="x86")]
#[cfg(target_os = "windows")]
use std::arch::x86::_mm_pause;
#[cfg(target_os = "windows")]
use std::ffi::OsString;
#[cfg(target_os = "windows")]
use std::os::windows::prelude::{AsRawHandle, OsStringExt};

#[cfg(target_os = "windows")]
use winapi::{
    ctypes::c_void,
    shared::{
        minwindef::{FARPROC, HMODULE},
        ntdef::LPCSTR,
    },
    um::{
        consoleapi::AllocConsole,
        errhandlingapi::GetLastError,
        libloaderapi::{DisableThreadLibraryCalls, FreeLibrary, GetModuleFileNameW, LoadLibraryA},
        processenv::SetStdHandle,
        processthreadsapi::{CreateThread, GetCurrentProcess, TerminateProcess},
        winbase::{STD_ERROR_HANDLE, STD_OUTPUT_HANDLE},
        winnt::{DLL_PROCESS_ATTACH, DLL_PROCESS_DETACH},
        winuser::{MessageBoxA, MB_OK},
        sysinfoapi::GetSystemDirectoryW,
    },
};


// Static handles
#[cfg(target_os = "windows")]
static mut THIS_HANDLE: Option<HMODULE> = None;
#[cfg(target_os = "windows")]
static mut ORIG_DLL_HANDLE: Option<HMODULE> = None;

// Original funcs
#[cfg(target_os = "windows")]
#[no_mangle]
static mut ORIGINAL_FUNCS: [FARPROC; TOTAL_EXPORTS] = [std::ptr::null_mut(); TOTAL_EXPORTS];
#[cfg(target_os = "windows")]
#[no_mangle]
static mut ORIG_FUNCS_PTR: *const FARPROC = std::ptr::null_mut();

const INFO_BUFFER_SIZE: usize = 32767;

/// Indicates once we are ready to accept incoming calls to proxied functions

static mut PROXYGEN_READY: bool = false;

#[cfg(target_os = "windows")]
#[no_mangle]
pub unsafe extern "stdcall" fn DllMain(module: HMODULE, reason: isize, _res: *const c_void) -> i32 {
    THIS_HANDLE = Some(module);

    if reason == 1 {
        init(std::ptr::null_mut());

        core::init().unwrap_or_else(|e| {
            internal_failure!("Failed to initialize MelonLoader: {}", e)
        });
    }

    1
}

#[cfg(not(target_os = "windows"))]
#[ctor::ctor]
fn init() {
    core::init().unwrap_or_else(|e| {
        internal_failure!("Failed to initialize MelonLoader: {}", e)
    });
}


/// Get the current DLLs path
#[cfg(target_os = "windows")]
unsafe fn get_dll_path() -> Option<String> {
    let mut buffer: Vec<u16> = vec![0; INFO_BUFFER_SIZE];
    if THIS_HANDLE.is_none() {
        return None;
    }
    let size = GetModuleFileNameW(
        THIS_HANDLE.unwrap(),
        buffer.as_mut_ptr(),
        buffer.len() as u32,
    );

    if size == 0 {
        return None;
    }

    buffer.truncate(size as usize);
    let os_string = OsString::from_wide(&buffer);
    Some(os_string.to_string_lossy().into_owned())
}

#[cfg(target_os = "windows")]
unsafe fn get_system32_path() -> Option<String> {
    let mut buffer: Vec<u16> = vec![0; INFO_BUFFER_SIZE];
    let size = GetSystemDirectoryW(
        buffer.as_mut_ptr(),
        buffer.len() as u32,
    );

    if size == 0 {
        return None;
    }

    buffer.truncate(size as usize);
    let os_string = OsString::from_wide(&buffer);
    Some(os_string.to_string_lossy().into_owned())
}

/// Called when the thread is spawned
#[cfg(target_os = "windows")]
unsafe extern "system" fn init(_: *mut c_void) -> u32 {
    use std::{path::PathBuf, ffi::{c_char, CString, CStr}};

    ORIG_FUNCS_PTR = ORIGINAL_FUNCS.as_ptr();
    
    if let Some(dll_path) = get_dll_path() {
        println!("This DLL path: {}", &dll_path);
        let path = PathBuf::from(dll_path);
        let orig_dll_name = path.file_name().unwrap_or_else(|| {
            internal_failure!("Failed to get DLL name");
        });


        let system32_path = get_system32_path().unwrap_or_else(|| {
            internal_failure!("Failed to get system32 path");
        });

        let path = PathBuf::from(&system32_path).join(orig_dll_name);

        if !path.exists() {
            internal_failure!("Original DLL does not exist");
        }

        let mut path = path.to_str().unwrap_or_else(|| {
            internal_failure!("Failed to convert path to string");
        }).to_owned();
        path.push('\0');

        ORIG_DLL_HANDLE = Some(LoadLibraryA(path.as_ptr() as *const i8));
    } else {
        internal_failure!("Failed to get DLL path");
        return 1;
    }
    if let Some(orig_dll_handle) = ORIG_DLL_HANDLE {
        if orig_dll_handle.is_null() {
            internal_failure!("Failed to load original DLL");
        }
        println!("Original DLL handle: {:?}", orig_dll_handle);
    } else {
        let err = GetLastError();
        internal_failure!("Failed to load original DLL: {}", err);
    }
    load_dll_funcs();
    PROXYGEN_READY = true;
    0
}

/// Call this before attempting to call a function in the proxied DLL
/// 
/// This will wait for proxygen to fully load up all the proxied function addresses before returning
#[cfg(target_os = "windows")]
#[no_mangle]
pub extern "C" fn wait_dll_proxy_init() {
    //leftover from proxygen
}
