use std::{error, path::Path};

use super::{exports::ExportError, windows_ext::get_module_path};
use libloading::Library;
use windows::Win32::Foundation::HINSTANCE;

pub trait ProxyDll {
    fn get_file_name(&self) -> Result<String, Box<dyn error::Error>>;
    fn is_compatible(&self) -> Result<bool, Box<dyn error::Error>>;
    fn load_original(&self) -> Result<Library, Box<dyn error::Error>>;
}

impl ProxyDll for HINSTANCE {
    fn get_file_name(&self) -> Result<String, Box<dyn error::Error>> {
        let path = get_module_path(*self)?;

        let file_name = path
            .file_name()
            .ok_or(ExportError::GetModuleName)?
            .to_str()
            .ok_or(ExportError::GetModuleName)?;
        
        Ok(file_name.to_lowercase().to_string())
    }

    fn is_compatible(&self) -> Result<bool, Box<dyn error::Error>> {
        let file_name = self.get_file_name()?;

        Ok(file_name.eq("version.dll") || file_name.eq("winhttp.dll") || file_name.eq("winmm.dll"))
    }

    fn load_original(&self) -> Result<Library, Box<dyn error::Error>> {
        if !self.is_compatible()? {
            return Err(ExportError::InvalidFileName(self.get_file_name()?).into());
        }

        let name = self.get_file_name()?;
        let path = Path::new("C:\\Windows\\System32").join(name);

        match path.exists() {
            true => unsafe { Ok(Library::new(path)?) },

            false => Err(ExportError::LibraryNotFound.into()),
        }
    }
}
