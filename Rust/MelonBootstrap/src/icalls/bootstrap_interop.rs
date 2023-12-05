use std::{ffi::c_void, ptr::null_mut};

use crate::{
    error,
    hooking::{self, NativeHook},
    internal_failure,
    logging::logger,
};

pub unsafe fn attach(target: *mut c_void, detour: *mut c_void) -> *mut c_void {
    let mut hook = NativeHook::<fn()>::new(target, detour);

    match hook.hook() {
        Ok(_) => hook.trampoline as *mut c_void,
        Err(e) => {
            let _ = error!("Failed to unhook function: {}", e.to_string());
            null_mut()
        }
    }
}

pub unsafe fn detach(target: *mut c_void) {
    hooking::functions::unhook(target as usize).unwrap_or_else(|e| {
        let _ = error!("Failed to unhook function: {}", e.to_string());
    });
}

pub fn write_log_file(text: *mut u8, length: i32) {
    logger::write_bytes(unsafe { std::slice::from_raw_parts(text, length as usize) })
        .unwrap_or_else(|e| internal_failure!("Failed to write log file: {}", e.to_string()));
}
