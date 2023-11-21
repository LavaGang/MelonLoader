use std::ffi::c_char;

use dobby_rs::Address;

pub type CharPointer = *mut c_char;
pub type LoadLibraryFn = fn(*mut c_char) -> Address;