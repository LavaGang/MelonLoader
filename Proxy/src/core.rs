//! the core logic of the proxy

use crate::utils::files;
use clap::Parser;
use lazy_static::lazy_static;
use libloading::Library;
use std::{error, path::PathBuf, sync::Mutex};

#[derive(Parser)]
struct Arguments {
    #[arg(long = "no-mods", default_value = "false")]
    no_mods: bool,

    #[arg(long = "melonloader.basedir")]
    base_dir: Option<String>,
}

lazy_static!(
    static ref BOOTSTRAP: Mutex<Option<Library>> = Mutex::new(None);
);

pub fn init() -> Result<(), Box<dyn error::Error>> {
    let file_path = std::env::current_exe()?;

    if !files::is_unity(&file_path)? {
        return Ok(());
    }

    let args = Arguments::parse_optimistic()?;

    //return Ok, and silently stop loading MelonLoader, if the user has specified to not load mods,
    //or if the game is not a Unity game
    if args.no_mods {
        return Ok(());
    }

    let base_path = match args.base_dir {
        Some(path) => PathBuf::from(path),
        None => std::env::current_dir()?,
    };

    let bootstrap_path = files::get_bootstrap_path(&base_path)?;

    unsafe {
        *BOOTSTRAP.try_lock()? = Some(Library::new(&bootstrap_path)?);
    }

    Ok(())
}
