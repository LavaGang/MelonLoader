use std::{path::Path, str::FromStr, error::Error};

use netcorehost::pdcstring::PdCString;

#[macro_export]
macro_rules! win_str {
    ($s:expr) => {
        std::ffi::CStr::from_bytes_with_nul_unchecked($s).as_ptr()
    };
}

#[macro_export]
macro_rules! rust_str {
    ($s:expr) => {
        unsafe { std::ffi::CStr::from_ptr($s) }.to_str()
    };
}

pub fn pdcstr<P: AsRef<Path>>(path: P) -> Result<PdCString, Box<dyn Error>> {
    Ok(PdCString::from_str(
        path.as_ref()
            .to_str()
            .ok_or("Failed to convert path to string!")?,
    )?)
}

pub fn wide_str<P: AsRef<Path>>(path: P) -> Result<Vec<u16>, Box<dyn Error>> {
    let s = path
        .as_ref()
        .to_str()
        .ok_or("Failed to convert path to string!")?;

    Ok(s.encode_utf16().chain(Some(0)).collect())
}
