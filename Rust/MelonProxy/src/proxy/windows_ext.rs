use std::{error::Error, path::PathBuf};

use windows::Win32::{
    Foundation::{MAX_PATH, HINSTANCE},
    System::LibraryLoader::GetModuleFileNameW,
};

pub fn get_module_path(module: HINSTANCE) -> Result<PathBuf, Box<dyn Error>> {
    let mut path = [0u16; MAX_PATH as usize];

    let len = unsafe { GetModuleFileNameW(module, &mut path) };

    if len <= 0 {
        return Err("GetModuleFileNameW failed".into());
    }

    let str = String::from_utf16_lossy(&path[..len as usize]);

    Ok(PathBuf::from(str))
}
