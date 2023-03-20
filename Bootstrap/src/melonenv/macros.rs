#[macro_export]
macro_rules! debug_enabled {
    () => {
        $crate::melonenv::args::ARGS.debug
    };
}
