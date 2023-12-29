#![feature(lazy_cell)]

use std::error::Error;
#[cfg(target_os = "windows")]
use hooking::hooks;
use logging::logger;

pub mod constants;
pub mod console;
pub mod dotnet;
pub mod environment;
pub mod hooking;
pub mod icalls;
pub mod logging;
pub mod utils;
pub mod errors;
#[cfg(target_os = "android")]
pub mod lib_android;

#[cfg_attr(
    not(target_os = "android"),
    ctor::ctor
)]
#[no_mangle]
fn startup() {
    init().unwrap_or_else(|e| internal_failure!("Failed to start MelonLoader: {}", e));
}

fn init() -> Result<(), Box<dyn Error>> {
    logger::init()?;
    
    #[cfg(target_os = "windows")] {
        console::init()?;
        hooks::load_library::init()?;
    }
    
    debug!("starting up")?;
    #[cfg(not(target_os = "windows"))]
    dotnet::startup::start()?;

    Ok(())
}

pub fn shutdown() {
    std::process::exit(0);
}
