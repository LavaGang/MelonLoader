#![allow(non_snake_case)]

use ctor::ctor;
use managers::core;

pub mod utils;
pub mod managers;

#[ctor]
fn init() {
    core::init().unwrap_or_else(|e| {
        internal_failure!("Failed to initialize Bootstrap: {}", e);
    })
}
