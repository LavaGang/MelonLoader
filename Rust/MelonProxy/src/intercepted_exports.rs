// Intercepted/replaced functions go here
// Remember to run `proxygen update .` in the root of this project every time you add or remove an export here
//
// Example function proxies/hooks.
// Note, if using any of the `proxy`, `pre_hook` or `post_hook` macros, you will have access to the original function
// You can easily call `orig_func` with the same args as your interceptor function
// NOTE: Use the correct arg types and return type for any functions you proxy, or else you will probably mess up the stack
//       and you will probably crash whatever program the DLL is loaded into
//
// #[pre_hook(sig="known")]
// #[export_name="SomeFunction"]
// pub extern "C" fn SomeFunction(some_arg_1: usize, some_arg_2: u32) -> bool {
//     println!("Pre-hooked SomeFunction. Args: {}, {}", some_arg_1, some_arg_2);
//     // After all our code in this pre-hook runs, if we don't return, the original function will be called
//     // and its result will be returned
// }
//
// #[proxy(sig="known")]
// #[export_name="SomeFunction"]
// pub extern "C" fn SomeFunction(some_arg_1: usize, some_arg_2: u32) -> bool {
//     let orig_result = orig_func(some_arg_1, some_arg_2);
//     println!("Manually proxied SomeFunction. Args: {}, {}. Result: {}", some_arg_1, some_arg_2, orig_result);
//     // This is just a normal/manual proxy. It is up to us to return a value.
//     // Also note that the original function `orig_func` will not be run in this case unless we explicitly call it
//     true
// }
//
// #[post_hook(sig="known")]
// #[export_name="SomeFunction"]
// pub extern "C" fn SomeFunction(some_arg_1: usize, some_arg_2: u32) -> bool {
//     // `orig_func` got run just before our code. Its result is stored in `orig_result`
//     println!("In post-hook for SomeFunction. Args: {}, {}. Result: {}", some_arg_1, some_arg_2, orig_result);
//     // We could manually return something here if we didn't want `orig_result` returned
// }
//
// #[pre_hook(sig="unknown")]
// #[export_name="SomeFunction"]
// pub extern "C" fn SomeFunction() {
//     println!("In pre-hook for SomeFunction. (signature unknown)")
// }

#![allow(unused_imports)]
use proxygen_macros::{post_hook, pre_hook, proxy, forward};

