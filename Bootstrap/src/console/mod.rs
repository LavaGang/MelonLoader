pub mod os;

#[cfg(all(unix))]
use os::unix as imp;

#[cfg(all(windows))]
use os::windows as imp;

use crate::{errors::DynErr, hide_console};

pub fn init() -> Result<(), DynErr> {
    if hide_console!() {
        return Ok(());
    }

    unsafe { imp::init() }
}

pub fn null_handles() -> Result<(), DynErr> {
    if hide_console!() {
        return Ok(());
    }

    imp::null_handles()
}

pub fn set_handles() -> Result<(), DynErr> {
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
