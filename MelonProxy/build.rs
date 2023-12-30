use std::env;

fn main() {
    let target_os = env::var("CARGO_CFG_TARGET_OS");

    match target_os.as_ref().map(|x| &**x) {
        Ok("linux") | Ok("android") => {}
        Ok("freebsd") | Ok("dragonfly") => {}
        Ok("openbsd") | Ok("bitrig") | Ok("netbsd") | Ok("macos") | Ok("ios") => {}

        Ok("windows") => link_exports(),

        tos => panic!("unknown target os {:?}!", tos),
    }
}

/// links Exports.def to the resulting dll, exporting all our asm functions.
fn link_exports() {
    let lib_path = env::current_dir().unwrap().join("deps").join("Exports.def");


    if !lib_path.exists() {
        println!("cargo:error=Exports.def not found at {}", lib_path.display());
    }

    println!("cargo:warning=Linking Exports File: {}", lib_path.display());
    println!("cargo:rustc-cdylib-link-arg=/DEF:{}", lib_path.display());
}
