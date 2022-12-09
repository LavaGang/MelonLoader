//! Entrypoint for our Bootstrap.

#![cfg(windows)]

use winapi::shared::minwindef;
use winapi::shared::minwindef::{BOOL, DWORD, HINSTANCE, LPVOID};
use winapi::um::libloaderapi::DisableThreadLibraryCalls;

use crate::{managers::core, internal_failure};

/// Entry point which will be called by the system once the DLL has been loaded
/// in the target process. Declaring this function is optional.
///
/// # Safety
///
/// What you can safely do inside here is very limited, see the Microsoft documentation
/// about "DllMain". Rust also doesn't officially support a "life before main()",
/// though it is unclear what that that means exactly for DllMain.
#[no_mangle]
#[allow(non_snake_case, unused_variables)]
extern "system" fn DllMain(
    dll_module: HINSTANCE,
    call_reason: DWORD,
    reserved: LPVOID)
    -> BOOL
{
    const DLL_PROCESS_ATTACH: DWORD = 1;

    if call_reason != DLL_PROCESS_ATTACH {
        return minwindef::TRUE;
    }

    unsafe { let _ = DisableThreadLibraryCalls(dll_module); }

    core::init().unwrap_or_else(|e| {
        internal_failure!("Failed to initialize proxy: {}", e);
    });

    minwindef::TRUE
}