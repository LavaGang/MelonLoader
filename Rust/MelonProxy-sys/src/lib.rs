//! A Macro for defining an entry point for a cdylib.
//!
//! On Windows, this macro will wrap your function in DllMain, and call it when the DLL attaches.
//! It will lookup exports of supported proxies, based on our own Module Name, and store them.
//! Effectively, creating a dynamic proxy that we could add any number of supported proxies to.
//!
//! # Supported Targets
//!
//! - Windows
//!     - `x86_64-pc-windows-msvc`
//!     - `i686-pc-windows-msvc`
//!
//! # Safety
//!
//! This crate is pretty unsafe

#![feature(naked_functions)]
#![feature(asm_const)]
#![deny(
missing_debug_implementations,
missing_docs,
unused_results,
warnings,
clippy::extra_unused_lifetimes,
clippy::from_over_into,
clippy::needless_borrow,
clippy::new_without_default,
clippy::useless_conversion
)]
#![forbid(rust_2018_idioms)]
#![allow(clippy::inherent_to_string, clippy::type_complexity, improper_ctypes)]
#![cfg_attr(docsrs, feature(doc_cfg))]

use syn::__private::{quote::quote, TokenStream};

/// Wraps your function in DllMain on windows, and passes #[ctor] on linux.
///
///
#[proc_macro_attribute]
pub fn proxy(_attribute: TokenStream, function: TokenStream) -> TokenStream {
    let fn_ts = function.clone();
    let item: syn::Item = syn::parse_macro_input!(fn_ts);
    if let syn::Item::Fn(function) = item {
        let syn::ItemFn {
            attrs,
            block,
            vis,
            sig:
            syn::Signature {
                ident,
                unsafety,
                constness,
                abi,
                output,
                ..
            },
            ..
        } = function;

        let output = quote!(
            #(#attrs)*
            #vis #unsafety #abi #constness fn #ident() #output #block

            //turn the original function into DllMain
            #[no_mangle]
            #[allow(non_snake_case)]
            pub extern "system" fn DllMain(
                _hinstDLL: proxy_dll::HINSTANCE,
                fdwReason: proxy_dll::DWORD,
                _lpvReserved: proxy_dll::LPVOID,
            ) -> proxy_dll::BOOL {

                if fdwReason == proxy_dll::DLL_PROCESS_ATTACH {
                    //call the original function
                    melon_proxy::exports::initialize(_hinstDLL).unwrap_or_else(|e| {
                        ::std::panic!("{}", e);
                    });
                    #ident();
                }
                proxy_dll::TRUE
            }

        );

        return output.into();
    }

    function
}