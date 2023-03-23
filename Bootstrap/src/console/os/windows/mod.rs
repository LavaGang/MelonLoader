//! Windows Console Module.

use std::sync::Mutex;

use lazy_static::lazy_static;
use windows::{
    core::PCSTR,
    Win32::{
        Foundation::{self, HANDLE, HWND},
        System::Console::*, UI::WindowsAndMessaging::{SetWindowPos, HWND_TOPMOST, SWP_NOMOVE, SWP_NOSIZE},
    },
};

use crate::{
    constants::{IS_ALPHA, MELON_VERSION},
    debug_enabled,
    errors::{conerr::ConsoleError, DynErr},
    win_str, should_set_title, console_on_top,
};

lazy_static! {
    static ref WINDOW: Mutex<HWND> = Mutex::new(HWND(0));
    static ref OUTPUT_HANDLE: Mutex<HANDLE> = Mutex::new(HANDLE(0));
}

pub unsafe fn init() -> Result<(), DynErr> {
    // creates a console window, if one already exists it'll just return true.
    if !AllocConsole().as_bool() {
        return Err(ConsoleError::FailedToAllocateConsole.into());
    }

    // store the console window handle
    let mut window = WINDOW.try_lock()?;
    *window = GetConsoleWindow();

    // a null check
    if window.0 == 0 {
        return Err(ConsoleError::FailedToGetConsoleWindow.into());
    }

    // this lets us hook into console close events, and run some cleanup logic.
    if SetConsoleCtrlHandler(Some(ctrl_handler_hook), Foundation::TRUE) == Foundation::FALSE {
        return Err(ConsoleError::FailedToSetConsoleCtrlHandler.into());
    }

    set_title(&default_title());

    if console_on_top!() {
        let _ = SetWindowPos(*window, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
    }

    let _ = libc::freopen(
        win_str!(b"CONIN$\0"),
        win_str!(b"r\0"),
        libc_stdhandle::stdin(),
    );
    let _ = libc::freopen(
        win_str!(b"CONOUT$\0"),
        win_str!(b"w\0"),
        libc_stdhandle::stdout(),
    );
    let _ = libc::freopen(
        win_str!(b"CONOUT$\0"),
        win_str!(b"w\0"),
        libc_stdhandle::stderr(),
    );

    // needs to be in its own scope to drop the lock
    {
        let mut output_handle = OUTPUT_HANDLE.try_lock()?;
        *output_handle = GetStdHandle(STD_OUTPUT_HANDLE)?;
    }

    set_handles()?;

    let output_handle = OUTPUT_HANDLE.try_lock()?;

    let mut mode = CONSOLE_MODE(0);
    let _ = GetConsoleMode(*output_handle, &mut mode);

    mode |= ENABLE_LINE_INPUT | ENABLE_PROCESSED_INPUT;

    if SetConsoleMode(*output_handle, mode) != Foundation::TRUE {
        mode &= !(ENABLE_LINE_INPUT | ENABLE_PROCESSED_INPUT);
    } else {
        mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;

        if SetConsoleMode(*output_handle, mode) != Foundation::TRUE {
            mode &= !ENABLE_VIRTUAL_TERMINAL_PROCESSING;
        }
    }

    mode |= ENABLE_EXTENDED_FLAGS;
    mode &= !(ENABLE_MOUSE_INPUT | ENABLE_WINDOW_INPUT | ENABLE_INSERT_MODE);

    let _ = SetConsoleMode(*output_handle, mode);
    Ok(())
}

fn default_title() -> String {
    let mut title = match debug_enabled!() {
        true => format!("[D] MelonLoader v{MELON_VERSION}"),
        false => format!("MelonLoader v{MELON_VERSION}"),
    };

    match IS_ALPHA {
        true => title.push_str(" Alpha-PreRelease\0"),
        false => title.push_str(" Open-Beta\0"),
    }

    title
}

pub fn set_title(title: &str) {
    if !should_set_title!() {
        return;
    }

    unsafe {
        let t = PCSTR(title.as_ptr());
        let _ = SetConsoleTitleA(t);
    }
}

pub fn set_handles() -> Result<(), DynErr> {
    unsafe {
        let handle = OUTPUT_HANDLE.try_lock()?;

        let _ = SetStdHandle(STD_OUTPUT_HANDLE, *handle);
        let _ = SetStdHandle(STD_ERROR_HANDLE, *handle);

        Ok(())
    }
}

pub fn null_handles() -> Result<(), DynErr> {
    unsafe {
        let _ = SetStdHandle(STD_OUTPUT_HANDLE, HANDLE(0));
        let _ = SetStdHandle(STD_ERROR_HANDLE, HANDLE(0));

        Ok(())
    }
}

unsafe extern "system" fn ctrl_handler_hook(ctrltype: u32) -> Foundation::BOOL {
    match ctrltype {
        CTRL_C_EVENT | CTRL_CLOSE_EVENT | CTRL_LOGOFF_EVENT | CTRL_SHUTDOWN_EVENT => {
            crate::core::shutdown();
            Foundation::TRUE
        }

        _ => Foundation::FALSE,
    }
}
