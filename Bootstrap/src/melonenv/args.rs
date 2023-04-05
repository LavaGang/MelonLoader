use clap::Parser;
use lazy_static::lazy_static;

use crate::{internal_failure};

#[derive(Debug, Parser)]
#[command(author, version, about, long_about = None)]
pub struct Cli {
    #[arg(long = "melonloader.debug")]
    pub debug: Option<bool>,

    #[arg(long = "melonloader.consoledst")]
    pub console_dst: Option<bool>,

    #[arg(long = "melonloader.consoleontop")]
    pub console_on_top: Option<bool>,

    #[arg(long = "melonloader.hideconsole")]
    pub hide_console: Option<bool>,

    #[arg(long = "melonloader.basedir")]
    pub base_dir: Option<String>,
}

lazy_static! {
    pub static ref ARGS: Cli = {
        Cli::parse_optimistic().unwrap_or_else(|e| {
            internal_failure!("Failed to parse command line arguments: {}", e.to_string());
        })
    };
}
