//! Entrypoint

#[cfg(target_os = "linux")]
use ctor::ctor;

#[cfg(target_os = "windows")]
use proxy_dll::proxy;

use crate::{core, internal_failure};

#[cfg(target_os = "linux")]
#[no_mangle]
#[ctor]
fn main(){
    core::init().unwrap_or_else(|e| {
        internal_failure!("Failed to initialize: {}", e);
    });
}

#[cfg(target_os = "windows")]
#[no_mangle]
#[proxy]
fn main() {
    core::init().unwrap_or_else(|e| {
        internal_failure!("Failed to initialize: {}", e);
    });
}

