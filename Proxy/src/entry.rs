//! These are platform specific init functions.
//! On Windows, we need to "proxy" a built-in Windows library.
//! The crate 'proxy-dll' takes care of this, dynamically proxying 3
//! different dlls, depending on the file name of the compiled binary.
//!
//! See https://github.com/RinLovesYou/dll-proxy-rs/
//!
//! On Linux, injection is done through LD_PRELOAD, so there's no need to proxy anything.
//! there we just use `ctor`.

#[cfg(not(target_os = "windows"))]
use ctor::ctor;

#[cfg(target_os = "windows")]
use proxy_dll::proxy;

use crate::{core, internal_failure};

#[cfg(not(target_os = "windows"))]
#[no_mangle]
#[ctor]
fn main() {
    core::init().unwrap_or_else(|e| {
        internal_failure!("Failed to initialize MelonLoader: {}", e);
    });
}

#[cfg(target_os = "windows")]
#[no_mangle]
#[proxy]
fn main() {
    core::init().unwrap_or_else(|e| {
        internal_failure!("Failed to initialize MelonLoader: {}", e);
    });
}
