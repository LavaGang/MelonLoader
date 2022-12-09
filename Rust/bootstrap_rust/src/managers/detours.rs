//! dynamic dobby wrapper

use std::{
    error, collections::HashMap, sync::Mutex, mem::transmute
};

use detour::RawDetour;
use thiserror::Error;

use crate::{internal_failure};

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

lazy_static::lazy_static! {
    static ref HOOKMAP: Mutex<HashMap<usize, RawDetour>> = Mutex::new(HashMap::new());
}

/// hook a function pointer
pub fn hook(target: usize, replacement: usize) -> Result<&'static (), Box<dyn error::Error>> {
    unsafe {
        let mut m = HOOKMAP.lock()?;

        if m.contains_key(&target) {
            let hook = m.get(&target)
            .unwrap_or_else(|| internal_failure!("Failed to get hook!"));

            return Ok(transmute(hook.trampoline()));
        }

        let hook: RawDetour = RawDetour::new(target as *const (), replacement as *const ())
            .unwrap_or_else(|e| internal_failure!("Failed hook function! {}", e));

        let _ = m.insert(target, hook);

        let hook = m.get(&target)
            .unwrap_or_else(|| internal_failure!("Failed to get hook!"));

        hook.enable()?;

        Ok(transmute(hook.trampoline()))
    }
}

/// unhook a function pointer
pub fn unhook(target: usize) -> Result<(), Box<dyn error::Error>> {
    unsafe {
        let mut m = HOOKMAP.lock()?;
        let hook = m.get(&target)
            .unwrap_or_else(|| internal_failure!("Failed to get hook!"));

        hook.disable()?;

        let _ = m.remove(&target);
    }

    Ok(())
}