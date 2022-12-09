//! Contains various utilities for files

use std::{error::Error, path::PathBuf, ffi::{CStr, c_char}, os::windows::prelude::OsStringExt};

/// evaluates wether or not a path contains non-ASCII characters.
pub fn check_ascii(path: &PathBuf) -> Result<(), Box<dyn Error>> {
    let base_string = path.to_str().ok_or("Failed to turn PathBuf into a string!")?;

    if base_string.is_ascii() {
        Ok(())
    } else {
        Err("The base directory path contains non-ASCII characters!")?
    }
}

/// joins a path with a file name, and appends the platform specific extension.
/// 
/// # Arguments
/// 
/// * `path` - The path to join with the file name.
/// * `file_name` - The file name to join with the path.
/// 
/// # Example
/// 
/// ```
/// use std::path::PathBuf;
/// 
/// use crate::utils::files::join_path;
/// 
/// let path = PathBuf::from("C:\\Users\\User\\Desktop");
/// let file_name = "test";
/// 
/// let joined_path = join_path(&path, file_name);
/// 
/// #[cfg(target_os = "windows")]
/// assert_eq!(joined_path, PathBuf::from("C:\\Users\\User\\Desktop\\test.dll"));
/// #[cfg(target_os = "linux")]
/// assert_eq!(joined_path, PathBuf::from("C:\\Users\\User\\Desktop\\test.so"));
/// #[cfg(target_os = "macos")]
/// assert_eq!(joined_path, PathBuf::from("C:\\Users\\User\\Desktop\\test.dylib"));
#[macro_export]
macro_rules! join_dll_path {
    //take two arguments, a PathBuf and a file name
    ($path:expr, $file:expr) => {
        //join the path and the file name
        $path.join($file)
        //add the correct extension
        .with_extension(std::env::consts::DLL_EXTENSION)
    };
}

/// turn a rust string into a windows string
pub fn win_str(bytes: &[u8]) -> *const c_char {
	unsafe { CStr::from_bytes_with_nul_unchecked(bytes).as_ptr() }
}

/// turn a windows string into a rust string
pub fn rust_str(to_convert: &[u16]) -> Result<String, Box<dyn std::error::Error>> {
    std::ffi::OsString::from_wide(to_convert)
        .into_string()
        .map_err(|_| "failed to convert to string".into())
}