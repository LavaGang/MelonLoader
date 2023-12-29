use std::{
    env::consts::DLL_EXTENSION,
    io::{self, Error, ErrorKind},
    path::PathBuf,
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

    Err(Error::new(
        ErrorKind::NotFound,
        "Failed to find MelonLoader Bootstrap",
    ))
}
