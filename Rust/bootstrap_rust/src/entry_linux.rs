//! Entrypoint for our Bootstrap.
 
#![cfg(unix)]

use ctor::ctor;

use crate::{managers::core, internal_failure};

/// entrypoint
#[no_mangle]
#[ctor]
fn main() {
    core::init()
    .unwrap_or_else(
        |e| 
        internal_failure!("Failed to initialize the core manager! {}", e)
    );
}