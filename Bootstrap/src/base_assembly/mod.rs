use unity_rs::runtime::{FerrexRuntime, RuntimeType};

use crate::{errors::DynErr, runtime};

pub mod dotnet;
pub mod mono;

pub fn init(runtime: &FerrexRuntime) -> Result<(), DynErr> {
    match runtime.get_type() {
        RuntimeType::Mono(_) => mono::init(&runtime),

        RuntimeType::Il2Cpp(_) => dotnet::init(),
    }
}

pub fn pre_start() -> Result<(), DynErr> {
    let runtime = runtime!()?;

    match runtime.get_type() {
        RuntimeType::Mono(_) => mono::pre_start(),

        RuntimeType::Il2Cpp(_) => dotnet::pre_start(),
    }
}

pub fn start() -> Result<(), DynErr> {
    let runtime = runtime!()?;

    match runtime.get_type() {
        RuntimeType::Mono(_) => mono::start(),

        RuntimeType::Il2Cpp(_) => dotnet::start(),
    }
}
