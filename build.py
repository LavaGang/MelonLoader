import sys
import os
import shutil

args = sys.argv[1:]



MELON_OUT_PATH = os.path.join("MelonLoader", "Output")
DEBUG: bool = "--release" not in args
MELON_FILES_PATH = os.path.join(MELON_OUT_PATH, "Debug" if DEBUG else "Release", "MelonLoader")
OUT_PATH = os.path.join("Output", "Debug" if DEBUG else "Release")

IS_LINUX = sys.platform == "linux" or sys.platform == "linux2"
IS_MAC = sys.platform == "darwin"
DLL_EXTENSION = "so" if IS_LINUX else "dll"
DOTNET_PATH = os.path.join("BaseLibs", "dotnet")

targets: dict = {
    "win64": "x86_64-pc-windows-msvc",
    "win32": "i686-pc-windows-msvc",
    "linux": "x86_64-unknown-linux-gnu",
    #"macos": "x86_64-apple-darwin"
}

def clean():
    path = os.path.join("Output", "Debug" if DEBUG else "Release")
    if os.path.exists(path):
        shutil.rmtree(path)

def build(target: str):
    if IS_LINUX and target == "win32":
        print("Cross compiling to a different arch is currently unsupported with cross compilers.")
        return
    
    xwin = IS_LINUX and (target == "win64" or target == "win32")
    os.makedirs(os.path.join(OUT_PATH, target))
    os.system("dotnet build MelonLoader/MelonLoader.sln{}".format("" if DEBUG else " --configuration Release"))

    cargo_args = "--target={}{}".format(targets[target], "" if DEBUG else " --release")
    cargo_tool = "cargo-xwin" if xwin else "cargo"

    os.system("{} build {}".format(cargo_tool, cargo_args))
    
    cargo_out_path = os.path.join("target", targets[target], "debug" if DEBUG else "release")
    bootstrap_destination = os.path.join(OUT_PATH, target, "MelonLoader", "Dependencies")

    os.replace(MELON_FILES_PATH, os.path.join(OUT_PATH, target, "MelonLoader"))
    os.makedirs(os.path.join(OUT_PATH, target, "MelonLoader", "Dependencies"))

    if target == "win32" or target == "win64":
        os.replace(os.path.join(cargo_out_path, "version.dll"), os.path.join(OUT_PATH, target, "version.dll"))
        os.replace(os.path.join(cargo_out_path, "melon_bootstrap.dll"), os.path.join(bootstrap_destination, "melon_bootstrap.dll"))

        shutil.copytree(os.path.join(DOTNET_PATH, "windows", "x86_64" if target == "win64" else "x86"), os.path.join(bootstrap_destination, "dotnet"))
        shutil.copy(os.path.join("BaseLibs", "dobby", "windows", "x86_64" if target == "win64" else "x86", "dobby.dll"), os.path.join(OUT_PATH, target, "dobby.dll"))
    elif target == "macos":
        os.replace(os.path.join(cargo_out_path, "libmelon_bootstrap.dylib"), os.path.join(bootstrap_destination, "libmelon_bootstrap.dylib"))
        shutil.copytree(os.path.join(DOTNET_PATH, "macos", "x86_64"), os.path.join(bootstrap_destination, "dotnet"))
    elif target == "linux":
        os.replace(os.path.join(cargo_out_path, "libmelon_bootstrap.so"), os.path.join(bootstrap_destination, "libmelon_bootstrap.so"))
        shutil.copytree(os.path.join(DOTNET_PATH, "linux", "x86_64"), os.path.join(bootstrap_destination, "dotnet"))

def main():
    clean()
    if "all" in args:
        for target in targets:
            build(target)
    else:
        for arg in args:
            if arg == "--release":
                continue

            build(arg)



if __name__ == "__main__":
    main()



