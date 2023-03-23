use clap::Parser;
use lazy_static::lazy_static;

use crate::{internal_failure};

#[derive(Debug, Parser)]
#[command(author, version, about, long_about = None)]
pub struct Cli {
    #[arg(long = "melonloader.debug", default_value = "false")]
    pub debug: bool,

    #[arg(long = "melonloader.consoledst", default_value = "false")]
    pub console_dst: bool,

    #[arg(long = "melonloader.consoleontop", default_value = "false")]
    pub console_on_top: bool,

    #[arg(long = "melonloader.hideconsole", default_value = "false")]
    pub hide_console: bool,

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
