use colored::Color;
use std::ffi::{c_char, c_void};

use unity_rs::{
    il2cpp::types::{Il2CppDomain, Il2CppMethod, Il2CppObject},
    mono::types::{MonoDomain, MonoMethod, MonoObject},
};

pub type InvokeFnMono =
    extern "C" fn(*mut MonoMethod, *mut MonoObject, *mut *mut c_void, *mut *mut MonoObject) -> *mut MonoObject;
pub type InvokeFnIl2Cpp = extern "C" fn(
    *mut Il2CppMethod,
    *mut Il2CppObject,
    *mut *mut c_void,
    *mut *mut Il2CppObject,
) -> *mut Il2CppObject;

pub type InitFnMono = extern "C" fn(*const c_char, *const c_char) -> *mut MonoDomain;
pub type InitFnIl2Cpp = extern "C" fn(*const c_char) -> *mut Il2CppDomain;

pub const MELON_VERSION: &str = "0.6.6";

pub const IS_ALPHA: bool = false;

pub const RED: Color = Color::TrueColor {
    r: (205),
    g: (92),
    b: (92),
};

pub const GREEN: Color = Color::TrueColor {
    r: (50),
    g: (205),
    b: (50),
};

pub const BLUE: Color = Color::TrueColor {
    r: (100),
    g: (149),
    b: (237),
};

pub const YELLOW: Color = Color::TrueColor {
    r: (255),
    g: (255),
    b: (0),
};

#[derive(Debug, Clone, Copy)]
pub struct W<T>(pub T);