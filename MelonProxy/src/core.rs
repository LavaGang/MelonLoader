use std::{
    error::Error,
    path::PathBuf,
    sync::{LazyLock, Mutex},
};

use libloading::Library;

use crate::utils::files;

pub static BOOTSTRAP: LazyLock<Mutex<Option<Library>>> = LazyLock::new(|| Mutex::new(None));

pub fn init() -> Result<(), Box<dyn Error>> {
    let args: Vec<String> = std::env::args().collect();

    //TODO: Support UTF-16 (it will suck)
    let mut base_dir = std::env::current_dir()?;
    let mut no_mods = false;

    let current_exe = std::env::current_exe()?;

    if !files::is_unity(&current_exe)? {
        return Ok(());
    }

    let game_name = current_exe
        .file_name()
        .ok_or("Failed to get game name")?
        .to_str()
        .ok_or("Failed to get game name")?;

    if game_name.starts_with("UnityCrashHandler64") || game_name.starts_with("UnityCrashHandler32")
    {
        return Ok(());
    }

	let game_dir = base_dir.clone();
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

        if arg.contains("--no-mods") {
            no_mods = true;
        }
    }

    //return Ok, and silently stop loading MelonLoader, if the user has specified to not load mods,
    //or if the game is not a Unity game
    if no_mods {
        return Ok(());
    }
	
	if cfg!(target_os = "windows") {
		//fix dobby_rs link
		let dobby_path = files::get_dobby_dir(base_dir.clone(), game_dir)?;
		add_dll_directory(dobby_path);
	}

    let bootstrap_path = files::get_bootstrap_path(&base_dir)?;

    unsafe {
        *BOOTSTRAP.try_lock()? = Some(Library::new(&bootstrap_path)?);
    }

    Ok(())
}

fn add_dll_directory(native_lib_dir: PathBuf) {
    // Determine the correct environment variable and separator
    let lib_var = if cfg!(target_os = "windows") { "PATH" } else { "LD_LIBRARY_PATH" };
    let separator = if cfg!(target_os = "windows") { ";" } else { ":" };

    // Get the current value of the library path
    let current_path = std::env::var(lib_var).unwrap_or_else(|_| String::new());

    // Append the new directory to the existing paths
    let new_path = if current_path.is_empty() {
        native_lib_dir.display().to_string()
    } else {
        format!("{}{}{}", native_lib_dir.display(), separator, current_path)
    };

    // Set the new value of the library path
    std::env::set_var(lib_var, new_path);
}