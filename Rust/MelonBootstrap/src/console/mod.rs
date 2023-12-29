pub mod os;

use std::error::Error;

#[cfg(not(target_os = "windows"))]
use os::not_windows as imp;

#[cfg(target_os = "windows")]
use os::windows as imp;

use crate::{hide_console};

pub fn init() -> Result<(), Box<dyn Error>> {
    if hide_console!() {
        return Ok(());
    }

    unsafe { imp::init() }
}

pub fn null_handles() -> Result<(), Box<dyn Error>> {
    if hide_console!() {
        return Ok(());
    }

    imp::null_handles()
}

pub fn set_handles() -> Result<(), Box<dyn Error>> {
    if hide_console!() {
        return Ok(());
    }

    imp::set_handles()
}

pub fn set_title(title: &str) {
    if hide_console!() {
        return;
    }

    imp::set_title(title)
}