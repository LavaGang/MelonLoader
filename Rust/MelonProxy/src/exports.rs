// //! all the logic for exporting the functions.

// use std::{
//     arch::global_asm,
//     error,
//     ffi::{CStr, CString, OsStr},
//     path::{PathBuf, Path},
// };

// use thiserror::Error;
// use utf16string::WStr;
// use windows::core::HSTRING;
// use windows::Win32::{Foundation::{FARPROC, HINSTANCE, MAX_PATH}, System::LibraryLoader::GetModuleFileNameW};

// #[no_mangle]
// static mut OriginalFuncs_version: [FARPROC; 17] = [Default::default(); 17];

// #[no_mangle]
// static mut OriginalFuncs_winhttp: [FARPROC; 65] = [Default::default(); 65];

// #[no_mangle]
// static mut OriginalFuncs_winmm: [FARPROC; 181] = [Default::default(); 181];

// #[cfg(target_pointer_width = "64")]
// global_asm!(include_str!("../deps/version.x64.S"));
// #[cfg(target_pointer_width = "32")]
// global_asm!(include_str!("../deps/version.x86.S"));

// #[cfg(target_pointer_width = "64")]
// global_asm!(include_str!("../deps/winhttp.x64.S"));
// #[cfg(target_pointer_width = "32")]
// global_asm!(include_str!("../deps/winhttp.x86.S"));

// #[cfg(target_pointer_width = "64")]
// global_asm!(include_str!("../deps/winmm.x64.S"));
// #[cfg(target_pointer_width = "32")]
// global_asm!(include_str!("../deps/winmm.x86.S"));

// #[derive(Debug, Error)]
// enum ExportError {
//     #[error("Failed to find original library")]
//     LibraryNotFound,
//     #[error("Failed to load library")]
//     LoadLibrary,
//     #[error("Failed to get module path")]
//     GetModulePath,
//     #[error("Failed to get module name")]
//     GetModuleName,
//     #[error("Proxy has an invalid file name")]
//     InvalidFileName,
// }

// const EXPORTS_VERSION: [&'static [u8]; 17] = [
//     b"GetFileVersionInfoA\0",
//     b"GetFileVersionInfoByHandle\0",
//     b"GetFileVersionInfoExA\0",
//     b"GetFileVersionInfoExW\0",
//     b"GetFileVersionInfoSizeA\0",
//     b"GetFileVersionInfoSizeExA\0",
//     b"GetFileVersionInfoSizeExW\0",
//     b"GetFileVersionInfoSizeW\0",
//     b"GetFileVersionInfoW\0",
//     b"VerFindFileA\0",
//     b"VerFindFileW\0",
//     b"VerInstallFileA\0",
//     b"VerInstallFileW\0",
//     b"VerLanguageNameA\0",
//     b"VerLanguageNameW\0",
//     b"VerQueryValueA\0",
//     b"VerQueryValueW\0",
// ];

// const EXPORTS_WINHTTP: [&'static [u8]; 27] = [
//     b"EmptyWorkingSet\0",
//     b"EnumDeviceDrivers\0",
//     b"EnumPageFilesA\0",
//     b"EnumPageFilesW\0",
//     b"EnumProcessModules\0",
//     b"EnumProcessModulesEx\0",
//     b"EnumProcesses\0",
//     b"GetDeviceDriverBaseNameA\0",
//     b"GetDeviceDriverBaseNameW\0",
//     b"GetDeviceDriverFileNameA\0",
//     b"GetDeviceDriverFileNameW\0",
//     b"GetMappedFileNameA\0",
//     b"GetMappedFileNameW\0",
//     b"GetModuleBaseNameA\0",
//     b"GetModuleBaseNameW\0",
//     b"GetModuleFileNameExA\0",
//     b"GetModuleFileNameExW\0",
//     b"GetModuleInformation\0",
//     b"GetPerformanceInfo\0",
//     b"GetProcessImageFileNameA\0",
//     b"GetProcessImageFileNameW\0",
//     b"GetProcessMemoryInfo\0",
//     b"GetWsChanges\0",
//     b"GetWsChangesEx\0",
//     b"InitializeProcessForWsWatch\0",
//     b"QueryWorkingSet\0",
//     b"QueryWorkingSetEx\0",
// ];

// const EXPORTS_WINMM: [&'static [u8]; 181] = [
//     b"CloseDriver\0",
//     b"DefDriverProc\0",
//     b"DriverCallback\0",
//     b"DrvGetModuleHandle\0",
//     b"GetDriverModuleHandle\0",
//     b"OpenDriver\0",
//     b"PlaySound\0",
//     b"PlaySoundA\0",
//     b"PlaySoundW\0",
//     b"SendDriverMessage\0",
//     b"WOWAppExit\0",
//     b"auxGetDevCapsA\0",
//     b"auxGetDevCapsW\0",
//     b"auxGetNumDevs\0",
//     b"auxGetVolume\0",
//     b"auxOutMessage\0",
//     b"auxSetVolume\0",
//     b"joyConfigChanged\0",
//     b"joyGetDevCapsA\0",
//     b"joyGetDevCapsW\0",
//     b"joyGetNumDevs\0",
//     b"joyGetPos\0",
//     b"joyGetPosEx\0",
//     b"joyGetThreshold\0",
//     b"joyReleaseCapture\0",
//     b"joySetCapture\0",
//     b"joySetThreshold\0",
//     b"mciDriverNotify\0",
//     b"mciDriverYield\0",
//     b"mciExecute\0",
//     b"mciFreeCommandResource\0",
//     b"mciGetCreatorTask\0",
//     b"mciGetDeviceIDA\0",
//     b"mciGetDeviceIDFromElementIDA\0",
//     b"mciGetDeviceIDFromElementIDW\0",
//     b"mciGetDeviceIDW\0",
//     b"mciGetDriverData\0",
//     b"mciGetErrorStringA\0",
//     b"mciGetErrorStringW\0",
//     b"mciGetYieldProc\0",
//     b"mciLoadCommandResource\0",
//     b"mciSendCommandA\0",
//     b"mciSendCommandW\0",
//     b"mciSendStringA\0",
//     b"mciSendStringW\0",
//     b"mciSetDriverData\0",
//     b"mciSetYieldProc\0",
//     b"midiConnect\0",
//     b"midiDisconnect\0",
//     b"midiInAddBuffer\0",
//     b"midiInClose\0",
//     b"midiInGetDevCapsA\0",
//     b"midiInGetDevCapsW\0",
//     b"midiInGetErrorTextA\0",
//     b"midiInGetErrorTextW\0",
//     b"midiInGetID\0",
//     b"midiInGetNumDevs\0",
//     b"midiInMessage\0",
//     b"midiInOpen\0",
//     b"midiInPrepareHeader\0",
//     b"midiInReset\0",
//     b"midiInStart\0",
//     b"midiInStop\0",
//     b"midiInUnprepareHeader\0",
//     b"midiOutCacheDrumPatches\0",
//     b"midiOutCachePatches\0",
//     b"midiOutClose\0",
//     b"midiOutGetDevCapsA\0",
//     b"midiOutGetDevCapsW\0",
//     b"midiOutGetErrorTextA\0",
//     b"midiOutGetErrorTextW\0",
//     b"midiOutGetID\0",
//     b"midiOutGetNumDevs\0",
//     b"midiOutGetVolume\0",
//     b"midiOutLongMsg\0",
//     b"midiOutMessage\0",
//     b"midiOutOpen\0",
//     b"midiOutPrepareHeader\0",
//     b"midiOutReset\0",
//     b"midiOutSetVolume\0",
//     b"midiOutShortMsg\0",
//     b"midiOutUnprepareHeader\0",
//     b"midiStreamClose\0",
//     b"midiStreamOpen\0",
//     b"midiStreamOut\0",
//     b"midiStreamPause\0",
//     b"midiStreamPosition\0",
//     b"midiStreamProperty\0",
//     b"midiStreamRestart\0",
//     b"midiStreamStop\0",
//     b"mixerClose\0",
//     b"mixerGetControlDetailsA\0",
//     b"mixerGetControlDetailsW\0",
//     b"mixerGetDevCapsA\0",
//     b"mixerGetDevCapsW\0",
//     b"mixerGetID\0",
//     b"mixerGetLineControlsA\0",
//     b"mixerGetLineControlsW\0",
//     b"mixerGetLineInfoA\0",
//     b"mixerGetLineInfoW\0",
//     b"mixerGetNumDevs\0",
//     b"mixerMessage\0",
//     b"mixerOpen\0",
//     b"mixerSetControlDetails\0",
//     b"mmDrvInstall\0",
//     b"mmGetCurrentTask\0",
//     b"mmTaskBlock\0",
//     b"mmTaskCreate\0",
//     b"mmTaskSignal\0",
//     b"mmTaskYield\0",
//     b"mmioAdvance\0",
//     b"mmioAscend\0",
//     b"mmioClose\0",
//     b"mmioCreateChunk\0",
//     b"mmioDescend\0",
//     b"mmioFlush\0",
//     b"mmioGetInfo\0",
//     b"mmioInstallIOProcA\0",
//     b"mmioInstallIOProcW\0",
//     b"mmioOpenA\0",
//     b"mmioOpenW\0",
//     b"mmioRead\0",
//     b"mmioRenameA\0",
//     b"mmioRenameW\0",
//     b"mmioSeek\0",
//     b"mmioSendMessage\0",
//     b"mmioSetBuffer\0",
//     b"mmioSetInfo\0",
//     b"mmioStringToFOURCCA\0",
//     b"mmioStringToFOURCCW\0",
//     b"mmioWrite\0",
//     b"mmsystemGetVersion\0",
//     b"sndPlaySoundA\0",
//     b"sndPlaySoundW\0",
//     b"timeBeginPeriod\0",
//     b"timeEndPeriod\0",
//     b"timeGetDevCaps\0",
//     b"timeGetSystemTime\0",
//     b"timeGetTime\0",
//     b"timeKillEvent\0",
//     b"timeSetEvent\0",
//     b"waveInAddBuffer\0",
//     b"waveInClose\0",
//     b"waveInGetDevCapsA\0",
//     b"waveInGetDevCapsW\0",
//     b"waveInGetErrorTextA\0",
//     b"waveInGetErrorTextW\0",
//     b"waveInGetID\0",
//     b"waveInGetNumDevs\0",
//     b"waveInGetPosition\0",
//     b"waveInMessage\0",
//     b"waveInOpen\0",
//     b"waveInPrepareHeader\0",
//     b"waveInReset\0",
//     b"waveInStart\0",
//     b"waveInStop\0",
//     b"waveInUnprepareHeader\0",
//     b"waveOutBreakLoop\0",
//     b"waveOutClose\0",
//     b"waveOutGetDevCapsA\0",
//     b"waveOutGetDevCapsW\0",
//     b"waveOutGetErrorTextA\0",
//     b"waveOutGetErrorTextW\0",
//     b"waveOutGetID\0",
//     b"waveOutGetNumDevs\0",
//     b"waveOutGetPitch\0",
//     b"waveOutGetPlaybackRate\0",
//     b"waveOutGetPosition\0",
//     b"waveOutGetVolume\0",
//     b"waveOutMessage\0",
//     b"waveOutOpen\0",
//     b"waveOutPause\0",
//     b"waveOutPrepareHeader\0",
//     b"waveOutReset\0",
//     b"waveOutRestart\0",
//     b"waveOutSetPitch\0",
//     b"waveOutSetPlaybackRate\0",
//     b"waveOutSetVolume\0",
//     b"waveOutUnprepareHeader\0",
//     b"waveOutWrite\0",
//     b"ExportByOrdinal2\0",
// ];



// pub trait ProxyDll {
//     fn get_path(&self) -> Result<PathBuf, Box<dyn error::Error>>;
//     fn get_file_name(&self) -> Result<String, Box<dyn error::Error>>;
//     fn is_compatible(&self) -> Result<bool, Box<dyn error::Error>>;
//     fn load_original(&self) -> Result<HINSTANCE, Box<dyn error::Error>>;
// }

// const MAX_PATH_SIZE: usize = MAX_PATH as usize;

// impl ProxyDll for HINSTANCE {
//     fn get_path(&self) -> Result<PathBuf, Box<dyn error::Error>> {
//         let mut path = [0u16; MAX_PATH as usize];

//         let len = unsafe {
//             GetModuleFileNameW(*self, &mut path)
//         };

//         if len <= 0 {
//             return Err("GetModuleFileNameW failed".into());
//         }

//         let path = HSTRING::from_wide(&path)?;

//         match len {
//             0 => Err("GetModuleFileNameW failed".into()),
//             _ => Ok(path)
//         }
//     }

//     fn get_file_name(&self) -> Result<String, Box<dyn error::Error>> {
//         let path = self.get_path()?;
//         let a = path.str
//     }

//     fn is_compatible(&self) -> Result<bool, Box<dyn error::Error>> {
//         let file_name = self.get_file_name()?;

//         Ok(file_name.eq("version.dll") || file_name.eq("winhttp.dll") || file_name.eq("winmm.dll"))
//     }

//     fn load_original(&self) -> Result<HINSTANCE, Box<dyn error::Error>> {
//         if !self.is_compatible()? {
//             return Err(ExportError::InvalidFileName.into());
//         }

//         let name = self.get_file_name()?;

//         let path = format!("C:\\Windows\\System32\\{}", name);
//         let path = Path::new(&path);

//         match path.exists() {
//             true => unsafe {
//                 let path_str = path.to_str().ok_or_else(|| ExportError::GetModulePath)?;
//                 let path_cstr = CString::new(path_str).map_err(|_| ExportError::GetModulePath)?;
//                 Ok(LoadLibraryA(path_cstr.as_ptr()))
//             },

//             false => Err(ExportError::LibraryNotFound.into()),
//         }
//     }
// }

// /// initializes the exports for the proxy
// ///
// /// this happens by calling `GetModuleFileName`, to find out which DLL we're trying to proxy
// /// if it's one we support, we load it from system32 and then call `GetProcAddress` on all the
// /// functions we want to proxy, those are stored in a static array, which is accessed by `global_asm!`
// ///
// /// if the DLL is not supported, we return an error
// ///
// /// # Safety
// ///
// /// this function is unsafe.
// pub fn initialize(module: HINSTANCE) -> Result<(), Box<dyn error::Error>> {
//     if module.is_invalid() {
//         return Err(Box::new(ExportError::LoadLibrary));
//     }

//     let original = module.load_original()?;

//     let name = module.get_file_name()?;

//     let exports = match name.as_str() {
//         "version.dll" => EXPORTS_VERSION.to_vec(),
//         "winhttp.dll" => EXPORTS_WINHTTP.to_vec(),
//         "winmm.dll" => EXPORTS_WINMM.to_vec(),
//         _ => return Err(Box::new(ExportError::InvalidFileName)),
//     };

//     for (index, export) in exports.iter().enumerate() {
//         let export = unsafe { CStr::from_bytes_with_nul_unchecked(export) }.as_ptr();

//         match name.as_str() {
//             "version.dll" => unsafe {
//                 OriginalFuncs_version[index] = GetProcAddress(original, export);
//             },

//             "winhttp.dll" => unsafe {
//                 OriginalFuncs_winhttp[index] = GetProcAddress(original, export);
//             },

//             "winmm.dll" => unsafe {
//                 OriginalFuncs_winmm[index] = GetProcAddress(original, export);
//             },

//             _ => return Err(Box::new(ExportError::InvalidFileName)),
//         }
//     }

//     Ok(())
// }
