use std::{error::Error, sync::{LazyLock, Mutex}, path::PathBuf};

use libloading::Library;

use crate::utils::files;

pub static BOOTSTRAP: LazyLock<Mutex<Option<Library>>> = LazyLock::new(|| Mutex::new(None));

pub fn init() -> Result<(), Box<dyn Error>> {
    let args: Vec<String> = std::env::args().collect();

    //TODO: Support UTF-16 (it will suck)
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