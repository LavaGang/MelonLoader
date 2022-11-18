//! various filesystem utils

use std::{ffi::{c_char, CStr}, os::windows::prelude::OsStringExt, path::PathBuf};

/// windows version
#[cfg(windows)]
pub mod windows {
    use std::path::PathBuf;

    use winapi::{
        shared::minwindef::{
            HMODULE, MAX_PATH
        }, 
        um::libloaderapi::GetModuleFileNameW
    };

    /// get the path to the current module
    pub fn get_module_file_path(module_handle: HMODULE) -> Result<[u16; MAX_PATH], Box<dyn std::error::Error>> {
        let mut path = [0u16; MAX_PATH];
        let len = unsafe {
            GetModuleFileNameW(
                module_handle,
                path.as_mut_ptr(),
                path.len() as u32,
            )
        };
        if len == 0 {
            return Err("GetModuleFileNameW failed".into());
        }
        Ok(path)
    }

    /// get system32 path
    pub fn get_system32_path() -> Result<PathBuf, Box<dyn std::error::Error>> {
        Ok(["c:\\", "windows", "system32"].iter().collect())
    }

    /// checks if file name is compatible on a pathbuf
    pub fn is_compatible(input: &str) -> Result<(bool, i32), Box<dyn std::error::Error>> {
        let path = PathBuf::from(input);

        let file_name = path.file_stem()
            .ok_or_else(|| "failed to get file name")?
            .to_str()
            .ok_or_else(|| "failed to get file name")?
            .to_lowercase();

        let compatible_names = vec![
            "psapi".to_string(),
            "version".to_string(),
            "winhttp".to_string(),
            "winmm".to_string(),
        ];

        for i in 0..compatible_names.len() {
            if file_name == compatible_names[i] {
                return Ok((true, i as i32));
            }
        }

        Ok((false, -1))
    }
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

/// get the path to bootstra
pub fn get_bootstrap_path(base_path: &PathBuf) -> Result<PathBuf, Box<dyn std::error::Error>> {
    let default = ["MelonLoader", "Dependencies", "Bootstrap"];
    let mut path = base_path.clone();

    // check for --melonloader.basepath arg
    let args: Vec<String> = std::env::args().collect();
    for i in 0..args.len() {
        if args[i] == "--melonloader.basepath" {
            if i + 1 >= args.len() {
                return Err("No path specified for --melonloader.basepath".into());
            }
            path = PathBuf::from(&args[i + 1]);
            break;
        }
    }

    path.extend(default.iter());

    path = path.with_extension(std::env::consts::DLL_EXTENSION);

    if !path.exists() {
        return Err("bootstrap not found!".into());
    }

    Ok(path)
}