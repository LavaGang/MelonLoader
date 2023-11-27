//! all the logic for exporting the functions.

use std::{
    arch::global_asm,
    error,
    marker::FnPtr,
    ptr::null_mut,
    sync::{LazyLock, Mutex},
};

use super::hinstance_ext::ProxyDll;
use libloading::Library;
use thiserror::Error;
use windows::Win32::Foundation::HINSTANCE;

// These arrays are accessed by assembly code to jump to the given function.
// TODO: once dynamically sized arrays are implemented, consider starting this off at 17 and dynamically resizing as needed 
#[no_mangle]
static mut OriginalFuncs: [*const (); 181] = [null_mut(); 181];

// These assembly files define the Windows DLL functions that we're proxying, they are exported through a linked .def file. 
#[cfg(target_arch = "x86_64")]
global_asm!(include_str!("../../deps/version.x64.S"));
#[cfg(target_arch = "x86")]
global_asm!(include_str!("../../deps/version.x86.S"));

#[cfg(target_arch = "x86_64")]
global_asm!(include_str!("../../deps/winhttp.x64.S"));
#[cfg(target_arch = "x86")]
global_asm!(include_str!("../../deps/winhttp.x86.S"));

#[cfg(target_arch = "x86_64")]
global_asm!(include_str!("../../deps/winmm.x64.S"));
#[cfg(target_arch = "x86")]
global_asm!(include_str!("../../deps/winmm.x86.S"));

#[derive(Debug, Error)]
pub enum ExportError {
    #[error("Failed to find original library")]
    LibraryNotFound,
    #[error("Failed to load library")]
    LoadLibrary,
    #[error("Failed to get module name")]
    GetModuleName,
    #[error("Proxy has an invalid file name {0}")]
    InvalidFileName(String),
}

const EXPORTS_VERSION: [&[u8]; 17] = [
    b"GetFileVersionInfoA",
    b"GetFileVersionInfoByHandle",
    b"GetFileVersionInfoExA",
    b"GetFileVersionInfoExW",
    b"GetFileVersionInfoSizeA",
    b"GetFileVersionInfoSizeExA",
    b"GetFileVersionInfoSizeExW",
    b"GetFileVersionInfoSizeW",
    b"GetFileVersionInfoW",
    b"VerFindFileA",
    b"VerFindFileW",
    b"VerInstallFileA",
    b"VerInstallFileW",
    b"VerLanguageNameA",
    b"VerLanguageNameW",
    b"VerQueryValueA",
    b"VerQueryValueW",
];

const EXPORTS_WINHTTP: [&[u8]; 27] = [
    b"EmptyWorkingSet",
    b"EnumDeviceDrivers",
    b"EnumPageFilesA",
    b"EnumPageFilesW",
    b"EnumProcessModules",
    b"EnumProcessModulesEx",
    b"EnumProcesses",
    b"GetDeviceDriverBaseNameA",
    b"GetDeviceDriverBaseNameW",
    b"GetDeviceDriverFileNameA",
    b"GetDeviceDriverFileNameW",
    b"GetMappedFileNameA",
    b"GetMappedFileNameW",
    b"GetModuleBaseNameA",
    b"GetModuleBaseNameW",
    b"GetModuleFileNameExA",
    b"GetModuleFileNameExW",
    b"GetModuleInformation",
    b"GetPerformanceInfo",
    b"GetProcessImageFileNameA",
    b"GetProcessImageFileNameW",
    b"GetProcessMemoryInfo",
    b"GetWsChanges",
    b"GetWsChangesEx",
    b"InitializeProcessForWsWatch",
    b"QueryWorkingSet",
    b"QueryWorkingSetEx",
];

const EXPORTS_WINMM: [&[u8]; 181] = [
    b"CloseDriver",
    b"DefDriverProc",
    b"DriverCallback",
    b"DrvGetModuleHandle",
    b"GetDriverModuleHandle",
    b"OpenDriver",
    b"PlaySound",
    b"PlaySoundA",
    b"PlaySoundW",
    b"SendDriverMessage",
    b"WOWAppExit",
    b"auxGetDevCapsA",
    b"auxGetDevCapsW",
    b"auxGetNumDevs",
    b"auxGetVolume",
    b"auxOutMessage",
    b"auxSetVolume",
    b"joyConfigChanged",
    b"joyGetDevCapsA",
    b"joyGetDevCapsW",
    b"joyGetNumDevs",
    b"joyGetPos",
    b"joyGetPosEx",
    b"joyGetThreshold",
    b"joyReleaseCapture",
    b"joySetCapture",
    b"joySetThreshold",
    b"mciDriverNotify",
    b"mciDriverYield",
    b"mciExecute",
    b"mciFreeCommandResource",
    b"mciGetCreatorTask",
    b"mciGetDeviceIDA",
    b"mciGetDeviceIDFromElementIDA",
    b"mciGetDeviceIDFromElementIDW",
    b"mciGetDeviceIDW",
    b"mciGetDriverData",
    b"mciGetErrorStringA",
    b"mciGetErrorStringW",
    b"mciGetYieldProc",
    b"mciLoadCommandResource",
    b"mciSendCommandA",
    b"mciSendCommandW",
    b"mciSendStringA",
    b"mciSendStringW",
    b"mciSetDriverData",
    b"mciSetYieldProc",
    b"midiConnect",
    b"midiDisconnect",
    b"midiInAddBuffer",
    b"midiInClose",
    b"midiInGetDevCapsA",
    b"midiInGetDevCapsW",
    b"midiInGetErrorTextA",
    b"midiInGetErrorTextW",
    b"midiInGetID",
    b"midiInGetNumDevs",
    b"midiInMessage",
    b"midiInOpen",
    b"midiInPrepareHeader",
    b"midiInReset",
    b"midiInStart",
    b"midiInStop",
    b"midiInUnprepareHeader",
    b"midiOutCacheDrumPatches",
    b"midiOutCachePatches",
    b"midiOutClose",
    b"midiOutGetDevCapsA",
    b"midiOutGetDevCapsW",
    b"midiOutGetErrorTextA",
    b"midiOutGetErrorTextW",
    b"midiOutGetID",
    b"midiOutGetNumDevs",
    b"midiOutGetVolume",
    b"midiOutLongMsg",
    b"midiOutMessage",
    b"midiOutOpen",
    b"midiOutPrepareHeader",
    b"midiOutReset",
    b"midiOutSetVolume",
    b"midiOutShortMsg",
    b"midiOutUnprepareHeader",
    b"midiStreamClose",
    b"midiStreamOpen",
    b"midiStreamOut",
    b"midiStreamPause",
    b"midiStreamPosition",
    b"midiStreamProperty",
    b"midiStreamRestart",
    b"midiStreamStop",
    b"mixerClose",
    b"mixerGetControlDetailsA",
    b"mixerGetControlDetailsW",
    b"mixerGetDevCapsA",
    b"mixerGetDevCapsW",
    b"mixerGetID",
    b"mixerGetLineControlsA",
    b"mixerGetLineControlsW",
    b"mixerGetLineInfoA",
    b"mixerGetLineInfoW",
    b"mixerGetNumDevs",
    b"mixerMessage",
    b"mixerOpen",
    b"mixerSetControlDetails",
    b"mmDrvInstall",
    b"mmGetCurrentTask",
    b"mmTaskBlock",
    b"mmTaskCreate",
    b"mmTaskSignal",
    b"mmTaskYield",
    b"mmioAdvance",
    b"mmioAscend",
    b"mmioClose",
    b"mmioCreateChunk",
    b"mmioDescend",
    b"mmioFlush",
    b"mmioGetInfo",
    b"mmioInstallIOProcA",
    b"mmioInstallIOProcW",
    b"mmioOpenA",
    b"mmioOpenW",
    b"mmioRead",
    b"mmioRenameA",
    b"mmioRenameW",
    b"mmioSeek",
    b"mmioSendMessage",
    b"mmioSetBuffer",
    b"mmioSetInfo",
    b"mmioStringToFOURCCA",
    b"mmioStringToFOURCCW",
    b"mmioWrite",
    b"mmsystemGetVersion",
    b"sndPlaySoundA",
    b"sndPlaySoundW",
    b"timeBeginPeriod",
    b"timeEndPeriod",
    b"timeGetDevCaps",
    b"timeGetSystemTime",
    b"timeGetTime",
    b"timeKillEvent",
    b"timeSetEvent",
    b"waveInAddBuffer",
    b"waveInClose",
    b"waveInGetDevCapsA",
    b"waveInGetDevCapsW",
    b"waveInGetErrorTextA",
    b"waveInGetErrorTextW",
    b"waveInGetID",
    b"waveInGetNumDevs",
    b"waveInGetPosition",
    b"waveInMessage",
    b"waveInOpen",
    b"waveInPrepareHeader",
    b"waveInReset",
    b"waveInStart",
    b"waveInStop",
    b"waveInUnprepareHeader",
    b"waveOutBreakLoop",
    b"waveOutClose",
    b"waveOutGetDevCapsA",
    b"waveOutGetDevCapsW",
    b"waveOutGetErrorTextA",
    b"waveOutGetErrorTextW",
    b"waveOutGetID",
    b"waveOutGetNumDevs",
    b"waveOutGetPitch",
    b"waveOutGetPlaybackRate",
    b"waveOutGetPosition",
    b"waveOutGetVolume",
    b"waveOutMessage",
    b"waveOutOpen",
    b"waveOutPause",
    b"waveOutPrepareHeader",
    b"waveOutReset",
    b"waveOutRestart",
    b"waveOutSetPitch",
    b"waveOutSetPlaybackRate",
    b"waveOutSetVolume",
    b"waveOutUnprepareHeader",
    b"waveOutWrite",
    b"ExportByOrdinal2",
];

// we have to statically store the original library here, because if it gets dropped, the function pointers we get out of it become invalid.
pub static ORIGINAL: LazyLock<Mutex<Option<Library>>> = LazyLock::new(|| Mutex::new(None));

//this function gets called by the #[proxy] macro in our entrypoint.
pub fn initialize(module: HINSTANCE) -> Result<(), Box<dyn error::Error>> {
    if module.is_invalid() {
        return Err(Box::new(ExportError::LoadLibrary));
    }

    let name = module.get_file_name()?;
    let original = module.load_original()?;

    let exports = match name.as_str() {
        "version.dll" => EXPORTS_VERSION.to_vec(),
        "winhttp.dll" => EXPORTS_WINHTTP.to_vec(),
        "winmm.dll" => EXPORTS_WINMM.to_vec(),
        _ => return Err(Box::new(ExportError::InvalidFileName(name))),
    };

    for (i, export) in exports.iter().enumerate() {
        unsafe {
            OriginalFuncs[i] = get_maybe(&original, export);
        }
    }

    //store the library so it doesn't get unloaded
    *ORIGINAL.try_lock()? = Some(original);

    Ok(())
}

unsafe fn get_maybe(lib: &Library, name: &[u8]) -> *const () {
    let func = lib.get::<fn()>(name);

    match func.is_err() {
        false => func.unwrap().addr(),
        true => null_mut(),
    }
}
