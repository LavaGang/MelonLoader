use std::error::Error;

use unity_rs::runtime::{Runtime, UnityRuntime};

use super::dotnet;

pub fn init() -> Result<(), Box<dyn Error>> {
    let runtime = Runtime::new()?;
    let runtime = runtime.runtime;

    match runtime {
        UnityRuntime::MonoRuntime(mono) => {
            super::mono::init(&mono)?;
        },

        UnityRuntime::Il2Cpp(il2cpp) => {
            dotnet::init(&il2cpp)?;
        }
    }
    Ok(())
}

pub fn pre_start() -> Result<(), Box<dyn Error>> {
    let runtime = Runtime::new()?;
    let runtime = runtime.runtime;

    match runtime {
        UnityRuntime::MonoRuntime(_) => {
            super::mono::pre_start()?;
        },

        UnityRuntime::Il2Cpp(_) => {
            dotnet::pre_start()?;
        }
    }
    Ok(())
}

pub fn start() -> Result<(), Box<dyn Error>> {
    let runtime = Runtime::new()?;
    let runtime = runtime.runtime;

    match runtime {
        UnityRuntime::MonoRuntime(_) => {
            super::mono::start()?;
        },

        UnityRuntime::Il2Cpp(_) => {
            dotnet::start()?;
        }
    }
    Ok(())
}