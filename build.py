import sys
import os
import shutil
from distutils.dir_util import copy_tree

os.environ["MSBUILDDISABLENODEREUSE"] = "1"

args = sys.argv

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

DotnetPaths: dict = {
    "win64": os.path.join(DotnetPath, "windows", "x86_64"),
    "win32": os.path.join(DotnetPath, "windows", "x86"),
    "linux": os.path.join(DotnetPath, "linux", "x86_64"),
    "macos": os.path.join(DotnetPath, "macos", "x86_64"),
}

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

def GameDirArg():
    for arg in args:
        print(arg)
        
        return None



BootstrapName = "libmelon_bootstrap"

def clean(target: str):
    # delete the output dir, if it already exists.
    if os.path.exists(os.path.join(OutputPath, target)):
        shutil.rmtree(os.path.join(OutputPath, target))

def build(target: str):
    dll_extension: str = "dll" if target == "win64" or target == "win32" else "so" if target == "linux" else "dylib"
    bootstrap_name = "libmelon_bootstrap.{}".format(dll_extension)
    version_name = "libversion.{}".format(dll_extension)
    
    # ex: target/x86_64-pc-windows-msvc/release/
    cargo_out_path = os.path.join("target", targets[target], "debug" if IsDebug else "release")

    # ex: Output/Release/win64/MelonLoader/Dependencies
    bootstrap_destination = os.path.join(OutputPath, target, "MelonLoader", "Dependencies")


    dotnet_path = DotnetPath

    # create Output/[Debug | Release]/platform/
    os.makedirs(os.path.join(OutputPath, target))

    # Compile C#
    os.system(DotnetCommand)

    # fully construct Cargo Command
    xwin = IsLinux and target == "win64" or target == "win32"
    command = CargoCommand
    command = command.replace("--target=", "--target={}".format(targets[target]))
    if xwin:
        os.environ["XWIN_CACHE_DIR"] = os.path.join(".cache", "x64")
        command = command.replace("cargo", "cargo xwin")
        if target == "win32":
            os.environ["XWIN_ARCH"] = "x86"
            os.environ["XWIN_CACHE_DIR"] = os.path.join(".cache", "x86")
    

    print(command)
    
    
    # compile rust
    os.system(command)
    

    # Move the MelonLoder folder
    os.replace(MelonOutputPath, os.path.join(OutputPath, target, "MelonLoader"))

    # Create the Dependencies folder
    os.makedirs(os.path.join(OutputPath, target, "MelonLoader", "Dependencies"))



    # Move Proxy/Bootstrap/Dobby/Dotnet to their right places.
    if target == "win32" or target == "win64":
        version_name = version_name.replace("lib", "")
        bootstrap_name = bootstrap_name.replace("lib", "")

        shutil.copy(os.path.join("BaseLibs", "dobby", "windows", "x86_64" if target == "win64" else "x86", "dobby.dll"), os.path.join(OutputPath, target, "dobby.dll"))

    os.replace(os.path.join(cargo_out_path, bootstrap_name), os.path.join(bootstrap_destination, bootstrap_name))
    os.replace(os.path.join(cargo_out_path, version_name), os.path.join(OutputPath, target, version_name))
    
    shutil.copytree(DotnetPaths[target], os.path.join(bootstrap_destination, "dotnet"))

    for arg in args:
        if arg.startswith("--gamedir="):
            copy_tree(os.path.join(OutputPath, target), arg.split("=")[1])

            


def main():
    if "all" in args:
        for target in targets:
            build(target)
    else:
        for arg in args:
            if arg == "--release":
                continue

            if arg.startswith("--gamedir="):
                continue

            if arg == "build.py":
                continue
            
            

            clean(arg)
            build(arg)



if __name__ == "__main__":
    main()



