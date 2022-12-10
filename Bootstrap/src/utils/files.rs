//! Contains various utilities for files

use std::{error::Error, path::PathBuf, env};

use unity_rs::runtime::{Runtime, UnityRuntime};

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

/// gets the game's base directory.
pub fn base_dir() -> Result<PathBuf, Box<dyn Error>> {
    let app_path = env::current_exe()?;

    let base_path = app_path.parent().ok_or("Failed to get the base path!")?;

    match base_path.exists() {
        true => Ok(base_path.to_path_buf()),
        false => Err("The base path does not exist!")?,
    }
}

pub fn managed_dir() -> Result<PathBuf, Box<dyn Error>> {
    let file_path = std::env::current_exe()?;

    let file_name = file_path.file_stem()
        .ok_or_else(|| "Data Path not found!")?
        .to_str()
        .ok_or_else(|| "Data Path not found!")?;

    let base_folder = file_path.parent()
        .ok_or_else(|| "Data Path not found!")?;

    let managed_path = base_folder.join(format!("{}_Data", file_name)).join("Managed");

    match managed_path.exists() {
        true => Ok(managed_path),
        false => {
            let managed_path = base_folder.join("MelonLoader").join("Managed");

            match managed_path.exists() {
                true => Ok(managed_path),
                false => Err("Failed to find the managed directory!")?,
            }
        }
    }
}

pub fn melonloader_dir() -> Result<PathBuf, Box<dyn Error>> {
    let melonloader_path = base_dir()?.join("MelonLoader");

    match melonloader_path.exists() {
        true => Ok(melonloader_path),
        false => Err("MelonLoader Folder does not exist!")?,
    }
}

pub fn runtime_dir() -> Result<PathBuf, Box<dyn Error>> {
    let runtime_dir = melonloader_dir()?;

    let runtime = Runtime::new()?;

    match runtime.runtime {
        UnityRuntime::MonoRuntime(_) => Ok(runtime_dir.join("net35")),
        UnityRuntime::Il2Cpp(_) => Ok(runtime_dir.join("net6")),
    }
}