use std::{
    env::consts::DLL_EXTENSION, error::Error, io::{self, ErrorKind}, path::PathBuf
};

/// search for Bootstrap in the given path
pub fn get_bootstrap_path(base_path: &PathBuf) -> Result<PathBuf, io::Error> {
    let bootstrap_names = ["Bootstrap", "libBootstrap"]; //by convention, on unix, the library is prefixed with "lib"

    let path = base_path.join("MelonLoader").join("Dependencies");

    for name in bootstrap_names.iter() {
        let bootstrap_path = path.join(name).with_extension(DLL_EXTENSION);

        if bootstrap_path.exists() {
            return Ok(bootstrap_path);
        }
    }

    Err(io::Error::new(
        ErrorKind::NotFound,
        "Failed to find MelonLoader Bootstrap",
    ))
}

/// search for Bootstrap in the given path
pub fn get_dobby_dir(base_path: PathBuf, game_dir: PathBuf) -> Result<PathBuf, io::Error> {
    let dobby_names = ["dobby", "libdobby"]; //by convention, on unix, the library is prefixed with "lib"
	
    for name in dobby_names.iter() {
        let dobby_path = base_path.join(name).with_extension(DLL_EXTENSION);

        if dobby_path.exists() {
            return Ok(base_path);
        }
    }
	
    let mut path = base_path.join("MelonLoader");
    for name in dobby_names.iter() {
        let dobby_path = path.join(name).with_extension(DLL_EXTENSION);

        if dobby_path.exists() {
            return Ok(path);
        }
    }
	
    path = base_path.join("MelonLoader").join("Dependencies");
    for name in dobby_names.iter() {
        let dobby_path = path.join(name).with_extension(DLL_EXTENSION);

        if dobby_path.exists() {
            return Ok(path);
        }
    }
	
    for name in dobby_names.iter() {
        let dobby_path = game_dir.join(name).with_extension(DLL_EXTENSION);

        if dobby_path.exists() {
            return Ok(game_dir);
        }
    }

    Err(io::Error::new(
        ErrorKind::NotFound,
        "Failed to find MelonLoader dobby",
    ))
}

pub fn is_unity(file_path: &PathBuf) -> Result<bool, Box<dyn Error>> {
    let file_name = file_path
        .file_stem()
        .ok_or("Failed to get file stem")?
        .to_str()
        .ok_or("Failed to get file stem")?;

    let base_folder = file_path.parent().ok_or("Failed to get Directory Parent")?;

    let data_path = base_folder.join(format!("{}_Data", file_name));

    if !data_path.exists() {
        return Ok(false);
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