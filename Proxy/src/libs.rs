//! cross platform utilities for permanently loading libraries
//!
//! Main use case here is just permanently having mono/dobby/etc loaded, unlike with libloading,
//! where they get unloaded when the library goes out of scope.
//!
//! # Safety
//! this is incredibly unsafe
//!
//! # Examples
//!
//! ```
//! use std::path::PathBuf;
//! use std::mem;
//!
//! use unity_rs::libs::load_lib;
//!
//! let path = PathBuf::from("path/to/lib");
//! let lib = load_lib(&path)?;
//!
//! let func: extern fn() = unsafe { mem::transmute(lib.get_fn_ptr("func_name")?) };

use std::{
    ffi::c_void,
    marker::PhantomData,
    ops::Deref,
    path::{Path, PathBuf},
};
use thiserror::Error;

/// possible library loading errors
#[derive(Debug, Error)]
pub enum LibError {
    /// failed to load library
    #[error("Failed to load library!")]
    FailedToLoadLib,

    /// failed to get lib name
    #[error("Failed to get lib name!")]
    FailedToGetLibName,

    /// failed to get lib path
    #[error("Failed to get lib path!")]
    FailedToGetLibPath,

    /// failed to get function pointer
    #[error("Failed to get function pointer!")]
    FailedToGetFnPtr,

    #[error("Failed to create C-String")]
    FailedToCreateCString,
}

/// a representation of a permanently loaded library
#[derive(Debug, Clone)]
pub struct NativeLibrary {
    /// the name of the lib
    pub name: String,
    /// the path to the lib
    pub path: PathBuf,
    /// the pointer to the lib
    pub handle: *mut c_void,
}

impl NativeLibrary {
    /// gets a function pointer
    #[cfg(target_os = "linux")]
    pub fn sym<T>(&self, name_str: &str) -> Result<NativeMethod<T>, LibError> {
        let name = std::ffi::CString::new(name_str).map_err(|_| LibError::FailedToCreateCString)?;
        let ptr = unsafe { libc::dlsym(self.handle, name.as_ptr()) };
        if ptr.is_null() {
            return Err(LibError::FailedToGetFnPtr);
        }

        Ok(NativeMethod {
            inner: ptr.cast(),
            pd: PhantomData,
        })
    }

    /// gets a function pointer
    #[cfg(target_os = "windows")]
    pub fn sym<T>(&self, name_str: &str) -> Result<NativeMethod<T>, LibError> {
        use std::ffi::CString;

        use winapi::um::libloaderapi::GetProcAddress;

        let name = CString::new(name_str).map_err(|_| LibError::FailedToCreateCString)?;

        let ptr = unsafe { GetProcAddress(self.handle.cast(), name.as_ptr()) };
        if ptr.is_null() {
            return Err(LibError::FailedToGetFnPtr);
        }

        Ok(NativeMethod {
            inner: ptr.cast(),
            pd: PhantomData,
        })
    }
}

/// loads a library permanently
///
/// # Arguments
///
/// * `path` - the path to the library
///
/// # Errors
///
/// * `LibError::FailedToLoadLib` - if the library failed to load
/// * `LibError::FailedToGetLibName` - if the library name failed to be retrieved
///
/// # Safety
///
/// this function is unsafe because it permanently loads a library
///
/// # Examples
///
/// ```
/// use std::path::PathBuf;
///
/// use unity_rs::libs::load_lib;
///
/// let path = PathBuf::from("path/to/lib");
/// let lib = load_lib(&path);
///
/// assert!(lib.is_ok());
#[cfg(target_os = "linux")]
pub fn load_lib<P: AsRef<Path>>(path: P) -> Result<NativeLibrary, LibError> {
    use std::ffi::CString;

    let path = path.as_ref();

    let path_string = path.to_str().ok_or(LibError::FailedToGetLibPath)?;

    let c_path = CString::new(path_string).map_err(|_| LibError::FailedToCreateCString)?;

    let lib = unsafe { libc::dlopen(c_path.as_ptr(), libc::RTLD_NOW | libc::RTLD_GLOBAL) };

    if lib.is_null() {
        return Err(LibError::FailedToLoadLib);
    }

    let lib_name = path
        .file_name()
        .ok_or(LibError::FailedToGetLibName)?
        .to_str()
        .ok_or(LibError::FailedToGetLibName)?
        .to_string();

    Ok(NativeLibrary {
        name: lib_name,
        path: path.to_path_buf(),
        handle: lib,
    })
}

/// loads a library permanently
///
/// # Arguments
///
/// * `path` - the path to the library
///
/// # Errors
///
/// * `LibError::FailedToLoadLib` - if the library failed to load
/// * `LibError::FailedToGetLibName` - if the library name failed to be retrieved
///
/// # Safety
///
/// this function is unsafe because it permanently loads a library
///
/// # Examples
///
/// ```
/// use std::path::PathBuf;
///
/// use unity_rs::libs::load_lib;
///
/// let path = PathBuf::from("path/to/lib");
/// let lib = load_lib(&path);
///
/// assert!(lib.is_ok());
#[cfg(target_os = "windows")]
pub fn load_lib<P: AsRef<Path>>(path: P) -> Result<NativeLibrary, LibError> {
    use std::ffi::CString;

    let path = path.as_ref();

    use winapi::um::libloaderapi::LoadLibraryA;

    let path_string = path.to_str().ok_or_else(|| LibError::FailedToGetLibPath)?;
    let win_path = CString::new(path_string).map_err(|_| LibError::FailedToCreateCString)?;

    let lib = unsafe { LoadLibraryA(win_path.as_ptr()) };

    if lib.is_null() {
        return Err(LibError::FailedToLoadLib);
    }

    let lib_name = path
        .file_name()
        .ok_or(LibError::FailedToGetLibName)?
        .to_str()
        .ok_or(LibError::FailedToGetLibName)?
        .to_string();

    Ok(NativeLibrary {
        name: lib_name,
        path: path.to_path_buf(),
        handle: lib.cast(),
    })
}

#[derive(Debug)]
pub struct NativeMethod<T> {
    pub inner: *mut c_void,
    pd: PhantomData<T>,
}

unsafe impl<T: Send> Send for NativeMethod<T> {}
unsafe impl<T: Sync> Sync for NativeMethod<T> {}

impl<T> Clone for NativeMethod<T> {
    fn clone(&self) -> NativeMethod<T> {
        NativeMethod { ..*self }
    }
}

impl<T> Deref for NativeMethod<T> {
    type Target = T;

    fn deref(&self) -> &T {
        unsafe { &*(&self.inner as *const *mut _ as *const T) }
    }
}
