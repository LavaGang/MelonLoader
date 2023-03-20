use thiserror::Error;

#[derive(Error, Debug)]
pub enum LogError {
    /// the log file could not be deleted
    #[error("Failed to delete the old log file. Please ensure you have permission to delete the file, and that you are not currently editing it.")]
    FailedToDeleteOldLog,

    /// the log file could not be written to
    #[error("Failed to write to log file. Please ensure you have permission to write to the file, and that you are not currently editing it.")]
    FailedToWriteToLog,

    /// the base path could not be fetched
    #[error("Failed to fetch base path")]
    FailedToGetBasePath,
}
