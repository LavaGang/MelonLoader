/// Throws an internal failure with the given message
///
/// This logs your message to file, creates a message box, and then panics.
/// It uses the same syntax as _format!_
///
/// # Examples
///
/// ```
/// # use utils::assert;
/// assert::internal_failure!("This is an internal failure");
/// ```
#[macro_export]
macro_rules! internal_failure {
    ($($arg:tt)*) => {{
        let msg = &format_args!($($arg)*).to_string();

        std::println!("{}", msg);
        let _ = $crate::logging::logger::log_console_file($crate::logging::logger::LogLevel::Error, msg);
        let _ = msgbox::create("Internal Failure", msg, msgbox::IconType::Error);
        std::process::exit(-1);
    }};
}
