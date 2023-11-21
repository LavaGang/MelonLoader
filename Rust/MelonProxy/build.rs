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

fn link_exports() {
    println!("cargo:warning=Linking Exports File..");
    use std::path::Path;
    let lib_path = Path::new("deps").join("Exports.def");
    let absolute_path = std::fs::canonicalize(&lib_path).unwrap();
    println!(
        "cargo:rustc-cdylib-link-arg=/DEF:{}",
        absolute_path.display()
    );
}