use std::{path::PathBuf, error::Error};

#[cfg(target_os = "android")]
use jni::{
    objects::{JObject, JString}, JNIEnv
};

#[cfg(target_os = "android")]
static mut DATA_DIR: Option<String> = None;

pub fn get_base_dir() -> Result<PathBuf, Box<dyn Error>> {
    let args: Vec<String> = std::env::args().collect();
    #[cfg(not(target_os = "android"))]
    let mut base_dir = std::env::current_dir()?;
    #[cfg(target_os = "android")]
    let mut base_dir = PathBuf::from(unsafe { DATA_DIR.clone().unwrap() });

    for arg in args.iter() {
        if arg.starts_with("--melonloader.basedir") {
            let a: Vec<&str> = arg.split("=").collect();
            base_dir = PathBuf::from(a[1]);
        }
    }

    Ok(base_dir)
}

#[cfg(target_os = "android")]
pub fn cache_data_dir(env: &mut JNIEnv) {
    use jni::objects::JValueGen;

    use crate::log;

    let unity_class_name = "com/unity3d/player/UnityPlayer";
    let unity_class = &env
        .find_class(unity_class_name)
        .expect("Failed to find class com/unity3d/player/UnityPlayer");

    let current_activity_obj: JObject = env
        .get_static_field(unity_class, "currentActivity", "Landroid/app/Activity;")
        .expect("Failed to get static field currentActivity")
        .l().unwrap();

    let ext_file_obj: JObject = env
        .call_method(
            current_activity_obj,
            "getExternalFilesDir",
            "(Ljava/lang/String;)Ljava/io/File;",
            &[JValueGen::from(&JObject::null())],
        )
        .expect("Failed to invoke getExternalFilesDir()")
        .l().unwrap();

    let file_string: JString = env
        .call_method(&ext_file_obj, "toString", "()Ljava/lang/String;", &[])
        .expect("Failed to invoke toString()")
        .l()
        .unwrap()
        .into();

    let str_data: String = env
        .get_string(&file_string)
        .expect("Failed to get string from jstring")
        .into();

    env.delete_local_ref(ext_file_obj).expect("Failed to delete local ref");
    env.delete_local_ref(file_string).expect("Failed to delete local ref");

    unsafe {
        DATA_DIR = Some(str_data.clone());
    }
}