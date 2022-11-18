//! exports
#![cfg(windows)]

use std::{arch::global_asm};


use winapi::{
	shared::{
		minwindef::{HINSTANCE, FARPROC},
	},
	um::{
		libloaderapi::{GetProcAddress},
	}
};

use crate::utils::files::win_str;

#[no_mangle]
static mut OriginalFuncs_version: [FARPROC; 17] = [0 as FARPROC; 17];

#[no_mangle]
static mut OriginalFuncs_psapi: [FARPROC; 27] = [0 as FARPROC; 27];

#[no_mangle]
static mut OriginalFuncs_winhttp: [FARPROC; 65] = [0 as FARPROC; 65];

#[no_mangle]
static mut OriginalFuncs_winmm: [FARPROC; 181] = [0 as FARPROC; 181];

#[cfg(target_pointer_width = "64")]
global_asm!(include_str!("dependencies/version.x64.S"));
#[cfg(target_pointer_width = "32")]
global_asm!(include_str!("dependencies/version.x86.S"));

#[cfg(target_pointer_width = "64")]
global_asm!(include_str!("dependencies/psapi.x64.S"));
#[cfg(target_pointer_width = "32")]
global_asm!(include_str!("dependencies/psapi.x86.S"));

#[cfg(target_pointer_width = "64")]
global_asm!(include_str!("dependencies/winhttp.x64.S"));
#[cfg(target_pointer_width = "32")]
global_asm!(include_str!("dependencies/winhttp.x86.S"));

#[cfg(target_pointer_width = "64")]
global_asm!(include_str!("dependencies/winmm.x64.S"));
#[cfg(target_pointer_width = "32")]
global_asm!(include_str!("dependencies/winmm.x86.S"));

// store a vec in a lazy static
lazy_static::lazy_static!{
	static ref EXPORTS_VERSION: Vec<&'static [u8]> = vec![
		b"GetFileVersionInfoA\0",
		b"GetFileVersionInfoByHandle\0",
		b"GetFileVersionInfoExA\0",
		b"GetFileVersionInfoExW\0",
		b"GetFileVersionInfoSizeA\0",
		b"GetFileVersionInfoSizeExA\0",
		b"GetFileVersionInfoSizeExW\0",
		b"GetFileVersionInfoSizeW\0",
		b"GetFileVersionInfoW\0",
		b"VerFindFileA\0",
		b"VerFindFileW\0",
		b"VerInstallFileA\0",
		b"VerInstallFileW\0",
		b"VerLanguageNameA\0",
		b"VerLanguageNameW\0",
		b"VerQueryValueA\0",
		b"VerQueryValueW\0"
	];

	static ref EXPORTS_PSAPI: Vec<&'static [u8]> = vec![
		b"EmptyWorkingSet\0",
		b"EnumDeviceDrivers\0",
		b"EnumPageFilesA\0",
		b"EnumPageFilesW\0",
		b"EnumProcessModules\0",
		b"EnumProcessModulesEx\0",
		b"EnumProcesses\0",
		b"GetDeviceDriverBaseNameA\0",
		b"GetDeviceDriverBaseNameW\0",
		b"GetDeviceDriverFileNameA\0",
		b"GetDeviceDriverFileNameW\0",
		b"GetMappedFileNameA\0",
		b"GetMappedFileNameW\0",
		b"GetModuleBaseNameA\0",
		b"GetModuleBaseNameW\0",
		b"GetModuleFileNameExA\0",
		b"GetModuleFileNameExW\0",
		b"GetModuleInformation\0",
		b"GetPerformanceInfo\0",
		b"GetProcessImageFileNameA\0",
		b"GetProcessImageFileNameW\0",
		b"GetProcessMemoryInfo\0",
		b"GetWsChanges\0",
		b"GetWsChangesEx\0",
		b"InitializeProcessForWsWatch\0",
		b"QueryWorkingSet\0",
		b"QueryWorkingSetEx\0"
	];

	static ref EXPORTS_WINHTTP: Vec<&'static [u8]> = vec![
		b"EmptyWorkingSet\0",
		b"EnumDeviceDrivers\0",
		b"EnumPageFilesA\0",
		b"EnumPageFilesW\0",
		b"EnumProcessModules\0",
		b"EnumProcessModulesEx\0",
		b"EnumProcesses\0",
		b"GetDeviceDriverBaseNameA\0",
		b"GetDeviceDriverBaseNameW\0",
		b"GetDeviceDriverFileNameA\0",
		b"GetDeviceDriverFileNameW\0",
		b"GetMappedFileNameA\0",
		b"GetMappedFileNameW\0",
		b"GetModuleBaseNameA\0",
		b"GetModuleBaseNameW\0",
		b"GetModuleFileNameExA\0",
		b"GetModuleFileNameExW\0",
		b"GetModuleInformation\0",
		b"GetPerformanceInfo\0",
		b"GetProcessImageFileNameA\0",
		b"GetProcessImageFileNameW\0",
		b"GetProcessMemoryInfo\0",
		b"GetWsChanges\0",
		b"GetWsChangesEx\0",
		b"InitializeProcessForWsWatch\0",
		b"QueryWorkingSet\0",
		b"QueryWorkingSetEx\0"
	];

	static ref EXPORTS_WINMM: Vec<&'static [u8]> = vec![
		b"CloseDriver\0",
		b"DefDriverProc\0",
		b"DriverCallback\0",
		b"DrvGetModuleHandle\0",
		b"GetDriverModuleHandle\0",
		b"OpenDriver\0",
		b"PlaySound\0",
		b"PlaySoundA\0",
		b"PlaySoundW\0",
		b"SendDriverMessage\0",
		b"WOWAppExit\0",
		b"auxGetDevCapsA\0",
		b"auxGetDevCapsW\0",
		b"auxGetNumDevs\0",
		b"auxGetVolume\0",
		b"auxOutMessage\0",
		b"auxSetVolume\0",
		b"joyConfigChanged\0",
		b"joyGetDevCapsA\0",
		b"joyGetDevCapsW\0",
		b"joyGetNumDevs\0",
		b"joyGetPos\0",
		b"joyGetPosEx\0",
		b"joyGetThreshold\0",
		b"joyReleaseCapture\0",
		b"joySetCapture\0",
		b"joySetThreshold\0",
		b"mciDriverNotify\0",
		b"mciDriverYield\0",
		b"mciExecute\0",
		b"mciFreeCommandResource\0",
		b"mciGetCreatorTask\0",
		b"mciGetDeviceIDA\0",
		b"mciGetDeviceIDFromElementIDA\0",
		b"mciGetDeviceIDFromElementIDW\0",
		b"mciGetDeviceIDW\0",
		b"mciGetDriverData\0",
		b"mciGetErrorStringA\0",
		b"mciGetErrorStringW\0",
		b"mciGetYieldProc\0",
		b"mciLoadCommandResource\0",
		b"mciSendCommandA\0",
		b"mciSendCommandW\0",
		b"mciSendStringA\0",
		b"mciSendStringW\0",
		b"mciSetDriverData\0",
		b"mciSetYieldProc\0",
		b"midiConnect\0",
		b"midiDisconnect\0",
		b"midiInAddBuffer\0",
		b"midiInClose\0",
		b"midiInGetDevCapsA\0",
		b"midiInGetDevCapsW\0",
		b"midiInGetErrorTextA\0",
		b"midiInGetErrorTextW\0",
		b"midiInGetID\0",
		b"midiInGetNumDevs\0",
		b"midiInMessage\0",
		b"midiInOpen\0",
		b"midiInPrepareHeader\0",
		b"midiInReset\0",
		b"midiInStart\0",
		b"midiInStop\0",
		b"midiInUnprepareHeader\0",
		b"midiOutCacheDrumPatches\0",
		b"midiOutCachePatches\0",
		b"midiOutClose\0",
		b"midiOutGetDevCapsA\0",
		b"midiOutGetDevCapsW\0",
		b"midiOutGetErrorTextA\0",
		b"midiOutGetErrorTextW\0",
		b"midiOutGetID\0",
		b"midiOutGetNumDevs\0",
		b"midiOutGetVolume\0",
		b"midiOutLongMsg\0",
		b"midiOutMessage\0",
		b"midiOutOpen\0",
		b"midiOutPrepareHeader\0",
		b"midiOutReset\0",
		b"midiOutSetVolume\0",
		b"midiOutShortMsg\0",
		b"midiOutUnprepareHeader\0",
		b"midiStreamClose\0",
		b"midiStreamOpen\0",
		b"midiStreamOut\0",
		b"midiStreamPause\0",
		b"midiStreamPosition\0",
		b"midiStreamProperty\0",
		b"midiStreamRestart\0",
		b"midiStreamStop\0",
		b"mixerClose\0",
		b"mixerGetControlDetailsA\0",
		b"mixerGetControlDetailsW\0",
		b"mixerGetDevCapsA\0",
		b"mixerGetDevCapsW\0",
		b"mixerGetID\0",
		b"mixerGetLineControlsA\0",
		b"mixerGetLineControlsW\0",
		b"mixerGetLineInfoA\0",
		b"mixerGetLineInfoW\0",
		b"mixerGetNumDevs\0",
		b"mixerMessage\0",
		b"mixerOpen\0",
		b"mixerSetControlDetails\0",
		b"mmDrvInstall\0",
		b"mmGetCurrentTask\0",
		b"mmTaskBlock\0",
		b"mmTaskCreate\0",
		b"mmTaskSignal\0",
		b"mmTaskYield\0",
		b"mmioAdvance\0",
		b"mmioAscend\0",
		b"mmioClose\0",
		b"mmioCreateChunk\0",
		b"mmioDescend\0",
		b"mmioFlush\0",
		b"mmioGetInfo\0",
		b"mmioInstallIOProcA\0",
		b"mmioInstallIOProcW\0",
		b"mmioOpenA\0",
		b"mmioOpenW\0",
		b"mmioRead\0",
		b"mmioRenameA\0",
		b"mmioRenameW\0",
		b"mmioSeek\0",
		b"mmioSendMessage\0",
		b"mmioSetBuffer\0",
		b"mmioSetInfo\0",
		b"mmioStringToFOURCCA\0",
		b"mmioStringToFOURCCW\0",
		b"mmioWrite\0",
		b"mmsystemGetVersion\0",
		b"sndPlaySoundA\0",
		b"sndPlaySoundW\0",
		b"timeBeginPeriod\0",
		b"timeEndPeriod\0",
		b"timeGetDevCaps\0",
		b"timeGetSystemTime\0",
		b"timeGetTime\0",
		b"timeKillEvent\0",
		b"timeSetEvent\0",
		b"waveInAddBuffer\0",
		b"waveInClose\0",
		b"waveInGetDevCapsA\0",
		b"waveInGetDevCapsW\0",
		b"waveInGetErrorTextA\0",
		b"waveInGetErrorTextW\0",
		b"waveInGetID\0",
		b"waveInGetNumDevs\0",
		b"waveInGetPosition\0",
		b"waveInMessage\0",
		b"waveInOpen\0",
		b"waveInPrepareHeader\0",
		b"waveInReset\0",
		b"waveInStart\0",
		b"waveInStop\0",
		b"waveInUnprepareHeader\0",
		b"waveOutBreakLoop\0",
		b"waveOutClose\0",
		b"waveOutGetDevCapsA\0",
		b"waveOutGetDevCapsW\0",
		b"waveOutGetErrorTextA\0",
		b"waveOutGetErrorTextW\0",
		b"waveOutGetID\0",
		b"waveOutGetNumDevs\0",
		b"waveOutGetPitch\0",
		b"waveOutGetPlaybackRate\0",
		b"waveOutGetPosition\0",
		b"waveOutGetVolume\0",
		b"waveOutMessage\0",
		b"waveOutOpen\0",
		b"waveOutPause\0",
		b"waveOutPrepareHeader\0",
		b"waveOutReset\0",
		b"waveOutRestart\0",
		b"waveOutSetPitch\0",
		b"waveOutSetPlaybackRate\0",
		b"waveOutSetVolume\0",
		b"waveOutUnprepareHeader\0",
		b"waveOutWrite\0",
		b"ExportByOrdinal2\0"
	];
}




static mut ORIGINAL_PROXY: Option<HINSTANCE> = None;


/// load exports
#[allow(named_asm_labels)]
pub fn load(orig: HINSTANCE, index: i32) -> Result<(), Box<dyn std::error::Error>> {
	let index = index as usize;
    unsafe {
		ORIGINAL_PROXY = Some(orig);

		let func_arrays: [Vec<&'static [u8]>; 4] = [
			EXPORTS_PSAPI.to_vec(),
			EXPORTS_VERSION.to_vec(),
			EXPORTS_WINHTTP.to_vec(),
			EXPORTS_WINMM.to_vec()
		];


        for (i, export) in func_arrays[index].iter().enumerate() {
			let export_name = win_str(export);
			let orig = ORIGINAL_PROXY.ok_or_else(|| "original proxy not loaded")?;

			match index {
				0 => {
					OriginalFuncs_psapi[i] = GetProcAddress(orig, export_name);
				},

				1 => {
					OriginalFuncs_version[i] = GetProcAddress(orig, export_name);
				},

				2 => {
					OriginalFuncs_winhttp[i] = GetProcAddress(orig, export_name);
				},

				3 => {
					OriginalFuncs_winmm[i] = GetProcAddress(orig, export_name);
				},

				_ => return Err("Invalid Proxy Filename".into()),
			}

		}
	}


	Ok(())
}