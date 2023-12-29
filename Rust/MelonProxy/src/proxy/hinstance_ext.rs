use std::{io, path::Path};

use super::windows_ext::get_module_path;
use libloading::Library;
use windows::Win32::Foundation::HINSTANCE;

pub trait ProxyDll {
    fn get_file_name(&self) -> Result<String, io::Error>;
    fn is_compatible(&self) -> Result<bool, io::Error>;
    fn load_original(&self) -> Result<Library, io::Error>;
}

impl ProxyDll for HINSTANCE {
    fn get_file_name(&self) -> Result<String, io::Error> {
        let path = get_module_path(*self)?;

        let file_name = path
            .file_name()
            .ok_or(io::Error::new(
                io::ErrorKind::NotFound,
                "Failed to get File Name",
            ))?
            .to_str()
            .ok_or(io::Error::new(
                io::ErrorKind::NotFound,
                "Failed to get File Name",
            ))?;

        Ok(file_name.to_lowercase().to_string())
    }

    fn is_compatible(&self) -> Result<bool, io::Error> {
        let file_name = self.get_file_name()?;

        Ok(file_name.eq("version.dll") || file_name.eq("winhttp.dll") || file_name.eq("winmm.dll"))
    }

    fn load_original(&self) -> Result<Library, io::Error> {

        let INVALID_FILE_NAME: io::Error =
            io::Error::new(io::ErrorKind::NotFound, "Invalid File Name");
        let LIBRARY_NOT_FOUND: io::Error =
            io::Error::new(io::ErrorKind::NotFound, "Failed to load original Proxy DLL");
        if !self.is_compatible()? {
            return Err(INVALID_FILE_NAME);
        }

        let name = self.get_file_name()?;
        let path = Path::new("C:\\Windows\\System32").join(name);

        if !path.exists() {
            return Err(LIBRARY_NOT_FOUND);
        }

        let lib = unsafe { Library::new(path) };

        match lib {
            Ok(lib) => Ok(lib),
            Err(_) => Err(LIBRARY_NOT_FOUND),
        }
    }
}
