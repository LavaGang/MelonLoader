import sys
import os
import shutil

args = sys.argv[1:] # The first argument is the file name.

IsDebug: bool = "--release" not in args
IsLinux: bool = sys.platform == "linux" or sys.platform == "linux2"
IsMac: bool = sys.platform == "darwin"

MelonOutputPath: str = os.path.join(
    "MelonLoader",
    "Output",
    "Debug" if IsDebug else "Release", 
    "MelonLoader"
)

OutputPath: str = os.path.join(
    "Output", 
    "Debug" if IsDebug else "Release"
)

DotnetPath: str = os.path.join(
    "BaseLibs", 
    "dotnet"
)

DllExtension: str = "so" if IsLinux else "dylib" if IsMac else "dll"

targets: dict = {
    "win64": "x86_64-pc-windows-msvc",
    "win32": "i686-pc-windows-msvc",
    "linux": "x86_64-unknown-linux-gnu",
    #"macos": "x86_64-apple-darwin"
}

DotnetCommand: str = "dotnet build MelonLoader/MelonLoader.sln"
if not IsDebug:
    DotnetCommand += " --configuration Release"

CargoCommand: str = "cargo build"
CargoCommand += " --target="
if not IsDebug:
    CargoCommand += " --release"


def clean():
    # delete the output dir, if it already exists.
    if os.path.exists(OutputPath):
        shutil.rmtree(OutputPath)

def build(target: str):
    if IsLinux and target == "win32":
        print("Cross compiling to a different arch is currently unsupported with cross compilers.")
        return
    
    # create Output/[Debug | Release]/platform/
    os.makedirs(os.path.join(OutputPath, target))

    # Compile C#
    os.system(DotnetCommand)

    # fully construct Cargo Command
    xwin = IsLinux and target == "win64"
    command = CargoCommand
    command = command.replace("--target=", "--target={}".format(targets[target]))
    if xwin:
        command = command.replace("cargo", "cargo-xwin")
    print(command)
    
    # compile rust
    os.system(command)
    
    # ex: target/x86_64-pc-windows-msvc/release/
    cargo_out_path = os.path.join("target", targets[target], "debug" if IsDebug else "release")

    # ex: Output/Release/win64/MelonLoader/Dependencies
    bootstrap_destination = os.path.join(OutputPath, target, "MelonLoader", "Dependencies")

    # Move the MelonLoder folder
    os.replace(MelonOutputPath, os.path.join(OutputPath, target, "MelonLoader"))

    # Create the Dependencies folder
    os.makedirs(os.path.join(OutputPath, target, "MelonLoader", "Dependencies"))

    # Move Proxy/Bootstrap/Dobby/Dotnet to their right places.
    if target == "win32" or target == "win64":
        os.replace(os.path.join(cargo_out_path, "version.dll"), os.path.join(OutputPath, target, "version.dll"))
        os.replace(os.path.join(cargo_out_path, "melon_bootstrap.dll"), os.path.join(bootstrap_destination, "melon_bootstrap.dll"))

        shutil.copytree(os.path.join(DotnetPath, "windows", "x86_64" if target == "win64" else "x86"), os.path.join(bootstrap_destination, "dotnet"))
        shutil.copy(os.path.join("BaseLibs", "dobby", "windows", "x86_64" if target == "win64" else "x86", "dobby.dll"), os.path.join(OutputPath, target, "dobby.dll"))
    elif target == "macos":
        os.replace(os.path.join(cargo_out_path, "libmelon_bootstrap.dylib"), os.path.join(bootstrap_destination, "libmelon_bootstrap.dylib"))
        shutil.copytree(os.path.join(DotnetPath, "macos", "x86_64"), os.path.join(bootstrap_destination, "dotnet"))
    elif target == "linux":
        os.replace(os.path.join(cargo_out_path, "libmelon_bootstrap.so"), os.path.join(bootstrap_destination, "libmelon_bootstrap.so"))
        shutil.copytree(os.path.join(DotnetPath, "linux", "x86_64"), os.path.join(bootstrap_destination, "dotnet"))

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



