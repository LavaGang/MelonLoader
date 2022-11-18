//! the core logic of the proxy

use std::{error, path::PathBuf};

use libloading::Library;
#[cfg(windows)]
use winapi::shared::minwindef::{HINSTANCE};
use crate::utils::files::get_bootstrap_path;
#[cfg(windows)]
use crate::{utils::files::{rust_str}, exports};

/// initialize
#[cfg(windows)]
pub fn init(hinst_dll: HINSTANCE) -> Result<(), Box<dyn error::Error>> {
    use crate::utils::files;

    let path = std::env::current_exe()?;

    let path_to_exe = path.to_str().ok_or_else(|| "failed to get path to exe")?;

    if path_to_exe.contains("steam-launch-wrapper") {
        return Ok(());
    }


    let file_name = files::windows::get_module_file_path(hinst_dll)?;
    let file_name = rust_str(&file_name)?;

    let (is_compatible, index) = files::windows::is_compatible(&file_name)?;

    if !is_compatible {
        return Err("Proxy has an incompatible file name!".into());
    }

    let original = load_original_proxy(index)?;


    exports::load(original, index)?;

    init_common()
}

#[cfg(windows)]
fn load_original_proxy(index: i32) -> Result<HINSTANCE, Box<dyn error::Error>> {
    use winapi::um::libloaderapi::LoadLibraryA;

    use crate::utils::files::{win_str};

    let system32_cstr = match index {
        0 => win_str(b"C:\\Windows\\System32\\psapi.dll\0"),
        1 => win_str(b"C:\\Windows\\System32\\version.dll\0"),
        2 => win_str(b"C:\\Windows\\System32\\winhttp.dll\0"),
        3 => win_str(b"C:\\Windows\\System32\\winmm.dll\0"),
        _ => return Err("Invalid Proxy Filename!".into()),
    };

    let original = unsafe { LoadLibraryA(system32_cstr) };

    if original.is_null() {
        return Err("Failed to load original proxy!".into());
    }

    Ok(original)
}

static mut BOOTSTRAP: Option<Library> = None;

fn init_common() -> Result<(), Box<dyn error::Error>> {
    let file_path = std::env::current_exe()?;

    if !check_unity(&file_path)? {
        return Ok(());
    }

    let args: Vec<String> = std::env::args().collect();
    for i in 0..args.len() {
        if args[i] == "--melonloader.no-mods" {
            return Ok(());
        }
    }

    let base_path = file_path.parent().ok_or_else(|| "failed to get base path")?.to_path_buf();

    let bootstrap_path = get_bootstrap_path(&base_path)?;

    unsafe {
        BOOTSTRAP = Some(Library::new(bootstrap_path)?);
    }

    Ok(())
}

fn check_unity(file_path: &PathBuf) -> Result<bool, Box<dyn error::Error>> {
    let file_name = file_path.file_stem()
        .ok_or_else(|| "failed to get file name")?
        .to_str()
        .ok_or_else(|| "failed to get file name")?
        .to_lowercase();

    let base_folder = file_path.parent()
        .ok_or_else(|| "failed to get base folder")?;

    let data_path = base_folder.join(format!("{}_Data", file_name));

    if !data_path.exists() {
        return Ok(false)
    }

    let global_game_managers = data_path.join("globalgamemanagers");
    let data_unity3d = data_path.join("data.unity3d");
    let main_data = data_path.join("mainData");

    if global_game_managers.exists() || data_unity3d.exists() || main_data.exists() {
        Ok(true)
    } else {
        Ok(false)
    }
}

/// initialize
#[cfg(not(windows))]
pub fn init() -> Result<(), Box<dyn error::Error>> {
    init_common()
}