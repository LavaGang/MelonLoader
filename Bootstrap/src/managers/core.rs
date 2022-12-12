use std::{error::Error};

use unity_rs::runtime::Runtime;

use crate::{utils::log, debug};

use super::hooks;

pub fn version() -> &'static str {
    "0.6.0"
}

pub fn is_alpha() -> bool {
    false
}

pub fn init() -> Result<(), Box<dyn Error>> {
    log::init()?;

    debug!("Logging initialized")?;

    #[cfg(windows)]
    {
        use crate::utils::console;
        console::init()?;
    }

    hooks::init(
        Runtime::new()?.runtime
    )?;

    #[cfg(windows)]
    {
        use crate::utils::console;
        console::null_handles();
    }
    Ok(())
}