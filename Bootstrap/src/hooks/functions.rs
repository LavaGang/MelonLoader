use crate::errors::{hookerr::HookError, DynErr};

pub fn hook(target: usize, detour: usize) -> Result<usize, HookError> {
    if target == 0 {
        return Err(HookError::Nullpointer("target".to_string()));
    }

    if detour == 0 {
        return Err(HookError::Nullpointer("detour".to_string()));
    }

    unsafe {
        let trampoline = dobby_rs::hook(target as dobby_rs::Address, detour as dobby_rs::Address)
            .map_err(|e| HookError::Failed(e.to_string()));

        let trampoline = match trampoline {
            Ok(t) => t,
            Err(e) => return Err(e),
        };

        if trampoline.is_null() {
            return Err(HookError::Null);
        }

        //return Ok with type annotations
        Ok(trampoline as usize)
    }
}

pub fn unhook(target: usize) -> Result<(), DynErr> {
    if target == 0 {
        return Err(HookError::Nullpointer("target".to_string()).into());
    }

    unsafe {
        dobby_rs::unhook(target as dobby_rs::Address)?;
    }

    Ok(())
}
