#[macro_export]
macro_rules! debug_enabled {
    () => {
        if cfg!(debug_assertions) {
            true
        } else {
            $crate::melonenv::args::ARGS.debug.is_some_and(|b| b)
        }
    };
}

#[macro_export]
macro_rules! should_set_title {
    () => {
        !$crate::melonenv::args::ARGS.console_dst.is_some_and(|b| b)
    };
}

#[macro_export]
macro_rules! console_on_top {
    () => {
        $crate::melonenv::args::ARGS.console_on_top.is_some_and(|b| b)
    };
}

#[macro_export]
macro_rules! hide_console {
    () => {
        $crate::melonenv::args::ARGS.hide_console.is_some_and(|b| b)
    };
}