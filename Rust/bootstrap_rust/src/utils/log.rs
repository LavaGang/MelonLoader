//! Handles logging across the entire package

use std::{
    io::Write,
    path::PathBuf, error,
};

use colored::{Color, Colorize};
use thiserror::Error;

use crate::managers::core;

use super::debug;

/// all possible errors that can occur when working with the logger
///
/// # Errors
/// ```
/// LogError::FailedToDeleteOldLog // Failed to delete the old log file
/// LogError::FailedToWriteToLog //Failed to write to the log file
/// LogError::FailedToGetBasePath //Failed to get the base path
/// ```
#[derive(Error, Debug)]
pub enum LogError {
    /// the log file could not be deleted
    #[error("Failed to delete the old log file")]
    FailedToDeleteOldLog,

    /// the log file could not be written to
    #[error("Failed to write to log file")]
    FailedToWriteToLog,

    /// the base path could not be fetched
    #[error("Failed to fetch base path")]
    FailedToGetBasePath,
}

/// gets the path to the log file
fn log_path() -> Result<PathBuf, Box<dyn error::Error>> {
    let base_path = core::base_path()?;
    let log_path = base_path.join("MelonLoader/Latest-Bootstrap.log");

    Ok(log_path)
}

/// Initializes MelonLogger, which takes care of both logging to console & file
pub fn init() -> Result<(), Box<dyn error::Error>> {
    let log_path = log_path().map_err(|_| LogError::FailedToGetBasePath)?;

    if log_path.exists() {
        std::fs::remove_file(&log_path).map_err(|_| LogError::FailedToDeleteOldLog)?;
    }

    Ok(())
}

/// the different log levels
/// 
/// # Levels
/// ```
/// LogLevel::Info
/// LogLevel::Warning
/// LogLevel::Error
/// LogLevel::Debug
#[derive(Debug)]
pub enum LogLevel {
    /// Informational, always printed to console
    Info,
    /// Warning, always printed to console
    Warning,
    /// Error, always printed to console
    Error,
    /// Debug, only printed to console if --melonloader.debug is passed as a launch option
    Debug,
}

static RED: Color = Color::TrueColor {
    r: (255),
    g: (0),
    b: (0),
};
static GREEN: Color = Color::TrueColor {
    r: (0),
    g: (255),
    b: (0),
};
static BLUE: Color = Color::TrueColor {
    r: (64),
    g: (64),
    b: (255),
};

fn write(msg: &str) -> Result<(), Box<dyn error::Error>> {
    let log_path = log_path().map_err(|_| LogError::FailedToGetBasePath)?;
    let mut file = std::fs::OpenOptions::new()
        .write(true)
        .append(true)
        .create(true)
        .open(&log_path)
        .map_err(|_| LogError::FailedToWriteToLog)?;

    let message = format!("{}\r\n", msg);

    file.write_all(message.as_bytes())
        .map_err(|_| LogError::FailedToWriteToLog)?;

    Ok(())
}

/// logs to console and file, should not be used, use the log! macro instead
pub fn log_console_file(level: LogLevel, message: &str) -> Result<(), Box<LogError>> {
    match level {
        LogLevel::Info => {
            let console_string = format!(
                "{}{}{} {}",
                "[".bright_black(),
                timestamp().color(GREEN),
                "]".bright_black(),
                message
            );

            let file_string = format!(
                "[{}] {}",
                timestamp(),
                message
            );

            println!("{}", console_string);
            write(&file_string).map_err(|_| LogError::FailedToWriteToLog)?;
        }
        LogLevel::Warning => {
            //same as log, but all colors are yellow
            let console_string = format!(
                "[{}] [WARNING] {}",
                timestamp(),
                message
            );

            let file_string = format!(
                "[{}] [WARNING] {}",
                timestamp(),
                message
            );

            println!("{}", console_string.yellow());

            write(&file_string).map_err(|_| LogError::FailedToWriteToLog)?;
        }
        LogLevel::Error => {
            //same as log, but all colors are red

            let log_string = format!(
                "[{}] [ERROR] {}",
                timestamp(),
                message
            );

            println!("{}", log_string.color(RED));
            write(&log_string).map_err(|_| LogError::FailedToWriteToLog)?;
        }
        LogLevel::Debug => {
            if !debug::enabled() {
                return Ok(());
            }

            let console_string = format!(
                "{}{}{} {}{}{} {}",
                "[".bright_black(),
                timestamp().color(GREEN),
                "]".bright_black(),
                "[".bright_black(),
                "DEBUG".color(BLUE),
                "]".bright_black(),
                message
            );

            let file_string = format!(
                "[{}] [DEBUG] {}",
                timestamp(),
                message
            );

            println!("{}", console_string);
            write(&file_string).map_err(|_| LogError::FailedToWriteToLog)?;
        }
    }

    Ok(())
}

/// Fetches the current time, and formats it.
///
/// returns a String, formatted as 15:23:24:123
fn timestamp() -> String {
    let now = chrono::Local::now();
    let time = now.time();

    time.format("%H:%M:%S.%3f").to_string()
}

/// Logs a message to the console and log file
/// 
/// # Example
/// ```
/// log!("Hello World!")?;
/// ```
/// log! returns a Result<(), Box<LogError>>, so please handle this.
#[macro_export]
macro_rules! log {
    //case 1: empty
    () => {
        $crate::utils::log::log_console_file($crate::utils::log::LogLevel::Info, "")
    };

    //case 2: single argument
    ($msg:expr) => {
        $crate::utils::log::log_console_file($crate::utils::log::LogLevel::Info, $msg)
    };

    //case 3: multiple arguments
    ($($msg:expr),+) => {
        $crate::utils::log::log_console_file($crate::utils::log::LogLevel::Info, &format!($($msg),+))
    };
}

/// Logs a warning to the console and log file
/// 
/// # Example
/// ```
/// warn!("Hello World!")?;
/// ```
/// warn! returns a Result<(), Box<LogError>>, so please handle this.
#[macro_export]
macro_rules! warn {
    //case 1: empty
    () => {
        $crate::utils::log::log_console_file($crate::utils::log::LogLevel::Warning, "")
    };

    //case 2: single argument
    ($msg:expr) => {
        $crate::utils::log::log_console_file($crate::utils::log::LogLevel::Warning, $msg)
    };

    //case 3: multiple arguments
    ($($msg:expr),+) => {
        $crate::utils::log::log_console_file($crate::utils::log::LogLevel::Warning, &format!($($msg),+))
    };
}

/// Logs an error to the console and log file
/// 
/// # Example
/// ```
/// error!("Hello World!")?;
/// ```
/// error! returns a Result<(), Box<LogError>>, so please handle this.
#[macro_export]
macro_rules! err {
    //case 1: empty
    () => {
        $crate::utils::log::log_console_file($crate::utils::log::LogLevel::Error, "")
    };

    //case 2: single argument
    ($msg:expr) => {
        $crate::utils::log::log_console_file($crate::utils::log::LogLevel::Error, $msg)
    };

    //case 3: multiple arguments
    ($($msg:expr),+) => {
        $crate::utils::log::log_console_file($crate::utils::log::LogLevel::Error, &format!($($msg),+))
    };
}

/// Logs a debug message to the console and log file
/// 
/// # Example
/// ```
/// debug!("Hello World!")?;
/// ```
/// debug! returns a Result<(), Box<LogError>>, so please handle this.
#[macro_export]
macro_rules! debug {
    //case 1: empty
    () => {
        $crate::utils::log::log_console_file($crate::utils::log::LogLevel::Debug, "")
    };

    //case 2: single argument
    ($msg:expr) => {
        $crate::utils::log::log_console_file($crate::utils::log::LogLevel::Debug, $msg)
    };

    //case 3: multiple arguments
    ($($msg:expr),+) => {
        $crate::utils::log::log_console_file($crate::utils::log::LogLevel::Debug, &format!($($msg),+))
    };
}
