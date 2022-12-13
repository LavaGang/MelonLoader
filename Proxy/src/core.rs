//! the core logic of the proxy

use std::{error, path::PathBuf};

use crate::{utils::files::get_bootstrap_path, libs::{NativeLibrary, load_lib}};

static mut BOOTSTRAP: Option<NativeLibrary> = None;

/// The Entrypoint
pub fn init() -> Result<(), Box<dyn error::Error>> {
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
        BOOTSTRAP = Some(load_lib(bootstrap_path)?);
    }

    Ok(())
}

fn check_unity(file_path: &PathBuf) -> Result<bool, Box<dyn error::Error>> {
    let file_name = file_path.file_stem()
        .ok_or_else(|| "failed to get file name")?
        .to_str()
        .ok_or_else(|| "failed to get file name")?;


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