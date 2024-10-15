use std::path::{PathBuf};

use lazy_static::lazy_static;
use unity_rs::runtime::RuntimeType;

use crate::{errors::DynErr, internal_failure, runtime, constants::W};



lazy_static! {
    pub static ref BASE_DIR: W<PathBuf> = {
        let args: Vec<String> = std::env::args().collect();
        let mut base_dir = std::env::current_dir().unwrap_or_else(|e|internal_failure!("Failed to get base dir: {e}"));
        let mut iterator = args.iter();
		while let Some(mut arg) = iterator.next() {
            if arg.starts_with("--melonloader.basedir") {
                if arg.contains("=") {
					let a: Vec<&str> = arg.split("=").collect();
					base_dir = PathBuf::from(a[1]);
				}
				else {
					arg = iterator.next().unwrap();
					base_dir = PathBuf::from(arg);
				}
            }
        }

        W(base_dir)
    };
    pub static ref GAME_DIR: W<PathBuf> = {
        let base_dir = std::env::current_dir().unwrap_or_else(|e|internal_failure!("Failed to get game dir: {e}"));
        W(base_dir)
    };
    pub static ref MELONLOADER_FOLDER: W<PathBuf> = W(BASE_DIR.join("MelonLoader"));
    pub static ref DEPENDENCIES_FOLDER: W<PathBuf> = W(MELONLOADER_FOLDER.join("Dependencies"));
    pub static ref SUPPORT_MODULES_FOLDER: W<PathBuf> = W(DEPENDENCIES_FOLDER.join("SupportModules"));
    pub static ref PRELOAD_DLL: W<PathBuf> = W(SUPPORT_MODULES_FOLDER.join("Preload.dll"));
}

pub fn runtime_dir() -> Result<PathBuf, DynErr> {
    let runtime = runtime!()?;

    let mut path = MELONLOADER_FOLDER.clone();

    //let version = runtime::get_netstandard_version()?;

    match runtime.get_type() {
        RuntimeType::Mono(_) => path.push("net35"),
        RuntimeType::Il2Cpp(_) => path.push("net6"),
    }

    Ok(path.to_path_buf())
}

pub fn get_managed_dir() -> Result<PathBuf, DynErr> {
    let file_path = std::env::current_exe()?;

    let file_name = file_path
        .file_stem()
        .ok_or_else(|| "Failed to get File Stem!")?
        .to_str()
        .ok_or_else(|| "Failed to get File Stem!")?;

    let base_folder = file_path.parent().ok_or_else(|| "Data Path not found!")?;

    let managed_path = base_folder
        .join(format!("{}_Data", file_name))
        .join("Managed");

    match managed_path.exists() {
        true => Ok(managed_path),
        false => {
            let managed_path = base_folder.join("MelonLoader").join("Dependencies").join("Mono");

            match managed_path.exists() {
                true => Ok(managed_path),
                false => Err("Failed to find the Managed directory!")?,
            }
        }
    }
}
