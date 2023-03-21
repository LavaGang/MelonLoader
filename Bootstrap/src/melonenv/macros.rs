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

#[macro_export]
macro_rules! should_set_title {
    () => {
        !$crate::melonenv::args::ARGS.console_dst
    };
}

#[macro_export]
macro_rules! console_on_top {
    () => {
        $crate::melonenv::args::ARGS.console_on_top
    };
}

#[macro_export]
macro_rules! hide_console {
    () => {
        $crate::melonenv::args::ARGS.hide_console
    };
}