use thiserror::Error;

#[derive(Debug, Error)]
pub enum ConsoleError {
    #[error("Failed to allocate console!")]
    FailedToAllocateConsole,

    #[error("Failed to get console window!")]
    FailedToGetConsoleWindow,

    #[error("Failed to set console control handler!")]
    FailedToSetConsoleCtrlHandler,
}
