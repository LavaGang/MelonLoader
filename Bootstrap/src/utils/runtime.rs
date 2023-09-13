use std::{error::Error, collections::HashMap, io, path::Path};

use exe::{ImageDirectoryEntry, ResourceDirectory, VecPE, PE, ImportDirectory, ImportData, CCharString, ImageDataDirectory, ImageResourceDirectory};
use unity_rs::runtime::FerrexRuntime;

use crate::{errors::DynErr, log, melonenv::paths};

#[allow(dead_code)]
pub static mut RUNTIME: Option<FerrexRuntime> = None;

#[macro_export]
macro_rules! runtime {
    () => {
        $crate::utils::runtime::get_runtime()
    };
}

pub fn get_runtime() -> Result<&'static FerrexRuntime, DynErr> {
    unsafe {
        if RUNTIME.is_none() {
            RUNTIME = Some(unity_rs::runtime::get_runtime()?)
        }

        Ok(RUNTIME.as_ref().ok_or("Failed to get runtime")?)
    }
}

#[derive(Debug)]
pub enum NetstandardVersion {
    Old,
    New,
}

impl Default for PeFileInfo {
    fn default () -> PeFileInfo {
        PeFileInfo {
            product_version: String::new(),
            original_filename: String::new(),
            file_description: String::new(),
            file_version: String::new(),
            product_name: String::new(),
            company_name: String::new(),
            internal_name: String::new(),
            legal_copyright: String::new()
        }
    }
}
#[derive(Clone)]
pub struct PeFileInfo {
    pub product_version: String,
    pub original_filename: String,
    pub file_description: String,
    pub file_version: String,
    pub product_name: String,
    pub company_name: String,
    pub internal_name: String,
    pub legal_copyright: String
}

pub fn get_netstandard_version() -> Result<NetstandardVersion, Box<dyn Error>> {
    let netstandard = paths::get_managed_dir()?.join("netstandard.dll");
    if !netstandard.exists() {
        return Ok(NetstandardVersion::Old);
    }

    let file_info = get_pe_file_info(&netstandard)?;

    match file_info.file_version.as_str() {
        "2.1.0.0" => Ok(NetstandardVersion::New),
        _ => Ok(NetstandardVersion::Old)
    }
}

fn get_pe_file_info(path: &Path) -> io::Result<PeFileInfo> {
    let mut file_info = PeFileInfo::default();
    let Ok(pefile) = exe::VecPE::from_disk_file(path) else { return Ok(file_info) };
    let Ok(vs_version_check) = exe::VSVersionInfo::parse(&pefile) else { return Ok(file_info) };
    let vs_version = vs_version_check;
    if let Some(string_file_info) = vs_version.string_file_info {
        let Ok(string_map) = string_file_info.children[0].string_map() else { return Ok(file_info) };
        file_info.product_version = get_hashmap_value(&string_map, "ProductVersion")?;
        file_info.original_filename = get_hashmap_value(&string_map, "OriginalFilename")?;
        file_info.file_description = get_hashmap_value(&string_map, "FileDescription")?;
        file_info.file_version = get_hashmap_value(&string_map, "FileVersion")?;
        file_info.product_name = get_hashmap_value(&string_map, "ProductName")?;
        file_info.company_name = get_hashmap_value(&string_map, "CompanyName")?;
        file_info.internal_name = get_hashmap_value(&string_map, "InternalName")?;
        file_info.legal_copyright = get_hashmap_value(&string_map, "LegalCopyright")?;
    }
    Ok(file_info)
}


fn get_hashmap_value(
    string_map: &HashMap<String, String>, 
    value_name: &str
    ) -> io::Result<String> 
{
let _v = match string_map.get(value_name) {
Some(v) => return Ok(v.to_string()),
None => return Ok("".to_string())
};
}