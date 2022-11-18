//! Entrypoint

#![cfg(unix)]

use ctor::ctor;

use crate::{core, internal_failure};

#[no_mangle]
#[ctor]
fn main(){
    core::init().unwrap_or_else(|e| {
        internal_failure!("Failed to initialize: {}", e);
    });

    println!("Hello, world!");
}