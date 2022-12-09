//! Windows console stuff

#![cfg(windows)]

use std::error;

use libc::{freopen, c_void};
use thiserror::Error;
use winapi::{um::{consoleapi::{AllocConsole, SetConsoleCtrlHandler, GetConsoleMode, SetConsoleMode}, wincon::{GetConsoleWindow, CTRL_C_EVENT, CTRL_CLOSE_EVENT, CTRL_LOGOFF_EVENT, CTRL_SHUTDOWN_EVENT, SetConsoleTitleA, ENABLE_LINE_INPUT, ENABLE_PROCESSED_INPUT, ENABLE_VIRTUAL_TERMINAL_PROCESSING, ENABLE_EXTENDED_FLAGS, ENABLE_MOUSE_INPUT, ENABLE_WINDOW_INPUT, ENABLE_INSERT_MODE}, winuser::{GetSystemMenu, SetForegroundWindow, ShowWindow}, processenv::{GetStdHandle, SetStdHandle}, winbase::{STD_OUTPUT_HANDLE, STD_ERROR_HANDLE}}, shared::{minwindef::{self, DWORD}, windef::HWND}};

use crate::{managers::core, debug};

use super::{debug, files::win_str};

/// console errors
#[derive(Debug, Error)]
pub enum ConsoleError {
    /// failed to allocate console
    #[error("Failed to allocate console!")]
    FailedToAllocateConsole,

    /// failed to get console window
    #[error("Failed to get console window!")]
    FailedToGetConsoleWindow,

    /// failed to get system menu
    #[error("Failed to get system menu!")]
    FailedToGetSystemMenu,
}

static mut OUTPUT_HANDLE: *mut c_void = std::ptr::null_mut();
static mut WINDOW: HWND = std::ptr::null_mut();

/// Initializes the console
pub fn init() -> Result<(), Box<dyn error::Error>> {
    unsafe {
        if AllocConsole() != 1 {
            return Err(Box::new(ConsoleError::FailedToAllocateConsole));
        }

        WINDOW = GetConsoleWindow();
        if WINDOW.is_null() {
            return Err(Box::new(ConsoleError::FailedToGetConsoleWindow));
        }

        let menu = GetSystemMenu(WINDOW, minwindef::FALSE);
        if menu.is_null() {
            return Err(Box::new(ConsoleError::FailedToGetSystemMenu));
        }

        let _ = SetConsoleCtrlHandler(Some(event_handler), minwindef::TRUE);

        set_default_title();

        let _ = SetForegroundWindow(WINDOW);

        let win_stdin = win_str(b"CONIN$\0");
        let win_stdout = win_str(b"CONOUT$\0");
        let read_mode = win_str(b"r\0");
        let write_mode = win_str(b"w\0");

        let _ = freopen(win_stdin, read_mode, libc_stdhandle::stdin());
        let _ = freopen(win_stdout, write_mode, libc_stdhandle::stdout());
        let _ = freopen(win_stdout, write_mode, libc_stdhandle::stderr());

        //let input_handle = GetStdHandle(STD_INPUT_HANDLE);
        OUTPUT_HANDLE = GetStdHandle(STD_OUTPUT_HANDLE);
        set_handles();

        let mut mode: DWORD = 0;
        let _ = GetConsoleMode(OUTPUT_HANDLE, &mut mode);

        mode |= ENABLE_LINE_INPUT | ENABLE_PROCESSED_INPUT;

        if SetConsoleMode(OUTPUT_HANDLE, mode) != 1 {
            mode &= !(ENABLE_LINE_INPUT | ENABLE_PROCESSED_INPUT);
        } else {
            mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;

            if SetConsoleMode(OUTPUT_HANDLE, mode) != 1 {
                mode &= !ENABLE_VIRTUAL_TERMINAL_PROCESSING;
            }
        }

        mode |= ENABLE_EXTENDED_FLAGS;
        mode &= !(ENABLE_MOUSE_INPUT | ENABLE_WINDOW_INPUT | ENABLE_INSERT_MODE);

        let _ = SetConsoleMode(OUTPUT_HANDLE, mode);
    }

    Ok(())
}

/// Sets the console handles
pub fn set_handles() {
    unsafe {
        let _ = SetStdHandle(STD_OUTPUT_HANDLE, OUTPUT_HANDLE);
        let _ = SetStdHandle(STD_ERROR_HANDLE, OUTPUT_HANDLE);
    }
}

/// nulls handles
pub fn null_handles() {
    unsafe {
        let _ = SetStdHandle(STD_OUTPUT_HANDLE, std::ptr::null_mut());
        let _ = SetStdHandle(STD_ERROR_HANDLE, std::ptr::null_mut());
    }
}

extern "system" fn event_handler(evt: DWORD) -> minwindef::BOOL {
    match evt {
        CTRL_C_EVENT | CTRL_CLOSE_EVENT | CTRL_LOGOFF_EVENT | CTRL_SHUTDOWN_EVENT => {
            close();
            std::process::exit(0);
        }
        _ => minwindef::FALSE
    }
}

fn close() {
    unsafe {
        let _ = ShowWindow(WINDOW, 0);
        let _ = debug!("goodbye!");
    }
}

fn set_default_title() {
    let mut version_str = core::get_version_str();
    if debug::enabled() {
        version_str = format!("[D] {}", version_str);
    }

    set_title(&version_str);
}

fn set_title(title: &str) {
    let c_title = std::ffi::CString::new(title).unwrap();

    unsafe {
        let _ = SetConsoleTitleA(c_title.as_ptr());
    }
}

