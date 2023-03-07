//! the core logic of the proxy

use crate::utils::files;
use clap::{Parser};
use libloading::Library;
use std::{error, path::PathBuf};


#[derive(Parser)]
#[command(author, version, about, long_about = None)]
struct Cli {
    #[arg(long)]
    no_mods: bool,
    #[arg(long="melonloader.basedir")]
    base_dir: Option<String>,
}

static mut BOOTSTRAP: Option<Library> = None;

pub unsafe fn init() -> Result<(), Box<dyn error::Error>> {
    let args = Cli::parse();
    let file_path = std::env::current_exe()?;

    //return Ok, and silently stop loading MelonLoader, if the user has specified to not load mods,
    //or if the game is not a Unity game
    if args.no_mods || !files::is_unity(&file_path)? {
        return Ok(());
    }

    let base_path = match args.base_dir {
        Some(path) => PathBuf::from(path),
        None => std::env::current_dir()?,
    };

    let bootstrap_path = files::get_bootstrap_path(&base_path)?;

    BOOTSTRAP = Some(Library::new(bootstrap_path)?); //needs to be stored, so it won't get unloaded when this function returns.

    Ok(())
}
