use thiserror::Error;

#[derive(Error, Debug)]
pub enum DotnetErr {
    #[error("Failed to load hostfxr. Please make sure you have installed the .NET 6.0 runtime.")]
    FailedHostFXRLoad,

    #[error("Failed to find MelonLoader.runtimeconfig.json. Please reinstall MelonLoader.")]
    RuntimeConfig
}