//! Contains various utilities to reduce rendundant code.

pub mod assert;
pub mod files;
pub mod log;
pub mod debug;
pub mod libs;

#[cfg(target_os = "windows")]
pub mod console;