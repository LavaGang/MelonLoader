#![feature(lazy_cell)]

use std::error::Error;

use hooking::hooks;
use logging::logger;

pub mod dotnet;
pub mod environment;
pub mod utils;
pub mod logging;
pub mod hooking;
pub mod constants;
pub mod icalls;

#[ctor::ctor]
fn main() {
    init().unwrap_or_else(|e| internal_failure!("Failed to start MelonLoader: {}", e));
}

fn init() -> Result<(), Box<dyn Error>> {
    logger::init()?;

    #[cfg(target_os = "windows")]
    hooks::load_library::init()?;

    #[cfg(not(target_os = "windows"))]
    dotnet::startup::start()?;
    
    Ok(())
}