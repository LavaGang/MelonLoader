use std::error::Error;

pub fn hook(target: usize, detour: usize) -> Result<usize, Box<dyn Error>> {
    if target == 0 {
        return Err("Target pointer for NativeHook is null!".into());
    }

    if detour == 0 {
        return Err("Detour pointer for NativeHook is null!".into());
    }

    unsafe {
        let trampoline = dobby_rs::hook(target as dobby_rs::Address, detour as dobby_rs::Address);

        let trampoline = match trampoline {
            Ok(t) => t,
            Err(e) => return Err(e.into()),
        };

        if trampoline.is_null() {
            return Err("The returned trampoline pointer is null!".into());
        }

        //return Ok with type annotations
        Ok(trampoline as usize)
    }
}

pub fn unhook(target: usize) -> Result<(), Box<dyn Error>> {
    if target == 0 {
        return Err("Target function for unhooking is null!".into());
    }

    unsafe {
        dobby_rs::unhook(target as dobby_rs::Address)?;
    }

    Ok(())
}
