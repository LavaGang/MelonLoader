//! Console interaction like windows is not standard anywhere

use std::error::Error;

pub unsafe fn init() -> Result<(), Box<dyn Error>> {
    Ok(())
}

#[allow(dead_code)]
pub fn set_title(_title: &str) {

}

pub fn set_handles() -> Result<(), Box<dyn Error>> {
    Ok(())
}

pub fn null_handles() -> Result<(), Box<dyn Error>> {
    Ok(())
}