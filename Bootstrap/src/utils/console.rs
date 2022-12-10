//! Windows console stuff

#![cfg(windows)]

use std::{error::Error, sync::RwLock, process, ffi::CString};

use crate::cstr;

use libc::freopen;
use windows::{Win32::{System::Console::{AllocConsole, GetConsoleWindow, SetConsoleCtrlHandler, SetConsoleTitleA, GetStdHandle, STD_INPUT_HANDLE, GetConsoleMode, STD_OUTPUT_HANDLE, CONSOLE_MODE, ENABLE_LINE_INPUT, ENABLE_PROCESSED_INPUT, SetConsoleMode, ENABLE_VIRTUAL_TERMINAL_PROCESSING, ENABLE_EXTENDED_FLAGS, ENABLE_MOUSE_INPUT, ENABLE_WINDOW_INPUT, ENABLE_INSERT_MODE, SetStdHandle, STD_ERROR_HANDLE}, UI::WindowsAndMessaging::{GetSystemMenu, SetForegroundWindow}, Foundation::{HWND, BOOL, HANDLE}}, core::PCSTR, s};

use crate::managers::core;

use super::debug;

static mut WINDOW: RwLock<Option<HWND>> = RwLock::new(None);
static mut OUTPUT_HANDLE: RwLock<Option<HANDLE>> = RwLock::new(None);

pub fn init() -> Result<(), Box<dyn Error>> {
    unsafe {
        if !AllocConsole().as_bool() {
            return Err("Failed to allocate console".into());
        }

        let window = GetConsoleWindow();
        if window.0 == 0 {
            return Err("Failed to get console window".into());
        }

        WINDOW.try_write()?.replace(window);

        let menu = GetSystemMenu(window, false);
        if menu.0 == 0 {
            return Err("Failed to get system menu".into());
        }

        SetConsoleCtrlHandler(Some(event_handler), true);

        let version = core::version();
        let distribution = match core::is_alpha() {
            true => "Alpha Pre-Release",
            false => "Open-Beta"
        };

        let title = match debug::enabled() {
            true => format!("[D] MelonLoader v{version} - {distribution}"),
            false => format!("MelonLoader v{version} - {distribution}")
        };

        let title = win_str(title)?;

        SetConsoleTitleA(title);
        SetForegroundWindow(window);

        freopen(cstr!("CONIN$"), cstr!("r"), libc_stdhandle::stdin());
        freopen(cstr!("CONOUT$"), cstr!("w"), libc_stdhandle::stdout());
        freopen(cstr!("CONOUT$"), cstr!("w"), libc_stdhandle::stderr());
        

        set_handles()?;

        let output_handle = GetStdHandle(STD_OUTPUT_HANDLE)?;
        OUTPUT_HANDLE.try_write()?.replace(output_handle);

        let mut mode: CONSOLE_MODE = CONSOLE_MODE(0);
        GetConsoleMode(output_handle, &mut mode);

        mode |= ENABLE_LINE_INPUT | ENABLE_PROCESSED_INPUT;

        if !SetConsoleMode(output_handle, mode).as_bool() {
            mode &= !(ENABLE_LINE_INPUT | ENABLE_PROCESSED_INPUT);
        } else {
            mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;

            if !SetConsoleMode(output_handle, mode).as_bool() {
                mode &= !ENABLE_VIRTUAL_TERMINAL_PROCESSING;
            }
        }

        mode |= ENABLE_EXTENDED_FLAGS;
        mode &= !(ENABLE_MOUSE_INPUT | ENABLE_WINDOW_INPUT | ENABLE_INSERT_MODE);

        SetConsoleMode(output_handle, mode);

    }
    Ok(())
}

pub fn set_handles() -> Result<(), Box<dyn Error>> {
    unsafe {
        let output_handle = OUTPUT_HANDLE.try_read()?;
        if let Some(output_handle) = output_handle.as_ref() {
            SetStdHandle(STD_OUTPUT_HANDLE, *output_handle);
            SetStdHandle(STD_ERROR_HANDLE, *output_handle);
        }
    }
    Ok(())
}

pub fn null_handles() -> Result<(), Box<dyn Error>> {
    unsafe {
        SetStdHandle(STD_OUTPUT_HANDLE, HANDLE(0));
        SetStdHandle(STD_ERROR_HANDLE, HANDLE(0));
    }
    Ok(())
}

#[no_mangle]
extern "system" fn event_handler(evt: u32) -> BOOL {
    match evt {
        windows::Win32::System::Console::CTRL_C_EVENT | 
        windows::Win32::System::Console::CTRL_CLOSE_EVENT | 
        windows::Win32::System::Console::CTRL_LOGOFF_EVENT | 
        windows::Win32::System::Console::CTRL_SHUTDOWN_EVENT => {
            unsafe {
                let window = WINDOW.try_read();
                if let Ok(window) = window {
                    if let Some(window) = window.as_ref() {
                        windows::Win32::UI::WindowsAndMessaging::DestroyWindow(*window);
                    }
                }

                process::exit(0);
            }
        }

        _ => {
            false.into()
        }
    }
}

fn win_str(s: impl Into<String>) -> Result<PCSTR, Box<dyn Error>> {
    let s = s.into();
    Ok(PCSTR::from_raw(format!("{s}\0").as_ptr()))
}