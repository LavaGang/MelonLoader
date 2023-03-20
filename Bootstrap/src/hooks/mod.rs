use crate::errors::{hookerr::HookError, DynErr};
use std::marker::PhantomData;
use std::ops::Deref;

pub mod functions;
pub mod init_hook;
pub mod invoke_hook;

#[derive(Debug)]
pub struct HookedFunction<T> {
    pub target: usize,
    pub trampoline: usize,
    pd: PhantomData<T>,
}

impl<T> HookedFunction<T> {
    pub fn new() -> Self {
        Self {
            target: 0,
            trampoline: 0,
            pd: PhantomData,
        }
    }

    pub fn from(target: usize, trampoline: usize) -> Result<Self, HookError> {
        if target == 0 {
            return Err(HookError::Nullpointer("target".to_string()));
        }

        if trampoline == 0 {
            return Err(HookError::Nullpointer("trampoline".to_string()));
        }

        Ok(Self {
            target,
            trampoline,
            pd: PhantomData,
        })
    }

    pub fn is_hooked(&self) -> bool {
        self.target != 0 && self.trampoline != 0
    }

    pub fn unhook(&self) -> Result<(), DynErr> {
        if !self.is_hooked() {
            return Ok(());
        }

        functions::unhook(self.target)?;

        Ok(())
    }
}

unsafe impl<T> Send for HookedFunction<T> {}
unsafe impl<T> Sync for HookedFunction<T> {}

impl<T> Clone for HookedFunction<T> {
    fn clone(&self) -> Self {
        HookedFunction { ..*self }
    }
}

impl<T> Deref for HookedFunction<T> {
    type Target = T;

    fn deref(&self) -> &T {
        let t = self.trampoline as *mut std::ffi::c_void;
        unsafe { &*(&t as *const *mut _ as *const T) }
    }
}
