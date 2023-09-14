#[cfg(windows)]
fn main() {
    println!("cargo:warning=Linking Exports File..");
    use std::path::Path;
    let lib_path = Path::new("deps").join("Exports.def");
    let absolute_path = std::fs::canonicalize(&lib_path).unwrap();
    println!(
        "cargo:rustc-cdylib-link-arg=/DEF:{}",
        absolute_path.display()
    );
}

#[cfg(not(windows))]
fn main() {

}