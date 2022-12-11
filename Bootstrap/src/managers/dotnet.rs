use std::{error, ffi::CString, str::FromStr, ptr::addr_of_mut, mem::transmute};

use libc::c_void;
use netcorehost::{nethost, pdcstr, pdcstring::PdCString};
use unity_rs::il2cpp::Il2Cpp;

use crate::{utils::files, debug};

use super::internal_calls;

#[repr(C)]
pub struct HostImports {
    //public delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, void**, void> LoadAssemblyAndGetPtr;
    pub load_assembly_get_ptr: *mut c_void,

    pub initialize: *mut c_void,
    pub pre_start: *mut c_void,
    pub start: *mut c_void,
}

#[repr(C)]
pub struct HostExports {
    pub hook_attach: fn(*mut *mut c_void, *mut c_void),
    pub hook_detach: fn(*mut *mut c_void, *mut c_void),
}

static mut IMPORTS: Option<HostImports> = None;

pub fn init(il2cpp: &Il2Cpp) -> Result<(), Box<dyn error::Error>> {
    let hostfxr = nethost::load_hostfxr()?;
    let config_path = files::runtime_dir()?.join("MelonLoader.runtimeconfig.json");

    if !config_path.exists() {
        Err("Failed to find the MelonLoader.runtimeconfig.json file!")?
    }

    let config_path = config_path.to_str().ok_or("Failed to convert the path to a string!")?;

    let context = hostfxr.initialize_for_runtime_config(PdCString::from_str(config_path)?)?;

    let runtime_dir = files::runtime_dir()?;

    let ml_managed_path = runtime_dir.join("MelonLoader.NativeHost.dll");
    let wide_ml_managed_path = wide_str(ml_managed_path.to_str().ok_or("Failed to convert the path to a string!")?);
    let ml_managed_path = PdCString::from_os_str(ml_managed_path.as_os_str())?;

    let dotnet_type = pdcstr!("MelonLoader.NativeHost.NativeEntryPoint, MelonLoader.NativeHost");
    let wide_dotnet_type = wide_str("MelonLoader.NativeHost.NativeEntryPoint, MelonLoader.NativeHost");
    let dotnet_method = pdcstr!("LoadStage1");
    let dotnet_method_stage_2 = wide_str("LoadStage2");

    let loader = context.get_delegate_loader_for_assembly(ml_managed_path.clone())?;
    let init = loader.get_function_with_unmanaged_callers_only::<fn(*mut HostImports)>(dotnet_type, dotnet_method)?;
    
    let mut imports = HostImports {
        load_assembly_get_ptr: std::ptr::null_mut(),
        initialize: std::ptr::null_mut(),
        pre_start: std::ptr::null_mut(),
        start: std::ptr::null_mut(),
    };

    let mut exports = HostExports {
        hook_attach: internal_calls::NativeHookAttach,
        hook_detach: internal_calls::NativeHookDetach,
    };

    debug!("[Dotnet] Invoking LoadStage1...")?;

    init(addr_of_mut!(imports));

    debug!("[Dotnet] Reloading NativeHost into correct load context and getting LoadStage2 pointer...")?;

    let mut init2 = std::ptr::null_mut();

    unsafe {

        let func: unsafe extern "C" fn(isize, isize, isize, *mut *mut c_void) = transmute(imports.load_assembly_get_ptr);

        func(
            wide_ml_managed_path.as_ptr() as isize,
            wide_dotnet_type.as_ptr() as isize,
            dotnet_method_stage_2.as_ptr() as isize,
            addr_of_mut!(init2),
        );
    }

    debug!("[Dotnet] Invoking LoadStage2...")?;
    unsafe {
        let func = transmute::<*mut c_void, fn(*mut HostImports, *mut HostExports)>(init2);
        func(addr_of_mut!(imports), addr_of_mut!(exports));

        if imports.initialize.is_null() {
            Err("Failed to find HostImports::Initialize!")?
        }

        let func: unsafe extern "system" fn() = transmute(imports.initialize);

        func();

        IMPORTS = Some(imports);
    }

    Ok(())
}

pub fn pre_start() -> Result<(), Box<dyn error::Error>> {
    unsafe {
        if let Some(imports) = &IMPORTS {
            let func: unsafe extern "system" fn() = transmute(imports.pre_start);
            func();
        } else {
            Err("Failed to find the HostImports::PreStart!")?
        }
    }

    Ok(())
}

pub fn start() -> Result<(), Box<dyn error::Error>> {
    unsafe {
        if let Some(imports) = &IMPORTS {
            let func: unsafe extern "system" fn() = transmute(imports.start);
            func();
        } else {
            Err("Failed to find the HostImports::Start!")?
        }
    }

    Ok(())
}

pub fn wide_str(s: &str) -> Vec<u16> {
    s.encode_utf16().chain(Some(0)).collect()
}