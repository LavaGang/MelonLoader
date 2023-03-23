use thiserror::Error;

#[derive(Error, Debug)]
pub enum MelonErr {
    #[error("{0}")]
    Generic(String),
}
