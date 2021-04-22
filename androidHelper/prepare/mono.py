import os
import helpers
import shutil

mono_assemblies_path = os.path.join(helpers.Settings.visual_studio_path(), "Common7", "IDE", "ReferenceAssemblies", "Microsoft", "Framework", "MonoAndroid", "v1.0")
mono_native_assemblies_path = os.path.join(helpers.Settings.visual_studio_path(), "MSBuild", "Xamarin", "Android", "lib")

# key value relationship so file can be renamed when copied
# key is orignal name
# value is copied name
mono_native_assemblies = {
    "libmono-native.so": "libmono-native.so",
    "libMonoPosixHelper.so": "libMonoPosixHelper.so",
    "libmonosgen-2.0.so": "libmonosgen-2.0.so"
}


def install_mono(path):
    if not os.path.isdir(mono_assemblies_path):
        print("Cannot find mono C# assemblies in %s" % mono_assemblies_path)
        return False

    assemblies_path = os.path.join(path, "assets", "melonloader", "etc", "managed")
    helpers.prepare_dir(assemblies_path)

    for path in os.listdir(mono_assemblies_path):
        if not os.path.isfile(os.path.join(mono_assemblies_path, path)):
            continue

        if not path.endswith(".dll"):
            continue

        shutil.copyfile(os.path.join(mono_assemblies_path, path), os.path.join(assemblies_path, path))

    return True


def install_mono_native(path):
    if not os.path.isdir(mono_native_assemblies_path):
        print("%s does not exist" % mono_native_assemblies_path)
        return False

    lib_dir = os.path.join(path, "lib")
    dest_dirs = os.listdir(lib_dir)
    mono_abi_dirs = os.listdir(mono_native_assemblies_path)

    abi_count = 0

    for abi in helpers.Settings.supported_abi:
        if abi not in dest_dirs:
            continue

        if abi not in mono_abi_dirs:
            print("WARNING: missing ABI %s in mono")
            continue

        abi_count += 1
        install_mono_abi(lib_dir, abi)

    return abi_count > 0


# TODO: Fix for multiple ABI
def install_mono_abi(path, abi):
    mono_abi_dir = os.path.join(mono_native_assemblies_path, abi)
    dest_abi_dir = os.path.join(path, abi)

    for key, value in mono_native_assemblies.items():
        shutil.copyfile(os.path.join(mono_abi_dir, key), os.path.join(dest_abi_dir, value))
