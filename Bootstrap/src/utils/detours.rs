//! dynamic dobby wrapper

use std::{
    error, mem::transmute
};

use thiserror::Error;

/// dobby errors
#[derive(Debug, Error)]
pub enum DobbyError {
    /// failed to load dobby
    #[error("Failed to load Dobby!")]
    FailedToLoadDobby,

    /// failed to get dobby path
    #[error("Failed to get Dobby path!")]
    FailedToGetDobbyPath,

    /// failed to get dobby name
    #[error("Failed to get Function!")]
    FailedToGetFunction,

    /// failed to hook function
    #[error("Failed to hook Function!")]
    FailedToHookFunction,

    /// failed to unhook function
    #[error("Failed to unhook Function!")]
    FailedToUnhookFunction,
}

/// hook a function pointer
pub fn hook(target: usize, replacement: usize) -> Result<&'static (), Box<dyn error::Error>> {
    use dobby_rs::{Address};

    unsafe {
        let res = dobby_rs::hook(target as Address, replacement as Address)?;
        Ok(transmute(res))
    }
}

/// hook a function pointer
pub fn unhook(target: usize) -> Result<(), Box<dyn error::Error>> {
    use dobby_rs::Address;

    unsafe {
        dobby_rs::unhook(target as Address)?;
    }

    Ok(())
}
