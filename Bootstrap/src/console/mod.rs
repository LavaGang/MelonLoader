pub mod os;

#[cfg(all(unix))]
use os::unix as imp;

#[cfg(all(windows))]
use os::windows as imp;

use crate::errors::DynErr;

pub fn init() -> Result<(), DynErr> {
    unsafe { imp::init() }
}

pub fn null_handles() -> Result<(), DynErr> {
    imp::null_handles()
}

pub fn set_handles() -> Result<(), DynErr> {
    imp::set_handles()
}

pub fn set_title(title: &str) {
    imp::set_title(title)
}
