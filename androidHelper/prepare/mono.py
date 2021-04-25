import os
from helper import common
import shutil
from variants import paths


def install_mono(path):
    if not os.path.isdir(paths.Paths.mono_assemblies_path):
        print("Cannot find mono C# assemblies in %s" % paths.Paths.mono_assemblies_path)
        return False

    assemblies_path = os.path.join(path, paths.Paths.mono_assemblies_target)
    common.prepare_dir(assemblies_path)

    for path in os.listdir(paths.Paths.mono_assemblies_path):
        if not os.path.isfile(os.path.join(paths.Paths.mono_assemblies_path, path)):
            continue

        if not path.endswith(".dll"):
            continue

        shutil.copyfile(os.path.join(paths.Paths.mono_assemblies_path, path), os.path.join(assemblies_path, path))

    return True


def install_mono_native(path):
    if not os.path.isdir(paths.Paths.mono_native_assemblies_path):
        print("%s does not exist" % paths.Paths.mono_native_assemblies_path)
        return False

    lib_dir = os.path.join(path, "lib")
    dest_dirs = os.listdir(lib_dir)
    mono_abi_dirs = os.listdir(paths.Paths.mono_native_assemblies_path)

    abi_count = 0

    for abi in common.Settings.supported_abi:
        if abi not in dest_dirs:
            continue

        if abi not in mono_abi_dirs:
            print("WARNING: missing ABI %s in mono")
            continue

        abi_count += 1
        install_mono_abi(lib_dir, abi)

    return abi_count > 0


def install_mono_abi(path, abi):
    mono_abi_dir = os.path.join(paths.Paths.mono_native_assemblies_path, abi)
    dest_abi_dir = os.path.join(path, abi)

    for key, value in paths.Paths.mono_native_assemblies.items():
        shutil.copyfile(os.path.join(mono_abi_dir, key), os.path.join(dest_abi_dir, value))
