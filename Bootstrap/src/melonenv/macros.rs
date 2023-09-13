#[macro_export]
macro_rules! debug_enabled {
    () => {{
        if cfg!(debug_assertions) {
            true
        } else {
            let args: Vec<String> = std::env::args().collect();
            args.contains(&"--melonloader.debug".to_string())
        }
    }};
}

#[macro_export]
macro_rules! should_set_title {
    () => {{
        let args: Vec<String> = std::env::args().collect();
        !args.contains(&"--melonloader.consoledst".to_string())
    }};
}

#[macro_export]
macro_rules! console_on_top {
    () => {{
        let args: Vec<String> = std::env::args().collect();
        args.contains(&"--melonloader.consoleontop".to_string())
    }};
}

#[macro_export]
macro_rules! hide_console {
    () => {{
        let args: Vec<String> = std::env::args().collect();
        args.contains(&"--melonloader.hideconsole".to_string())
    }};
}