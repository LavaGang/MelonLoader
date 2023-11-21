use std::{ffi::c_void, ptr::null_mut};

use crate::{error, hooking::{self, NativeHook}};

pub unsafe fn attach(target: *mut c_void, detour: *mut c_void) -> *mut c_void {
    // match NativeHook::<fn()>::new(*target as usize, detour as usize).hook() {
    //     Ok(res) => *target = res.trampoline as *mut c_void,
    //     Err(e) => {
    //         let _ = error!("Failed to hook function: {}", e.to_string());
    //     }
    // };

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
