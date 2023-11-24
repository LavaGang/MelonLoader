pub mod utils;
pub mod nativeloader;

use jni::{
    objects::{JClass, JString},
    sys::{jboolean, jint, JNI_VERSION_1_6},
    JNIEnv, JavaVM, NativeMethod,
};
use std::{mem, os::raw::c_void, panic::catch_unwind, path::PathBuf};

use crate::impl_android::utils::libs::load_lib;

const INVALID_JNI_VERSION: jint = 0;

#[allow(non_snake_case)]
#[no_mangle]
pub extern "system" fn JNI_OnLoad(vm: JavaVM, _: *mut c_void) -> jint {
    android_log::init("MelonLoader").unwrap();

    let mut env: JNIEnv = vm.get_env().expect("Cannot get reference to the JNIEnv");
    vm.attach_current_thread()
        .expect("Unable to attach current thread to the JVM");

    let self_lib = load_lib(&PathBuf::from("libmain.so"), libc::RTLD_NOW | libc::RTLD_GLOBAL)
        .expect("Failed to load self");

    let load_handle: utils::libs::NativeMethod<fn(JNIEnv, JClass, JString) -> jboolean> =
        self_lib.sym("load")
        .expect("Failed to find load function");

    let unload_handle: utils::libs::NativeMethod<fn(JNIEnv, JClass)> = self_lib
        .sym("unload")
        .expect("Failed to find unload function");

    let NativeLoader_Methods: &[NativeMethod] = &[
        NativeMethod {
            name: "load".into(),
            sig: "(Ljava/lang/String;)Z".into(),
            fn_ptr: unsafe { mem::transmute(load_handle) },
        },
        NativeMethod {
            name: "unload".into(),
            sig: "()Z".into(),
            fn_ptr: unsafe { mem::transmute(unload_handle) },
        },
    ];

    let nativeloader_class = env
        .find_class("com/unity3d/player/NativeLoader")
        .expect("Cannot find NativeLoader class");
    env.register_native_methods(nativeloader_class, NativeLoader_Methods)
        .expect("Failed to register native methods");

    catch_unwind(|| JNI_VERSION_1_6).unwrap_or(INVALID_JNI_VERSION)
}