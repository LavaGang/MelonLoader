use crate::{
    constants, debug_enabled,
};
use colored::Colorize;
use std::{io::Write, error::Error};

#[derive(Debug)]
#[repr(u8)]
pub enum LogLevel {
    Info,
    Warning,
    Error,
    Debug,
}

impl std::convert::TryFrom<u8> for LogLevel {
    type Error = Box<dyn Error>;

    fn try_from(value: u8) -> Result<Self, <LogLevel as std::convert::TryFrom<u8>>::Error> {
        match value {
            0 => Ok(LogLevel::Info),
            1 => Ok(LogLevel::Warning),
            2 => Ok(LogLevel::Error),
            3 => Ok(LogLevel::Debug),
            _ => Err("Invalid value for enum `LogLevel` possible: [1, 2, 3]".into()),
        }
    }
}

macro_rules! log_path {
    () => {
        $crate::environment::paths::get_base_dir().unwrap().join("MelonLoader").join("Latest.log")
    };
}

pub fn init() -> Result<(), Box<dyn Error>> {
    let log_file = log_path!();

    if log_file.exists() {
        std::fs::remove_file(&log_file)?;
        std::fs::File::create(log_file)?;

        //BOM for UTF-16
        write_bytes(&[255,254])?; 
    }

    Ok(())
}

pub fn write(msg: &str) -> Result<(), Box<dyn Error>> {
    let log_file = log_path!();

    let mut file = std::fs::OpenOptions::new()
        .write(true)
        .append(true)
        .create(true)
        .open(&log_file)?;

    let message = format!("{}\r\n", msg);

    for utf16 in message.encode_utf16() {
        file.write_all(&utf16.to_le_bytes())?;
    }

    Ok(())
}

pub fn write_bytes(bytes: &[u8]) -> Result<(), Box<dyn Error>> {
    let log_file = log_path!();

    let mut file = std::fs::OpenOptions::new()
        .write(true)
        .append(true)
        .create(true)
        .open(&log_file)?;

    file.write_all(bytes)?;

    Ok(())
}

/// logs to console and file, should not be used, use the log! macro instead
pub fn log_console_file(level: LogLevel, message: &str) -> Result<(), Box<dyn Error>> {
    match level {
        LogLevel::Info => {
            // [19:11:50.321] message
            let console_string = format!(
                "{}{}{} {}",
                "[".bright_black(),
                timestamp().color(constants::GREEN),
                "]".bright_black(),
                message
            );

            let file_string = format!("[{}] {}", timestamp(), message);

            #[cfg(target_os = "android")]
            crate::log_console!(level, "{}", message);
            #[cfg(not(target_os = "android"))]
            crate::log_console!(level, "{}", console_string);
            write(&file_string)?;
        }
        LogLevel::Warning => {
            //[19:11:50.321] [WARNING] message
            let console_string = format!("[{}] [WARNING] {}", timestamp(), message);

            let file_string = format!("[{}] [WARNING] {}", timestamp(), message);

            #[cfg(target_os = "android")]
            crate::log_console!(level, "{}", message.bright_yellow());
            #[cfg(not(target_os = "android"))]
            crate::log_console!(level, "{}", console_string.bright_yellow());

            write(&file_string)?;
        }
        LogLevel::Error => {
            //[19:11:50.321] [ERROR] message
            let log_string = format!("[{}] [ERROR] {}", timestamp(), message);

            #[cfg(target_os = "android")]
            crate::log_console!(level, "{}", message.color(constants::RED));
            #[cfg(not(target_os = "android"))]
            crate::log_console!(level, "{}", log_string.color(constants::RED));
            write(&log_string)?;
        }
        LogLevel::Debug => {
            if !debug_enabled!() {
                return Ok(());
            }
            //[19:11:50.321] [DEBUG] message
            let console_string = format!(
                "{}{}{} {}{}{} {}",
                "[".bright_black(),
                timestamp().color(constants::GREEN),
                "]".bright_black(),
                "[".bright_black(),
                "DEBUG".color(constants::BLUE),
                "]".bright_black(),
                message
            );

            let file_string = format!("[{}] [DEBUG] {}", timestamp(), message);

            #[cfg(target_os = "android")]
            crate::log_console!(level, "{}", message);
            #[cfg(not(target_os = "android"))]
            crate::log_console!(level, "{}", console_string);

            write(&file_string)?;
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
        $crate::logging::logger::log_console_file($crate::logging::logger::LogLevel::Info, "")
    };

    //case 3: multiple arguments
    ($($arg:tt)*) => {{
        $crate::logging::logger::log_console_file($crate::logging::logger::LogLevel::Info, &format_args!($($arg)*).to_string())
    }};
}

/// Logs a message to the console and log file, with a warning prefix
///
/// # Example
/// ```
/// warn!("Hello World!")?;
/// ```
///
/// warn! returns a Result<(), Box<LogError>>, so please handle this.
#[macro_export]
macro_rules! warn {
    //case 1: empty
    () => {
        $crate::logging::logger::log_console_file($crate::logging::logger::LogLevel::Warning, "")
    };

    //case 3: multiple arguments
    ($($arg:tt)*) => {{
        $crate::logging::logger::log_console_file($crate::logging::logger::LogLevel::Warning, &format_args!($($arg)*).to_string())
    }};
}

/// Logs a message to the console and log file, with an error prefix
///
/// # Example
/// ```
/// error!("Hello World!")?;
/// ```
///
/// error! returns a Result<(), Box<LogError>>, so please handle this.
#[macro_export]
macro_rules! error {
    //case 1: empty
    () => {
        $crate::logging::logger::log_console_file($crate::logging::logger::LogLevel::Error, "")
    };

    //case 3: multiple arguments
    ($($arg:tt)*) => {{
        $crate::logging::logger::log_console_file($crate::logging::logger::LogLevel::Error, &format_args!($($arg)*).to_string())
    }};
}

/// Logs a message to the console and log file, with a debug prefix
///
/// # Example
/// ```
/// debug!("Hello World!")?;
/// ```
///
/// debug! returns a Result<(), Box<LogError>>, so please handle this.
#[macro_export]
macro_rules! debug {
    //case 1: empty
    () => {
        $crate::logging::logger::log_console_file($crate::logging::logger::LogLevel::Debug, "")
    };

    //case 3: multiple arguments
    ($($arg:tt)*) => {{
        $crate::logging::logger::log_console_file($crate::logging::logger::LogLevel::Debug, &format_args!($($arg)*).to_string())
    }};
}

#[macro_export]
macro_rules! cstr {
    ($s:expr) => {
        std::ffi::CString::new($s)?.as_ptr()
    };

    //format str
    ($($arg:tt)*) => {
       std::ffi::CString::new(format!($($arg)*))?.as_ptr()
    };
}

#[cfg(not(target_os = "android"))]
#[macro_export]
macro_rules! log_console {
    ($level:expr, $($arg:tt)*) => {
        println!($($arg)*);
    };
}

#[cfg(target_os = "android")]
#[macro_export]
macro_rules! log_console {
    ($level:expr, $($arg:tt)*) => {
        let format = std::ffi::CString::new(format!($($arg)*)).unwrap();

        let prio = match $level {
            LogLevel::Error => android_liblog_sys::LogPriority::ERROR,
            LogLevel::Warning => android_liblog_sys::LogPriority::WARN,
            LogLevel::Info => android_liblog_sys::LogPriority::INFO,
            LogLevel::Debug => android_liblog_sys::LogPriority::DEBUG,
        };

        unsafe {
            android_liblog_sys::__android_log_write(prio as _, crate::cstr!("MelonLoader"), format.as_ptr());
        }
    };
}
