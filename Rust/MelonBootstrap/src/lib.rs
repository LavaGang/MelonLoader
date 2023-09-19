#![feature(lazy_cell)]

use std::error::Error;

use logging::logger;

pub mod dotnet;
pub mod environment;
pub mod utils;
pub mod logging;
pub mod hooks;
pub mod constants;
pub mod icalls;

#[ctor::ctor]
fn main() {
    init().unwrap_or_else(|e| internal_failure!("Failed to start MelonLoader: {}", e));
}

fn init() -> Result<(), Box<dyn Error>> {
    logger::init()?;
    dotnet::startup::start()?;
    Ok(())
}