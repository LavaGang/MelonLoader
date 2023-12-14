use std::{env::consts::DLL_EXTENSION, path::PathBuf};

use super::errors::ProxyError;

/// search for Bootstrap in the given path
pub fn get_bootstrap_path(base_path: &PathBuf) -> Result<PathBuf, ProxyError> {
    let bootstrap_names = ["Bootstrap", "libmelon_bootstrap"]; //by convention, on unix, the library is prefixed with "lib"

    let path = base_path.join("MelonLoader").join("Dependencies");

    for name in bootstrap_names.iter() {
        let bootstrap_path = path.join(name).with_extension(DLL_EXTENSION);

        if bootstrap_path.exists() {
            return Ok(bootstrap_path);
        }
    }

    Err(ProxyError::BootstrapNotFound(base_path.to_owned()))
}