use std::ffi::c_void;

use crate::{error, hooks::{self, NativeHook}};

pub unsafe fn attach(target: *mut *mut c_void, detour: *mut c_void) {
    // match NativeHook::<fn()>::new(*target as usize, detour as usize).hook() {
    //     Ok(res) => *target = res.trampoline as *mut c_void,
    //     Err(e) => {
    //         let _ = error!("Failed to hook function: {}", e.to_string());
    //     }
    // };

    let mut hook = NativeHook::<fn()>::new(*target, detour);
    match hook.hook() {
        Ok(_) => *target = hook.trampoline as *mut c_void,
        Err(e) => {
            let _ = error!("Failed to hook function: {}", e.to_string());
        }
    };
}

pub unsafe fn detach(target: *mut *mut c_void, _detour: *mut c_void) {
    hooks::functions::unhook(*target as usize).unwrap_or_else(|e| {
        let _ = error!("Failed to unhook function: {}", e.to_string());
    });
}
