//! Takes care of initializing game paths, and determining the runtime of the game.

use std::{path::PathBuf, env, error};

use thiserror::Error;

/// Contains some vital information about the current game
/// 
/// This is used to determine the runtime of the game, and the game's paths.
#[derive(Debug)]
pub struct Game {
    /// The name of the game's executable
    pub app_path: PathBuf,
    /// The directory the game is located in
    pub base_path: PathBuf,
    /// The unity data path
    pub data_path: PathBuf,
    /// is the game il2cpp?
    pub il2cpp: bool
}

/// game errors
#[derive(Debug, Error)]
pub enum GameError {
    /// Failed to get the base path
    #[error("Failed to get the base path!")]
    FailedToGetBasePath,
    /// Failed to get the data path
    #[error("Failed to get the data path!")]
    FailedToGetDataPath,
    /// Failed to get the app path
    #[error("Failed to get the app path!")]
    FailedToGetAppPath,
    /// Failed to get the il2cpp path
    #[error("Failed to get the il2cpp path!")]
    FailedToGetIl2CppPath,
}

/// initializes the game manager
pub fn init() -> Result<Game, Box<dyn error::Error>> {
    let app_path = env::current_exe()
        .map_err(|_| GameError::FailedToGetAppPath)?;

    let base_path = app_path.parent()
        .ok_or(GameError::FailedToGetBasePath)?
        .to_path_buf();
    
    let app_name = app_path.file_stem().
        ok_or(GameError::FailedToGetAppPath)?
        .to_str()
        .ok_or(GameError::FailedToGetAppPath)?;
    
    let data_folder = format!("{}_Data", app_name);
    let data_path = base_path.join(data_folder);

    if !data_path.exists() {
        return Err(Box::new(GameError::FailedToGetDataPath));
    }

    let assembly_path = base_path.join("GameAssembly.dll");
    let il2cpp = assembly_path.exists();

    let game = Game {
        app_path,
        base_path,
        data_path,
        il2cpp
    };

    Ok(game)
}