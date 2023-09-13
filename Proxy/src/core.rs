//! the core logic of the proxy

use crate::utils::files;
use lazy_static::lazy_static;
use libloading::Library;
use std::{error, path::PathBuf, sync::Mutex};

lazy_static! {
    static ref BOOTSTRAP: Mutex<Option<Library>> = Mutex::new(None);
}

pub fn init() -> Result<(), Box<dyn error::Error>> {
    let file_path = std::env::current_exe()?;

    if !files::is_unity(&file_path)? {
        return Ok(());
    }

    let args: Vec<String> = std::env::args().collect();
    let mut base_dir = std::env::current_dir()?;
    let mut no_mods = false;

    for arg in args.iter() {
        if arg.starts_with("--melonloader.basedir") {
            let a: Vec<&str> = arg.split("=").collect();
            base_dir = PathBuf::from(a[1]);
        }

        if arg.contains("--no-mods") {
            no_mods = true;
        }
    }
    

    //return Ok, and silently stop loading MelonLoader, if the user has specified to not load mods,
    //or if the game is not a Unity game
    if no_mods {
        return Ok(());
    }

    let bootstrap_path = files::get_bootstrap_path(&base_dir)?;

    unsafe {
        *BOOTSTRAP.try_lock()? = Some(Library::new(&bootstrap_path)?);
    }

    Ok(())
}
