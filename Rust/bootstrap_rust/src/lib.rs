//! Cross platform reimplementation of MelonLoader's Bootstrap in rust.
#![deny(
    missing_debug_implementations,
    missing_docs,
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

#[cfg_attr(target_os = "linux", path = "entry_linux.rs")]
#[cfg_attr(windows, path = "entry_windows.rs")]
pub mod entry;

pub mod managers;
pub mod utils;