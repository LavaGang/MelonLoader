use std::{path::PathBuf, error::Error};

pub fn get_base_dir() -> Result<PathBuf, Box<dyn Error>> {
    let args: Vec<String> = std::env::args().collect();
    let mut base_dir = std::env::current_dir()?;

    for arg in args.iter() {
        if arg.starts_with("--melonloader.basedir") {
            let a: Vec<&str> = arg.split("=").collect();
            base_dir = PathBuf::from(a[1]);
        }
    }

    Ok(base_dir)
}