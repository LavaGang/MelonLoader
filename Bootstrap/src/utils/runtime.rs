use unity_rs::runtime::FerrexRuntime;

use crate::errors::DynErr;

#[allow(dead_code)]
pub static mut RUNTIME: Option<FerrexRuntime> = None;

#[macro_export]
macro_rules! runtime {
    () => {
        $crate::utils::runtime::get_runtime()
    };
}

pub fn get_runtime() -> Result<&'static FerrexRuntime, DynErr> {
    unsafe {
        if RUNTIME.is_none() {
            RUNTIME = Some(unity_rs::runtime::get_runtime()?)
        }

        Ok(RUNTIME.as_ref().ok_or("Failed to get runtime")?)
    }
}
