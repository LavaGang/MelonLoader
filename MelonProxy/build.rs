use std::env;

fn main() {
    let target_os = env::var("CARGO_CFG_TARGET_OS");

    match target_os.as_ref().map(|x| &**x) {
        Ok("linux") | Ok("android") => {},
        Ok("freebsd") | Ok("dragonfly") => {},
        Ok("openbsd") | Ok("bitrig") | Ok("netbsd") | Ok("macos") | Ok("ios") => {}

        Ok("windows") => link_exports(),

        tos => panic!("unknown target os {:?}!", tos)
    }
}

/// links Exports.def to the resulting dll, exporting all our asm functions.
fn link_exports() {
    use std::path::Path;
    let lib_path = Path::new("deps").join("Exports.def");
    let mut absolute_path = std::fs::canonicalize(&lib_path).unwrap().to_str().unwrap().to_string();
    if absolute_path.contains("\\\\?\\") {
        absolute_path = absolute_path.replace("\\\\?\\", "");
    }

    if !lib_path.exists() {
        panic!("Exports.def not found at {}", lib_path.display());
    }

    println!("cargo:warning=Linking Exports File: {}", absolute_path);
    println!(
        "cargo:rustc-cdylib-link-arg=/DEF:{}",
        absolute_path
    );
}