extern crate cc;

fn main() {
    use cc::Build;
    Build::new()
        .cpp(true)
        .file("freopen_s.cpp")
        .compile("libfoo.a");
}