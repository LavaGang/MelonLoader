//! contains exports and structs for il2cpp

use std::{ffi::c_char, mem::transmute};

use libc::c_void;

use crate::{managers::il2cpp::Il2Cpp, utils::libs::MelonLib};

/// the appdomain
#[derive(Debug)]
#[repr(C)]
pub struct Il2CppDomain {}

/// the appdomain
#[derive(Debug)]
#[repr(C)]
pub struct Il2CppMethod {}

/// the appdomain
#[derive(Debug)]
#[repr(C)]
pub struct Il2CppObject {}

/// il2cpp_init
pub static mut IL2CPP_INIT: Option<extern "C" fn(*mut c_char) -> *mut Il2CppDomain> = None;
/// il2cpp_runtime_invoke
pub static mut IL2CPP_RUNTIME_INVOKE: Option<extern "C" fn(*mut Il2CppMethod, *mut Il2CppObject, *mut *mut c_void, *mut *mut Il2CppObject) -> *mut Il2CppObject> = None;
/// il2cpp_method_get_name
pub static mut IL2CPP_METHOD_GET_NAME: Option<extern "C" fn(*mut Il2CppMethod) -> *mut c_char> = None;

/// the il2cpp library
pub static mut IL2CPP_LIB: Option<MelonLib> = None;

/// init
pub fn init(il2cpp: &Il2Cpp) -> Result<(), Box<dyn std::error::Error>> {
    unsafe {
        IL2CPP_INIT = Some(transmute(il2cpp.lib.get_fn_ptr("il2cpp_init")?));
        IL2CPP_RUNTIME_INVOKE = Some(transmute(il2cpp.lib.get_fn_ptr("il2cpp_runtime_invoke")?));
        IL2CPP_METHOD_GET_NAME = Some(transmute(il2cpp.lib.get_fn_ptr("il2cpp_method_get_name")?));

        IL2CPP_LIB = Some(il2cpp.lib.clone());
    }

    Ok(())
}

/// get a function name
pub fn il2cpp_method_get_name(method: *mut Il2CppMethod) -> Result<String, Box<dyn std::error::Error>> {
    unsafe {
        let name = IL2CPP_METHOD_GET_NAME.unwrap()(method);
        Ok(std::ffi::CStr::from_ptr(name).to_str()?.to_string())
    }
}