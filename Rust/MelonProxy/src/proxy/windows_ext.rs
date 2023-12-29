use std::{io, path::PathBuf};

use windows::Win32::{
    Foundation::{HINSTANCE, MAX_PATH},
    System::LibraryLoader::GetModuleFileNameW,
};

pub fn get_module_path(module: HINSTANCE) -> Result<PathBuf, io::Error> {
    let mut path = [0u16; MAX_PATH as usize];

    let len = unsafe { GetModuleFileNameW(module, &mut path) };

    if len <= 0 {
        return Err(io::Error::new(
            io::ErrorKind::NotFound,
            "Failed to get File Name",
        ));
    }

    let str = String::from_utf16_lossy(&path[..len as usize]);

    Ok(PathBuf::from(str))
}
