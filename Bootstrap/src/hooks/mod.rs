use crate::errors::{DynErr};
use std::marker::PhantomData;
use std::ops::Deref;
use std::ffi::c_void;
use std::ptr::null_mut;

pub mod functions;
pub mod init_hook;
pub mod invoke_hook;

#[derive(Debug)]
pub struct NativeHook<T> {
    pub target: *mut c_void,
    pub trampoline: *mut c_void,
    pub detour: *mut c_void,
    pd: PhantomData<T>,
}

impl<T> NativeHook<T> {
    pub fn new(target: *mut c_void, detour: *mut c_void) -> Self {
        Self {
            target,
            trampoline: null_mut(),
            detour,
            pd: PhantomData,
        }
    }

    pub fn is_hooked(&self) -> bool {
        !self.target.is_null() && !self.trampoline.is_null()
    }

    pub fn hook(&mut self) -> Result<(), DynErr> {
        if self.is_hooked() {
            return Ok(());
        }

        let trampoline = functions::hook(self.target as usize, self.detour as usize)?;

        self.trampoline = trampoline as *mut c_void;
        Ok(())
    }

    pub fn unhook(&self) -> Result<(), DynErr> {
        if !self.is_hooked() {
            return Ok(());
        }

        functions::unhook(self.target as usize)?;

        Ok(())
    }
}

unsafe impl<T> Send for NativeHook<T> {}
unsafe impl<T> Sync for NativeHook<T> {}

impl<T> Clone for NativeHook<T> {
    fn clone(&self) -> Self {
        NativeHook { ..*self }
    }
}

impl<T> Deref for NativeHook<T> {
    type Target = T;

    fn deref(&self) -> &T {
        unsafe { &*(&self.trampoline as *const *mut _ as *const T) }
    }
}
