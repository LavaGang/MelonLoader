#![feature(fn_ptr_trait)]
#![feature(lazy_cell)]

#![allow(non_snake_case)]
#![deny(
    missing_debug_implementations,
    unused_results,
    warnings,
    clippy::extra_unused_lifetimes,
    clippy::from_over_into,
    clippy::needless_borrow,
    clippy::new_without_default,
    clippy::useless_conversion
)]
#![forbid(rust_2018_idioms)]
#![allow(clippy::inherent_to_string, clippy::type_complexity, improper_ctypes)]
#![cfg_attr(docsrs, feature(doc_cfg))]

#[cfg(target_os = "windows")]
pub use windows::Win32::Foundation::HINSTANCE;

#[cfg(target_os = "windows")]
pub mod proxy;
pub mod utils;
pub mod core;

/// this function will get called by our proxy macro. See MelonProxy-sys
#[cfg_attr(
    not(target_os = "windows"),
    ctor::ctor
)]
#[cfg_attr(
    target_os = "windows",
    melon_proxy_sys::proxy
)]
#[allow(dead_code)]
fn main() {
    core::init().unwrap_or_else(|e| {
        internal_failure!("Failed to initialize MelonLoader: {}", e);
    });
}