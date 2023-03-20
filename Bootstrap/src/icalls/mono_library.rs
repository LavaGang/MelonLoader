use libc::c_void;
use unity_rs::{mono::{types::MonoReflectionAssembly}, runtime::{RuntimeType}};

use crate::{runtime, internal_failure};

pub fn get_lib_ptr() -> *mut c_void {
    let runtime = runtime!().unwrap_or_else(|e| {
        internal_failure!("Failed to get runtime: {e}");
    });

    if let RuntimeType::Mono(mono) = runtime.get_type() {
        mono.mono_lib.handle
    } else {
        internal_failure!("Game is not mono");
    }
}

pub fn cast_assembly_ptr(assembly: *mut c_void) -> *mut MonoReflectionAssembly {
    if assembly.is_null() {
        internal_failure!("Failed to cast assembly ptr: Assembly is null");
    }

    assembly.cast()
}

pub fn get_domain_ptr() -> *mut c_void {
    let runtime = runtime!().unwrap_or_else(|e| {
        internal_failure!("Failed to get runtime: {e}");
    });

    runtime.get_domain().unwrap_or_else(|e| {
        internal_failure!("Failed to get domain: {e}");
    }).inner
}