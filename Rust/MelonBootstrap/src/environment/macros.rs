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