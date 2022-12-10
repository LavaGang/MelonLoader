//! il2cpp

use std::{error, path::PathBuf};

use crate::{utils::libs::{MelonLib, self}};
use super::{game::Game, exports};

#[derive(Debug)]
/// the il2cpp struct 
pub struct Il2Cpp {
    /// the path to the game's assembly
    pub game_assembly: PathBuf,
    /// the assembly
    pub lib: MelonLib,
}

/// init
pub fn init(game: &Game) -> Result<Option<Il2Cpp>, Box<dyn error::Error>> {
    if !game.il2cpp {
        return Ok(None);
    }

    let lib_path = game.base_path.join("GameAssembly.dll");
    let lib = libs::load_lib(&lib_path)?;

    let il2cpp = Il2Cpp {
        game_assembly: lib_path,
        lib: lib,
    };

    exports::il2cpp::init(&il2cpp)?;

    Ok(Some(il2cpp))
}