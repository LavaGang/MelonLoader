//! functions for starting MelonLoader

use std::{error};



use super::{mono::{Mono, self, MonoError}, game::Game, il2cpp::Il2Cpp, dotnet};

/// initialize
pub fn init(mono: Option<&Mono>, game_data: &Game, il2cpp: Option<&Il2Cpp>) -> Result<(), Box<dyn error::Error>> {
    if game_data.il2cpp {
        dotnet::load(game_data, il2cpp.unwrap())?;
        return dotnet::call_init(game_data);
    }

    if mono.is_none() {
        return Err(Box::new(MonoError::FailedToFindLib));
    }

    mono::invoke_init(&mono.unwrap(), &game_data)
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