use std::{path::{PathBuf}, ops::{DerefMut, Deref}};

use crate::{constants::W, errors::{melonerr::MelonErr}};

impl TryFrom<W<PathBuf>> for String {
    type Error = MelonErr;

    fn try_from(value: W<PathBuf>) -> Result<Self, Self::Error> {
        value.0.to_str().map(String::from).ok_or_else(|| MelonErr::Generic("Failed to convert PathBuf to &str".to_string()))
    }
}

impl Deref for W<PathBuf> {
    type Target = PathBuf;

    fn deref(&self) -> &Self::Target {
        &self.0
    }
}

impl DerefMut for W<PathBuf> {
    fn deref_mut(&mut self) -> &mut Self::Target {
        &mut self.0
    }
}