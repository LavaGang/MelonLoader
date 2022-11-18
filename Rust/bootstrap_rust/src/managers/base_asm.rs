//! functions for starting MelonLoader

use std::error;

use super::{mono::{Mono, self}, game::Game};

/// initialize
pub fn init(mono: &Mono, game_data: &Game) -> Result<(), Box<dyn error::Error>> {
    if game_data.il2cpp {
        //todo

        return Ok(());
    }

    mono::invoke_init(&mono, &game_data)
}

/// prestart
pub fn pre_start(game_data: &Game) -> Result<(), Box<dyn error::Error>> {
    if game_data.il2cpp {
        //todo

        return Ok(());
    }

    mono::invoke_pre_start()
}

/// start
pub fn start(game_data: &Game) -> Result<(), Box<dyn error::Error>> {
    if game_data.il2cpp {
        //todo

        return Ok(());
    }

    mono::invoke_start()
}