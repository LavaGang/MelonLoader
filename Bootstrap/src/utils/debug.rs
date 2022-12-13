//! module for debugging

use std::env;

/// returns a bool, indicating whether or not we're running in debug mode
pub fn enabled() -> bool {
    for arg in env::args() {
        if arg == "--melonloader.debug" {
            return true;
        }
    }

    false
}