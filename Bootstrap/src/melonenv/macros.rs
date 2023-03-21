#[macro_export]
macro_rules! debug_enabled {
    () => {
        if cfg!(debug_assertions) {
            true
        } else {
            $crate::melonenv::args::ARGS.debug
        }
    };
}
