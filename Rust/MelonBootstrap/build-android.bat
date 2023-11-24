@echo off
cargo ndk -t arm64-v8a -o ./jniLibs build
rem --release
copy "%~dp0jniLibs\arm64-v8a\libmelon_bootstrap.so" "C:/Users/trevo/Desktop/android_universe/lib/arm64-v8a"
copy "%~dp0jniLibs\arm64-v8a\libmain.so" "C:/Users/trevo/Desktop/android_universe/lib/arm64-v8a"
IF "%1" NEQ "auto" (
    pause
)