@echo off
cargo ndk -t arm64-v8a -o ./jniLibs build
rem add --release to build as release
IF "%1" NEQ "auto" (
    pause
)