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

use melon_proxy_sys::proxy;
pub use windows::Win32::Foundation::HINSTANCE;

pub mod proxy;
pub mod utils;
pub mod core;

#[proxy]
fn main() {
    core::init().unwrap_or_else(|e| {
        internal_failure!("Failed to initialize MelonLoader: {}", e);
    });
}