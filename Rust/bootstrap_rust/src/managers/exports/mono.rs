//! exports for mono

use std::{error, ffi::{c_char, c_void, c_ushort, CStr}, fmt::{self, Display}, ptr::{self, addr_of_mut}};

use thiserror::Error;

use crate::{utils::libs::MelonLib, managers::{game, mono}, debug};

/// mono export errors
#[derive(Debug, Error)]
pub enum MonoExportError {
    /// MonoLib is None
    #[error("MonoLib is None!")]
    MonoLibIsNone,

    /// Failed to find function
    #[error("Failed to find function!")]
    FailedToFindFunction,

    /// Returned null
    #[error("Returned null!")]
    ReturnedNull,
}

/// the appdomain
#[derive(Debug)]
#[repr(C)]
pub struct MonoDomain {}

/// a thread
#[derive(Debug)]
#[repr(C)]
pub struct MonoThread {}

/// a method
#[derive(Debug)]
#[repr(C)]
pub struct MonoMethod {}

/// a class
#[derive(Debug)]
#[repr(C)]
pub struct MonoClass {}

/// an assembly
#[derive(Debug)]
#[repr(C)]
pub struct MonoAssembly {}

/// a mono image
#[derive(Debug)]
#[repr(C)]
pub struct MonoImage {}

/// a mono string
#[derive(Debug)]
#[repr(C)]
pub struct MonoString {}

/// a mono object
#[derive(Debug)]
#[repr(C)]
pub struct MonoObject {
    /// the vtable
    pub vtable: *mut c_void,
    /// the sync
    pub syncchronisation: *mut c_void,
}

/// a reflection assembly
#[derive(Debug)]
#[repr(C)]
pub struct MonoReflectionAssembly {
    /// the object
    pub object: MonoObject,
    /// the assembly
    pub assembly: *mut MonoAssembly,
    /// evidence
    pub evidence: *mut MonoObject,
}

/// an assembly name
#[derive(Debug)]
#[repr(C)]
pub struct AssemblyName {
    /// the name
    pub name: *mut c_char,
    /// the culture
    pub culture: *mut c_char,
    /// the hash value
    pub hash_value: *mut c_char,
    /// the public key
    pub public_key: *mut c_char,

    /// the hash algorithm
    pub public_key_token: [c_char; 17],

    /// the hash algorithm
    pub hash_alg: u32,
    /// the hash algorithm
    pub hash_len: u32,

    /// the flags
    pub flags: u32,
    /// the major version
    pub major: c_ushort,
    /// the minor version
    pub minor: c_ushort,
    /// the build number
    pub build: c_ushort,
    /// the revision number
    pub revision: c_ushort,
    /// the processor architecture
    pub arch: u32,
}

/// The Mono Library
pub static mut MONO_LIB: Option<MelonLib> = None;

/// initializes mono
pub fn mono_jit_init_version(domain_name: &str, version: &str) -> Result<Box<MonoDomain>, Box<dyn error::Error>> {
    let domain_name = std::ffi::CString::new(domain_name)?;
    let version = std::ffi::CString::new(version)?;

    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr("mono_jit_init_version")?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    let func: unsafe extern "C" fn(*const c_char, *const c_char) -> *mut MonoDomain = unsafe { std::mem::transmute(func) };

    let domain = unsafe {
        func(domain_name.as_ptr(), version.as_ptr())
    };

    if domain.is_null() {
        return Err(Box::new(MonoExportError::ReturnedNull));
    }

    unsafe { Ok(Box::from_raw(domain)) }
}

/// initializes mono, but unprotected (i lost)
pub unsafe fn mono_jit_init_raw(domain_name: *const c_char, version: *const c_char) -> *mut MonoDomain {
    let lib = MONO_LIB.as_ref();

    if lib.is_none() {
        return std::ptr::null_mut();
    }

    let lib = lib.unwrap();

    let func = lib.get_fn_ptr("mono_jit_init_version");

    if func.is_err() {
        return std::ptr::null_mut();
    }

    let func = func.unwrap();

    let func: unsafe extern "C" fn(*const c_char, *const c_char) -> *mut MonoDomain = std::mem::transmute(func);

    func(domain_name, version)
}

/// turns the domain into a debug domain
pub fn mono_debug_domain_create(domain: *mut MonoDomain) -> Result<(), Box<dyn error::Error>> {
    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr("mono_debug_domain_create")
    };

    if func.is_err() {
        return Ok(()); //Some games don't have this function
    }

    let func = func.unwrap();

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    unsafe {
        let func: unsafe extern "C" fn(*mut MonoDomain) = std::mem::transmute(func);
    
        func(domain);
    }

    Ok(())
}

/// returns the current thread
pub fn mono_thread_current() -> Result<Box<MonoThread>, Box<dyn error::Error>> {
    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr("mono_thread_current")?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    let func: unsafe extern "C" fn() -> *mut MonoThread = unsafe { std::mem::transmute(func) };

    let thread = unsafe {
        func()
    };

    if thread.is_null() {
        return Err(Box::new(MonoExportError::ReturnedNull));
    }

    unsafe { Ok(Box::from_raw(thread)) }
}

/// sets the main thread
pub fn mono_thread_set_main(thread: Box<MonoThread>) -> Result<(), Box<dyn error::Error>> {
    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr("mono_thread_set_main")?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    unsafe {
        let func: unsafe extern "C" fn(*mut MonoThread) = std::mem::transmute(func);
    
        func(Box::into_raw(thread));
    }

    Ok(())
}

/// sets the domain config
pub fn mono_domain_set_config(domain: *mut MonoDomain, base_dir: &str, config_file_name: *const c_char) -> Result<(), Box<dyn error::Error>> {
    let base_dir = std::ffi::CString::new(base_dir)?;

    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr("mono_domain_set_config")?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    let func: unsafe extern "C" fn(*mut MonoDomain, *const c_char, *const c_char) = unsafe { std::mem::transmute(func) };

    unsafe {
        func(domain, base_dir.as_ptr(), config_file_name);
    }

    Ok(())
}

/// adds an internal call
pub fn mono_add_internal_call(name: &str, method: *const c_void) -> Result<(), Box<dyn error::Error>> {
    let name = std::ffi::CString::new(name)?;

    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr("mono_add_internal_call")?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    let func: unsafe extern "C" fn(*const c_char, *const c_void) = unsafe { std::mem::transmute(func) };

    unsafe {
        func(name.as_ptr(), method);
    }

    Ok(())
}

/// get the root domain
pub fn mono_get_root_domain() -> Result<*mut MonoDomain, Box<dyn error::Error>> {
    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr("mono_get_root_domain")?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    let func: unsafe extern "C" fn() -> *mut MonoDomain = unsafe { std::mem::transmute(func) };

    let domain = unsafe {
        func()
    };

    if domain.is_null() {
        return Err(Box::new(MonoExportError::ReturnedNull));
    }

    Ok(domain)
}

/// creates a new MonoString from raw data
pub fn mono_string_new_raw(str: *const c_char) -> Result<*mut MonoString, Box<dyn error::Error>> {
    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr("mono_string_new")?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    let func: unsafe extern "C" fn(*mut MonoDomain, *const c_char) -> *mut MonoString = unsafe { std::mem::transmute(func) };

    let domain = mono_get_root_domain()?;

    let res = unsafe {
        func(domain, str)
    };

    if res.is_null() {
        return Err(Box::new(MonoExportError::ReturnedNull));
    }

    Ok(res)
}

/// creates a new MonoString
pub fn mono_string_new(str: &str) -> Result<*mut MonoString, Box<dyn error::Error>> {
    let str = std::ffi::CString::new(str)?;

    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr("mono_string_new")?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    let func: unsafe extern "C" fn(*mut MonoDomain, *const c_char) -> *mut MonoString = unsafe { std::mem::transmute(func) };

    let domain = mono_get_root_domain()?;

    let res = unsafe {
        func(domain, str.as_ptr())
    };

    if res.is_null() {
        return Err(Box::new(MonoExportError::ReturnedNull));
    }

    Ok(res)
}

/// open an assemly
pub fn mono_domain_assembly_open(path: &str) -> Result<*mut MonoAssembly, Box<dyn error::Error>> {
    let path = std::ffi::CString::new(path)?;

    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr("mono_domain_assembly_open")?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    let func: unsafe extern "C" fn(*mut MonoDomain, *const c_char) -> *mut MonoAssembly = unsafe { std::mem::transmute(func) };

    let domain = mono_get_root_domain()?;

    let res = unsafe {
        func(domain, path.as_ptr())
    };

    if res.is_null() {
        return Err(Box::new(MonoExportError::ReturnedNull));
    }

    Ok(res)
}

/// get the image from an assembly
pub fn mono_assembly_get_image(assembly: *mut MonoAssembly) -> Result<*mut MonoImage, Box<dyn error::Error>> {
    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr("mono_assembly_get_image")?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    let func: unsafe extern "C" fn(*mut MonoAssembly) -> *mut MonoImage = unsafe { std::mem::transmute(func) };

    let res = unsafe {
        func(assembly)
    };

    if res.is_null() {
        return Err(Box::new(MonoExportError::ReturnedNull));
    }

    Ok(res)
}

/// get the class from an image
pub fn mono_class_from_name(image: *mut MonoImage, namespace: &str, name: &str) -> Result<*mut MonoClass, Box<dyn error::Error>> {
    let namespace = std::ffi::CString::new(namespace)?;
    let name = std::ffi::CString::new(name)?;

    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr("mono_class_from_name")?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    let func: unsafe extern "C" fn(*mut MonoImage, *const c_char, *const c_char) -> *mut MonoClass = unsafe { std::mem::transmute(func) };

    let res = unsafe {
        func(image, namespace.as_ptr(), name.as_ptr())
    };

    if res.is_null() {
        return Err(Box::new(MonoExportError::ReturnedNull));
    }

    Ok(res)
}

/// get the method from a class
pub fn mono_class_get_method_from_name(class: *mut MonoClass, name: &str, param_count: i32) -> Result<*mut MonoMethod, Box<dyn error::Error>> {
    let name = std::ffi::CString::new(name)?;

    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr("mono_class_get_method_from_name")?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    let func: unsafe extern "C" fn(*mut MonoClass, *const c_char, i32) -> *mut MonoMethod = unsafe { std::mem::transmute(func) };


    let res = unsafe {
        func(class, name.as_ptr(), param_count)
    };

    if res.is_null() {
        return Err(Box::new(MonoExportError::ReturnedNull));
    }

    Ok(res)
}

/// invoke a method
pub fn mono_runtime_invoke(method: *mut MonoMethod, obj: Option<*mut MonoObject>, params: Option<&mut Vec<*mut c_void>>) -> Result<Option<*mut MonoObject>, Box<dyn error::Error>> {
    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr("mono_runtime_invoke")?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    let func: unsafe extern "C" fn(*mut MonoMethod, *mut MonoObject, *mut *mut c_void, *mut *mut MonoObject) -> *mut MonoObject = unsafe { std::mem::transmute(func) };
    
    let exc: *mut MonoObject = std::ptr::null_mut();

    let object = match obj {
        Some(obj) => obj,
        None => std::ptr::null_mut()
    };

    #[allow(unused_assignments)]
    let mut res: *mut MonoObject = ptr::null_mut();

    match params.is_none() {
        true => {
            res = unsafe {
                func(method, object, ptr::null_mut(), exc as *mut *mut MonoObject)
            };
        }
        false => {
            let params = params.unwrap();

            res = unsafe {
                func(method, object, addr_of_mut!(params[0]), exc as *mut *mut MonoObject)
            };
        }
    }

    if !exc.is_null() {
        let exception = MonoException::new(exc)?;
        return Err(Box::new(exception));
    }


    if res.is_null() {
        return Ok(None);
    }

    Ok(Some(res))
}

/// invoke a method raw
pub fn mono_runtime_invoke_raw(method: *mut MonoMethod, obj: *mut MonoObject, params: *mut *mut c_void, exception: *mut *mut MonoObject) -> Result<*mut MonoObject, Box<dyn error::Error>> {
    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr("mono_runtime_invoke")?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    let func: unsafe extern "C" fn(*mut MonoMethod, *mut MonoObject, *mut *mut c_void, *mut *mut MonoObject) -> *mut MonoObject = unsafe { std::mem::transmute(func) };
    
    unsafe { Ok(func(method, obj, params, exception)) }
}

/// a mono exception
#[derive(Debug, Error)]
pub struct MonoException {
    /// a string representation of the exception
    pub message: String,
}

impl MonoException {
    /// takes a MonoObject and returns a MonoException
    pub fn new(exc: *mut MonoObject) -> Result<MonoException, Box<dyn error::Error>> {
        let exc_str = unsafe { exception_to_str(exc)? };
        let exc_utf8 = mono_string_to_utf8(exc_str)?;

        Ok(MonoException {
            message: exc_utf8,
        })
    }
}

impl fmt::Display for MonoException {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        write!(f, "{}", self.message)
    }
}

unsafe fn exception_to_str(exception: *mut MonoObject) -> Result<*mut MonoString, Box<dyn error::Error>> {
    if exception.is_null() {
        return Err(Box::new(MonoExportError::ReturnedNull));
    }

    let game_data = game::init()?;
    let mono = mono::init(&game_data.base_path, &game_data.data_path, game_data.il2cpp)?
        .ok_or(mono::MonoError::FailedToInit)?;

    match mono.old {
        true => {
            let mscorlib = mono_domain_assembly_open("mscorlib")?;
            let mscorlib_image = mono_assembly_get_image(mscorlib)?;
            let exception_class = mono_class_from_name(mscorlib_image, "System", "Exception")?;
            let message_method = mono_class_get_method_from_name(exception_class, "ToString", 0)?;

            let res = mono_runtime_invoke(message_method, Some(exception), None)?
                .ok_or(MonoExportError::ReturnedNull)?;
    
            Ok(res.cast())
        }
        false => {
            let res = mono_object_to_string_raw(exception, ptr::null_mut())?;
            match res.is_null() {
                true => Err(Box::new(MonoExportError::ReturnedNull)),
                false => Ok(res)
            }
        }
    }
}

/// get the string representation of a mono object
pub fn mono_object_to_string_raw(obj: *mut MonoObject, exc: *mut *mut MonoObject) -> Result<*mut MonoString, Box<dyn error::Error>> {
    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr("mono_object_to_string")?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    let func: unsafe extern "C" fn(*mut MonoObject, *mut *mut MonoObject) -> *mut MonoString = unsafe { std::mem::transmute(func) };

    let res = unsafe {
        func(obj, exc)
    };

    if res.is_null() {
        return Err(Box::new(MonoExportError::ReturnedNull));
    }

    Ok(res)
}

/// turn string into valid utf8
pub fn mono_string_to_utf8(str: *mut MonoString) -> Result<String, Box<dyn error::Error>> {
    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr("mono_string_to_utf8")?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    let func: unsafe extern "C" fn(*mut MonoString) -> *const c_char = unsafe { std::mem::transmute(func) };

    let res = unsafe {
        func(str)
    };

    if res.is_null() {
        return Err(Box::new(MonoExportError::ReturnedNull));
    }

    let res = unsafe { CStr::from_ptr(res) };
    let res = res.to_str()?;

    Ok(res.to_string())
}

/// get the name of a method
pub fn mono_method_get_name(method: *mut MonoMethod) -> Result<String, Box<dyn error::Error>> {
    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr("mono_method_get_name")?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    let func: unsafe extern "C" fn(*mut MonoMethod) -> *const c_char = unsafe { std::mem::transmute(func) };

    let res = unsafe {
        func(method)
    };

    if res.is_null() {
        return Err(Box::new(MonoExportError::ReturnedNull));
    }

    let res = unsafe { CStr::from_ptr(res) };
    let res = res.to_str()?;

    Ok(res.to_string())
}

/// install an assembly hook
pub fn mono_install_assembly_hook(hook_type: AssemblyHookType, address: *mut c_void) -> Result<(), Box<dyn error::Error>> {
    let name = format!("mono_install_assembly_{}_hook", hook_type);
    debug!("installing assembly hook: {}", name)?;

    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr(&name)?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    let func: unsafe extern "C" fn(*mut c_void, *mut c_void) = unsafe { std::mem::transmute(func) };

    unsafe {
        func(address, ptr::null_mut());
    }

    Ok(())
}

/// assembly to object
pub fn mono_assembly_get_object(asm: *mut MonoAssembly) -> Result<*mut MonoObject, Box<dyn error::Error>> {
    let func = unsafe {
        MONO_LIB
        .as_ref()
        .ok_or(MonoExportError::MonoLibIsNone)?
        .get_fn_ptr("mono_assembly_get_object")?
    };

    if func.is_null() {
        return Err(Box::new(MonoExportError::FailedToFindFunction));
    }

    let func: unsafe extern "C" fn(*mut MonoDomain, *mut MonoAssembly) -> *mut MonoObject = unsafe { std::mem::transmute(func) };

    let domain = mono_get_root_domain()?;

    let res = unsafe {
        func(domain, asm)
    };

    if res.is_null() {
        return Err(Box::new(MonoExportError::ReturnedNull));
    }

    Ok(res)
}

/// assembly hook types
#[derive(Debug, Clone, Copy)]
pub enum AssemblyHookType {
    /// called when an assembly is loaded
    Preload,
    /// called when an assembly is unloaded
    Load,
    /// called when an assembly is searched
    Search,
}

impl Display for AssemblyHookType {
    fn fmt(&self, f: &mut fmt::Formatter<'_>) -> fmt::Result {
        match self {
            AssemblyHookType::Preload => write!(f, "preload"),
            AssemblyHookType::Load => write!(f, "load"),
            AssemblyHookType::Search => write!(f, "search"),
        }
    }
}