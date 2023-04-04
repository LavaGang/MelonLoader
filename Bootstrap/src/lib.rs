#![feature(is_some_and)]

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

pub mod base_assembly;
pub mod console;
pub mod constants;
pub mod errors;
pub mod hooks;
pub mod icalls;
pub mod logging;
pub mod melonenv;
pub mod utils;

pub mod core;
