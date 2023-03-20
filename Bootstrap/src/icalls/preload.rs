use unity_rs::{mono::types::MonoString, common::string::UnityString};

use crate::{melonenv, internal_failure, runtime};

pub fn get_managed_dir() -> *mut MonoString {
    let path = melonenv::paths::get_managed_dir().unwrap_or_else(|e| {
        internal_failure!("Failed to get managed dir: {e}");
    });

    let path = path.to_str().unwrap_or_else(|| {
        internal_failure!("Failed to convert managed dir to str");
    });

    let runtime = runtime!().unwrap_or_else(|e| {
        internal_failure!("Failed to get runtime: {e}");
    });

    UnityString::from_string(path, runtime).unwrap_or_else(|e| {
        internal_failure!("Failed to create UnityString: {e}");
    }).inner.cast()
}